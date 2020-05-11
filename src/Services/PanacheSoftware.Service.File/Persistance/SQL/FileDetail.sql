SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [FileDetail] (
    [TenantId]       UNIQUEIDENTIFIER NOT NULL,
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [Status]         NVARCHAR (25)   NOT NULL,
    [CreatedDate]    DATETIME2 (7)    NOT NULL,
    [LastUpdateDate] DATETIME2 (7)    NOT NULL,
    [CreatedBy]      UNIQUEIDENTIFIER NOT NULL,
    [LastUpdateBy]   UNIQUEIDENTIFIER NOT NULL,
    [FileHeaderId]   UNIQUEIDENTIFIER NOT NULL,
    [FileTitle]      NVARCHAR (1000)   NULL,
    [Description]    NVARCHAR (1000)   NULL,
    [FileType]       NVARCHAR (1000)   NULL,
    [FileExtension]  NVARCHAR (1000)   NULL
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_FileDetail_FileHeaderId]
    ON [FileDetail]([FileHeaderId] ASC);


GO
ALTER TABLE [FileDetail]
    ADD CONSTRAINT [PK_FileDetail] PRIMARY KEY CLUSTERED ([Id] ASC);


GO
ALTER TABLE [FileDetail]
    ADD CONSTRAINT [FK_FileDetail_FileHeader_FileHeaderId] FOREIGN KEY ([FileHeaderId]) REFERENCES [FileHeader] ([Id]) ON DELETE CASCADE;

GO


