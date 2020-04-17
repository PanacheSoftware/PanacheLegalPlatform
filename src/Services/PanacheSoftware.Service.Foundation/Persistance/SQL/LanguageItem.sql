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


