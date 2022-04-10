CREATE TABLE [cfp].[wikicfp_CFP] (
    [CFP_SK]             INT            NOT NULL,
    [CFP_URL]            NVARCHAR (255) NOT NULL,
    [CFP_Series]         INT            NULL,
    [CFP_text]           NVARCHAR (MAX) NULL,
    [wasExtracted]       BIT            NULL,
    [SubmissionDeadline] DATETIME       NULL,
    [When]               NVARCHAR (255) NULL,
    [Where]              NVARCHAR (255) NULL
);

