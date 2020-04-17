SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [TaskHeader] (
    [TenantId]                  UNIQUEIDENTIFIER    NOT NULL,
    [Id]						UNIQUEIDENTIFIER	NOT NULL,
    [Status]					NVARCHAR (25)		NULL,
    [CreatedDate]				DATETIME2 (7)		NOT NULL,
    [LastUpdateDate]			DATETIME2 (7)		NOT NULL,
    [CreatedBy]					UNIQUEIDENTIFIER	NOT NULL,
    [LastUpdateBy]				UNIQUEIDENTIFIER	NOT NULL,
    [TaskGroupHeaderId]			UNIQUEIDENTIFIER	NOT NULL,
    [DateFrom]					DATETIME2 (7)		NOT NULL,
    [DateTo]					DATETIME2 (7)		NOT NULL,
	[Description]				NVARCHAR (4000)		NULL,
	[Title]						NVARCHAR (1000)		NULL,
	[CompletionDate]			DATETIME2 (7)		NOT NULL,
    [OriginalCompletionDate]	DATETIME2 (7)		NOT NULL,
    [CompletedOnDate]			DATETIME2 (7)		NOT NULL,
	[TaskType]					NVARCHAR (1000)		NULL,
    [SequenceNumber]			INT					NOT NULL,
	[Completed]					BIT					NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_TaskHeader_TaskGroupHeaderId]
    ON [TaskHeader]([TaskGroupHeaderId] ASC);


GO
ALTER TABLE [TaskHeader]
    ADD CONSTRAINT [PK_TaskHeader] PRIMARY KEY CLUSTERED ([Id] ASC);


GO
ALTER TABLE [TaskHeader]
    ADD CONSTRAINT [FK_TaskHeader_TaskGroupHeader_TaskGroupHeaderId] FOREIGN KEY ([TaskGroupHeaderId]) REFERENCES [TaskGroupHeader] ([Id]) ON DELETE CASCADE;
