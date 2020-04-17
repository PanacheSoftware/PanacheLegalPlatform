SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [IdentityUsers] (
    [Id]                   UNIQUEIDENTIFIER   NOT NULL,
    [UserName]             NVARCHAR (256)     NULL,
    [NormalizedUserName]   NVARCHAR (256)     NULL,
    [Email]                NVARCHAR (256)     NULL,
    [NormalizedEmail]      NVARCHAR (256)     NULL,
    [EmailConfirmed]       BIT                NOT NULL,
    [PasswordHash]         NVARCHAR (4000)     NULL,
    [SecurityStamp]        NVARCHAR (4000)     NULL,
    [ConcurrencyStamp]     NVARCHAR (4000)     NULL,
    [PhoneNumber]          NVARCHAR (1000)     NULL,
    [PhoneNumberConfirmed] BIT                NOT NULL,
    [TwoFactorEnabled]     BIT                NOT NULL,
    [LockoutEnd]           DATETIMEOFFSET (7) NULL,
    [LockoutEnabled]       BIT                NOT NULL,
    [AccessFailedCount]    INT                NOT NULL,
    [Status]               NVARCHAR (1000)     NULL,
    [CreatedDate]          DATETIME2 (7)      NOT NULL,
    [LastUpdateDate]       DATETIME2 (7)      NOT NULL,
    [CreatedBy]            UNIQUEIDENTIFIER   NOT NULL,
    [LastUpdateBy]         UNIQUEIDENTIFIER   NOT NULL,
    [FirstName]            NVARCHAR (1000)     NULL,
    [Surname]              NVARCHAR (1000)     NULL,
    [FullName]             NVARCHAR (1000)     NULL,
    [Base64ProfileImage]   VARCHAR (MAX)     NULL,
    [DateFrom]             DATETIME2 (7)      NOT NULL,
    [DateTo]               DATETIME2 (7)      NOT NULL,
    [Description]          NVARCHAR (1000)     NULL
);


GO
CREATE NONCLUSTERED INDEX [EmailIndex]
    ON [IdentityUsers]([NormalizedEmail] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex]
    ON [IdentityUsers]([NormalizedUserName] ASC) WHERE ([NormalizedUserName] IS NOT NULL);


GO
ALTER TABLE [IdentityUsers]
    ADD CONSTRAINT [PK_IdentityUsers] PRIMARY KEY CLUSTERED ([Id] ASC);



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


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [IdentityUserTokens] (
    [UserId]        UNIQUEIDENTIFIER NOT NULL,
    [LoginProvider] NVARCHAR (450)   NOT NULL,
    [Name]          NVARCHAR (450)   NOT NULL,
    [Value]         NVARCHAR (4000)   NULL
);


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [IdentityTenants] (
    [Id]                   UNIQUEIDENTIFIER   NOT NULL,
    [Domain]               NVARCHAR (1000)     NOT NULL,
    [Description]          NVARCHAR (1000)     NULL,
    [Status]               NVARCHAR (256)     NOT NULL,
    [CreatedByEmail]       NVARCHAR (1000)     NOT NULL,
    [CreatedDate]          DATETIME2 (7)      NOT NULL,
    [LastUpdateDate]       DATETIME2 (7)      NOT NULL,
    [DateFrom]             DATETIME2 (7)      NOT NULL,
    [DateTo]               DATETIME2 (7)      NOT NULL
);

GO
ALTER TABLE [IdentityTenants]
    ADD CONSTRAINT [PK_IdentityTenants] PRIMARY KEY CLUSTERED ([Id] ASC);

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [ClientHeader] (
    [TenantId]       UNIQUEIDENTIFIER NOT NULL,
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [Status]         NVARCHAR (25)   NOT NULL,
    [CreatedDate]    DATETIME2 (7)    NOT NULL,
    [LastUpdateDate] DATETIME2 (7)    NOT NULL,
    [CreatedBy]      UNIQUEIDENTIFIER NOT NULL,
    [LastUpdateBy]   UNIQUEIDENTIFIER NOT NULL,
    [ShortName]      NVARCHAR (1000)   NOT NULL,
    [LongName]       NVARCHAR (1000)   NOT NULL,
    [Description]    NVARCHAR (1000)   NULL,
    [DateFrom]       DATETIME2 (7)    NOT NULL,
    [DateTo]         DATETIME2 (7)    NOT NULL
);

