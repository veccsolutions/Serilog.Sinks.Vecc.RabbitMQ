-- CREATE DATABASE LogEntries
-- GO

USE LogEntries
GO

DROP TABLE IF EXISTS Logs
GO
DROP TABLE IF EXISTS Applications
GO

CREATE TABLE Applications
(
    ApplicationId SMALLINT IDENTITY (1,1),
    ApplicationName NVARCHAR(100),

    CONSTRAINT pk_Applications PRIMARY KEY CLUSTERED ([ApplicationId]),
    INDEX ix_ApplicationName UNIQUE NONCLUSTERED (ApplicationName, ApplicationId)
)
GO

CREATE TABLE Logs
(
    LogId BIGINT IDENTITY (1,1),
    UtcTime DATETIME2 NULL,
    LocalTime DATETIME2 NULL,
    ApplicationId SMALLINT,
    LogLevel AS JSON_VALUE(LogEntry, '$.Level'),
    RenderedMessage AS JSON_VALUE(LogEntry, '$.RenderedMessage'),
    Exception AS JSON_VALUE(LogEntry, '$.Exception'),
    LogEntry NVARCHAR(MAX),

    INDEX cci CLUSTERED COLUMNSTORE,
    CONSTRAINT pk_Logs PRIMARY KEY (LogId),
    CONSTRAINT fk_ApplicationId FOREIGN KEY (ApplicationId) REFERENCES Applications([ApplicationId])
)
GO

CREATE OR ALTER PROCEDURE spAddLogEntry
    @applicationName VARCHAR(100),
    @logTime DATETIME2,
    @localTime DATETIME2,
    @logEntry VARCHAR(MAX)
AS
BEGIN
    DECLARE @applicationId SMALLINT
    SET @applicationId = 0

    SELECT @applicationId = ApplicationId
    FROM Applications WHERE ApplicationName = @applicationName

    IF @applicationId = 0
    BEGIN
        INSERT INTO Applications
        (ApplicationName)
        VALUES
        (@applicationName)

        SET @applicationId = SCOPE_IDENTITY()
    END

    INSERT INTO Logs
    (UtcTime, LocalTime, ApplicationId, LogEntry)
    VALUES
    (@logTime, @localTime, @applicationId, @logEntry)
END
GO

CREATE OR ALTER  VIEW LogEntries
AS
    SELECT LogId, ApplicationName, l.ApplicationId, UtcTime, LocalTime, LogLevel, RenderedMessage, Exception, LogEntry
    FROM Logs l
        INNER JOIN Applications a ON a.ApplicationId = l.ApplicationId
GO

CREATE OR ALTER  PROCEDURE spGetLogEntries
    @startTime DATETIME2 = NULL,
    @endTime DATETIME2 = NULL,
    @application NVARCHAR(100) = NULL,
    @verbose BIT = 1,
    @debug BIT = 1,
    @information BIT = 1,
    @warning BIT = 1,
    @error BIT = 1,
    @fatal BIT = 1
AS
BEGIN
    IF @endTime IS NULL
        SET @endTime = GETUTCDATE()

    IF @starttime IS NULL
        SET @startTime = DATEADD(DAY, -1, @endTime)

    DECLARE @levels AS TABLE(LogLevel VARCHAR(100))
    DECLARE @applicationId SMALLINT
    SET @applicationId = NULL

    SELECT @applicationId = ApplicationId
    FROM Applications WHERE ApplicationName = @application

    IF (@verbose = 1)
        INSERT INTO @levels VALUES ('Verbose')
    IF (@debug = 1)
        INSERT INTO @levels VALUES ('Debug')
    IF (@information = 1)
        INSERT INTO @levels VALUES ('Information')
    IF (@warning = 1)
        INSERT INTO @levels VALUES ('Warning')
    IF (@error = 1)
        INSERT INTO @levels VALUES ('Error')
    IF (@fatal = 1)
        INSERT INTO @levels VALUES ('Fatal')

    SELECT LogId, ApplicationName, UtcTime, LocalTime, LogLevel, RenderedMessage, Exception, LogEntry
    FROM LogEntries
    WHERE UtcTime BETWEEN @startTime AND @endTime
        AND (@applicationId IS NULL OR ApplicationId = @applicationId)
        AND LogLevel IN (SELECT LogLevel FROM @levels)
    ORDER BY LogId
END
GO
