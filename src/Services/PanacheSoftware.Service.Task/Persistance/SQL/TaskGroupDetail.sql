SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [TaskGroupDetail] (
    [TenantId]       UNIQUEIDENTIFIER NOT NULL,
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [Status]         NVARCHAR (25)   NULL,
    [CreatedDate]    DATETIME2 (7)    NOT NULL,
    [LastUpdateDate] DATETIME2 (7)    NOT NULL,
    [CreatedBy]      UNIQUEIDENTIFIER NOT NULL,
    [LastUpdateBy]   UNIQUEIDENTIFIER NOT NULL,
    [TaskGroupHeaderId] UNIQUEIDENTIFIER NOT NULL,
    [DateFrom]       DATETIME2 (7)    NOT NULL,
    [DateTo]         DATETIME2 (7)    NOT NULL
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_TaskGroupDetail_TaskGroupHeaderId]
    ON [TaskGroupDetail]([TaskGroupHeaderId] ASC);


GO
ALTER TABLE [TaskGroupDetail]
    ADD CONSTRAINT [PK_TaskGroupDetail] PRIMARY KEY CLUSTERED ([Id] ASC);


GO
ALTER TABLE [TaskGroupDetail]
    ADD CONSTRAINT [FK_TaskGroupDetail_TaskGroupHeader_TaskGroupHeaderId] FOREIGN KEY ([TaskGroupHeaderId]) REFERENCES [TaskGroupHeader] ([Id]) ON DELETE CASCADE;