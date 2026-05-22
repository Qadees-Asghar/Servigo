-- ============================================================
--  SERVIGO – Smart Appointment Booking System
--  Full Database Script  |  SQL Server  |  3NF Normalized
-- ============================================================

USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'SERVIGO')
BEGIN
    ALTER DATABASE SERVIGO SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE SERVIGO;
END
GO

CREATE DATABASE SERVIGO;
GO
USE SERVIGO;
GO

-- ============================================================
--  LOOKUP / REFERENCE TABLES
-- ============================================================

CREATE TABLE Roles (
    RoleID   INT          PRIMARY KEY IDENTITY(1,1),
    RoleName NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE BookingStatuses (
    StatusID   INT          PRIMARY KEY IDENTITY(1,1),
    StatusName NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE ServiceCategories (
    CategoryID   INT           PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(100) NOT NULL UNIQUE
);

-- ============================================================
--  CORE TABLES
-- ============================================================

CREATE TABLE Users (
    UserID       NVARCHAR(10)  PRIMARY KEY,            -- SRV-00001
    FullName     NVARCHAR(100) NOT NULL,
    Email        NVARCHAR(150) NOT NULL UNIQUE,
    Phone        NCHAR(11)     NOT NULL UNIQUE,         -- 11 digits
    CNIC         NCHAR(13)     NOT NULL UNIQUE,         -- 13 digits
    PasswordHash NVARCHAR(256) NOT NULL,
    RoleID       INT           NOT NULL REFERENCES Roles(RoleID),
    IsActive     BIT           NOT NULL DEFAULT 1,
    CreatedAt    DATETIME      NOT NULL DEFAULT GETDATE()
);

CREATE TABLE ServiceProviders (
    ProviderID    INT           PRIMARY KEY IDENTITY(1,1),
    UserID        NVARCHAR(10)  NOT NULL UNIQUE REFERENCES Users(UserID),
    CategoryID    INT           NOT NULL REFERENCES ServiceCategories(CategoryID),
    Description   NVARCHAR(500) NULL,
    IsApproved    BIT           NOT NULL DEFAULT 0,
    AverageRating DECIMAL(3,2)  NOT NULL DEFAULT 0.00,
    CreatedAt     DATETIME      NOT NULL DEFAULT GETDATE()
);

CREATE TABLE Services (
    ServiceID       INT           PRIMARY KEY IDENTITY(1,1),
    ProviderID      INT           NOT NULL REFERENCES ServiceProviders(ProviderID),
    ServiceName     NVARCHAR(150) NOT NULL,
    Description     NVARCHAR(500) NULL,
    Price           DECIMAL(10,2) NOT NULL,
    DurationMinutes INT           NOT NULL,
    IsActive        BIT           NOT NULL DEFAULT 1
);

CREATE TABLE TimeSlots (
    SlotID     INT          PRIMARY KEY IDENTITY(1,1),
    ProviderID INT          NOT NULL REFERENCES ServiceProviders(ProviderID),
    SlotDate   DATE         NOT NULL,
    StartTime  TIME(0)      NOT NULL,
    EndTime    TIME(0)      NOT NULL,
    IsAvailable BIT         NOT NULL DEFAULT 1,
    CONSTRAINT UQ_ProviderDateStart UNIQUE (ProviderID, SlotDate, StartTime)
);

CREATE TABLE Bookings (
    BookingID  INT           PRIMARY KEY IDENTITY(1,1),
    CustomerID NVARCHAR(10)  NOT NULL REFERENCES Users(UserID),
    SlotID     INT           NOT NULL REFERENCES TimeSlots(SlotID),
    ServiceID  INT           NOT NULL REFERENCES Services(ServiceID),
    StatusID   INT           NOT NULL REFERENCES BookingStatuses(StatusID),
    Notes      NVARCHAR(500) NULL,
    BookedAt   DATETIME      NOT NULL DEFAULT GETDATE(),
    UpdatedAt  DATETIME      NOT NULL DEFAULT GETDATE()
);

CREATE TABLE Notifications (
    NotificationID INT           PRIMARY KEY IDENTITY(1,1),
    UserID         NVARCHAR(10)  NOT NULL REFERENCES Users(UserID),
    Message        NVARCHAR(500) NOT NULL,
    IsRead         BIT           NOT NULL DEFAULT 0,
    CreatedAt      DATETIME      NOT NULL DEFAULT GETDATE()
);

CREATE TABLE Ratings (
    RatingID   INT IDENTITY(1,1) PRIMARY KEY,
    BookingID  INT          NOT NULL UNIQUE,
    ProviderID INT          NOT NULL,
    CustomerID NVARCHAR(10) NULL,
    Stars      TINYINT      NOT NULL CHECK (Stars BETWEEN 1 AND 5),
    Comment    NVARCHAR(500) NULL,
    CreatedAt  DATETIME     NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (BookingID)  REFERENCES Bookings(BookingID),
    FOREIGN KEY (ProviderID) REFERENCES ServiceProviders(ProviderID)
);

CREATE TABLE FeedbackReports (
    ReportID     INT IDENTITY(1,1) PRIMARY KEY,
    SubmittedBy  NVARCHAR(20)  NOT NULL,
    ReportType   NVARCHAR(30)  NOT NULL,
    TargetUserID NVARCHAR(20)  NULL,
    Subject      NVARCHAR(200) NOT NULL,
    Description  NVARCHAR(1000) NOT NULL,
    IsResolved   BIT           NOT NULL DEFAULT 0,
    ResolvedAt   DATETIME      NULL,
    CreatedAt    DATETIME      NOT NULL DEFAULT GETDATE()
);

CREATE TABLE AuditLogs (
    LogID       INT            PRIMARY KEY IDENTITY(1,1),
    TableName   NVARCHAR(100)  NOT NULL,
    Action      NVARCHAR(50)   NOT NULL,
    RecordID    NVARCHAR(100)  NULL,
    PerformedBy NVARCHAR(10)   NULL,
    Details     NVARCHAR(MAX)  NULL,
    LoggedAt    DATETIME       NOT NULL DEFAULT GETDATE()
);

-- ============================================================
--  INDEXES
-- ============================================================

CREATE INDEX IX_Users_Email          ON Users(Email);
CREATE INDEX IX_Users_CNIC           ON Users(CNIC);
CREATE INDEX IX_Users_Phone          ON Users(Phone);
CREATE INDEX IX_Users_RoleID         ON Users(RoleID);
CREATE INDEX IX_Bookings_CustomerID  ON Bookings(CustomerID);
CREATE INDEX IX_Bookings_StatusID    ON Bookings(StatusID);
CREATE INDEX IX_Bookings_SlotID      ON Bookings(SlotID);
CREATE INDEX IX_TimeSlots_Provider   ON TimeSlots(ProviderID, SlotDate);
CREATE INDEX IX_Notifications_User   ON Notifications(UserID, IsRead);
CREATE INDEX IX_AuditLogs_Table      ON AuditLogs(TableName, LoggedAt);
CREATE INDEX IX_Services_Provider    ON Services(ProviderID);
CREATE INDEX IX_Ratings_Provider     ON Ratings(ProviderID);
CREATE INDEX IX_Ratings_Customer     ON Ratings(CustomerID);
CREATE INDEX IX_FeedbackReports_By   ON FeedbackReports(SubmittedBy);

-- ============================================================
--  SEED DATA
-- ============================================================

INSERT INTO Roles (RoleName) VALUES ('Admin'), ('Customer'), ('ServiceProvider');

INSERT INTO BookingStatuses (StatusName)
VALUES ('Pending'), ('Accepted'), ('Completed'), ('Cancelled'), ('Rejected');

INSERT INTO ServiceCategories (CategoryName)
VALUES ('Electrician'), ('Plumber'), ('Mechanic'), ('Laundry'),
       ('Painter'), ('Carpenter'), ('Cleaner'), ('AC Repair'), ('Mason'), ('Gardener');

-- ============================================================
--  STORED PROCEDURES
-- ============================================================

GO
CREATE OR ALTER PROCEDURE sp_CreateBooking
    @CustomerID NVARCHAR(10),
    @SlotID     INT,
    @ServiceID  INT,
    @Notes      NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        -- Validate slot availability
        IF NOT EXISTS (
            SELECT 1 FROM TimeSlots
            WHERE SlotID = @SlotID AND IsAvailable = 1
        )
        BEGIN
            RAISERROR('Selected time slot is not available.', 16, 1);
            ROLLBACK; RETURN;
        END

        -- Validate 7-day advance limit
        IF (SELECT SlotDate FROM TimeSlots WHERE SlotID = @SlotID)
            > DATEADD(DAY, 7, CAST(GETDATE() AS DATE))
        BEGIN
            RAISERROR('Booking can only be made up to 7 days in advance.', 16, 1);
            ROLLBACK; RETURN;
        END

        -- Validate slot is not in the past
        DECLARE @SlotDate DATE;
        DECLARE @SlotStart TIME(0);
        SELECT @SlotDate = SlotDate, @SlotStart = StartTime
        FROM TimeSlots WHERE SlotID = @SlotID;

        IF @SlotDate < CAST(GETDATE() AS DATE)
        BEGIN
            RAISERROR('Cannot book a past time slot.', 16, 1);
            ROLLBACK; RETURN;
        END

        -- Validate customer hasn't already booked same slot
        IF EXISTS (
            SELECT 1 FROM Bookings
            WHERE CustomerID = @CustomerID AND SlotID = @SlotID
              AND StatusID NOT IN (4, 5)
        )
        BEGIN
            RAISERROR('You already have a booking for this slot.', 16, 1);
            ROLLBACK; RETURN;
        END

        -- Create booking
        INSERT INTO Bookings (CustomerID, SlotID, ServiceID, StatusID, Notes)
        VALUES (@CustomerID, @SlotID, @ServiceID, 1, @Notes);

        DECLARE @BookingID INT = SCOPE_IDENTITY();

        -- Lock slot
        UPDATE TimeSlots SET IsAvailable = 0 WHERE SlotID = @SlotID;

        -- Notify provider
        DECLARE @ProviderUserID NVARCHAR(10);
        SELECT @ProviderUserID = u.UserID
        FROM   TimeSlots ts
        JOIN   ServiceProviders sp ON ts.ProviderID = sp.ProviderID
        JOIN   Users u             ON sp.UserID = u.UserID
        WHERE  ts.SlotID = @SlotID;

        INSERT INTO Notifications (UserID, Message)
        VALUES (@ProviderUserID,
                'New booking request #' + CAST(@BookingID AS NVARCHAR) +
                ' from customer ' + @CustomerID);

        COMMIT;
        SELECT @BookingID AS NewBookingID;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END;
GO

CREATE OR ALTER PROCEDURE sp_UpdateBookingStatus
    @BookingID   INT,
    @NewStatusID INT,
    @PerformedBy NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @OldStatusID  INT;
        DECLARE @CustomerID   NVARCHAR(10);
        DECLARE @SlotID       INT;
        DECLARE @OldStatus    NVARCHAR(50);
        DECLARE @NewStatus    NVARCHAR(50);

        SELECT @OldStatusID = StatusID,
               @CustomerID  = CustomerID,
               @SlotID      = SlotID
        FROM   Bookings WHERE BookingID = @BookingID;

        IF @OldStatusID IS NULL
        BEGIN
            RAISERROR('Booking not found.', 16, 1);
            ROLLBACK; RETURN;
        END

        SELECT @OldStatus = StatusName FROM BookingStatuses WHERE StatusID = @OldStatusID;
        SELECT @NewStatus = StatusName FROM BookingStatuses WHERE StatusID = @NewStatusID;

        UPDATE Bookings
        SET    StatusID = @NewStatusID, UpdatedAt = GETDATE()
        WHERE  BookingID = @BookingID;

        -- Release slot on cancellation or rejection
        IF @NewStatusID IN (4, 5)
            UPDATE TimeSlots SET IsAvailable = 1 WHERE SlotID = @SlotID;

        -- Notify customer
        INSERT INTO Notifications (UserID, Message)
        VALUES (@CustomerID,
                'Booking #' + CAST(@BookingID AS NVARCHAR) +
                ' status changed: ' + @OldStatus + ' → ' + @NewStatus);

        -- Notify provider when customer cancels
        IF @NewStatusID = 4
        BEGIN
            DECLARE @ProviderUserID NVARCHAR(10);
            SELECT @ProviderUserID = u.UserID
            FROM   TimeSlots ts
            JOIN   ServiceProviders sp ON ts.ProviderID = sp.ProviderID
            JOIN   Users u             ON sp.UserID = u.UserID
            WHERE  ts.SlotID = @SlotID;

            INSERT INTO Notifications (UserID, Message)
            VALUES (@ProviderUserID,
                    'Booking #' + CAST(@BookingID AS NVARCHAR) + ' was cancelled by the customer.');
        END

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END;
GO

CREATE OR ALTER PROCEDURE sp_GetDashboardStats
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        (SELECT COUNT(*) FROM Users  WHERE RoleID = 2)              AS TotalCustomers,
        (SELECT COUNT(*) FROM Users  WHERE RoleID = 3)              AS TotalProviders,
        (SELECT COUNT(*) FROM ServiceProviders WHERE IsApproved = 0) AS PendingApprovals,
        (SELECT COUNT(*) FROM Bookings)                              AS TotalBookings,
        (SELECT COUNT(*) FROM Bookings WHERE StatusID = 1)           AS PendingBookings,
        (SELECT COUNT(*) FROM Bookings WHERE StatusID = 3)           AS CompletedBookings;
END;
GO

-- ============================================================
--  USER DEFINED FUNCTIONS
-- ============================================================

CREATE OR ALTER FUNCTION fn_GetCustomerBookingCount(@UserID NVARCHAR(10))
RETURNS INT
AS
BEGIN
    RETURN ISNULL(
        (SELECT COUNT(*) FROM Bookings WHERE CustomerID = @UserID), 0);
END;
GO

CREATE OR ALTER FUNCTION fn_GetProviderCompletedCount(@ProviderID INT)
RETURNS INT
AS
BEGIN
    RETURN ISNULL(
        (SELECT COUNT(*)
         FROM   Bookings b
         JOIN   TimeSlots ts ON b.SlotID = ts.SlotID
         WHERE  ts.ProviderID = @ProviderID
           AND  b.StatusID = 3), 0);
END;
GO

CREATE OR ALTER FUNCTION fn_GenerateUserID()
RETURNS NVARCHAR(10)
AS
BEGIN
    DECLARE @NextVal INT;
    SELECT @NextVal = ISNULL(MAX(CAST(SUBSTRING(UserID, 5, 5) AS INT)), 0) + 1
    FROM   Users
    WHERE  UserID LIKE 'SRV-%';
    RETURN 'SRV-' + RIGHT('00000' + CAST(@NextVal AS NVARCHAR(5)), 5);
END;
GO

-- ============================================================
--  TRIGGERS
-- ============================================================

CREATE OR ALTER TRIGGER trg_Bookings_Audit
ON Bookings
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM inserted) AND EXISTS (SELECT 1 FROM deleted)
    BEGIN
        INSERT INTO AuditLogs (TableName, Action, RecordID, Details, LoggedAt)
        SELECT 'Bookings', 'UPDATE',
               CAST(i.BookingID AS NVARCHAR),
               'BookingID=' + CAST(i.BookingID AS NVARCHAR) +
               ' | Customer=' + i.CustomerID +
               ' | StatusID=' + CAST(i.StatusID AS NVARCHAR),
               GETDATE()
        FROM inserted i;
    END
    ELSE IF EXISTS (SELECT 1 FROM inserted)
    BEGIN
        INSERT INTO AuditLogs (TableName, Action, RecordID, Details, LoggedAt)
        SELECT 'Bookings', 'INSERT',
               CAST(BookingID AS NVARCHAR),
               'BookingID=' + CAST(BookingID AS NVARCHAR) +
               ' | Customer=' + CustomerID +
               ' | SlotID=' + CAST(SlotID AS NVARCHAR),
               GETDATE()
        FROM inserted;
    END
    ELSE
    BEGIN
        INSERT INTO AuditLogs (TableName, Action, RecordID, Details, LoggedAt)
        SELECT 'Bookings', 'DELETE',
               CAST(BookingID AS NVARCHAR),
               'BookingID=' + CAST(BookingID AS NVARCHAR) +
               ' | Customer=' + CustomerID,
               GETDATE()
        FROM deleted;
    END
END;
GO

CREATE OR ALTER TRIGGER trg_Users_Audit
ON Users
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM inserted) AND EXISTS (SELECT 1 FROM deleted)
    BEGIN
        INSERT INTO AuditLogs (TableName, Action, RecordID, Details, LoggedAt)
        SELECT 'Users', 'UPDATE', i.UserID,
               'UserID=' + i.UserID +
               ' | Name=' + i.FullName +
               ' | Email=' + i.Email +
               ' | Active=' + CAST(i.IsActive AS NVARCHAR),
               GETDATE()
        FROM inserted i;
    END
    ELSE IF EXISTS (SELECT 1 FROM inserted)
    BEGIN
        INSERT INTO AuditLogs (TableName, Action, RecordID, Details, LoggedAt)
        SELECT 'Users', 'INSERT', UserID,
               'UserID=' + UserID +
               ' | Name=' + FullName +
               ' | RoleID=' + CAST(RoleID AS NVARCHAR),
               GETDATE()
        FROM inserted;
    END
    ELSE
    BEGIN
        INSERT INTO AuditLogs (TableName, Action, RecordID, Details, LoggedAt)
        SELECT 'Users', 'DELETE', UserID,
               'UserID=' + UserID + ' | Name=' + FullName,
               GETDATE()
        FROM deleted;
    END
