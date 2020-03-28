CREATE TABLE [dbo].[Setting]
(
	[Id] INT NOT NULL PRIMARY KEY Identity(1,1), 
    [KeyName] VARCHAR(50) NOT NULL, 
    [KeyValue] VARBINARY(20) NOT NULL, 
    [IsActive] BIT NOT NULL
)

GO

CREATE INDEX [IX_Setting_Index_OnKeyName] ON [dbo].[Setting] (KeyName)
