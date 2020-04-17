SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [TeamTask] (
    [TenantId]       UNIQUEIDENTIFIER NOT NULL,
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [Status]         NVARCHAR (25)   NULL,
    [CreatedDate]    DATETIME2 (7)    NOT NULL,
    [LastUpdateDate] DATETIME2 (7)    NOT NULL,
    [CreatedBy]      UNIQUEIDENTIFIER NOT NULL,
    [LastUpdateBy]   UNIQUEIDENTIFIER NOT NULL,
    [TaskGroupHeaderId] UNIQUEIDENTIFIER NOT NULL,
    [TeamHeaderId]   UNIQUEIDENTIFIER NOT NULL,
    [DateFrom]       DATETIME2 (7)    NOT NULL,
    [DateTo]         DATETIME2 (7)    NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_TeamTask_TeamHeaderId]
    ON [TeamTask]([TeamHeaderId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TeamTask_TaskGroupHeaderId]
    ON [TeamTask]([TaskGroupHeaderId] ASC);


GO
ALTER TABLE [TeamTask]
    ADD CONSTRAINT [PK_TeamTask] PRIMARY KEY CLUSTERED ([Id] ASC);


GO
ALTER TABLE [TeamTask]
    ADD CONSTRAINT [FK_TeamTask_TaskGroupHeader_TaskGroupHeaderId] FOREIGN KEY ([TaskGroupHeaderId]) REFERENCES [TaskGroupHeader] ([Id]) ON DELETE CASCADE;