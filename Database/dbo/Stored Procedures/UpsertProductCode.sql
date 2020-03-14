  
CREATE PROCEDURE [dbo].[UpsertProductCode]   
(
	@Model varchar(200),
	@EAN varchar(200),
	@Active bit
)

AS   

BEGIN

SET NOCOUNT ON;  

 IF EXISTS(SELECT TOP 1 1 FROM DBO.ProductCode WITH (NOLOCK) WHERE Model=@Model)
 BEGIN
	UPDATE DBO.ProductCode
		SET 
		EAN=@EAN,
		UpdatedDate=GETDATE()
		WHERE Model=@Model
 END
 ELSE
 BEGIN
	INSERT INTO DBO.ProductCode
	(
		Model,EAN,Active,CreatedDate
	) VALUES(@Model,@EAN,@Active,getdate())
 END


END

