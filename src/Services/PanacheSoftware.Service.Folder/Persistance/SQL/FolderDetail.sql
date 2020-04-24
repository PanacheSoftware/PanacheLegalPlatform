﻿SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [FolderDetail] (
    [TenantId]       UNIQUEIDENTIFIER NOT NULL,
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [Status]         NVARCHAR (25)   NULL,
    [CreatedDate]    DATETIME2 (7)    NOT NULL,
    [LastUpdateDate] DATETIME2 (7)    NOT NULL,
    [CreatedBy]      UNIQUEIDENTIFIER NOT NULL,
    [LastUpdateBy]   UNIQUEIDENTIFIER NOT NULL,
    [FolderHeaderId] UNIQUEIDENTIFIER NOT NULL,
    [DateFrom]       DATETIME2 (7)    NOT NULL,
    [DateTo]         DATETIME2 (7)    NOT NULL
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_FolderDetail_FolderHeaderId]
    ON [FolderDetail]([FolderHeaderId] ASC);


GO
ALTER TABLE [FolderDetail]
    ADD CONSTRAINT [PK_FolderDetail] PRIMARY KEY CLUSTERED ([Id] ASC);


GO
ALTER TABLE [FolderDetail]
    ADD CONSTRAINT [FK_FolderDetail_FolderHeader_FolderHeaderId] FOREIGN KEY ([FolderHeaderId]) REFERENCES [FolderHeader] ([Id]) ON DELETE CASCADE;