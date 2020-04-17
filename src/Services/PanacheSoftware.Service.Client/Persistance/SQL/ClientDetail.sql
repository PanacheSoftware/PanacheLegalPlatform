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


