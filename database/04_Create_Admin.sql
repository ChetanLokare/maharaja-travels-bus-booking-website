/* Creates (or resets) an administrator account.
   1. Change @AdminId, @DisplayName and @TempPassword below  (do NOT keep the example values).
   2. Run after 02_Upgrade_Security_Payments.sql.
   The temporary password is converted to a salted hash the first time you sign in, and you are forced to
   choose a new password immediately (MustChangePassword = 1). Never commit your edited copy. */
USE [OnlineBusBooking];
GO
DECLARE @AdminId       nvarchar(50)  = N'admin';
DECLARE @DisplayName   nvarchar(50)  = N'Administrator';
DECLARE @TempPassword  nvarchar(50)  = N'CHANGE-ME-BEFORE-RUNNING';

IF @TempPassword = N'CHANGE-ME-BEFORE-RUNNING'
BEGIN
    RAISERROR('Edit @TempPassword (and @AdminId) before running this script.', 16, 1);
    RETURN;
END

IF EXISTS (SELECT 1 FROM dbo.Admin_Login WHERE Admin_ID = @AdminId)
    UPDATE dbo.Admin_Login
    SET UserName = @DisplayName, [Password] = @TempPassword, PasswordHash = NULL,
        MustChangePassword = 1, FailedAttempts = 0, LockoutEnd = NULL
    WHERE Admin_ID = @AdminId;
ELSE
    INSERT INTO dbo.Admin_Login(Admin_ID, UserName, [Password], MustChangePassword)
    VALUES(@AdminId, @DisplayName, @TempPassword, 1);
PRINT 'Admin account ready. Sign in at /Admin/AdminLogin.aspx and set a new password.';
GO
