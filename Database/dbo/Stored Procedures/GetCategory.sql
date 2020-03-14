  
CREATE PROCEDURE [dbo].[GetCategory]   
(
    @CategoryId INT=null   
    
)

AS   

BEGIN

SET NOCOUNT ON;  

 SELECT ID , CategoryId ,ParentCategoryId ,[Description], Active FROM DBO.CategoryMaster WITH (NOLOCK) WHERE (@CategoryId IS NULL OR ID=@CategoryId) order by [Description]


END

