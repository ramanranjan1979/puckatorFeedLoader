  
CREATE PROCEDURE [dbo].[GetProductImageFileNames]   
(
    @ProductModel varchar(20)   
    
)

AS   

BEGIN

SET NOCOUNT ON;  

 SELECT  [Id]
      ,[Model]
      ,[FileName]
      ,[Number]
      ,[IsMain]
      ,[CreatedDate]
      ,[UpdatedDate]
      ,[Active]
  FROM [PuckSource].[dbo].[ImageMaster] WITH (NOLOCK) 
  WHERE 
  Active=1 
  AND Model=@ProductModel 
  AND ProcessedDateTime IS NULL
 
  ORDER BY ISNULL(Number,0)

END

