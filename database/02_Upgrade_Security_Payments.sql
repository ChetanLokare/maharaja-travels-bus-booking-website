/* =====================================================================
   Maharaja Travel's - database upgrade (v2)
   Run ONCE against the existing [OnlineBusBooking] database, after taking a backup.
   Requires SQL Server 2016 SP1+ (CREATE OR ALTER, STRING_SPLIT, TRY_CONVERT)
   and database compatibility level >= 130.
   The script is re-runnable (every step checks before it changes anything).
   ===================================================================== */
USE [OnlineBusBooking];
GO

/* ---------- 1. Registration (customer accounts) ---------- */
IF COL_LENGTH('dbo.Registration','PasswordHash')     IS NULL ALTER TABLE dbo.Registration ADD PasswordHash nvarchar(256) NULL;
IF COL_LENGTH('dbo.Registration','EmailVerified')    IS NULL ALTER TABLE dbo.Registration ADD EmailVerified bit NOT NULL CONSTRAINT DF_Reg_EmailVerified DEFAULT (0);
IF COL_LENGTH('dbo.Registration','MobileVerified')   IS NULL ALTER TABLE dbo.Registration ADD MobileVerified bit NOT NULL CONSTRAINT DF_Reg_MobileVerified DEFAULT (0);
IF COL_LENGTH('dbo.Registration','IsActive')         IS NULL ALTER TABLE dbo.Registration ADD IsActive bit NOT NULL CONSTRAINT DF_Reg_IsActive DEFAULT (1);
IF COL_LENGTH('dbo.Registration','FailedAttempts')   IS NULL ALTER TABLE dbo.Registration ADD FailedAttempts int NOT NULL CONSTRAINT DF_Reg_FailedAttempts DEFAULT (0);
IF COL_LENGTH('dbo.Registration','LockoutEnd')       IS NULL ALTER TABLE dbo.Registration ADD LockoutEnd datetime NULL;
IF COL_LENGTH('dbo.Registration','LastLoginAt')      IS NULL ALTER TABLE dbo.Registration ADD LastLoginAt datetime NULL;
IF COL_LENGTH('dbo.Registration','LastLoginIp')      IS NULL ALTER TABLE dbo.Registration ADD LastLoginIp varchar(45) NULL;
IF COL_LENGTH('dbo.Registration','PasswordChangedAt') IS NULL ALTER TABLE dbo.Registration ADD PasswordChangedAt datetime NULL;
GO
-- e-mail addresses were limited to 50 characters; the hash column replaces the plain-text Password column
ALTER TABLE dbo.Registration ALTER COLUMN EmailId varchar(150) NULL;
ALTER TABLE dbo.Registration ALTER COLUMN Address varchar(200) NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_Registration_Email' AND object_id = OBJECT_ID('dbo.Registration'))
BEGIN
    IF EXISTS (SELECT 1 FROM dbo.Registration WHERE EmailId IS NOT NULL GROUP BY EmailId HAVING COUNT(*) > 1)
        PRINT 'WARNING: duplicate e-mail addresses exist in dbo.Registration - clean them up, then re-run to create UX_Registration_Email.';
    ELSE
        CREATE UNIQUE INDEX UX_Registration_Email ON dbo.Registration(EmailId) WHERE EmailId IS NOT NULL;
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_Registration_Contact' AND object_id = OBJECT_ID('dbo.Registration'))
BEGIN
    IF EXISTS (SELECT 1 FROM dbo.Registration WHERE Contact IS NOT NULL GROUP BY Contact HAVING COUNT(*) > 1)
        PRINT 'WARNING: duplicate mobile numbers exist in dbo.Registration - clean them up, then re-run to create UX_Registration_Contact.';
    ELSE
        CREATE UNIQUE INDEX UX_Registration_Contact ON dbo.Registration(Contact) WHERE Contact IS NOT NULL;
END
GO

/* ---------- 2. Admin_Login ---------- */
IF COL_LENGTH('dbo.Admin_Login','PasswordHash')       IS NULL ALTER TABLE dbo.Admin_Login ADD PasswordHash nvarchar(256) NULL;
IF COL_LENGTH('dbo.Admin_Login','Email')              IS NULL ALTER TABLE dbo.Admin_Login ADD Email nvarchar(150) NULL;
IF COL_LENGTH('dbo.Admin_Login','FailedAttempts')     IS NULL ALTER TABLE dbo.Admin_Login ADD FailedAttempts int NOT NULL CONSTRAINT DF_Admin_FailedAttempts DEFAULT (0);
IF COL_LENGTH('dbo.Admin_Login','LockoutEnd')         IS NULL ALTER TABLE dbo.Admin_Login ADD LockoutEnd datetime NULL;
IF COL_LENGTH('dbo.Admin_Login','MustChangePassword') IS NULL ALTER TABLE dbo.Admin_Login ADD MustChangePassword bit NOT NULL CONSTRAINT DF_Admin_MustChange DEFAULT (1);
IF COL_LENGTH('dbo.Admin_Login','LastLoginAt')        IS NULL ALTER TABLE dbo.Admin_Login ADD LastLoginAt datetime NULL;
GO

