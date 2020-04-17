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


