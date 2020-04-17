SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [IdentityUserClaims] (
    [Id]         INT              IDENTITY (1, 1) NOT NULL,
    [UserId]     UNIQUEIDENTIFIER NOT NULL,
    [ClaimType]  NVARCHAR (1000)   NULL,
    [ClaimValue] NVARCHAR (1000)   NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_IdentityUserClaims_UserId]
    ON [IdentityUserClaims]([UserId] ASC);


GO
ALTER TABLE [IdentityUserClaims]
    ADD CONSTRAINT [PK_IdentityUserClaims] PRIMARY KEY CLUSTERED ([Id] ASC);


GO
ALTER TABLE [IdentityUserClaims]
    ADD CONSTRAINT [FK_IdentityUserClaims_IdentityUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [IdentityUsers] ([Id]) ON DELETE CASCADE;