GO
ALTER TABLE [ClientHeader]
    ADD CONSTRAINT [PK_ClientHeader] PRIMARY KEY CLUSTERED ([Id] ASC);


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [ClientDetail] (
    [TenantId]       UNIQUEIDENTIFIER NOT NULL,
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [Status]         NVARCHAR (25)   NOT NULL,
    [CreatedDate]    DATETIME2 (7)    NOT NULL,
    [LastUpdateDate] DATETIME2 (7)    NOT NULL,
    [CreatedBy]      UNIQUEIDENTIFIER NOT NULL,
    [LastUpdateBy]   UNIQUEIDENTIFIER NOT NULL,
    [ClientHeaderId] UNIQUEIDENTIFIER NOT NULL,
    [DateFrom]       DATETIME2 (7)    NOT NULL,
    [DateTo]         DATETIME2 (7)    NOT NULL,
    [Base64Image]    NVARCHAR (MAX)   NULL,
    [Country]        NVARCHAR (1000)   NULL,
    [Region]         NVARCHAR (1000)   NULL,
    [url]            NVARCHAR (1000)   NULL
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_ClientDetail_ClientHeaderId]
    ON [ClientDetail]([ClientHeaderId] ASC);


GO
ALTER TABLE [ClientDetail]
    ADD CONSTRAINT [PK_ClientDetail] PRIMARY KEY CLUSTERED ([Id] ASC);


GO
ALTER TABLE [ClientDetail]
    ADD CONSTRAINT [FK_ClientDetail_ClientHeader_ClientHeaderId] FOREIGN KEY ([ClientHeaderId]) REFERENCES [ClientHeader] ([Id]) ON DELETE CASCADE;



SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [ClientContact] (
    [TenantId]       UNIQUEIDENTIFIER NOT NULL,
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [Status]         NVARCHAR (25)   NOT NULL,
    [CreatedDate]    DATETIME2 (7)    NOT NULL,
    [LastUpdateDate] DATETIME2 (7)    NOT NULL,
    [CreatedBy]      UNIQUEIDENTIFIER NOT NULL,
    [LastUpdateBy]   UNIQUEIDENTIFIER NOT NULL,
    [ClientHeaderId] UNIQUEIDENTIFIER NOT NULL,
    [Title]          NVARCHAR (1000)   NULL,
    [FirstName]      NVARCHAR (1000)   NULL,
    [MiddleName]     NVARCHAR (1000)   NULL,
    [LastName]       NVARCHAR (1000)   NULL,
    [Email]          NVARCHAR (1000)   NULL,
    [Phone]          NVARCHAR (1000)   NULL,
    [DateFrom]       DATETIME2 (7)    NOT NULL,
    [DateTo]         DATETIME2 (7)    NOT NULL,
    [MainContact]    BIT              NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_ClientContact_ClientHeaderId]
    ON [ClientContact]([ClientHeaderId] ASC);


GO
ALTER TABLE [ClientContact]
    ADD CONSTRAINT [PK_ClientContact] PRIMARY KEY CLUSTERED ([Id] ASC);


GO
ALTER TABLE [ClientContact]
    ADD CONSTRAINT [FK_ClientContact_ClientHeader_ClientHeaderId] FOREIGN KEY ([ClientHeaderId]) REFERENCES [ClientHeader] ([Id]) ON DELETE CASCADE;



SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [ClientAddress] (
    [TenantId]        UNIQUEIDENTIFIER NOT NULL,
    [Id]              UNIQUEIDENTIFIER NOT NULL,
    [Status]          NVARCHAR (25)   NOT NULL,
    [CreatedDate]     DATETIME2 (7)    NOT NULL,
    [LastUpdateDate]  DATETIME2 (7)    NOT NULL,
    [CreatedBy]       UNIQUEIDENTIFIER NOT NULL,
    [LastUpdateBy]    UNIQUEIDENTIFIER NOT NULL,
    [AddressType]     NVARCHAR (1000)   NOT NULL,
    [AddressLine1]    NVARCHAR (1000)   NULL,
    [AddressLine2]    NVARCHAR (1000)   NULL,
    [AddressLine3]    NVARCHAR (1000)   NULL,
    [AddressLine4]    NVARCHAR (1000)   NULL,
    [AddressLine5]    NVARCHAR (1000)   NULL,
    [PostalCode]      NVARCHAR (1000)   NULL,
    [Country]         NVARCHAR (1000)   NULL,
    [Region]          NVARCHAR (1000)   NULL,
    [Email1]          NVARCHAR (1000)   NULL,
    [Email2]          NVARCHAR (1000)   NULL,
    [Email3]          NVARCHAR (1000)   NULL,
    [Phone1]          NVARCHAR (1000)   NULL,
    [Phone2]          NVARCHAR (1000)   NULL,
    [Phone3]          NVARCHAR (1000)   NULL,
    [DateFrom]        DATETIME2 (7)    NOT NULL,
    [DateTo]          DATETIME2 (7)    NOT NULL,
    [ClientContactId] UNIQUEIDENTIFIER NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_ClientAddress_ClientContactId]
    ON [ClientAddress]([ClientContactId] ASC);


GO
ALTER TABLE [ClientAddress]
    ADD CONSTRAINT [PK_ClientAddress] PRIMARY KEY CLUSTERED ([Id] ASC);


GO
ALTER TABLE [ClientAddress]
    ADD CONSTRAINT [FK_ClientAddress_ClientContact_ClientContactId] FOREIGN KEY ([ClientContactId]) REFERENCES [ClientContact] ([Id]) ON DELETE CASCADE;



SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [TeamHeader] (
    [TenantId]       UNIQUEIDENTIFIER NOT NULL,
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [Status]         NVARCHAR (MAX)   NULL,
    [CreatedDate]    DATETIME2 (7)    NOT NULL,
    [LastUpdateDate] DATETIME2 (7)    NOT NULL,
    [CreatedBy]      UNIQUEIDENTIFIER NOT NULL,
    [LastUpdateBy]   UNIQUEIDENTIFIER NOT NULL,
    [ShortName]      NVARCHAR (MAX)   NULL,
    [LongName]       NVARCHAR (MAX)   NULL,
    [Description]    NVARCHAR (MAX)   NULL,
    [DateFrom]       DATETIME2 (7)    NOT NULL,
    [DateTo]         DATETIME2 (7)    NOT NULL,
    [ParentTeamId]   UNIQUEIDENTIFIER NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_TeamHeader_ParentTeamId]
    ON [TeamHeader]([ParentTeamId] ASC);


GO
ALTER TABLE [TeamHeader]
    ADD CONSTRAINT [PK_TeamHeader] PRIMARY KEY CLUSTERED ([Id] ASC);


GO
ALTER TABLE [TeamHeader]
    ADD CONSTRAINT [FK_TeamHeader_TeamHeader_ParentTeamId] FOREIGN KEY ([ParentTeamId]) REFERENCES [TeamHeader] ([Id]);



SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [TeamDetail] (
    [TenantId]       UNIQUEIDENTIFIER NOT NULL,
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [Status]         NVARCHAR (MAX)   NULL,
    [CreatedDate]    DATETIME2 (7)    NOT NULL,
    [LastUpdateDate] DATETIME2 (7)    NOT NULL,
    [CreatedBy]      UNIQUEIDENTIFIER NOT NULL,
    [LastUpdateBy]   UNIQUEIDENTIFIER NOT NULL,
    [TeamHeaderId]   UNIQUEIDENTIFIER NOT NULL,
    [DateFrom]       DATETIME2 (7)    NOT NULL,
    [DateTo]         DATETIME2 (7)    NOT NULL
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_TeamDetail_TeamHeaderId]
    ON [TeamDetail]([TeamHeaderId] ASC);


GO
ALTER TABLE [TeamDetail]
    ADD CONSTRAINT [PK_TeamDetail] PRIMARY KEY CLUSTERED ([Id] ASC);


