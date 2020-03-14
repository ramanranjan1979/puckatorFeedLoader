  
CREATE PROCEDURE [dbo].[GetCategoryByParentCategoryId]   
(
    @ParentCategoryId INT  
    
)

AS   

BEGIN

SET NOCOUNT ON;  

 SELECT ID , CategoryId ,ParentCategoryId ,[Description], Active FROM DBO.CategoryMaster WITH (NOLOCK) WHERE ParentCategoryId=@ParentCategoryId order by [Description]


END

