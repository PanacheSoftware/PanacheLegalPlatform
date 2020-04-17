SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [IdentityRoleClaims] (
    [Id]         INT              IDENTITY (1, 1) NOT NULL,
    [RoleId]     UNIQUEIDENTIFIER NOT NULL,
    [ClaimType]  NVARCHAR (4000)   NULL,
    [ClaimValue] NVARCHAR (4000)   NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_IdentityRoleClaims_RoleId]
    ON [IdentityRoleClaims]([RoleId] ASC);


GO
ALTER TABLE [IdentityRoleClaims]
    ADD CONSTRAINT [PK_IdentityRoleClaims] PRIMARY KEY CLUSTERED ([Id] ASC);


GO
ALTER TABLE [IdentityRoleClaims]
    ADD CONSTRAINT [FK_IdentityRoleClaims_IdentityRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [IdentityRoles] ([Id]) ON DELETE CASCADE;


