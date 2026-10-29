# Manual test plan

Run the site (`F5`) with Razorpay **Test Mode** keys. With SMTP unset, one-time codes appear in a yellow *Developer mode* box on the code page (localhost only) and e-mails are written to `App_Data/dev-outbox.log`.

## 1. Smoke
- [ ] Home page loads; hero animation, counters, carousel work.
- [ ] Theme switch (Light / Dark / System default) works and survives a refresh.
- [ ] Phone-width layout (browser dev tools) is usable.

## 2. Accounts
- [ ] Sign up → e-mail code → signed in automatically (welcome + sign-in e-mails in the outbox).
- [ ] Wrong / expired code is rejected; 5 wrong tries block the code; resend waits 60 s.
- [ ] Password login asks for an e-mail code (second step); *One-time code* tab works without a password.
- [ ] Forgot password → code → new password → login works.
- [ ] 5 wrong passwords lock the account for 15 minutes.
- [ ] Weak passwords and duplicate e-mail/mobile are rejected with a visible message.

## 3. Booking and payment
- [ ] Search finds a trip (run `database/03_Optional_RefreshDemoSchedules.sql` for demo dates).
- [ ] Seat map: free / selected / held / booked states; limit of 3 seats; boarding point required.
- [ ] Passenger page shows a hold countdown; fare equals server price × seats.
- [ ] Pay with UPI `success@razorpay` → *Booking confirmed*; `failure@razorpay` → error, seats stay held.
- [ ] Ticket and receipt PDFs download; both are listed as attachments in the outbox e-mail.
- [ ] Close the tab right after paying → booking is still confirmed via the webhook.
- [ ] Two browsers cannot hold the same seat; cancelling releases it.

## 4. Admin
- [ ] Admin sign-in forces a password change (create the account with `database/04_Create_Admin.sql`).
- [ ] Dashboard and all report pages load; anonymous or customer access to `/Admin/*` is refused.

## 5. Security spot-checks
- [ ] `' OR 1=1 --` as admin ID does not log in.
- [ ] `<script>alert(1)</script>` in feedback is shown as text, never executed.
- [ ] `Download.ashx?pnr=<someone else's PNR>&type=ticket` returns 404.
- [ ] Response headers include `Content-Security-Policy`, `X-Frame-Options`, `X-Content-Type-Options`.
