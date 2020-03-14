  
CREATE PROCEDURE [dbo].[UpsertProductImage]   
(
	@Model varchar(20),
	@FileName varchar(200),
	@Number int,
	@IsMain bit,
	@Active bit
	
)

AS   

BEGIN

SET NOCOUNT ON;  

 IF EXISTS(SELECT TOP 1 1 FROM DBO.ImageMaster WITH (NOLOCK) WHERE Model=@model and [FileName]=@FileName)
 BEGIN
	UPDATE DBO.ImageMaster
		SET 
		FileName=@FileName,
		Number=@Number,
		IsMain=@IsMain,
		Active=@Active,
		UpdatedDate=GETDATE()
		WHERE model=@Model AND [FileName]=@fileName
 END
 ELSE
 BEGIN
	INSERT INTO DBO.ImageMaster
	(
		Model,[FileName],Number,ISMain,CreatedDate,Active
	) VALUES(@Model,@FileName,@Number,@ismain,getdate(),@Active)
 END


END

