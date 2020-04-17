SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [IdentityUserRoles] (
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [RoleId] UNIQUEIDENTIFIER NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_IdentityUserRoles_RoleId]
    ON [IdentityUserRoles]([RoleId] ASC);


GO
ALTER TABLE [IdentityUserRoles]
    ADD CONSTRAINT [PK_IdentityUserRoles] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC);


GO
ALTER TABLE [IdentityUserRoles]
    ADD CONSTRAINT [FK_IdentityUserRoles_IdentityRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [IdentityRoles] ([Id]) ON DELETE CASCADE;


GO
ALTER TABLE [IdentityUserRoles]
    ADD CONSTRAINT [FK_IdentityUserRoles_IdentityUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [IdentityUsers] ([Id]) ON DELETE CASCADE;

