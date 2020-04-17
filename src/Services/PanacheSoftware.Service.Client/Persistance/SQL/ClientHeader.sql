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

