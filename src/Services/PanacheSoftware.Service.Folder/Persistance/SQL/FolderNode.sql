SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [FolderNode] (
    [TenantId]                  UNIQUEIDENTIFIER    NOT NULL,
    [Id]						UNIQUEIDENTIFIER	NOT NULL,
    [Status]					NVARCHAR (25)		NULL,
    [CreatedDate]				DATETIME2 (7)		NOT NULL,
    [LastUpdateDate]			DATETIME2 (7)		NOT NULL,
    [CreatedBy]					UNIQUEIDENTIFIER	NOT NULL,
    [LastUpdateBy]				UNIQUEIDENTIFIER	NOT NULL,
    [FolderHeaderId]			UNIQUEIDENTIFIER	NOT NULL,
    [DateFrom]					DATETIME2 (7)		NOT NULL,
    [DateTo]					DATETIME2 (7)		NOT NULL,
	[Description]				NVARCHAR (4000)		NULL,
	[Title]						NVARCHAR (1000)		NULL,
	[CompletionDate]			DATETIME2 (7)		NOT NULL,
    [OriginalCompletionDate]	DATETIME2 (7)		NOT NULL,
    [CompletedOnDate]			DATETIME2 (7)		NOT NULL,
	[NodeType]					NVARCHAR (1000)		NULL,
    [SequenceNumber]			INT					NOT NULL,
	[Completed]					BIT					NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_FolderNode_FolderHeaderId]
    ON [FolderNode]([FolderHeaderId] ASC);


GO
ALTER TABLE [FolderNode]
    ADD CONSTRAINT [PK_FolderNode] PRIMARY KEY CLUSTERED ([Id] ASC);


GO
ALTER TABLE [FolderNode]
    ADD CONSTRAINT [FK_FolderNode_FolderHeader_FolderHeaderId] FOREIGN KEY ([FolderHeaderId]) REFERENCES [FolderHeader] ([Id]) ON DELETE CASCADE;
