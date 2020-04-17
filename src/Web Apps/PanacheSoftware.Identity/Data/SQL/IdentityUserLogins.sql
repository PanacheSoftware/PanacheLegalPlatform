SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [IdentityUserLogins] (
    [LoginProvider]       NVARCHAR (450)   NOT NULL,
    [ProviderKey]         NVARCHAR (450)   NOT NULL,
    [ProviderDisplayName] NVARCHAR (1000)   NULL,
    [UserId]              UNIQUEIDENTIFIER NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_IdentityUserLogins_UserId]
    ON [IdentityUserLogins]([UserId] ASC);


GO
ALTER TABLE [IdentityUserLogins]
    ADD CONSTRAINT [PK_IdentityUserLogins] PRIMARY KEY CLUSTERED ([LoginProvider] ASC, [ProviderKey] ASC);


GO
ALTER TABLE [IdentityUserLogins]
    ADD CONSTRAINT [FK_IdentityUserLogins_IdentityUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [IdentityUsers] ([Id]) ON DELETE CASCADE;


