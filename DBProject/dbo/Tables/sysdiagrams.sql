CREATE TABLE [dbo].[sysdiagrams] (
    [name]         NVARCHAR (128)  NOT NULL,
    [principal_id] INT             NOT NULL,
    [diagram_id]   INT             NOT NULL,
    [version]      INT             NULL,
    [definition]   VARBINARY (MAX) NULL
);

