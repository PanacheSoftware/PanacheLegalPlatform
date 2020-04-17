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

