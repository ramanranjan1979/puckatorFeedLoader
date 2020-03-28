CREATE TABLE [dbo].[LogType]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [Name] VARCHAR(50) NOT NULL
)

GO

CREATE INDEX [IX_LogType_On_Name] ON [dbo].[LogType] (Name)
