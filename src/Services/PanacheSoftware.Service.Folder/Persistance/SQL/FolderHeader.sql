SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [FolderHeader] (
    [TenantId]                  UNIQUEIDENTIFIER NOT NULL,
    [Id]                        UNIQUEIDENTIFIER NOT NULL,
    [Status]                    NVARCHAR (25)   NULL,
    [CreatedDate]               DATETIME2 (7)    NOT NULL,
	[ClientHeaderId]            UNIQUEIDENTIFIER NOT NULL,
    [LastUpdateDate]            DATETIME2 (7)    NOT NULL,
    [CreatedBy]                 UNIQUEIDENTIFIER NOT NULL,
    [LastUpdateBy]              UNIQUEIDENTIFIER NOT NULL,
    [ShortName]                 NVARCHAR (1000)   NULL,
    [LongName]                  NVARCHAR (1000)   NULL,
    [Description]               NVARCHAR (4000)   NULL,
    [DateFrom]                  DATETIME2 (7)    NOT NULL,
    [DateTo]                    DATETIME2 (7)    NOT NULL,
    [ParentFolderId]            UNIQUEIDENTIFIER NULL,
	[MainUserId]	            UNIQUEIDENTIFIER NOT NULL,
	[TeamHeaderId]	            UNIQUEIDENTIFIER NOT NULL,
    [CompletionDate]			DATETIME2 (7)		NOT NULL,
    [OriginalCompletionDate]	DATETIME2 (7)		NOT NULL,
    [CompletedOnDate]			DATETIME2 (7)		NOT NULL,
	[Completed]					BIT					NOT NULL,
	[SequenceNumber]            INT NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_FolderHeader_ParentFolderId]
    ON [FolderHeader]([ParentFolderId] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_FolderHeader_MainUserId]
    ON [FolderHeader]([MainUserId] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_FolderHeader_TeamHeaderId]
    ON [FolderHeader]([TeamHeaderId] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_FolderHeader_ClientHeaderId]
    ON [FolderHeader]([ClientHeaderId] ASC);

GO
ALTER TABLE [FolderHeader]
    ADD CONSTRAINT [PK_FolderHeader] PRIMARY KEY CLUSTERED ([Id] ASC);


GO
ALTER TABLE [FolderHeader]
    ADD CONSTRAINT [FK_FolderHeader_FolderHeader_ParentFolderId] FOREIGN KEY ([ParentFolderId]) REFERENCES [FolderHeader] ([Id]);