GO
ALTER TABLE [TeamDetail]
    ADD CONSTRAINT [FK_TeamDetail_TeamHeader_TeamHeaderId] FOREIGN KEY ([TeamHeaderId]) REFERENCES [TeamHeader] ([Id]) ON DELETE CASCADE;



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


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [LanguageHeader] (
    [TenantId]       UNIQUEIDENTIFIER NOT NULL,
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [Status]         NVARCHAR (25)   NOT NULL,
    [CreatedDate]    DATETIME2 (7)    NOT NULL,
    [LastUpdateDate] DATETIME2 (7)    NOT NULL,
    [CreatedBy]      UNIQUEIDENTIFIER NOT NULL,
    [LastUpdateBy]   UNIQUEIDENTIFIER NOT NULL,
    [TextCode]       BIGINT			  NOT NULL UNIQUE,
    [Text]           NVARCHAR (4000)  NOT NULL,
    [Description]    NVARCHAR (4000)  NULL,
    [DateFrom]       DATETIME2 (7)    NOT NULL,
    [DateTo]         DATETIME2 (7)    NOT NULL
);

GO
ALTER TABLE [LanguageHeader]
    ADD CONSTRAINT [PK_LanguageHeader] PRIMARY KEY CLUSTERED ([Id] ASC);


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [LanguageCode] (
    [TenantId]       UNIQUEIDENTIFIER NOT NULL,
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [Status]         NVARCHAR (25)   NOT NULL,
    [CreatedDate]    DATETIME2 (7)    NOT NULL,
    [LastUpdateDate] DATETIME2 (7)    NOT NULL,
    [CreatedBy]      UNIQUEIDENTIFIER NOT NULL,
    [LastUpdateBy]   UNIQUEIDENTIFIER NOT NULL,
    [LanguageCodeId] NVARCHAR (100)   NOT NULL,
    [Description]    NVARCHAR (4000)  NULL,
    [DateFrom]       DATETIME2 (7)    NOT NULL,
    [DateTo]         DATETIME2 (7)    NOT NULL
);

GO
ALTER TABLE [LanguageCode]
    ADD CONSTRAINT [PK_LanguageCode] PRIMARY KEY CLUSTERED ([Id] ASC);


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [LanguageItem] (
    [TenantId]          UNIQUEIDENTIFIER NOT NULL,
    [Id]                UNIQUEIDENTIFIER NOT NULL,
    [Status]            NVARCHAR (25)    NOT NULL,
    [CreatedDate]       DATETIME2 (7)    NOT NULL,
    [LastUpdateDate]    DATETIME2 (7)    NOT NULL,
    [CreatedBy]         UNIQUEIDENTIFIER NOT NULL,
    [LastUpdateBy]      UNIQUEIDENTIFIER NOT NULL,
    [LanguageHeaderId]  UNIQUEIDENTIFIER NOT NULL,
    [LanguageCodeId]    NVARCHAR (100)   NULL,
    [Text]              NVARCHAR (4000)  NULL,
    [DateFrom]          DATETIME2 (7)    NOT NULL,
    [DateTo]            DATETIME2 (7)    NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_LanguageItem_LanguageHeaderId]
    ON [LanguageItem]([LanguageHeaderId] ASC);


GO
ALTER TABLE [LanguageItem]
    ADD CONSTRAINT [PK_LanguageItem] PRIMARY KEY CLUSTERED ([Id] ASC);


GO
ALTER TABLE [LanguageItem]
    ADD CONSTRAINT [FK_LanguageItem_LanguageHeader_LanguageHeaderId] FOREIGN KEY ([LanguageHeaderId]) REFERENCES [LanguageHeader] ([Id]) ON DELETE CASCADE;



SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [SettingHeader] (
    [TenantId]       UNIQUEIDENTIFIER NOT NULL,
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [Status]         NVARCHAR (25)    NOT NULL,
    [CreatedDate]    DATETIME2 (7)    NOT NULL,
    [LastUpdateDate] DATETIME2 (7)    NOT NULL,
    [CreatedBy]      UNIQUEIDENTIFIER NOT NULL,
    [LastUpdateBy]   UNIQUEIDENTIFIER NOT NULL,
    [Name]           NVARCHAR (100)   NOT NULL,
    [Value]          NVARCHAR (1000)  NOT NULL,
    [DefaultValue]   NVARCHAR (1000)  NOT NULL,
    [SettingType]    NVARCHAR (100)   NOT NULL,
    [Description]    NVARCHAR (4000)  NULL
);

