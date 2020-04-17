SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [TeamDetail] (
    [TenantId]       UNIQUEIDENTIFIER NOT NULL,
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [Status]         NVARCHAR (MAX)   NULL,
    [CreatedDate]    DATETIME2 (7)    NOT NULL,
    [LastUpdateDate] DATETIME2 (7)    NOT NULL,
    [CreatedBy]      UNIQUEIDENTIFIER NOT NULL,
    [LastUpdateBy]   UNIQUEIDENTIFIER NOT NULL,
    [TeamHeaderId]   UNIQUEIDENTIFIER NOT NULL,
    [DateFrom]       DATETIME2 (7)    NOT NULL,
    [DateTo]         DATETIME2 (7)    NOT NULL
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_TeamDetail_TeamHeaderId]
    ON [TeamDetail]([TeamHeaderId] ASC);


GO
ALTER TABLE [TeamDetail]
    ADD CONSTRAINT [PK_TeamDetail] PRIMARY KEY CLUSTERED ([Id] ASC);


GO
ALTER TABLE [TeamDetail]
    ADD CONSTRAINT [FK_TeamDetail_TeamHeader_TeamHeaderId] FOREIGN KEY ([TeamHeaderId]) REFERENCES [TeamHeader] ([Id]) ON DELETE CASCADE;


