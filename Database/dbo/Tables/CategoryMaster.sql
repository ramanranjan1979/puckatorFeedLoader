CREATE TABLE [dbo].[CategoryMaster] (
    [ID]               INT            IDENTITY (1, 1) NOT NULL,
    [CategoryId]       INT            NOT NULL,
    [ParentCategoryId] INT            NULL,
    [Description]      NVARCHAR (200) NOT NULL,
    [Active]           BIT            NOT NULL,
    [CreatedDate]      DATETIME       NOT NULL,
    [UpdatedDate]      DATETIME       NULL,
    CONSTRAINT [PK_CategoryMaster] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_CategoryMaster]
    ON [dbo].[CategoryMaster]([CategoryId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CategoryMaster_Description]
    ON [dbo].[CategoryMaster]([Description] ASC);

