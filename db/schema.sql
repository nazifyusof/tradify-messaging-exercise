-- Reference only. The exercise uses an in-memory repository and does not run this script.

CREATE TABLE dbo.Message (
    Id         UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    JobId      NVARCHAR(50)     NOT NULL,
    SenderId   NVARCHAR(50)     NOT NULL,
    Body       NVARCHAR(2000)   NOT NULL,
    CreatedAt  DATETIMEOFFSET   NOT NULL
);

CREATE INDEX IX_Message_JobId_CreatedAt ON dbo.Message (JobId, CreatedAt);
