SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [UserTeam] (
    [TenantId]       UNIQUEIDENTIFIER NOT NULL,
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [Status]         NVARCHAR (MAX)   NULL,
    [CreatedDate]    DATETIME2 (7)    NOT NULL,
    [LastUpdateDate] DATETIME2 (7)    NOT NULL,
    [CreatedBy]      UNIQUEIDENTIFIER NOT NULL,
    [LastUpdateBy]   UNIQUEIDENTIFIER NOT NULL,
    [UserId]         UNIQUEIDENTIFIER NOT NULL,
    [TeamHeaderId]   UNIQUEIDENTIFIER NOT NULL,
    [DateFrom]       DATETIME2 (7)    NOT NULL,
    [DateTo]         DATETIME2 (7)    NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_UserTeam_TeamHeaderId]
    ON [UserTeam]([TeamHeaderId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_UserTeam_UserId]
    ON [UserTeam]([UserId] ASC);


GO
ALTER TABLE [UserTeam]
    ADD CONSTRAINT [PK_UserTeam] PRIMARY KEY CLUSTERED ([Id] ASC);


GO
ALTER TABLE [UserTeam]
    ADD CONSTRAINT [FK_UserTeam_TeamHeader_TeamHeaderId] FOREIGN KEY ([TeamHeaderId]) REFERENCES [TeamHeader] ([Id]) ON DELETE CASCADE;

/******
GO
ALTER TABLE [dbo].[UserTeams]
    ADD CONSTRAINT [FK_UserTeams_UserDetail_UserDetailId] FOREIGN KEY ([UserDetailId]) REFERENCES [dbo].[UserDetail] ([Id]) ON DELETE CASCADE;
******/