GO
ALTER TABLE [SettingHeader]
    ADD CONSTRAINT [PK_SettingHeader] PRIMARY KEY CLUSTERED ([Id] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_SettingHeader_Name]
    ON [SettingHeader]([Name] ASC);


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [UserSetting] (
    [TenantId]          UNIQUEIDENTIFIER NOT NULL,
    [Id]                UNIQUEIDENTIFIER NOT NULL,
    [Status]            NVARCHAR (25)    NOT NULL,
    [CreatedDate]       DATETIME2 (7)    NOT NULL,
    [LastUpdateDate]    DATETIME2 (7)    NOT NULL,
    [CreatedBy]         UNIQUEIDENTIFIER NOT NULL,
    [LastUpdateBy]      UNIQUEIDENTIFIER NOT NULL,
    [SettingHeaderId]   UNIQUEIDENTIFIER NOT NULL,
    [UserId]            UNIQUEIDENTIFIER NOT NULL,
    [Value]             NVARCHAR (1000)   NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_UserSetting_SettingHeaderId]
    ON [UserSetting]([SettingHeaderId] ASC);


GO
ALTER TABLE [UserSetting]
    ADD CONSTRAINT [PK_UserSetting] PRIMARY KEY CLUSTERED ([Id] ASC);


GO
ALTER TABLE [UserSetting]
    ADD CONSTRAINT [FK_UserSetting_SettingHeader_SettingHeaderId] FOREIGN KEY ([SettingHeaderId]) REFERENCES [SettingHeader] ([Id]) ON DELETE CASCADE;



SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [FolderHeader] (
    [TenantId]                  UNIQUEIDENTIFIER NOT NULL,
    [Id]                        UNIQUEIDENTIFIER NOT NULL,
    [Status]                    NVARCHAR (25)   NULL,
    [CreatedDate]               DATETIME2 (7)    NOT NULL,
	[ClientHeaderId]            UNIQUEIDENTIFIER NOT NULL,
    [LastUpdateDate]            DATETIME2 (7)    NOT NULL,
    [CreatedBy]                 UNIQUEIDENTIFIER NOT NULL,
    [LastUpdateBy]              UNIQUEIDENTIFIER NOT NULL,
    [ShortName]                 NVARCHAR (1000)   NULL,
    [LongName]                  NVARCHAR (1000)   NULL,
    [Description]               NVARCHAR (4000)   NULL,
    [DateFrom]                  DATETIME2 (7)    NOT NULL,
    [DateTo]                    DATETIME2 (7)    NOT NULL,
    [ParentFolderId]            UNIQUEIDENTIFIER NULL,
	[MainUserId]	            UNIQUEIDENTIFIER NOT NULL,
	[TeamHeaderId]	            UNIQUEIDENTIFIER NOT NULL,
    [CompletionDate]			DATETIME2 (7)		NOT NULL,
    [OriginalCompletionDate]	DATETIME2 (7)		NOT NULL,
    [CompletedOnDate]			DATETIME2 (7)		NOT NULL,
	[Completed]					BIT					NOT NULL,
	[SequenceNumber]            INT NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_FolderHeader_ParentFolderId]
    ON [FolderHeader]([ParentFolderId] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_FolderHeader_MainUserId]
    ON [FolderHeader]([MainUserId] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_FolderHeader_TeamHeaderId]
    ON [FolderHeader]([TeamHeaderId] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_FolderHeader_ClientHeaderId]
    ON [FolderHeader]([ClientHeaderId] ASC);

GO
ALTER TABLE [FolderHeader]
    ADD CONSTRAINT [PK_FolderHeader] PRIMARY KEY CLUSTERED ([Id] ASC);


GO
ALTER TABLE [FolderHeader]
    ADD CONSTRAINT [FK_FolderHeader_FolderHeader_ParentFolderId] FOREIGN KEY ([ParentFolderId]) REFERENCES [FolderHeader] ([Id]);
