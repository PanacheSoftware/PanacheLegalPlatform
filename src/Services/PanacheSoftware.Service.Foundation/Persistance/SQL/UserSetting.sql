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


