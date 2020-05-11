SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [FileHeader] (
    [TenantId]                  UNIQUEIDENTIFIER NOT NULL,
    [Id]                        UNIQUEIDENTIFIER NOT NULL,
    [Status]                    NVARCHAR (25)   NULL,
    [CreatedDate]               DATETIME2 (7)    NOT NULL,
    [LastUpdateDate]            DATETIME2 (7)    NOT NULL,
    [CreatedBy]                 UNIQUEIDENTIFIER NOT NULL,
    [LastUpdateBy]              UNIQUEIDENTIFIER NOT NULL
);


GO

ALTER TABLE [FileHeader]
    ADD CONSTRAINT [PK_FileHeader] PRIMARY KEY CLUSTERED ([Id] ASC);
GO