SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [FileLink] (
    [TenantId]       UNIQUEIDENTIFIER NOT NULL,
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [Status]         NVARCHAR (25)    NULL,
    [CreatedDate]    DATETIME2 (7)    NOT NULL,
    [LastUpdateDate] DATETIME2 (7)    NOT NULL,
    [CreatedBy]      UNIQUEIDENTIFIER NOT NULL,
    [LastUpdateBy]   UNIQUEIDENTIFIER NOT NULL,
    [LinkId]         UNIQUEIDENTIFIER NOT NULL,
    [LinkType]	     NVARCHAR (1000)  NULL,
    [FileHeaderId]   UNIQUEIDENTIFIER NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_FileLink_FileHeaderId]
    ON [FileLink]([FileHeaderId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FileLink_LinkId]
    ON [FileLink]([LinkId] ASC);


GO
ALTER TABLE [FileLink]
    ADD CONSTRAINT [PK_FileLink] PRIMARY KEY CLUSTERED ([Id] ASC);


GO
ALTER TABLE [FileLink]
    ADD CONSTRAINT [FK_FileLink_FileHeader_FileHeaderId] FOREIGN KEY ([FileHeaderId]) REFERENCES [FileHeader] ([Id]) ON DELETE CASCADE;