<#
  Pre-push privacy check.  Run from the repository root (the folder that contains .git):

      powershell -ExecutionPolicy Bypass -File scripts\pre-push-check.ps1
      powershell -ExecutionPolicy Bypass -File scripts\pre-push-check.ps1 -Terms "myname","mylaptop","mynumber"

  It inspects every file Git would commit (tracked files plus untracked files that are not ignored) and reports:
  secrets.config / logs / publish profiles, API keys and tokens, e-mail addresses, phone numbers, local paths,
  a non-placeholder SQL connection string, ngrok addresses, extra -Terms you give it, and your Git identity.
  It changes nothing. Exit code 0 = nothing found, 1 = review the findings.
#>
param([string[]]$Terms = @())

if (-not (Test-Path '.git')) { Write-Host 'Run this from the repository root (the folder that contains .git).' -ForegroundColor Red; exit 2 }

$findings = New-Object System.Collections.Generic.List[string]
function Add-Finding([string]$text) { $findings.Add($text) | Out-Null }

$files = @(git ls-files --cached --others --exclude-standard)

# 1. files that must never be committed
foreach ($f in $files) {
    if ($f -match '(^|/)secrets\.config$')               { Add-Finding "SECRET FILE would be committed: $f" }
    elseif ($f -match '\.(log|pubxml|user|suo|mdf|ldf)$') { Add-Finding "LOCAL/RUNTIME FILE would be committed: $f" }
    elseif ($f -match 'DevDiagnostics\.aspx$')            { Add-Finding "DEV-ONLY PAGE would be committed: $f" }
    elseif ($f -match '(^|/)App_Data/' -and $f -notmatch 'App_Data/README\.txt$') { Add-Finding "APP_DATA file would be committed: $f" }
    elseif ($f -match '04_Create_Admin\.local\.sql$')     { Add-Finding "ADMIN SCRIPT with your password would be committed: $f" }
}

# 2. file contents
$skipExt  = '\.(dll|png|jpe?g|gif|ico|woff2?|ttf|eot|zip|pdf|suo|pdb)$'
$skipPath = '^(code/BusBookingProject/BusBookingProject/(fontawesome|img)/|scripts/pre-push-check\.ps1$)'
$allowEmail = 'example\.(com|org)$|^you@gmail\.com$|^MaharajaTravels@hotmail\.com$|noreply\.github\.com$|^git@|@razorpay$'
$allowPhone = '^(0?8844567244|9876543210)$'

$patterns = @(
    @{ Name='Razorpay key';     Rx='rzp_(test|live)_[A-Za-z0-9]{8,}';                 Ignore='x{6,}|PASTE|YOUR' },
    @{ Name='Private key';      Rx='-----BEGIN [A-Z ]*PRIVATE KEY-----';              Ignore='' },
    @{ Name='AWS key';          Rx='AKIA[0-9A-Z]{16}';                                Ignore='' },
    @{ Name='GitHub token';     Rx='gh[pousr]_[A-Za-z0-9]{30,}';                      Ignore='' },
    @{ Name='Google API key';   Rx='AIza[0-9A-Za-z_\-]{30,}';                         Ignore='' },
    @{ Name='E-mail address';   Rx='[A-Za-z0-9._%+\-]+@[A-Za-z0-9.\-]+\.[A-Za-z]{2,}'; Ignore=$allowEmail },
    @{ Name='Phone number';     Rx='(?<![0-9])0?[6-9][0-9]{9}(?![0-9])';              Ignore=$allowPhone },
    @{ Name='Local Windows path'; Rx='[A-Za-z]:\\Users\\[^\\\s"<]+';                  Ignore='' },
    @{ Name='ngrok address';    Rx='[a-z0-9\-]+\.ngrok-free\.(app|dev)';              Ignore='your' },
    @{ Name='SQL server name';  Rx='Data Source=[^;"]+';                              Ignore='^Data Source=MYPC\\SQLEXPRESS$' }
)

foreach ($f in $files) {
    if ($f -match $skipExt -or $f -match $skipPath) { continue }
    if (-not (Test-Path -LiteralPath $f -PathType Leaf)) { continue }
    $text = Get-Content -LiteralPath $f -Raw -ErrorAction SilentlyContinue
    if (-not $text) { continue }
    foreach ($p in $patterns) {
        foreach ($m in [regex]::Matches($text, $p.Rx)) {
            if ($p.Ignore -and $m.Value -match $p.Ignore) { continue }
            Add-Finding ("{0}: {1}  ->  {2}" -f $p.Name, $f, $m.Value)
        }
    }
    foreach ($t in $Terms) {
        if ($t -and $text.IndexOf($t, [System.StringComparison]::OrdinalIgnoreCase) -ge 0) { Add-Finding "YOUR TERM '$t' found in: $f" }
    }
}

# 3. git identity and history
$email = (git config user.email)
if (-not $email)                                   { Add-Finding 'Git user.email is not set.' }
elseif ($email -notmatch 'users\.noreply\.github\.com$') { Add-Finding "Git user.email is '$email' - commits will publish it. Use your GitHub no-reply address (GitHub > Settings > Emails)." }

git rev-parse --verify HEAD 2>$null | Out-Null
if ($LASTEXITCODE -eq 0) {
    $authors = @(git log --all --format='%ae' | Sort-Object -Unique)
    foreach ($a in $authors) { if ($a -notmatch 'users\.noreply\.github\.com$') { Add-Finding "Existing commits are authored with '$a'." } }
    $touched = @(git log --all --oneline -- '*secrets.config' '*dev-outbox.log' '*.pubxml')
    if ($touched.Count -gt 0) { Add-Finding "History contains commits that touched secret/local files:`n   $($touched -join "`n   ")" }
}

# report
Write-Host ''
if ($findings.Count -eq 0) {
    Write-Host 'PASS - nothing sensitive found in what Git would commit.' -ForegroundColor Green
    exit 0
}
Write-Host ("REVIEW - {0} finding(s):" -f $findings.Count) -ForegroundColor Yellow
$findings | ForEach-Object { Write-Host "  * $_" }
Write-Host ''
Write-Host 'Fix each item (or add the file to .gitignore), then run this script again.' -ForegroundColor Yellow
exit 1
