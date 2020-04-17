SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [TeamHeader] (
    [TenantId]       UNIQUEIDENTIFIER NOT NULL,
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [Status]         NVARCHAR (MAX)   NULL,
    [CreatedDate]    DATETIME2 (7)    NOT NULL,
    [LastUpdateDate] DATETIME2 (7)    NOT NULL,
    [CreatedBy]      UNIQUEIDENTIFIER NOT NULL,
    [LastUpdateBy]   UNIQUEIDENTIFIER NOT NULL,
    [ShortName]      NVARCHAR (MAX)   NULL,
    [LongName]       NVARCHAR (MAX)   NULL,
    [Description]    NVARCHAR (MAX)   NULL,
    [DateFrom]       DATETIME2 (7)    NOT NULL,
    [DateTo]         DATETIME2 (7)    NOT NULL,
    [ParentTeamId]   UNIQUEIDENTIFIER NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_TeamHeader_ParentTeamId]
    ON [TeamHeader]([ParentTeamId] ASC);


GO
ALTER TABLE [TeamHeader]
    ADD CONSTRAINT [PK_TeamHeader] PRIMARY KEY CLUSTERED ([Id] ASC);


GO
ALTER TABLE [TeamHeader]
    ADD CONSTRAINT [FK_TeamHeader_TeamHeader_ParentTeamId] FOREIGN KEY ([ParentTeamId]) REFERENCES [TeamHeader] ([Id]);


