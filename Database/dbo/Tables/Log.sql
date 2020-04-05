CREATE TABLE [dbo].[Log]
(
	[Id] INT NOT NULL PRIMARY KEY Identity(1,1), 
    [Logtype] INT NOT NULL, 
    [RequestData] VARCHAR(MAX) NULL, 
    [ResponseData] VARCHAR(MAX) NULL, 
    [ErrorCode] VARCHAR(50) NULL, 
    [CreatedDate] DATETIME NOT NULL DEFAULT getdate(), 
    [CreatedBy] VARCHAR(50) NULL, 
    [UpdatedDate] DATETIME NULL DEFAULT Getdate()
)

GO

CREATE INDEX [IX_Log_On_LogType] ON [dbo].[Log] (LogType)

GO

CREATE INDEX [IX_Log_On_CreatedBy] ON [dbo].[Log] (CreatedBy)
