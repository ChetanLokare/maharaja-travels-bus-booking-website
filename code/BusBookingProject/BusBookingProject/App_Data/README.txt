This folder is private (IIS blocks direct requests to App_Data).
The application writes here:
  errors.log      - unexpected errors with a reference id (shown to users as "ref")
  dev-outbox.log  - on a developer PC with no SMTP configured, e-mails/OTP codes are written here instead of being sent
Make sure the IIS application-pool identity has write permission on this folder when you deploy.
