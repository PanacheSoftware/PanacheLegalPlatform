SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [ClientAddress] (
    [TenantId]        UNIQUEIDENTIFIER NOT NULL,
    [Id]              UNIQUEIDENTIFIER NOT NULL,
    [Status]          NVARCHAR (25)   NOT NULL,
    [CreatedDate]     DATETIME2 (7)    NOT NULL,
    [LastUpdateDate]  DATETIME2 (7)    NOT NULL,
    [CreatedBy]       UNIQUEIDENTIFIER NOT NULL,
    [LastUpdateBy]    UNIQUEIDENTIFIER NOT NULL,
    [AddressType]     NVARCHAR (1000)   NOT NULL,
    [AddressLine1]    NVARCHAR (1000)   NULL,
    [AddressLine2]    NVARCHAR (1000)   NULL,
    [AddressLine3]    NVARCHAR (1000)   NULL,
    [AddressLine4]    NVARCHAR (1000)   NULL,
    [AddressLine5]    NVARCHAR (1000)   NULL,
    [PostalCode]      NVARCHAR (1000)   NULL,
    [Country]         NVARCHAR (1000)   NULL,
    [Region]          NVARCHAR (1000)   NULL,
    [Email1]          NVARCHAR (1000)   NULL,
    [Email2]          NVARCHAR (1000)   NULL,
    [Email3]          NVARCHAR (1000)   NULL,
    [Phone1]          NVARCHAR (1000)   NULL,
    [Phone2]          NVARCHAR (1000)   NULL,
    [Phone3]          NVARCHAR (1000)   NULL,
    [DateFrom]        DATETIME2 (7)    NOT NULL,
    [DateTo]          DATETIME2 (7)    NOT NULL,
    [ClientContactId] UNIQUEIDENTIFIER NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_ClientAddress_ClientContactId]
    ON [ClientAddress]([ClientContactId] ASC);


GO
ALTER TABLE [ClientAddress]
    ADD CONSTRAINT [PK_ClientAddress] PRIMARY KEY CLUSTERED ([Id] ASC);


GO
ALTER TABLE [ClientAddress]
    ADD CONSTRAINT [FK_ClientAddress_ClientContact_ClientContactId] FOREIGN KEY ([ClientContactId]) REFERENCES [ClientContact] ([Id]) ON DELETE CASCADE;


