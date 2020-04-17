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
