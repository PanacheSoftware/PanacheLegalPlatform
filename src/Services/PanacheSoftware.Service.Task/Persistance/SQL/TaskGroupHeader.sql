SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [TaskGroupHeader] (
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
    [ParentTaskGroupId]         UNIQUEIDENTIFIER NULL,
	[MainUserId]	            UNIQUEIDENTIFIER NOT NULL,
	[TeamHeaderId]	            UNIQUEIDENTIFIER NOT NULL,
    [CompletionDate]			DATETIME2 (7)		NOT NULL,
    [OriginalCompletionDate]	DATETIME2 (7)		NOT NULL,
    [CompletedOnDate]			DATETIME2 (7)		NOT NULL,
	[Completed]					BIT					NOT NULL,
	[SequenceNumber]            INT NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_TaskGroupHeader_ParentTaskGroupId]
    ON [TaskGroupHeader]([ParentTaskGroupId] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_TaskGroupHeader_MainUserId]
    ON [TaskGroupHeader]([MainUserId] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_TaskGroupHeader_TeamHeaderId]
    ON [TaskGroupHeader]([TeamHeaderId] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_TaskGroupHeader_ClientHeaderId]
    ON [TaskGroupHeader]([ClientHeaderId] ASC);

GO
ALTER TABLE [TaskGroupHeader]
    ADD CONSTRAINT [PK_TaskGroupHeader] PRIMARY KEY CLUSTERED ([Id] ASC);


GO
ALTER TABLE [TaskGroupHeader]
    ADD CONSTRAINT [FK_TaskGroupHeader_TaskGroupHeader_ParentTaskGroupId] FOREIGN KEY ([ParentTaskGroupId]) REFERENCES [TaskGroupHeader] ([Id]);