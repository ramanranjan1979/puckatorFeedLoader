CREATE TABLE [dbo].[ProductMaster] (
    [Id]           BIGINT        IDENTITY (1, 1) NOT NULL,
    [ProductId]    BIGINT        NOT NULL,
    [Model]        VARCHAR (200) NULL,
    [EAN]          VARCHAR (200) NULL,
    [Name]         VARCHAR (200) NULL,
    [Description]  VARCHAR (MAX) NULL,
    [Dimension]    VARCHAR (MAX) NULL,
    [Price]        VARCHAR (200) NOT NULL,
    [DeliveryCode] VARCHAR (200) NOT NULL,
    [Quantity]     VARCHAR (200) NULL,
    [Categories]   VARCHAR (MAX) NOT NULL,
    [Options]      VARCHAR (MAX) NULL,
    [MOQ]          VARCHAR (200) NULL,
    [ImageUrl]     VARCHAR (MAX) NULL,
    [CreatedDate]  DATETIME      NOT NULL,
    [UpdatedDate]  DATETIME      NULL,
    CONSTRAINT [PK_ProductMaster] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_Table_PID]
    ON [dbo].[ProductMaster]([ProductId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Table_EAN]
    ON [dbo].[ProductMaster]([EAN] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Table_Name]
    ON [dbo].[ProductMaster]([Name] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ProductMaster]
    ON [dbo].[ProductMaster]([Id] ASC);

