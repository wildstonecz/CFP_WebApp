CREATE PROCEDURE [cfp].[p_getCategory]
    @pageNumber int = 0,
    @pageSize int=10
AS
BEGIN
    SELECT [Category_SK],
    [CategoryName],
    [CategoryCount]
    FROM cfp.wikicfp_Categories
    ORDER BY [CategoryCount] 
    OFFSET @pageNumber*@pageSize ROWS
    FETCH NEXT @pageSize ROWS ONLY;
END
