CREATE TABLE [dbo].[Log]
(
	[Id] INT NOT NULL PRIMARY KEY Identity(1,1), 
    [Logtype] NCHAR(10) NOT NULL, 
    [RequestData] VARBINARY(MAX) NULL, 
    [ResponseData] VARCHAR(MAX) NULL, 
    [ErrorCode] VARBINARY(50) NULL, 
    [CreatedDate] DATETIME NOT NULL DEFAULT getdate(), 
    [CreatedBy] VARCHAR(50) NULL
)

GO

CREATE INDEX [IX_Log_On_LogType] ON [dbo].[Log] (LogType)

GO

CREATE INDEX [IX_Log_On_CreatedBy] ON [dbo].[Log] (CreatedBy)