/* ---------- 3. Bookings: status, passenger details, de-duplication guarantees ---------- */
IF COL_LENGTH('dbo.BookingMaster','Status')    IS NULL ALTER TABLE dbo.BookingMaster ADD Status varchar(20) NOT NULL CONSTRAINT DF_Booking_Status DEFAULT ('Confirmed');
IF COL_LENGTH('dbo.BookingMaster','Age')       IS NULL ALTER TABLE dbo.BookingMaster ADD Age tinyint NULL;
IF COL_LENGTH('dbo.BookingMaster','Gender')    IS NULL ALTER TABLE dbo.BookingMaster ADD Gender char(1) NULL;
IF COL_LENGTH('dbo.BookingMaster','CreatedAt') IS NULL ALTER TABLE dbo.BookingMaster ADD CreatedAt datetime NULL CONSTRAINT DF_Booking_CreatedAt DEFAULT (GETDATE());

IF COL_LENGTH('dbo.PNRDetails','Status')        IS NULL ALTER TABLE dbo.PNRDetails ADD Status varchar(20) NOT NULL CONSTRAINT DF_PNR_Status DEFAULT ('Confirmed');
IF COL_LENGTH('dbo.PNRDetails','ScheduleId')    IS NULL ALTER TABLE dbo.PNRDetails ADD ScheduleId int NULL;
IF COL_LENGTH('dbo.PNRDetails','BoardingId')    IS NULL ALTER TABLE dbo.PNRDetails ADD BoardingId int NULL;
IF COL_LENGTH('dbo.PNRDetails','ContactEmail')  IS NULL ALTER TABLE dbo.PNRDetails ADD ContactEmail varchar(150) NULL;
IF COL_LENGTH('dbo.PNRDetails','ContactMobile') IS NULL ALTER TABLE dbo.PNRDetails ADD ContactMobile varchar(20) NULL;
IF COL_LENGTH('dbo.PNRDetails','HoldExpiresAt') IS NULL ALTER TABLE dbo.PNRDetails ADD HoldExpiresAt datetime NULL;
IF COL_LENGTH('dbo.PNRDetails','CreatedAt')     IS NULL ALTER TABLE dbo.PNRDetails ADD CreatedAt datetime NULL CONSTRAINT DF_PNR_CreatedAt DEFAULT (GETDATE());
IF COL_LENGTH('dbo.PNRDetails','ConfirmedAt')   IS NULL ALTER TABLE dbo.PNRDetails ADD ConfirmedAt datetime NULL;
GO
ALTER TABLE dbo.BookingMaster ALTER COLUMN Email varchar(150) NULL;
GO
-- the same seat on the same trip can never be confirmed twice, whatever the application does
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_Booking_Seat' AND object_id = OBJECT_ID('dbo.BookingMaster'))
BEGIN
    IF EXISTS (SELECT 1 FROM dbo.BookingMaster WHERE Status = 'Confirmed' AND ScheduleID IS NOT NULL GROUP BY ScheduleID, SeatNo HAVING COUNT(*) > 1)
        PRINT 'WARNING: duplicate confirmed seats exist in dbo.BookingMaster - fix them, then re-run to create UX_Booking_Seat.';
    ELSE
        CREATE UNIQUE INDEX UX_Booking_Seat ON dbo.BookingMaster(ScheduleID, SeatNo) WHERE Status = 'Confirmed' AND ScheduleID IS NOT NULL;
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_PNR_PNRNo' AND object_id = OBJECT_ID('dbo.PNRDetails'))
BEGIN
    IF EXISTS (SELECT 1 FROM dbo.PNRDetails GROUP BY PNRNo HAVING COUNT(*) > 1)
        PRINT 'WARNING: duplicate PNR numbers exist in dbo.PNRDetails - fix them, then re-run to create UX_PNR_PNRNo.';
    ELSE
        CREATE UNIQUE INDEX UX_PNR_PNRNo ON dbo.PNRDetails(PNRNo);
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Booking_Schedule' AND object_id = OBJECT_ID('dbo.BookingMaster'))
    CREATE INDEX IX_Booking_Schedule ON dbo.BookingMaster(ScheduleID, Status) INCLUDE (SeatNo, PNRNo, RegId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Booking_PNR' AND object_id = OBJECT_ID('dbo.BookingMaster'))
    CREATE INDEX IX_Booking_PNR ON dbo.BookingMaster(PNRNo);
GO

/* ---------- 4. New tables ---------- */
IF OBJECT_ID('dbo.SeatHolds') IS NULL
BEGIN
    CREATE TABLE dbo.SeatHolds(
        HoldId     bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_SeatHolds PRIMARY KEY,
        ScheduleId int NOT NULL,
        SeatNo     int NOT NULL,
        UserId     int NOT NULL,
        PNRNo      varchar(50) NULL,
        ExpiresAt  datetime NOT NULL,
        CreatedAt  datetime NOT NULL CONSTRAINT DF_SeatHolds_Created DEFAULT (GETDATE())
    );
    CREATE UNIQUE INDEX UX_SeatHolds_Seat ON dbo.SeatHolds(ScheduleId, SeatNo);
    CREATE INDEX IX_SeatHolds_Expiry ON dbo.SeatHolds(ExpiresAt);
END
GO
IF OBJECT_ID('dbo.Payments') IS NULL
BEGIN
    CREATE TABLE dbo.Payments(
        PaymentId         bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_Payments PRIMARY KEY,
        PNRNo             varchar(50)  NOT NULL,
        UserId            int          NOT NULL,
        Amount            decimal(18,2) NOT NULL,
        Currency          char(3)      NOT NULL CONSTRAINT DF_Payments_Currency DEFAULT ('INR'),
        Gateway           varchar(30)  NOT NULL CONSTRAINT DF_Payments_Gateway DEFAULT ('Razorpay'),
        GatewayOrderId    varchar(60)  NOT NULL,
        GatewayPaymentId  varchar(60)  NULL,
        Method            varchar(30)  NULL,          -- upi / card / netbanking / wallet ...
        Status            varchar(20)  NOT NULL CONSTRAINT DF_Payments_Status DEFAULT ('Created'),  -- Created, Paid, Failed, Refunded, RefundPending
        SignatureVerified bit          NOT NULL CONSTRAINT DF_Payments_Sig DEFAULT (0),
        FailureReason     nvarchar(200) NULL,
        RefundId          varchar(60)  NULL,
        CreatedAt         datetime     NOT NULL CONSTRAINT DF_Payments_Created DEFAULT (GETDATE()),
        PaidAt            datetime     NULL
    );
    CREATE UNIQUE INDEX UX_Payments_Order ON dbo.Payments(GatewayOrderId);
    CREATE INDEX IX_Payments_PNR ON dbo.Payments(PNRNo);
END
GO
IF OBJECT_ID('dbo.OtpCodes') IS NULL
BEGIN
    CREATE TABLE dbo.OtpCodes(
        OtpId      bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_OtpCodes PRIMARY KEY,
        Purpose    varchar(30)  NOT NULL,
        Target     varchar(150) NOT NULL,
        CodeHash   varchar(64)  NOT NULL,
        ExpiresAt  datetime     NOT NULL,
        Attempts   int          NOT NULL CONSTRAINT DF_Otp_Attempts DEFAULT (0),
        ConsumedAt datetime     NULL,
        RequestIp  varchar(45)  NULL,
        CreatedAt  datetime     NOT NULL CONSTRAINT DF_Otp_Created DEFAULT (GETDATE())
    );
    CREATE INDEX IX_OtpCodes_Lookup ON dbo.OtpCodes(Target, Purpose, CreatedAt DESC);
END
GO
IF OBJECT_ID('dbo.ExternalLogins') IS NULL
BEGIN
    CREATE TABLE dbo.ExternalLogins(
        Provider       varchar(20)  NOT NULL,
        ProviderUserId varchar(128) NOT NULL,
        RegId          int          NOT NULL,
        Email          varchar(150) NULL,
        CreatedAt      datetime     NOT NULL CONSTRAINT DF_Ext_Created DEFAULT (GETDATE()),
        CONSTRAINT PK_ExternalLogins PRIMARY KEY (Provider, ProviderUserId)
    );
    CREATE INDEX IX_ExternalLogins_Reg ON dbo.ExternalLogins(RegId);
END
GO
IF OBJECT_ID('dbo.AuditLog') IS NULL
BEGIN
    CREATE TABLE dbo.AuditLog(
        AuditId   bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_AuditLog PRIMARY KEY,
        UserId    int NULL,
        Event     varchar(50)  NOT NULL,
        Detail    nvarchar(400) NULL,
        Ip        varchar(45)  NULL,
        UserAgent nvarchar(250) NULL,
        CreatedAt datetime NOT NULL CONSTRAINT DF_Audit_Created DEFAULT (GETDATE())
    );
    CREATE INDEX IX_AuditLog_User ON dbo.AuditLog(UserId, CreatedAt DESC);
END
GO
IF TYPE_ID('dbo.PassengerList') IS NULL
    CREATE TYPE dbo.PassengerList AS TABLE(
        SeatNo  int          NOT NULL,
        Fname   varchar(50)  NOT NULL,
        Lname   varchar(50)  NOT NULL,
        Age     tinyint      NULL,
        Gender  char(1)      NULL,
        Email   varchar(150) NULL,
        Contact varchar(20)  NULL,
        City    varchar(50)  NULL
    );
GO

/* ---------- 5. Remove raw card storage (card numbers + CVV must never be stored) ---------- */
IF OBJECT_ID('dbo.ispAddCardDetails') IS NOT NULL DROP PROCEDURE dbo.ispAddCardDetails;
IF OBJECT_ID('dbo.CardDetails') IS NOT NULL DROP TABLE dbo.CardDetails;
-- obsolete procedures replaced by parameterised application code / the procedures below
IF OBJECT_ID('dbo.ispUserLogin')          IS NOT NULL DROP PROCEDURE dbo.ispUserLogin;
IF OBJECT_ID('dbo.ispUserRegistration')   IS NOT NULL DROP PROCEDURE dbo.ispUserRegistration;
IF OBJECT_ID('dbo.ispAddPassengerDetails') IS NOT NULL DROP PROCEDURE dbo.ispAddPassengerDetails;
IF OBJECT_ID('dbo.ispAddPNRDetails')      IS NOT NULL DROP PROCEDURE dbo.ispAddPNRDetails;
IF OBJECT_ID('dbo.ispGetBookedSeatNo')    IS NOT NULL DROP PROCEDURE dbo.ispGetBookedSeatNo;
IF OBJECT_ID('dbo.GetPassengerDetails')   IS NOT NULL DROP PROCEDURE dbo.GetPassengerDetails;
GO

/* ---------- 6. Search: every matching trip (the old procedure only returned one bus per date) ---------- */
CREATE OR ALTER PROCEDURE dbo.ispGetAvailableBusDetails
    @Origin varchar(50), @Destination varchar(50), @TravelDate varchar(50)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @d date = ISNULL(TRY_CONVERT(date, REPLACE(@TravelDate, '/', '-'), 105), '19000101');

    SELECT SM.ScheduleId, BM.BusId, BM.BusName, BM.BusNo, BM.BustType, BM.SeatRow, BM.SeatColumn, BM.TotalSeat,
           BM.TotalSeat - ISNULL(T.Taken, 0) AS AvailableSeats,
           SM.Fare, SM.EstimatedTime, SM.DepartureTime, SM.ArivalTime,
           LTRIM(RTRIM(RD.Origin)) AS Origin, LTRIM(RTRIM(RD.Destination)) AS Destination, X.DepartAt
    FROM dbo.ScheduleMaster SM
    JOIN dbo.BusMaster BM    ON BM.BusId = SM.BusId
    JOIN dbo.RouteDetails RD ON RD.RouteID = SM.RouteID
    CROSS APPLY (SELECT TRY_CONVERT(datetime, CONVERT(char(10), TRY_CONVERT(date, REPLACE(SM.[Date], '/', '-'), 105), 120) + ' ' + SM.DepartureTime) AS DepartAt) X
    OUTER APPLY (SELECT (SELECT COUNT(*) FROM dbo.BookingMaster b WHERE b.ScheduleID = SM.ScheduleId AND b.Status = 'Confirmed')
                       + (SELECT COUNT(*) FROM dbo.SeatHolds h WHERE h.ScheduleId = SM.ScheduleId AND h.ExpiresAt > GETDATE()) AS Taken) T
    WHERE TRY_CONVERT(date, REPLACE(SM.[Date], '/', '-'), 105) = @d
      AND LTRIM(RTRIM(RD.Origin)) = LTRIM(RTRIM(@Origin))
      AND LTRIM(RTRIM(RD.Destination)) = LTRIM(RTRIM(@Destination))
      AND (X.DepartAt IS NULL OR X.DepartAt > DATEADD(MINUTE, 15, GETDATE()))   -- no sales within 15 min of departure
    ORDER BY X.DepartAt, SM.ScheduleId;
END
GO

CREATE OR ALTER PROCEDURE dbo.ispGetScheduleForBooking @ScheduleId int
AS
BEGIN
    SET NOCOUNT ON;
    SELECT SM.ScheduleId, BM.BusId, BM.BusName, BM.BusNo, BM.BustType, BM.SeatRow, BM.SeatColumn, BM.TotalSeat,
           SM.Fare, SM.EstimatedTime, SM.DepartureTime, SM.ArivalTime, SM.[Date] AS TravelDate,
           LTRIM(RTRIM(RD.Origin)) AS Origin, LTRIM(RTRIM(RD.Destination)) AS Destination, X.DepartAt,
           CASE WHEN X.DepartAt IS NULL OR X.DepartAt > DATEADD(MINUTE, 15, GETDATE()) THEN 1 ELSE 0 END AS IsBookable
    FROM dbo.ScheduleMaster SM
    JOIN dbo.BusMaster BM    ON BM.BusId = SM.BusId
    JOIN dbo.RouteDetails RD ON RD.RouteID = SM.RouteID
    CROSS APPLY (SELECT TRY_CONVERT(datetime, CONVERT(char(10), TRY_CONVERT(date, REPLACE(SM.[Date], '/', '-'), 105), 120) + ' ' + SM.DepartureTime) AS DepartAt) X
    WHERE SM.ScheduleId = @ScheduleId;
END
GO

CREATE OR ALTER PROCEDURE dbo.ispGetSeatStatus @ScheduleId int, @UserId int
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TRY_CAST(b.SeatNo AS int) AS SeatNo, 'Booked' AS State
    FROM dbo.BookingMaster b
    WHERE b.ScheduleID = @ScheduleId AND b.Status = 'Confirmed' AND TRY_CAST(b.SeatNo AS int) IS NOT NULL
    UNION ALL
    SELECT h.SeatNo, CASE WHEN h.UserId = @UserId THEN 'Mine' ELSE 'Held' END
    FROM dbo.SeatHolds h
    WHERE h.ScheduleId = @ScheduleId AND h.ExpiresAt > GETDATE();
END
GO

/* ---------- 7. Seat holds (10 min) - atomic, so two people can never take the same seat ---------- */
CREATE OR ALTER PROCEDURE dbo.ispHoldSeats
    @ScheduleId int, @UserId int, @Seats varchar(100), @HoldMinutes int,
    @Result int OUTPUT, @ConflictSeat int OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
    SET @Result = 0; SET @ConflictSeat = NULL;

    DECLARE @total int = (SELECT BM.TotalSeat FROM dbo.ScheduleMaster SM JOIN dbo.BusMaster BM ON BM.BusId = SM.BusId WHERE SM.ScheduleId = @ScheduleId);
    IF @total IS NULL BEGIN SET @Result = -3; RETURN; END     -- unknown trip

    DECLARE @S TABLE (SeatNo int PRIMARY KEY);
    INSERT INTO @S(SeatNo)
    SELECT DISTINCT TRY_CAST(LTRIM(RTRIM(value)) AS int) FROM STRING_SPLIT(@Seats, ',')
    WHERE TRY_CAST(LTRIM(RTRIM(value)) AS int) IS NOT NULL;

    IF NOT EXISTS (SELECT 1 FROM @S) BEGIN SET @Result = -4; RETURN; END                       -- nothing selected
    IF EXISTS (SELECT 1 FROM @S WHERE SeatNo < 1 OR SeatNo > @total) BEGIN SET @Result = -2; RETURN; END  -- invalid seat number

    BEGIN TRY
        BEGIN TRAN;
        DELETE FROM dbo.SeatHolds WHERE ExpiresAt <= GETDATE();
        DELETE FROM dbo.SeatHolds WHERE ScheduleId = @ScheduleId AND UserId = @UserId;       -- release this user's earlier hold

        SELECT TOP 1 @ConflictSeat = s.SeatNo
        FROM @S s
        WHERE EXISTS (SELECT 1 FROM dbo.BookingMaster b WITH (UPDLOCK, HOLDLOCK)
                      WHERE b.ScheduleID = @ScheduleId AND b.Status = 'Confirmed' AND b.SeatNo = CAST(s.SeatNo AS nvarchar(50)))
           OR EXISTS (SELECT 1 FROM dbo.SeatHolds h WITH (UPDLOCK, HOLDLOCK)
                      WHERE h.ScheduleId = @ScheduleId AND h.SeatNo = s.SeatNo)
        ORDER BY s.SeatNo;

        IF @ConflictSeat IS NOT NULL BEGIN ROLLBACK TRAN; SET @Result = -1; RETURN; END

        INSERT INTO dbo.SeatHolds(ScheduleId, SeatNo, UserId, ExpiresAt)
        SELECT @ScheduleId, SeatNo, @UserId, DATEADD(MINUTE, @HoldMinutes, GETDATE()) FROM @S;
        COMMIT TRAN;
        SET @Result = (SELECT COUNT(*) FROM @S);
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        IF ERROR_NUMBER() IN (2601, 2627)
        BEGIN
            SET @Result = -1;   -- unique-index race = seat was just taken
        END
        ELSE
        BEGIN
            THROW;
        END
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE dbo.ispReleaseHolds @ScheduleId int, @UserId int
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.SeatHolds WHERE ScheduleId = @ScheduleId AND UserId = @UserId;
END
GO

/* ---------- 8. Pending booking: the fare is read from the schedule, never from the browser ---------- */
CREATE OR ALTER PROCEDURE dbo.ispCreatePendingBooking
    @UserId int, @ScheduleId int, @BoardingId int, @PNRNo varchar(50),
    @ContactEmail varchar(150), @ContactMobile varchar(20),
    @Passengers dbo.PassengerList READONLY,
    @TotalAmount decimal(18,2) OUTPUT, @Result int OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    SET @Result = 0; SET @TotalAmount = 0;

    DECLARE @fare decimal(18,2), @busId int, @date varchar(50), @origin varchar(50), @dest varchar(50), @n int, @expires datetime;
    SELECT @fare = SM.Fare, @busId = SM.BusId, @date = SM.[Date], @origin = LTRIM(RTRIM(RD.Origin)), @dest = LTRIM(RTRIM(RD.Destination))
    FROM dbo.ScheduleMaster SM JOIN dbo.RouteDetails RD ON RD.RouteID = SM.RouteID
    WHERE SM.ScheduleId = @ScheduleId;

    IF @fare IS NULL BEGIN SET @Result = -1; RETURN; END                                              -- unknown trip
    IF NOT EXISTS (SELECT 1 FROM dbo.PickUpStand WHERE StandId = @BoardingId AND BusID = @busId) BEGIN SET @Result = -2; RETURN; END  -- bad boarding point
    SELECT @n = COUNT(*) FROM @Passengers;
    IF @n = 0 BEGIN SET @Result = -3; RETURN; END

    -- the user must hold every seat that is being booked, and the hold must still be alive
    IF EXISTS (SELECT 1 FROM @Passengers p WHERE NOT EXISTS (
                 SELECT 1 FROM dbo.SeatHolds h
                 WHERE h.ScheduleId = @ScheduleId AND h.UserId = @UserId AND h.SeatNo = p.SeatNo AND h.ExpiresAt > GETDATE()))
    BEGIN SET @Result = -4; RETURN; END

    SELECT @expires = MIN(ExpiresAt) FROM dbo.SeatHolds WHERE ScheduleId = @ScheduleId AND UserId = @UserId;
    SET @TotalAmount = @fare * @n;

    BEGIN TRAN;
    UPDATE dbo.BookingMaster SET Status = 'Cancelled' WHERE RegId = @UserId AND ScheduleID = @ScheduleId AND Status = 'Pending';
    UPDATE dbo.PNRDetails    SET Status = 'Cancelled' WHERE CreatedBy = @UserId AND ScheduleId = @ScheduleId AND Status = 'Pending';

    INSERT INTO dbo.PNRDetails(PNRNo, TotalAmount, TotalTickets, CreatedBy, Status, ScheduleId, BoardingId, ContactEmail, ContactMobile, HoldExpiresAt, CreatedAt)
    VALUES(@PNRNo, @TotalAmount, @n, @UserId, 'Pending', @ScheduleId, @BoardingId, @ContactEmail, @ContactMobile, @expires, GETDATE());

    INSERT INTO dbo.BookingMaster(RegId, BusId, Fname, Lname, Email, Contact, City, SeatNo, TravelDate, Origin, Destination, BoardingID, Fare, PNRNo, ScheduleID, Status, Age, Gender, CreatedAt)
    SELECT @UserId, @busId, p.Fname, p.Lname, ISNULL(p.Email, @ContactEmail), ISNULL(p.Contact, @ContactMobile), p.City,
           CAST(p.SeatNo AS nvarchar(50)), @date, @origin, @dest, @BoardingId, @fare, @PNRNo, @ScheduleId, 'Pending', p.Age, p.Gender, GETDATE()
    FROM @Passengers p;

    UPDATE dbo.SeatHolds SET PNRNo = @PNRNo WHERE ScheduleId = @ScheduleId AND UserId = @UserId;
    COMMIT TRAN;
    SET @Result = @n;
END
GO

/* ---------- 9. Confirm booking after the gateway payment has been verified (idempotent) ----------
   @Result: 1 confirmed, 2 already confirmed, -1 not found, -2 amount mismatch, -3 seat no longer available */
CREATE OR ALTER PROCEDURE dbo.ispConfirmBooking
    @PNRNo varchar(50), @UserId int, @GatewayOrderId varchar(60), @GatewayPaymentId varchar(60),
    @Method varchar(30), @AmountPaid decimal(18,2), @Result int OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    SET @Result = 0;
    DECLARE @status varchar(20), @total decimal(18,2), @sid int, @cnt int;

    BEGIN TRY
        BEGIN TRAN;
        SELECT @status = Status, @total = TotalAmount, @sid = ScheduleId, @cnt = TotalTickets
        FROM dbo.PNRDetails WITH (UPDLOCK, HOLDLOCK)
        WHERE PNRNo = @PNRNo AND CreatedBy = @UserId;

        IF @status IS NULL BEGIN ROLLBACK TRAN; SET @Result = -1; RETURN; END

        IF @status = 'Confirmed'
        BEGIN
            UPDATE dbo.Payments SET Status = 'Paid', GatewayPaymentId = ISNULL(GatewayPaymentId, @GatewayPaymentId), Method = ISNULL(Method, @Method),
                   PaidAt = ISNULL(PaidAt, GETDATE()), SignatureVerified = 1
            WHERE PNRNo = @PNRNo AND GatewayOrderId = @GatewayOrderId AND Status <> 'Refunded';
            COMMIT TRAN; SET @Result = 2; RETURN;
        END

        IF @AmountPaid <> @total BEGIN ROLLBACK TRAN; SET @Result = -2; RETURN; END

        IF EXISTS (SELECT 1 FROM dbo.BookingMaster me
                   JOIN dbo.BookingMaster o ON o.ScheduleID = me.ScheduleID AND o.SeatNo = me.SeatNo AND o.Status = 'Confirmed' AND o.PNRNo <> me.PNRNo
                   WHERE me.PNRNo = @PNRNo)
           OR EXISTS (SELECT 1 FROM dbo.BookingMaster me
                      JOIN dbo.SeatHolds h ON h.ScheduleId = me.ScheduleID AND h.SeatNo = TRY_CAST(me.SeatNo AS int) AND h.UserId <> @UserId AND h.ExpiresAt > GETDATE()
                      WHERE me.PNRNo = @PNRNo)
        BEGIN ROLLBACK TRAN; SET @Result = -3; RETURN; END

        UPDATE dbo.BookingMaster SET Status = 'Confirmed' WHERE PNRNo = @PNRNo;
        UPDATE dbo.PNRDetails SET Status = 'Confirmed', ConfirmedAt = GETDATE() WHERE PNRNo = @PNRNo;
        UPDATE dbo.Payments SET Status = 'Paid', GatewayPaymentId = @GatewayPaymentId, Method = @Method, PaidAt = GETDATE(), SignatureVerified = 1
        WHERE PNRNo = @PNRNo AND GatewayOrderId = @GatewayOrderId;
        DELETE FROM dbo.SeatHolds WHERE ScheduleId = @sid AND UserId = @UserId;
        UPDATE dbo.ScheduleMaster SET BookedSeats = ISNULL(BookedSeats, 0) + @cnt, AvailableSeats = ISNULL(AvailableSeats, 0) - @cnt WHERE ScheduleId = @sid;
        COMMIT TRAN;
        SET @Result = 1;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        IF ERROR_NUMBER() IN (2601, 2627)
        BEGIN
            SET @Result = -3;
        END
        ELSE
        BEGIN
            THROW;
        END
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE dbo.ispCancelPendingBooking @PNRNo varchar(50), @UserId int
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @sid int;
    SELECT @sid = ScheduleId FROM dbo.PNRDetails WHERE PNRNo = @PNRNo AND CreatedBy = @UserId AND Status = 'Pending';
    IF @sid IS NULL RETURN;
    UPDATE dbo.PNRDetails SET Status = 'Cancelled' WHERE PNRNo = @PNRNo AND CreatedBy = @UserId AND Status = 'Pending';
    UPDATE dbo.BookingMaster SET Status = 'Cancelled' WHERE PNRNo = @PNRNo AND RegId = @UserId AND Status = 'Pending';
    DELETE FROM dbo.SeatHolds WHERE ScheduleId = @sid AND UserId = @UserId;
END
GO

/* ---------- 10. Reads ---------- */
CREATE OR ALTER PROCEDURE dbo.ispGetBookingByPNR @PNRNo varchar(50), @UserId int = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT P.PNRNo, P.TotalAmount, P.TotalTickets, ISNULL(P.Status, 'Confirmed') AS Status, P.CreatedAt, P.ConfirmedAt, P.HoldExpiresAt,
           P.ContactEmail, P.ContactMobile, P.CreatedBy AS UserId, b.ScheduleID AS ScheduleId,
           BM.BusName, BM.BusNo, BM.BustType, b.Origin, b.Destination, b.TravelDate,
           SM.DepartureTime, SM.ArivalTime, SM.EstimatedTime,
           PS.PlaceName AS BoardingPlace, PS.PlaceTime AS BoardingTime,
           pay.GatewayPaymentId, pay.GatewayOrderId, pay.Method AS PaymentMethod, pay.PaidAt, pay.Status AS PaymentStatus
    FROM dbo.PNRDetails P
    OUTER APPLY (SELECT TOP 1 bk.BusId, bk.Origin, bk.Destination, bk.TravelDate, bk.BoardingID, bk.ScheduleID FROM dbo.BookingMaster bk WHERE bk.PNRNo = P.PNRNo ORDER BY bk.BookingId) b
    LEFT JOIN dbo.BusMaster BM ON BM.BusId = b.BusId
    LEFT JOIN dbo.ScheduleMaster SM ON SM.ScheduleId = b.ScheduleID
    LEFT JOIN dbo.PickUpStand PS ON PS.StandId = b.BoardingID
    OUTER APPLY (SELECT TOP 1 py.GatewayPaymentId, py.GatewayOrderId, py.Method, py.PaidAt, py.Status FROM dbo.Payments py WHERE py.PNRNo = P.PNRNo ORDER BY py.PaymentId DESC) pay
    WHERE P.PNRNo = @PNRNo AND (@UserId IS NULL OR P.CreatedBy = @UserId);

    SELECT bk.Fname, bk.Lname, bk.Age, bk.Gender, bk.SeatNo, bk.Fare, bk.Contact, bk.Email
    FROM dbo.BookingMaster bk
    WHERE bk.PNRNo = @PNRNo
      AND (@UserId IS NULL OR EXISTS (SELECT 1 FROM dbo.PNRDetails P WHERE P.PNRNo = bk.PNRNo AND P.CreatedBy = @UserId))
      AND bk.Status <> 'Cancelled'
    ORDER BY TRY_CAST(bk.SeatNo AS int);
END
GO

CREATE OR ALTER PROCEDURE dbo.ispGetPNRDetails @UserID int
AS
BEGIN
    SET NOCOUNT ON;
    SELECT P.PNRNo, P.TotalAmount, P.TotalTickets, ISNULL(P.Status, 'Confirmed') AS Status, ISNULL(P.ConfirmedAt, P.CreatedAt) AS BookedOn,
           b.Origin, b.Destination, b.TravelDate, BM.BusName
    FROM dbo.PNRDetails P
    OUTER APPLY (SELECT TOP 1 bk.BusId, bk.Origin, bk.Destination, bk.TravelDate FROM dbo.BookingMaster bk WHERE bk.PNRNo = P.PNRNo ORDER BY bk.BookingId) b
    LEFT JOIN dbo.BusMaster BM ON BM.BusId = b.BusId
    WHERE P.CreatedBy = @UserID AND ISNULL(P.Status, 'Confirmed') = 'Confirmed'
    ORDER BY P.PNRDetailsID DESC;
END
GO

-- admin report: confirmed bookings only, names no longer glued together
CREATE OR ALTER PROCEDURE dbo.ispGetBookingReportByAdmin
AS
BEGIN
    SET NOCOUNT ON;
    SELECT BM.Fname + ' ' + BM.Lname AS PaxName, BM.Email, BM.Contact, BM.SeatNo,
           CONVERT(varchar(11), TRY_CONVERT(date, REPLACE(BM.TravelDate, '/', '-'), 105), 13) AS TravelDate,
           BM.Origin, BM.Destination, BM.Fare, UM.Fname + ' ' + UM.Lname AS BookedBy, BB.BusName
    FROM dbo.BookingMaster BM
    LEFT JOIN dbo.BusMaster BB ON BM.BusId = BB.BusId
    LEFT JOIN dbo.Registration UM ON UM.regId = BM.RegId
    WHERE BM.Status = 'Confirmed';
END
GO
PRINT 'Upgrade complete. Existing plain-text passwords are converted to salted hashes automatically the first time each person signs in.';
GO