SET ANSI_NULLS ON
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
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [FolderNode] (
    [TenantId]                  UNIQUEIDENTIFIER    NOT NULL,
    [Id]						UNIQUEIDENTIFIER	NOT NULL,
    [Status]					NVARCHAR (25)		NULL,
    [CreatedDate]				DATETIME2 (7)		NOT NULL,
    [LastUpdateDate]			DATETIME2 (7)		NOT NULL,
    [CreatedBy]					UNIQUEIDENTIFIER	NOT NULL,
    [LastUpdateBy]				UNIQUEIDENTIFIER	NOT NULL,
    [FolderHeaderId]			UNIQUEIDENTIFIER	NOT NULL,
    [DateFrom]					DATETIME2 (7)		NOT NULL,
    [DateTo]					DATETIME2 (7)		NOT NULL,
	[Description]				NVARCHAR (4000)		NULL,
	[Title]						NVARCHAR (1000)		NULL,
	[CompletionDate]			DATETIME2 (7)		NOT NULL,
    [OriginalCompletionDate]	DATETIME2 (7)		NOT NULL,
    [CompletedOnDate]			DATETIME2 (7)		NOT NULL,
	[NodeType]					NVARCHAR (1000)		NULL,
    [SequenceNumber]			INT					NOT NULL,
	[Completed]					BIT					NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_FolderNode_FolderHeaderId]
    ON [FolderNode]([FolderHeaderId] ASC);


GO
ALTER TABLE [FolderNode]
    ADD CONSTRAINT [PK_FolderNode] PRIMARY KEY CLUSTERED ([Id] ASC);


GO
ALTER TABLE [FolderNode]
    ADD CONSTRAINT [FK_FolderNode_FolderHeader_FolderHeaderId] FOREIGN KEY ([FolderHeaderId]) REFERENCES [FolderHeader] ([Id]) ON DELETE CASCADE;

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [FolderNodeDetail] (
    [TenantId]       UNIQUEIDENTIFIER NOT NULL,
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [Status]         NVARCHAR (25)   NULL,
    [CreatedDate]    DATETIME2 (7)    NOT NULL,
    [LastUpdateDate] DATETIME2 (7)    NOT NULL,
    [CreatedBy]      UNIQUEIDENTIFIER NOT NULL,
    [LastUpdateBy]   UNIQUEIDENTIFIER NOT NULL,
    [FolderNodeId]   UNIQUEIDENTIFIER NOT NULL,
    [DateFrom]       DATETIME2 (7)    NOT NULL,
    [DateTo]         DATETIME2 (7)    NOT NULL,
	[Data]           NVARCHAR (MAX)   NULL,
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_FolderNodeDetail_FolderNodeId]
    ON [FolderNodeDetail]([FolderNodeId] ASC);


GO
ALTER TABLE [FolderNodeDetail]
    ADD CONSTRAINT [PK_FolderNodeDetail] PRIMARY KEY CLUSTERED ([Id] ASC);


GO
ALTER TABLE [FolderNodeDetail]
    ADD CONSTRAINT [FK_FolderNodeDetail_FolderNode_FolderNodeId] FOREIGN KEY ([FolderNodeId]) REFERENCES [FolderNode] ([Id]) ON DELETE CASCADE;
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


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [TaskGroupHeader] (
    [TenantId]                  UNIQUEIDENTIFIER NOT NULL,
    [Id]                        UNIQUEIDENTIFIER NOT NULL,
    [Status]                    NVARCHAR (25)   NULL,
    [CreatedDate]               DATETIME2 (7)    NOT NULL,
	[ClientHeaderId]            UNIQUEIDENTIFIER NOT NULL,
    [LastUpdateDate]            DATETIME2 (7)    NOT NULL,
    [CreatedBy]                 UNIQUEIDENTIFIER NOT NULL,
    [LastUpdateBy]              UNIQUEIDENTIFIER NOT NULL,
    [ShortName]                 NVARCHAR (1000)   NULL,
    [LongName]                  NVARCHAR (1000)   NULL,
    [Description]               NVARCHAR (4000)   NULL,
    [DateFrom]                  DATETIME2 (7)    NOT NULL,
    [DateTo]                    DATETIME2 (7)    NOT NULL,
    [ParentTaskGroupId]         UNIQUEIDENTIFIER NULL,
	[MainUserId]	            UNIQUEIDENTIFIER NOT NULL,
	[TeamHeaderId]	            UNIQUEIDENTIFIER NOT NULL,
    [CompletionDate]			DATETIME2 (7)		NOT NULL,
    [OriginalCompletionDate]	DATETIME2 (7)		NOT NULL,
    [CompletedOnDate]			DATETIME2 (7)		NOT NULL,
	[Completed]					BIT					NOT NULL,
	[SequenceNumber]            INT NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_TaskGroupHeader_ParentTaskGroupId]
    ON [TaskGroupHeader]([ParentTaskGroupId] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_TaskGroupHeader_MainUserId]
    ON [TaskGroupHeader]([MainUserId] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_TaskGroupHeader_TeamHeaderId]
    ON [TaskGroupHeader]([TeamHeaderId] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_TaskGroupHeader_ClientHeaderId]
    ON [TaskGroupHeader]([ClientHeaderId] ASC);

