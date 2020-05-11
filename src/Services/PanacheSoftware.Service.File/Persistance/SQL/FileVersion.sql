SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [FileVersion] (
    [TenantId]                  UNIQUEIDENTIFIER    NOT NULL,
    [Id]						UNIQUEIDENTIFIER	NOT NULL,
    [Status]					NVARCHAR (25)		NULL,
    [CreatedDate]				DATETIME2 (7)		NOT NULL,
    [LastUpdateDate]			DATETIME2 (7)		NOT NULL,
    [CreatedBy]					UNIQUEIDENTIFIER	NOT NULL,
    [LastUpdateBy]				UNIQUEIDENTIFIER	NOT NULL,
    [FileHeaderId]              UNIQUEIDENTIFIER    NOT NULL,
    [Content]			        VARBINARY (MAX) 	NOT NULL,
    [URI]				        NVARCHAR (4000)		NULL,
	[UntrustedName]				NVARCHAR (1000)		NULL,
    [TrustedName]				NVARCHAR (1000)		NULL,
	[UploadDate]			    DATETIME2 (7)		NOT NULL,
    [VersionNumber]		    	INT					NOT NULL,
	[Size]			    		BIGINT				NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_FileVersion_FileHeaderId]
    ON [FileVersion]([FileHeaderId] ASC);


GO
ALTER TABLE [FileVersion]
    ADD CONSTRAINT [PK_FileVersion] PRIMARY KEY CLUSTERED ([Id] ASC);


GO
ALTER TABLE [FileVersion]
    ADD CONSTRAINT [FK_FileVersion_FileHeader_FileHeaderId] FOREIGN KEY ([FileHeaderId]) REFERENCES [FileHeader] ([Id]) ON DELETE CASCADE;

GO
