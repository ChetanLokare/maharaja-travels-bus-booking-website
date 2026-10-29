# Maharaja Travel's – upgraded (v2)

ASP.NET WebForms · .NET Framework 4.8 · SQL Server · Bootstrap 5.3.8

> **Status:** the code was written and statically checked without a .NET compiler or live SQL Server/Razorpay/SMTP,
> so it may need a few small fixes on first build. Issues and pull requests are welcome.

## 1. First run checklist

1. In SSMS, run the scripts in `database/` in this order (SQL Server 2016 SP1+, compatibility level ≥ 130):
   1. `01_Schema_And_Reference_Data.sql` – creates the `OnlineBusBooking` database, tables, cities, buses, routes and sample schedules. *(Upgrading an existing database from the original project? Skip this one and **back up first**.)*
   2. `02_Upgrade_Security_Payments.sql` – security, OTP, payments and seat-hold tables and procedures. Safe to re-run.
   3. `03_Optional_RefreshDemoSchedules.sql` – *demo only*: the sample trips are dated 31‑03‑2024 and are hidden as "past"; this moves them to the next few days.
   4. `04_Create_Admin.sql` – edit the three variables at the top, then run to create your admin account (you must set a new password at first sign-in).
2. Open `code/BusBookingProject/BusBookingProject.sln` in **Visual Studio 2022**, edit the connection string in `Web.config`, then *Build*.
3. Copy `secrets.config.example` → `secrets.config` (same folder) and fill in what you have. Nothing is required just to browse; each feature in section 2 needs its own keys.
4. Run. On `localhost`, with SMTP empty, every e-mail/OTP is written to `App_Data/dev-outbox.log` (open it in File Explorer, or click *Show All Files* in Visual Studio's Solution Explorer – new files are not listed otherwise). While debugging (F5) the same text is also printed in *View → Output → Show output from: Debug*.
5. Customers from the original project keep their passwords – each is converted to a salted hash the first time they sign in.

## 2. Turning on the integrations

| Feature | What to do |
|---|---|
| **Payments (UPI / cards / net banking)** | Create a Razorpay account → *Settings → API Keys* (test mode needs no KYC) → put `Razorpay:KeyId` / `KeySecret` in `secrets.config`. Add a webhook `https://YOUR-SITE/PaymentWebhook.ashx` (events `payment.captured`, `payment.failed`) with the same secret as `Razorpay:WebhookSecret`. Go‑live needs Razorpay KYC. Test UPI/card values are in Razorpay's docs. |
| **E-mail (OTP, sign-in alert, ticket + receipt PDFs)** | Fill `Smtp:*`. Gmail: use an *App Password*; any SMTP relay works. |
| **Google login** | Google Cloud Console → OAuth client (Web). Authorised redirect URI: `{App:BaseUrl}/OAuthCallback.ashx`. Fill `Google:ClientId/ClientSecret`. |
| **Microsoft login** | Entra ID → App registration → *accounts in any directory and personal accounts*; redirect URI (Web): `{App:BaseUrl}/OAuthCallback.ashx`; create a client secret. Fill `Microsoft:*`. |
| **Mobile OTP by SMS** | `Sms:Provider=Twilio` + `Sms:Twilio:*`. India requires DLT registration for SMS. Without SMS, "mobile" login sends the code to the account's e-mail instead. |
| **Production** | Set `App:BaseUrl` to your https URL, publish with *Release* (Web.Release.config turns on Secure cookies), give the app pool write access to `App_Data`, and set a long random `Security:OtpPepper`. |

### Testing payments (Razorpay **Test Mode**)

* Use **Test Mode** keys only. In the checkout window choose **UPI** and type `success@razorpay` (or `failure@razorpay` to test a failed payment); net banking shows a mock bank page with Success/Failure buttons; cards use Razorpay's published test cards. Real UPI apps **cannot** pay a Test Mode QR code – QR/intent payments only work in Live Mode.
* Webhook from your PC: run `ngrok http 1778 --host-header="localhost:1778"`, then add `https://<your-id>.ngrok-free.app/PaymentWebhook.ashx` in Razorpay → Webhooks (events `payment.captured`, `payment.failed`) with the same secret as `Razorpay:WebhookSecret`.
* A fuller manual test plan is in [`docs/TESTING.md`](docs/TESTING.md).

## 3. What changed

**Payments & booking**
* Razorpay Standard Checkout; the server creates the order, then **verifies the signature, asks Razorpay for the payment, checks amount/order/currency**, and only then confirms – in one SQL transaction. A webhook confirms bookings even if the customer closes the tab.
* **Fare is computed in the database.** (Before, the price travelled in the URL – anyone could change `Fare=1`.)
* 10‑minute **seat hold** with live countdown; unique index makes double-booking impossible; if a paid seat is lost, the payment is refunded automatically.
* Card numbers/CVV used to be stored in `CardDetails` – table and procedure removed; card data never touches your server now.
* Fixed: search returned only one bus per date; passenger rows used the wrong schedule; PNR was predictable (`Random`) – now 8 crypto-random characters.

**Login & accounts**
* PBKDF2‑SHA256 (600k iterations, per-password salt); password policy + strength meter; lockout after 5 failures; rate limits; no user enumeration; timing equalised.
* Sign in with: **password (+ e-mail OTP second step)**, **one-time code by e-mail/SMS**, **Google**, **Microsoft** (PKCE + state + nonce). Registration verifies the e-mail by OTP before the account exists. Forgot/reset password via OTP.
* "You just signed in" e-mail on every login; password-changed e-mail; welcome e-mail.
* Tickets: **ticket PDF (with QR) and receipt PDF e-mailed on payment**, and downloadable from *My bookings* (owner only).

**Front end**
* Bootstrap **5.3.8** (CDN, SRI) – jQuery, Bootstrap 3/4 files and Glyphicons removed.
* **Theme switch: Light / Dark / System default** (remembered, no flash on load).
* New responsive layout, animated hero, scroll reveals, counters, page transitions, toasts, loading overlay with double-submit guard, step tracker; `prefers-reduced-motion` respected.
* **Seat map:** bus-shaped layout with aisle and driver, live availability, keyboard accessible, sticky summary (bottom sheet on phones). Seat numbers are unchanged, so old bookings still line up.
* Search results can be sorted/filtered; admin console restyled with a sidebar and a real dashboard.

## 4. Security review – findings and fixes

| Finding | Severity | Fix |
|---|---|---|
| Admin login built SQL by string concatenation (**SQL injection**, auth bypass) | Critical | Parameterised queries |
| Passwords (users **and admins**) stored in plain text | Critical | Salted PBKDF2; legacy upgraded on first login |
| Card number + CVV stored in database | Critical | Table dropped; gateway handles cards |
| Ticket price taken from the query string | Critical | Server-side fare |
| Admin report pages reachable without login (customer PII) | High | `web.config` role rule + master-page guard |
| No CSRF protection on forms | High | `ViewStateUserKey` bound to session, MAC + encrypted ViewState |
| Open redirects / no lockout / no throttling / user enumeration | Medium | Local-only return URLs, lockout, rate limits, uniform messages |
| Stored XSS via un-encoded Labels, feedback echo | Medium | Encoded output (`<%#: %>`, `HtmlEncode`) |
| Debug on, detailed errors, version headers, HTTP allowed | Medium | Custom error page + log with reference id, HTTPS redirect, HSTS, CSP, X-Frame-Options, nosniff, Referrer/Permissions-Policy |
| jQuery 1.4 / Bootstrap 3-4 (known XSS issues) | Medium | Removed / replaced |
| Secrets in `Web.config` | Medium | `secrets.config` (git-ignored) |
| `Response.Write(pdfdoc)` corrupted exported PDFs; exception text shown to users | Low | Removed |

## 5. Known limits / suggestions

* **Not compiled or executed** (see top). Bootstrap's SRI hashes were copied from a published copy of the Bootstrap docs – if the page ever shows unstyled, compare them with <https://getbootstrap.com/docs/5.3/getting-started/introduction/> (a wrong hash blocks the file; it never weakens security).
* Rate limiting is in-memory (resets on app restart, per server). Use a shared store if you scale out.
* CSP still allows inline scripts (WebForms needs them). Moving to nonces is the next hardening step.
* Admin sign-in is password + lockout only; adding e-mail OTP for admins is a small extension (`Admin_Login.Email` column exists).
* Users whose stored e-mail address is fake can't complete the e-mail second step. Fix their `EmailId`, or set `Security:TwoStepForPasswordLogin` to `false`.
* Dates are stored as text (`dd-MM-yyyy`) – the new procedures parse both `-` and `/`; migrating to `date` columns is recommended.
* The sample images in `/img` are large (11 MB total); compressing them will speed up the home page.
* iTextSharp 5 (already in your `/bin`) is AGPL-licensed; consider QuestPDF or a commercial iText licence for a commercial site.
* There is no booking cancellation/refund flow for confirmed tickets yet – the Razorpay refund call exists in `RazorpayService.Refund`.

## 6. For anyone cloning this repository

* **No secrets are in the repo.** `secrets.config`, build output and local publish profiles are git-ignored. Never commit real SMTP passwords, Razorpay keys or OAuth secrets; if one leaks, rotate it.
* The database scripts contain **no personal data and no default passwords** – create your own admin with `04_Create_Admin.sql`.
* `bin/itextsharp.dll` and `bin/itextsharp.xmlworker.dll` are committed on purpose (the project references them). iTextSharp 5 is **AGPL-licensed** – see its terms before using it commercially, or swap it for another PDF library.
* The business details on the *About us* page and `App:SupportEmail` in `Web.config` are sample values – replace them with your own.
* Use Razorpay **test** keys while developing. Go-live requires Razorpay KYC and HTTPS.
* Run `scripts/pre-push-check.ps1` before pushing: it looks for keys, e-mail addresses, phone numbers, local paths and stray log files in everything Git would commit.
#   m a h a r a j a - t r a v e l s - b u s - b o o k i n g - w e b s i t e  
 