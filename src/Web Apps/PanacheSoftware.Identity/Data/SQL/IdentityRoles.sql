SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [IdentityRoles] (
    [Id]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             NVARCHAR (256)   NULL,
    [NormalizedName]   NVARCHAR (256)   NULL,
    [ConcurrencyStamp] NVARCHAR (4000)   NULL
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex]
    ON [IdentityRoles]([NormalizedName] ASC) WHERE ([NormalizedName] IS NOT NULL);


GO
ALTER TABLE [IdentityRoles]
    ADD CONSTRAINT [PK_IdentityRoles] PRIMARY KEY CLUSTERED ([Id] ASC);


