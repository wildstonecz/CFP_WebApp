CREATE TABLE [dbo].[publications] (
    [publ_id]    INT            NOT NULL,
    [title]      NVARCHAR (255) NULL,
    [journal_id] INT            NULL,
    [ee]         NVARCHAR (255) NULL,
    [month]      INT            NULL,
    [year]       INT            NULL,
    [timeId]     INT            NULL,
    [EST]        INT            NULL
);

