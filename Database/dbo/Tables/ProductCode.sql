CREATE TABLE [dbo].[ProductCode] (
    [Id]          BIGINT       IDENTITY (1, 1) NOT NULL,
    [Model]       VARCHAR (20) NOT NULL,
    [EAN]         VARCHAR (20) NOT NULL,
    [Active]      BIT          NOT NULL,
    [CreatedDate] DATETIME     NOT NULL,
    [UpdatedDate] DATETIME     NULL,
    CONSTRAINT [PK_ProductCode] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_Table_Model]
    ON [dbo].[ProductCode]([Model] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Table_EAN]
    ON [dbo].[ProductCode]([EAN] ASC);