END;
GO

CREATE OR ALTER TRIGGER trg_ServiceProviders_Audit
ON ServiceProviders
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM inserted) AND EXISTS (SELECT 1 FROM deleted)
    BEGIN
        INSERT INTO AuditLogs (TableName, Action, RecordID, Details, LoggedAt)
        SELECT 'ServiceProviders', 'UPDATE',
               CAST(i.ProviderID AS NVARCHAR),
               'ProviderID=' + CAST(i.ProviderID AS NVARCHAR) +
               ' | UserID=' + i.UserID +
               ' | IsApproved=' + CAST(i.IsApproved AS NVARCHAR),
               GETDATE()
        FROM inserted i;
    END
    ELSE IF EXISTS (SELECT 1 FROM inserted)
    BEGIN
        INSERT INTO AuditLogs (TableName, Action, RecordID, Details, LoggedAt)
        SELECT 'ServiceProviders', 'INSERT',
               CAST(ProviderID AS NVARCHAR),
               'ProviderID=' + CAST(ProviderID AS NVARCHAR) + ' | UserID=' + UserID,
               GETDATE()
        FROM inserted;
    END
    ELSE
    BEGIN
        INSERT INTO AuditLogs (TableName, Action, RecordID, Details, LoggedAt)
        SELECT 'ServiceProviders', 'DELETE',
               CAST(ProviderID AS NVARCHAR),
               'ProviderID=' + CAST(ProviderID AS NVARCHAR) + ' | UserID=' + UserID,
               GETDATE()
        FROM deleted;
    END
