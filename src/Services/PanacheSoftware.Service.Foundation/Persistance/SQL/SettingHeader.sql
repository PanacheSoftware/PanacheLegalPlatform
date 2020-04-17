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

