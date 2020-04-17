SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [ClientContact] (
    [TenantId]       UNIQUEIDENTIFIER NOT NULL,
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [Status]         NVARCHAR (25)   NOT NULL,
    [CreatedDate]    DATETIME2 (7)    NOT NULL,
    [LastUpdateDate] DATETIME2 (7)    NOT NULL,
    [CreatedBy]      UNIQUEIDENTIFIER NOT NULL,
    [LastUpdateBy]   UNIQUEIDENTIFIER NOT NULL,
    [ClientHeaderId] UNIQUEIDENTIFIER NOT NULL,
    [Title]          NVARCHAR (1000)   NULL,
    [FirstName]      NVARCHAR (1000)   NULL,
    [MiddleName]     NVARCHAR (1000)   NULL,
    [LastName]       NVARCHAR (1000)   NULL,
    [Email]          NVARCHAR (1000)   NULL,
    [Phone]          NVARCHAR (1000)   NULL,
    [DateFrom]       DATETIME2 (7)    NOT NULL,
    [DateTo]         DATETIME2 (7)    NOT NULL,
    [MainContact]    BIT              NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_ClientContact_ClientHeaderId]
    ON [ClientContact]([ClientHeaderId] ASC);


GO
ALTER TABLE [ClientContact]
    ADD CONSTRAINT [PK_ClientContact] PRIMARY KEY CLUSTERED ([Id] ASC);


GO
ALTER TABLE [ClientContact]
    ADD CONSTRAINT [FK_ClientContact_ClientHeader_ClientHeaderId] FOREIGN KEY ([ClientHeaderId]) REFERENCES [ClientHeader] ([Id]) ON DELETE CASCADE;


