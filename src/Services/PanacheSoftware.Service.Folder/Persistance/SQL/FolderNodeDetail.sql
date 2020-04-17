SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [FolderNodeDetail] (
    [TenantId]       UNIQUEIDENTIFIER NOT NULL,
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [Status]         NVARCHAR (25)   NULL,
    [CreatedDate]    DATETIME2 (7)    NOT NULL,
    [LastUpdateDate] DATETIME2 (7)    NOT NULL,
    [CreatedBy]      UNIQUEIDENTIFIER NOT NULL,
    [LastUpdateBy]   UNIQUEIDENTIFIER NOT NULL,
    [FolderNodeId]   UNIQUEIDENTIFIER NOT NULL,
    [DateFrom]       DATETIME2 (7)    NOT NULL,
    [DateTo]         DATETIME2 (7)    NOT NULL,
	[Data]           NVARCHAR (MAX)   NULL,
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_FolderNodeDetail_FolderNodeId]
    ON [FolderNodeDetail]([FolderNodeId] ASC);


GO
ALTER TABLE [FolderNodeDetail]
    ADD CONSTRAINT [PK_FolderNodeDetail] PRIMARY KEY CLUSTERED ([Id] ASC);


GO
ALTER TABLE [FolderNodeDetail]
    ADD CONSTRAINT [FK_FolderNodeDetail_FolderNode_FolderNodeId] FOREIGN KEY ([FolderNodeId]) REFERENCES [FolderNode] ([Id]) ON DELETE CASCADE;