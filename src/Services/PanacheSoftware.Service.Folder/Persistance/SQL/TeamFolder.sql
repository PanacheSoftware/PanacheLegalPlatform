SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [TeamFolder] (
    [TenantId]       UNIQUEIDENTIFIER NOT NULL,
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [Status]         NVARCHAR (25)   NULL,
    [CreatedDate]    DATETIME2 (7)    NOT NULL,
    [LastUpdateDate] DATETIME2 (7)    NOT NULL,
    [CreatedBy]      UNIQUEIDENTIFIER NOT NULL,
    [LastUpdateBy]   UNIQUEIDENTIFIER NOT NULL,
    [FolderHeaderId] UNIQUEIDENTIFIER NOT NULL,
    [TeamHeaderId]   UNIQUEIDENTIFIER NOT NULL,
    [DateFrom]       DATETIME2 (7)    NOT NULL,
    [DateTo]         DATETIME2 (7)    NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_TeamFolder_TeamHeaderId]
    ON [TeamFolder]([TeamHeaderId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TeamFolder_FolderHeaderId]
    ON [TeamFolder]([FolderHeaderId] ASC);


GO
ALTER TABLE [TeamFolder]
    ADD CONSTRAINT [PK_TeamFolder] PRIMARY KEY CLUSTERED ([Id] ASC);


GO
ALTER TABLE [TeamFolder]
    ADD CONSTRAINT [FK_TeamFolder_FolderHeader_FolderHeaderId] FOREIGN KEY ([FolderHeaderId]) REFERENCES [FolderHeader] ([Id]) ON DELETE CASCADE;

/******
GO
ALTER TABLE [dbo].[UserTeams]
    ADD CONSTRAINT [FK_UserTeams_UserDetail_UserDetailId] FOREIGN KEY ([UserDetailId]) REFERENCES [dbo].[UserDetail] ([Id]) ON DELETE CASCADE;
******/