END;
GO

CREATE OR ALTER TRIGGER trg_UpdateAverageRating
ON Ratings
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @PID INT;
    SELECT @PID = ProviderID FROM inserted;
    IF @PID IS NULL SELECT @PID = ProviderID FROM deleted;
    UPDATE ServiceProviders
    SET AverageRating = (
        SELECT ISNULL(AVG(CAST(Stars AS DECIMAL(3,1))), 0)
        FROM Ratings WHERE ProviderID = @PID
    )
    WHERE ProviderID = @PID;
END;
GO

-- ============================================================
--  BOOKING SUMMARY VIEW (GROUP BY report)
-- ============================================================

CREATE OR ALTER VIEW vw_BookingSummary AS
SELECT
    bs.StatusName,
    COUNT(b.BookingID)          AS TotalBookings,
    CAST(GETDATE() AS DATE)     AS AsOfDate
FROM Bookings b
JOIN BookingStatuses bs ON b.StatusID = bs.StatusID
GROUP BY bs.StatusName;
GO

CREATE OR ALTER VIEW vw_ProviderBookingStats AS
SELECT
    sp.ProviderID,
    u.FullName        AS ProviderName,
    sc.CategoryName,
    COUNT(b.BookingID)                                          AS TotalBookings,
    SUM(CASE WHEN b.StatusID = 3 THEN 1 ELSE 0 END)            AS Completed,
    SUM(CASE WHEN b.StatusID IN (4,5) THEN 1 ELSE 0 END)       AS CancelledOrRejected,
    sp.AverageRating
FROM ServiceProviders sp
JOIN Users             u  ON sp.UserID     = u.UserID
JOIN ServiceCategories sc ON sp.CategoryID = sc.CategoryID
LEFT JOIN TimeSlots    ts ON ts.ProviderID  = sp.ProviderID
LEFT JOIN Bookings     b  ON b.SlotID       = ts.SlotID
GROUP BY sp.ProviderID, u.FullName, sc.CategoryName, sp.AverageRating;
GO

PRINT 'SERVIGO database created successfully.';
