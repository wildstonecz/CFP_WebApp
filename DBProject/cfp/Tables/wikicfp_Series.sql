CREATE TABLE [cfp].[wikicfp_Series] (
    [Series_SK]       INT            NOT NULL,
    [SeriesShortName] NVARCHAR (50)  NULL,
    [SeriesName]      NVARCHAR (255) NULL,
    [SeriesUrl]       NVARCHAR (255) NULL,
    [IsScrapped]      BIT            NOT NULL
);