GO
ALTER TABLE [TaskGroupHeader]
    ADD CONSTRAINT [PK_TaskGroupHeader] PRIMARY KEY CLUSTERED ([Id] ASC);


GO
ALTER TABLE [TaskGroupHeader]
    ADD CONSTRAINT [FK_TaskGroupHeader_TaskGroupHeader_ParentTaskGroupId] FOREIGN KEY ([ParentTaskGroupId]) REFERENCES [TaskGroupHeader] ([Id]);
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
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [TaskHeader] (
    [TenantId]                  UNIQUEIDENTIFIER    NOT NULL,
    [Id]						UNIQUEIDENTIFIER	NOT NULL,
    [Status]					NVARCHAR (25)		NULL,
    [CreatedDate]				DATETIME2 (7)		NOT NULL,
    [LastUpdateDate]			DATETIME2 (7)		NOT NULL,
    [CreatedBy]					UNIQUEIDENTIFIER	NOT NULL,
    [LastUpdateBy]				UNIQUEIDENTIFIER	NOT NULL,
    [TaskGroupHeaderId]			UNIQUEIDENTIFIER	NOT NULL,
    [DateFrom]					DATETIME2 (7)		NOT NULL,
    [DateTo]					DATETIME2 (7)		NOT NULL,
	[Description]				NVARCHAR (4000)		NULL,
	[Title]						NVARCHAR (1000)		NULL,
	[CompletionDate]			DATETIME2 (7)		NOT NULL,
    [OriginalCompletionDate]	DATETIME2 (7)		NOT NULL,
    [CompletedOnDate]			DATETIME2 (7)		NOT NULL,
	[TaskType]					NVARCHAR (1000)		NULL,
    [SequenceNumber]			INT					NOT NULL,
	[Completed]					BIT					NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_TaskHeader_TaskGroupHeaderId]
    ON [TaskHeader]([TaskGroupHeaderId] ASC);


GO
ALTER TABLE [TaskHeader]
    ADD CONSTRAINT [PK_TaskHeader] PRIMARY KEY CLUSTERED ([Id] ASC);


GO
ALTER TABLE [TaskHeader]
    ADD CONSTRAINT [FK_TaskHeader_TaskGroupHeader_TaskGroupHeaderId] FOREIGN KEY ([TaskGroupHeaderId]) REFERENCES [TaskGroupHeader] ([Id]) ON DELETE CASCADE;

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
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [TeamTask] (
    [TenantId]       UNIQUEIDENTIFIER NOT NULL,
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [Status]         NVARCHAR (25)   NULL,
    [CreatedDate]    DATETIME2 (7)    NOT NULL,
    [LastUpdateDate] DATETIME2 (7)    NOT NULL,
    [CreatedBy]      UNIQUEIDENTIFIER NOT NULL,
    [LastUpdateBy]   UNIQUEIDENTIFIER NOT NULL,
    [TaskGroupHeaderId] UNIQUEIDENTIFIER NOT NULL,
    [TeamHeaderId]   UNIQUEIDENTIFIER NOT NULL,
    [DateFrom]       DATETIME2 (7)    NOT NULL,
    [DateTo]         DATETIME2 (7)    NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_TeamTask_TeamHeaderId]
    ON [TeamTask]([TeamHeaderId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TeamTask_TaskGroupHeaderId]
    ON [TeamTask]([TaskGroupHeaderId] ASC);


GO
ALTER TABLE [TeamTask]
    ADD CONSTRAINT [PK_TeamTask] PRIMARY KEY CLUSTERED ([Id] ASC);


GO
ALTER TABLE [TeamTask]
    ADD CONSTRAINT [FK_TeamTask_TaskGroupHeader_TaskGroupHeaderId] FOREIGN KEY ([TaskGroupHeaderId]) REFERENCES [TaskGroupHeader] ([Id]) ON DELETE CASCADE;