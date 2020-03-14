  
CREATE PROCEDURE [dbo].[UpsertCategory]   
(
    @CategoryId INT,   
    @ParentCategoryId INT,
	@Description nvarchar(200),
	@Active bit
)

AS   

BEGIN

SET NOCOUNT ON;  

 IF EXISTS(SELECT TOP 1 1 FROM DBO.CategoryMaster WITH (NOLOCK) WHERE CategoryId=@CategoryId)
 BEGIN
	UPDATE DBO.CategoryMaster
		SET 
		ParentCategoryId=@ParentCategoryId,
		[Description]=@Description,
		UpdatedDate=GETDATE()
		WHERE categoryID=@CategoryId
 END
 ELSE
 BEGIN
	INSERT INTO DBO.CategoryMaster(CategoryId,ParentCategoryId,[Description],Active,CreatedDate) VALUES(@CategoryId,@ParentCategoryId,@Description,@Active,Getdate())
 END


END

