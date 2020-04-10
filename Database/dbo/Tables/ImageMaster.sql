CREATE TABLE [dbo].[ImageMaster] (
    [Id]          BIGINT        IDENTITY (1, 1) NOT NULL,
    [Model]       NVARCHAR (20) NOT NULL,
    [FileName]    VARCHAR (200) NOT NULL,
    [Number]      INT           NOT NULL,
    [IsMain]      BIT           NULL,
    [CreatedDate] DATETIME      NOT NULL,
    [UpdatedDate] DATETIME      NULL,
    [Active]      BIT           NOT NULL,
    [ProcessedDateTime] DATETIME NULL, 
    [ProcessedReferenceData] VARCHAR(200) NULL, 
    CONSTRAINT [PK_ImageMaster] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_Table_Model]
    ON [dbo].[ImageMaster]([Model] ASC);


GO

CREATE INDEX [IX_ImageMaster_ProcessedDateTime] ON [dbo].[ImageMaster] (ProcessedDateTime)


GO

CREATE INDEX [IX_ImageMaster_ProcessedReferenceData] ON [dbo].[ImageMaster] (ProcessedReferenceData)
