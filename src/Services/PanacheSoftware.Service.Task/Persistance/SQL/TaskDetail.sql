SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [TaskDetail] (
    [TenantId]       UNIQUEIDENTIFIER NOT NULL,
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [Status]         NVARCHAR (25)   NULL,
    [CreatedDate]    DATETIME2 (7)    NOT NULL,
    [LastUpdateDate] DATETIME2 (7)    NOT NULL,
    [CreatedBy]      UNIQUEIDENTIFIER NOT NULL,
    [LastUpdateBy]   UNIQUEIDENTIFIER NOT NULL,
    [TaskHeaderId]   UNIQUEIDENTIFIER NOT NULL,
    [DateFrom]       DATETIME2 (7)    NOT NULL,
    [DateTo]         DATETIME2 (7)    NOT NULL,
	[Data]           NVARCHAR (MAX)   NULL,
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_TaskDetail_TaskHeaderId]
    ON [TaskDetail]([TaskHeaderId] ASC);


GO
ALTER TABLE [TaskDetail]
    ADD CONSTRAINT [PK_TaskDetail] PRIMARY KEY CLUSTERED ([Id] ASC);


GO
ALTER TABLE [TaskDetail]
    ADD CONSTRAINT [FK_TaskDetail_TaskHeader_TaskHeaderId] FOREIGN KEY ([TaskHeaderId]) REFERENCES [TaskHeader] ([Id]) ON DELETE CASCADE;