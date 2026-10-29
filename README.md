# Maharaja Travels — Online Bus Booking System

A web-based bus reservation system built with **ASP.NET Web Forms, C#, .NET Framework 4.8, SQL Server, and Bootstrap 5.3.8**.

The application allows customers to search for buses, select seats, book tickets, make online payments, manage bookings, and download tickets. It also provides an administrative area for managing buses, routes, schedules, and booking reports.

> **Project status:** The code has been written and statically checked, but a complete build and runtime test with Visual Studio, SQL Server, Razorpay, and SMTP has not yet been completed. Some fixes may be necessary during the first build or test. Contributions and bug reports are welcome.

---

## Table of Contents

1. [Project Overview](#1-project-overview)
2. [Technology Stack](#2-technology-stack)
3. [Features](#3-features)
4. [System Requirements](#4-system-requirements)
5. [Project Structure](#5-project-structure)
6. [Installation and Setup](#6-installation-and-setup)
7. [Database Configuration](#7-database-configuration)
8. [Visual Studio Configuration](#8-visual-studio-configuration)
9. [Application Configuration and Secrets](#9-application-configuration-and-secrets)
10. [Running the Application](#10-running-the-application)
11. [How the Application Works](#11-how-the-application-works)
12. [Testing Online Payments](#12-testing-online-payments)
13. [Optional Integrations](#13-optional-integrations)
14. [Security Features](#14-security-features)
15. [Troubleshooting](#15-troubleshooting)
16. [Known Limitations](#16-known-limitations)
17. [License and Third-Party Components](#17-license-and-third-party-components)
18. [Contributing](#18-contributing)

---

## 1. Project Overview

Maharaja Travels is an online bus booking application developed using ASP.NET Web Forms.

It provides a customer-facing booking website and an administrative dashboard. Customers can search for available trips, select seats, enter passenger details, pay online, and access their booking information.

Administrators can manage the operational data required by the booking system and review booking-related reports.

The application also includes authentication, one-time passwords (OTPs), external sign-in providers, PDF ticket generation, and Razorpay payment integration.

### Main objectives

- Simplify the online bus reservation process.
- Display available buses and schedules.
- Allow customers to select seats and reserve them.
- Process payments through a payment gateway.
- Generate downloadable tickets and payment receipts.
- Provide administrative tools for managing bus operations.
- Improve the security of authentication, booking, and payment workflows.

---

## 2. Technology Stack

| Technology | Purpose |
|---|---|
| ASP.NET Web Forms | Server-side web application framework |
| C# | Application and business logic |
| .NET Framework 4.8 | Application runtime and framework |
| Microsoft SQL Server | Database and booking data storage |
| SQL Server Management Studio (SSMS) | Database setup and administration |
| ADO.NET | Database connectivity and queries |
| Bootstrap 5.3.8 | Responsive user interface |
| HTML, CSS, JavaScript | Front-end structure, styling, and interaction |
| Razorpay Standard Checkout | Online payment processing |
| SMTP | Email delivery |
| Google OAuth | Google sign-in |
| Microsoft identity platform | Microsoft account sign-in |
| Twilio | Optional SMS OTP delivery |
| iTextSharp 5 | PDF generation |
| Visual Studio 2022 | Development, building, debugging, and running |

---

## 3. Features

### 3.1 Customer features

- Search for buses and available schedules.
- View trip and fare information.
- Select seats using an interactive seat map.
- Enter passenger information.
- Hold selected seats temporarily during the booking process.
- Pay using supported Razorpay payment methods.
- View booking confirmations and booking history.
- Download tickets and payment receipts as PDFs.
- Receive ticket and receipt emails when email delivery is configured.
- Register and sign in using supported authentication methods.
- Verify accounts and perform password recovery using OTPs.
- Choose Light, Dark, or System theme.

### 3.2 Administrative features

- Administrator login.
- Manage bus information.
- Manage routes and boarding information.
- Manage schedules.
- Review bookings and booking reports.
- View bus and schedule reports.
- Review customer feedback.
- Access an administrative dashboard.

Administrator access is intended to be restricted to authenticated administrators.

### 3.3 Payment features

- Razorpay Standard Checkout integration.
- Server-side payment signature verification.
- Payment verification against the order, amount, and currency.
- Webhook handling for payment events.
- Booking confirmation after payment verification.
- Temporary seat reservations.
- Protection against double-booking through database constraints.
- Refund handling for the supported seat-loss scenario.

### 3.4 Authentication and security

- Salted PBKDF2-SHA256 password hashing.
- Password-strength checks.
- Account lockout and rate limiting.
- Email OTP verification.
- Optional SMS OTP delivery.
- Google and Microsoft sign-in.
- Password recovery.
- Session and ViewState security measures.
- Encoded output to help prevent cross-site scripting.
- HTTPS-related production configuration.

These features are described by the project implementation; their effectiveness still needs to be verified through build, integration, and security testing.

---

## 4. System Requirements

Install the following software before setting up the project.

### Required

1. **Windows** with a supported development environment.
2. **Visual Studio 2022** with the ASP.NET and web development workload.
3. **.NET Framework 4.8 Developer Pack**.
4. **Microsoft SQL Server 2016 SP1 or later**, with database compatibility level 130 or higher as required by the supplied scripts.
5. **SQL Server Management Studio (SSMS)**.
6. An internet connection for downloading dependencies and using external integrations.

### Optional

- A Razorpay account for payment testing.
- An SMTP account or relay for sending emails.
- Google Cloud credentials for Google sign-in.
- Microsoft Entra ID credentials for Microsoft sign-in.
- Twilio credentials for SMS OTP delivery.
- ngrok for receiving payment webhooks during local development.

**Important:** A working SQL Server database is required for the application's database-backed functionality. Payment, email, SMS, and external sign-in features additionally require their respective configuration.

---

## 5. Project Structure

The repository is organized as follows:

```text
MaharajaTravels/
│
├── code/
│   └── BusBookingProject/
│       ├── BusBookingProject.sln
│       │
│       └── BusBookingProject/
│           ├── Admin/
│           ├── App_Data/
│           ├── Security/
│           ├── Services/
│           ├── Properties/
│           ├── bin/
│           ├── css/
│           ├── fontawesome/
│           ├── img/
│           ├── js/
│           │
│           ├── Default.aspx
│           ├── Home.aspx
│           ├── Login.aspx
│           ├── UserRegistration.aspx
│           ├── BusBookingSearchDetails.aspx
│           ├── SeatDetails.aspx
│           ├── PassengerDetailsInfo.aspx
│           ├── Payment.aspx
│           ├── BookingConfirmation.aspx
│           ├── Web.config
│           └── secrets.config.example
│
├── database/
│   ├── 01_Schema_And_Reference_Data.sql
│   ├── 02_Upgrade_Security_Payments.sql
│   ├── 03_Optional_RefreshDemoSchedules.sql
│   └── 04_Create_Admin.sql
│
├── docs/
│   └── TESTING.md
│
├── scripts/
│   └── pre-push-check.ps1
│
├── .gitattributes
├── .gitignore
└── README.md
```

### Important folders

- **Admin/** — administrative pages and reports.
- **Security/** — authentication, validation, password hashing, and related security components.
- **Services/** — database access and application services, including booking, email, OTP, OAuth, PDF, SMS, and Razorpay functionality.
- **App_Data/** — local application data, including development-only email output.
- **css/** — application stylesheets.
- **js/** — front-end scripts, including seat selection and payment-related functionality.
- **img/** — images used by the website.
- **database/** — SQL scripts for database creation, upgrades, sample data, and administrator setup.
- **docs/** — testing documentation.
- **scripts/** — development and repository safety checks.

---

## 6. Installation and Setup

Follow these steps in order to run the project locally.

### Step 1: Clone the repository

Open PowerShell, Command Prompt, or Git Bash.

Run:

```bash
git clone https://github.com/ChetanLokare/maharaja-travels-bus-booking-website.git
```

Move into the project directory:

```bash
cd maharaja-travels-bus-booking-website
```

If you already have the repository, open your existing local copy instead of cloning it again.

### Step 2: Install Visual Studio components

Open Visual Studio Installer and modify your Visual Studio 2022 installation.

Select the **ASP.NET and web development** workload.

Ensure that the .NET Framework 4.8 development tools are installed. If the required targeting pack is missing, install the .NET Framework 4.8 Developer Pack.

Restart Visual Studio if the installer requests it.

### Step 3: Install and start SQL Server

Install or use an existing SQL Server instance.

Examples of local instance names include:

- `localhost`
- `.\SQLEXPRESS`
- `(localdb)\MSSQLLocalDB`

The correct name depends on which SQL Server instance is installed.

Open SSMS and connect to your instance.

Make sure the SQL Server service is running and that your Windows account or SQL login has permission to create and modify the required database.

### Step 4: Create the database

Follow [Database Configuration](#7-database-configuration) to execute the SQL scripts in the correct order.

Do not continue until the database has been created successfully.

### Step 5: Open the Visual Studio solution

Navigate to:

```text
code/BusBookingProject/BusBookingProject.sln
```

Open `BusBookingProject.sln` in Visual Studio 2022.

Wait for the solution to load and for Visual Studio to finish any dependency or project-loading operations.

If Visual Studio reports a missing framework targeting pack or an unavailable dependency, resolve that issue before building.

### Step 6: Configure the application

Open:

```text
code/BusBookingProject/BusBookingProject/Web.config
```

Configure the database connection string for your local SQL Server instance.

Next, copy `secrets.config.example` to `secrets.config` in the same directory as `Web.config`.

Fill in only the credentials for the integrations you intend to use.

See [Application Configuration and Secrets](#9-application-configuration-and-secrets).

### Step 7: Build the solution

In Visual Studio:

1. Select **Build → Restore NuGet Packages**, if applicable.
2. Select **Build → Rebuild Solution**.
3. Review the Error List and Output windows.
4. Fix any compilation or configuration errors reported by your environment.

The project has not yet been confirmed to build successfully on every machine. If you encounter an error, record the exact message and the file and line number involved.

### Step 8: Run the application

Set the web project as the startup project if it is not already selected.

Choose the appropriate startup page or configured web launch target.

Press **F5** to run with the debugger, or **Ctrl+F5** to run without debugging.

Visual Studio should launch the site in a browser using the configured local web server.

Use the local address shown by Visual Studio. Do not assume that the application always uses the same port.

---

## 7. Database Configuration

The SQL scripts are located in the repository's `database/` directory.

Open SSMS, connect to the SQL Server instance, and execute the scripts in the following order.

### Script 1: Create the database and initial data

File:

```text
database/01_Schema_And_Reference_Data.sql
```

This script creates the `OnlineBusBooking` database and the initial schema and reference data, including cities, buses, routes, and sample schedules.

**Existing database warning:** If you are upgrading an existing database from the original project, do not run this initial-creation script blindly. Back up the existing database first and determine which upgrade steps apply.

### Script 2: Upgrade security and payments

File:

```text
database/02_Upgrade_Security_Payments.sql
```

This script adds the security and payment-related database structures, including OTP, payment, and seat-hold functionality.

The supplied documentation describes this script as safe to rerun. Nevertheless, test database changes in a development environment and maintain backups.

### Script 3: Refresh demonstration schedules

File:

```text
database/03_Optional_RefreshDemoSchedules.sql
```

This script is optional and intended for demonstration purposes.

The original sample trips use dates from 2024. This script moves demonstration schedules into the next few days so that they can appear in searches that exclude past trips.

Do not use demonstration schedule refreshes as a substitute for proper schedule management in a production system.

### Script 4: Create the administrator account

File:

```text
database/04_Create_Admin.sql
```

Before running the script:

1. Open the SQL file.
2. Read the three variables at the top.
3. Replace the example values with the intended administrator details and a strong, unique initial password.
4. Execute the script against the correct database.
5. Confirm that the administrator account was created.

Use your own credentials. Never commit a real administrator password to Git.

The project instructions specify that a new password must be set at first sign-in.

### Verify the database

After running the scripts, confirm that:

- `OnlineBusBooking` exists.
- The expected tables and stored procedures are present.
- The sample reference data exists if the initial-creation script was used.
- The administrator account exists.
- The application database user has the permissions required by the application.

If a script fails, inspect the error before proceeding to the next script.

---

## 8. Visual Studio Configuration

### Configure the connection string

Open the application's `Web.config` file.

Locate the database connection string and replace the server or instance name with the one configured on your machine.

For example, a SQL Server Express connection may use an instance such as:

```text
.\SQLEXPRESS
```

A local default instance may use:

```text
localhost
```

Use the actual connection-string name, authentication mode, and database name expected by the project.

The examples above identify possible SQL Server instance names; they are not complete connection strings. Do not replace the application's existing configuration blindly.

If using SQL authentication, protect the username and password. Prefer an appropriately configured, least-privileged database account for production deployments.

### Select the startup project

In Solution Explorer:

1. Locate the web application project.
2. Right-click it.
3. Select **Set as Startup Project**.

### Build and run

Use:

- **Build → Rebuild Solution** to compile the application.
- **F5** to run with debugging.
- **Ctrl+F5** to run without debugging.

Check Visual Studio's **Error List** and **Output** windows if the project does not build or launch.

---

## 9. Application Configuration and Secrets

The application uses `Web.config` for general ASP.NET configuration and `secrets.config` for integration credentials.

The example file is:

```text
code/BusBookingProject/BusBookingProject/secrets.config.example
```

Copy it to:

```text
code/BusBookingProject/BusBookingProject/secrets.config
```

Keep the same filename and location expected by the application's configuration.

Open `secrets.config` and fill in the settings for the integrations you want to enable. Preserve the configuration structure expected by the project.

### Configuration categories

| Setting category | Purpose |
|---|---|
| Database connection string | Connect the application to SQL Server |
| `Razorpay:*` | Payment gateway credentials and webhook secret |
| `Smtp:*` | Outgoing email delivery |
| `Google:*` | Google OAuth client configuration |
| `Microsoft:*` | Microsoft sign-in configuration |
| `Sms:*` | Optional SMS provider configuration |
| `App:BaseUrl` | Base URL used in integration redirects |
| `Security:OtpPepper` | Additional secret used by OTP security logic |
| `Security:TwoStepForPasswordLogin` | Controls the documented optional email second step |

The exact setting names and supported values must match the application's configuration and code.

### Local development

You can browse the site without configuring every external integration, provided the database and required application settings are correct.

For example:

- Email delivery can use the development outbox when SMTP is empty and the application is running on localhost.
- Razorpay payments require appropriate gateway credentials.
- Google and Microsoft sign-in require their respective OAuth applications.
- SMS delivery requires a configured provider.

Never commit `secrets.config`, real API keys, database passwords, OAuth secrets, or SMTP app passwords.

---

## 10. Running the Application

Once the database and configuration are ready:

1. Open the solution in Visual Studio 2022.
2. Rebuild the solution.
3. Start the application using F5.
4. Open the local URL launched by Visual Studio.
5. Test customer registration and sign-in.
6. Search for a bus and select a schedule.
7. Select available seats.
8. Enter the passenger information.
9. Test the booking workflow.
10. Configure Razorpay test credentials before testing online payments.
11. Verify the booking status and generated ticket after a successful test payment.

Some features depend on external credentials and services. A page loading successfully does not, by itself, confirm that email, OAuth, SMS, payment verification, or webhook processing is working.

### Development email outbox

When running locally with SMTP unconfigured, the application is designed to write development email/OTP messages to:

```text
code/BusBookingProject/BusBookingProject/App_Data/dev-outbox.log
```

Open the file in a text editor to inspect the messages.

In Visual Studio, you may also inspect **View → Output** and select the Debug output source while debugging.

Treat the outbox as sensitive because it can contain email addresses and OTP codes. Do not commit it or expose it through a public web server.

---

## 11. How the Application Works

The application consists of a browser-based front end, ASP.NET Web Forms pages, C# application services, a SQL Server database, and optional external services.

### 11.1 High-level architecture

```text
Customer / Administrator
          |
          v
    Web Browser
 HTML / CSS / JavaScript
       Bootstrap
          |
          v
 ASP.NET Web Forms
       (.NET 4.8)
          |
          v
      C# Services
   /       |        \
  v        v         v
SQL Server  Payment  External
Database    Gateway  Integrations
              |      - SMTP
              |      - Google OAuth
              |      - Microsoft OAuth
              |      - SMS provider
              v
       Payment Webhook
              |
              v
    Booking/Payment Update
```

This diagram describes the major components and their intended relationships. Exact execution paths depend on the implementation of the individual pages, services, and database procedures.

### 11.2 Bus search

The customer searches for a trip using the available search fields.

The application queries the database for matching routes and schedules, then displays the results.

Customers can inspect the available trip information before continuing to seat selection.

### 11.3 Seat selection and temporary holds

The seat map displays seat availability and lets the customer select seats.

The application uses a temporary seat-hold mechanism intended to prevent another customer from booking the same seat while the first customer completes the booking.

The documented hold period is ten minutes.

A database uniqueness constraint provides additional protection against double-booking. The actual behavior should be verified with concurrent-booking tests.

### 11.4 Passenger details

After selecting seats, the customer provides the required passenger information.

The application associates the passenger information with the relevant schedule and booking.

The server must validate the information and derive the fare from trusted database data rather than accepting a customer-supplied price.

### 11.5 Payment processing

For an online payment, the intended workflow is:

1. The server creates the payment order.
2. The browser opens Razorpay Checkout.
3. The customer completes the payment interaction.
4. The server verifies the payment signature.
5. The server retrieves and validates the payment details, including the order, amount, and currency.
6. The application updates the booking and payment records.
7. A payment webhook can handle the payment event if the customer closes the browser before returning to the confirmation page.

Payment status must be verified server-side. A browser redirect or client-side success message alone must never be treated as proof of payment.

### 11.6 Booking confirmation and tickets

After a valid payment is confirmed, the application updates the booking status and makes the confirmation available to the customer.

The project includes functionality for generating ticket and receipt PDFs, including a ticket QR code.

When email delivery is configured, the ticket and receipt can be emailed to the customer. The ticket is also intended to be downloadable from the customer's bookings area, subject to ownership checks.

### 11.7 Customer authentication

The application supports password-based authentication and additional sign-in methods.

The documented options include:

- Password sign-in with an email OTP second step.
- One-time codes delivered by email or, when configured, SMS.
- Google sign-in.
- Microsoft sign-in.
- OTP-based password recovery.

Registration is designed to verify the email address before creating the account.

The project also includes password hashing, account lockout, rate limiting, and other security controls.

### 11.8 Administration

Administrators use the administrative area to manage operational information and review reports.

The included pages cover bus details, routes, boarding information, schedules, bookings, and customer feedback.

Administrative access is intended to be protected by authentication and authorization checks.

---

## 12. Testing Online Payments

**Always start with Razorpay Test Mode. Do not use live credentials for development testing.**

### Step 1: Obtain test credentials

Create or sign in to a Razorpay account and obtain the API credentials for Test Mode.

Configure the corresponding `Razorpay:KeyId` and `Razorpay:KeySecret` values in your local `secrets.config`.

Do not commit these values.

### Step 2: Configure the webhook

The webhook endpoint is:

```text
https://YOUR-SITE/PaymentWebhook.ashx
```

The documented event types are:

- `payment.captured`
- `payment.failed`

Configure the webhook secret to match the `Razorpay:WebhookSecret` value in your local configuration.

Use an HTTPS endpoint accessible to Razorpay.

### Step 3: Test from localhost

Razorpay cannot normally reach a webhook endpoint that is accessible only on your own computer. A temporary HTTPS tunnel can expose your local development server for testing.

If ngrok is installed and authenticated, a command for a local application listening on port 1778 is:

```bash
ngrok http 1778 --host-header="localhost:1778"
```

Use the actual port displayed by Visual Studio.

Register the HTTPS forwarding address provided by ngrok, followed by `/PaymentWebhook.ashx`, in the Razorpay webhook configuration.

For example, the URL pattern is:

```text
https://YOUR-NGROK-HOST/PaymentWebhook.ashx
```

The hostname above is a placeholder. Use the address generated by your own ngrok session.

Do not publish the tunnel URL as a permanent production endpoint. Stop the tunnel when testing is complete.

### Step 4: Test payment scenarios

The supplied project notes describe these Test Mode scenarios:

- Use `success@razorpay` for a simulated successful UPI payment.
- Use `failure@razorpay` for a simulated failed UPI payment.
- Use the mock net-banking flow to test success and failure.
- Use Razorpay's published test card details for card testing.

Follow the current Razorpay Test Mode documentation for supported methods and test values.

Real UPI applications cannot pay a Test Mode QR code. QR and intent payment behavior depends on the gateway's current live-mode support.

### Step 5: Verify the result

For each test, verify the following:

- The browser receives the expected response.
- The payment status is verified by the server.
- The booking is confirmed only after successful payment verification.
- Failed payments do not incorrectly confirm bookings.
- Seat holds and availability are handled correctly.
- Webhook events are processed safely, including repeated event deliveries.
- Tickets and receipts are generated when appropriate.

See [`docs/TESTING.md`](docs/TESTING.md) for the project's fuller manual test plan.

---

## 13. Optional Integrations

### Email

Configure the `Smtp:*` settings in `secrets.config`.

For Gmail, use an App Password where supported rather than your regular account password. Other SMTP relays can also be used.

Test registration verification, login OTPs, password recovery, sign-in notifications, and ticket delivery.

### Google sign-in

Create a web OAuth client in Google Cloud Console.

Configure the authorized redirect URI as:

```text
{App:BaseUrl}/OAuthCallback.ashx
```

Replace the expression with the actual base URL configured for your application.

Add the resulting client ID and client secret to the corresponding Google settings.

### Microsoft sign-in

Create an application registration in Microsoft Entra ID.

Configure the supported account types and a Web redirect URI matching:

```text
{App:BaseUrl}/OAuthCallback.ashx
```

Create a client secret and configure the Microsoft settings in `secrets.config`.

The redirect URI configured with the identity provider must match the URI used by the application.

### SMS OTP

Configure:

```text
Sms:Provider=Twilio
```

Add the required Twilio credentials and other provider settings.

SMS delivery in India may require applicable DLT registration and template compliance.

If SMS is not configured, the project documentation describes email delivery as the fallback for the mobile login flow.

### Production deployment

Before deploying publicly:

- Configure a real HTTPS base URL.
- Use production secrets stored outside source control.
- Publish a Release build.
- Apply the appropriate production configuration transforms.
- Give the application pool only the file permissions it requires.
- Ensure the application can write to `App_Data` if required by the implementation.
- Set a long, random `Security:OtpPepper`.
- Configure and verify the payment webhook.
- Test all authentication and payment flows in the deployed environment.
- Review logging, monitoring, backups, access control, and data protection.

Do not deploy with test credentials or expose development logs and diagnostic files.

---

## 14. Security Features

The project includes changes intended to address several common web application vulnerabilities.

| Area | Documented improvement |
|---|---|
| SQL injection | Parameterized database queries |
| Password storage | Salted PBKDF2-SHA256 hashing |
| Legacy passwords | Upgrade to a salted hash on successful sign-in |
| Card data | Card processing delegated to the payment gateway |
| Fare manipulation | Server-side fare calculation |
| Double-booking | Seat holds and database uniqueness constraints |
| Authentication | Lockout, rate limiting, and uniform error messages |
| Cross-site scripting | Encoded output for applicable values |
| CSRF and ViewState | Session-bound ViewState user key, MAC, and encryption |
| Open redirects | Local-only return URLs |
| Error handling | Custom error pages and reference-based logging |
| Browser security | HTTPS-related settings and security headers |
| Credentials | Local secrets configuration excluded from source control |

These are implementation claims, not a substitute for an independent security audit.

Before accepting real customers or payments, test the authentication flows, authorization boundaries, database permissions, payment verification, webhook validation, session handling, and deployment configuration.

---

## 15. Troubleshooting

### Problem: Visual Studio cannot load the project

**Possible causes:**
- The ASP.NET and web development workload is missing.
- The .NET Framework 4.8 Developer Pack is not installed.
- A project dependency is unavailable.

**What to do:**
1. Open Visual Studio Installer.
2. Verify the required workload and targeting pack.
3. Reopen the solution.
4. Inspect the Error List for the exact failure.

### Problem: SQL Server connection fails

**Possible causes:**
- The SQL Server service is stopped.
- The instance name is incorrect.
- The database has not been created.
- The login lacks permission.
- The connection string uses the wrong authentication method.

**What to do:**
1. Connect to the same instance through SSMS.
2. Verify that `OnlineBusBooking` exists.
3. Check the connection string in `Web.config`.
4. Verify the credentials and database permissions.

### Problem: No buses appear in search

**Possible causes:**
- No matching schedules exist.
- The schedules have past dates.
- The route or city data is missing.
- The search criteria do not match the sample data.

**What to do:**
1. Verify the reference data in SQL Server.
2. Check the dates of the schedules.
3. If appropriate for a demo environment, run `03_Optional_RefreshDemoSchedules.sql`.
4. Repeat the search with matching cities and dates.

### Problem: Emails or OTPs are not received

**What to do:**
1. Verify the `Smtp:*` settings.
2. Confirm that the SMTP provider allows the selected authentication method.
3. Check the development outbox when running locally without SMTP.
4. Review the application logs for the associated error reference.

Do not expose outbox files or OTPs publicly.

### Problem: Razorpay payments fail

**What to do:**
1. Verify that the configured keys belong to Test Mode.
2. Check that the checkout configuration uses the corresponding key.
3. Inspect browser and server-side errors.
4. Confirm that the payment order and amount match the expected booking.
5. Verify the webhook URL and secret.
6. Check webhook delivery and application logs.

### Problem: The payment webhook does not arrive

**What to do:**
1. Confirm that the local web server is running.
2. Check the port used by Visual Studio.
3. Verify that the HTTPS tunnel is active.
4. Register the correct tunnel URL ending in `/PaymentWebhook.ashx`.
5. Check the webhook event configuration and delivery logs.

### Problem: The website appears unstyled

Bootstrap is loaded from a CDN with Subresource Integrity (SRI).

If Bootstrap fails to load, check browser developer tools for a blocked resource or SRI error. Compare the configured stylesheet URL and integrity hash with the official [Bootstrap 5.3 introduction documentation](https://getbootstrap.com/docs/5.3/getting-started/introduction/).

### Problem: The administrator cannot sign in

**What to do:**
1. Confirm that `04_Create_Admin.sql` ran successfully against the correct database.
2. Verify the administrator record.
3. Check the initial-password and first-sign-in requirements.
4. Inspect the application logs for authentication errors.

### Problem: Build errors appear after opening the solution

Because the application has not been confirmed to compile in a clean environment, an initial build may reveal issues.

Record the exact error message, file, and line number. Resolve compilation errors before investigating runtime behavior.

---

## 16. Known Limitations

- A complete build and live integration test has not yet been confirmed.
- Rate limiting is in memory and resets when the application restarts. A distributed store is recommended for multi-server deployments.
- The Content Security Policy still allows inline scripts because of Web Forms requirements. A nonce-based strategy would be a possible hardening improvement.
- Administrator sign-in currently uses a password and lockout; an additional email OTP step for administrators is a potential improvement.
- Users with invalid stored email addresses may not be able to complete email-based second-step authentication.
- Dates are stored as text in `dd-MM-yyyy` format. Migration to SQL Server `date` columns is recommended.
- The sample images occupy approximately 11 MB. Image compression may improve page load times.
- A full cancellation and refund workflow for confirmed bookings is not yet implemented, although a refund operation exists in the Razorpay service.
- Payment webhooks, authentication, and concurrent booking behavior require integration testing before production use.

---

## 17. License and Third-Party Components

This project includes `itextsharp.dll` and `itextsharp.xmlworker.dll` because they are referenced by the application.

**iTextSharp 5 is licensed under the GNU Affero General Public License (AGPL).** Review the applicable license terms before using or distributing the application commercially. Depending on your use case, a commercial license or an alternative PDF library may be appropriate.

The application also includes Bootstrap, Font Awesome, and other dependencies. Review the relevant licenses and attribution requirements before redistribution or commercial deployment.

---

## 18. Contributing

Contributions, bug reports, and suggestions are welcome.

Before submitting a change:

1. Build the solution.
2. Test the affected functionality.
3. Run the relevant SQL scripts in a development database if needed.
4. Never include real credentials or personal data.
5. Run the repository pre-push check:

   ```powershell
   powershell -ExecutionPolicy Bypass -File "scripts/pre-push-check.ps1"
   ```

6. Review the staged files before committing.
7. Document any setup changes or new configuration settings.

When reporting an issue, include the steps to reproduce it, the expected behavior, the actual behavior, and the relevant error message. Remove passwords, API keys, OTPs, and personal information from logs before sharing them.

---

## Final Notes

- Use a development database and Razorpay Test Mode while learning and testing.
- Back up existing databases before running upgrade scripts.
- Keep all real credentials out of source control.
- Do not assume that a successful build proves that payments and authentication are secure.
- Complete build, integration, and security testing before deploying to production.

**Repository:** [Maharaja Travels — Bus Booking Website](https://github.com/ChetanLokare/maharaja-travels-bus-booking-website)
