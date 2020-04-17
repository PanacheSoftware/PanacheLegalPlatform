SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [IdentityUserTokens] (
    [UserId]        UNIQUEIDENTIFIER NOT NULL,
    [LoginProvider] NVARCHAR (450)   NOT NULL,
    [Name]          NVARCHAR (450)   NOT NULL,
    [Value]         NVARCHAR (4000)   NULL
);

