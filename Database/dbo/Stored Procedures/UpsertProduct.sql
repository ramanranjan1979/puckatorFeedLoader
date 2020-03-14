  
CREATE PROCEDURE [dbo].[UpsertProduct]   
(
	@ProductId BIGINT,
	@Model varchar(200),
	@EAN varchar(200),
	@Name varchar(200),
	@Description varchar(max),
	@Dimension varchar(max),
	@Price varchar(200),
	@DeliveryCode varchar(200),
	@Quantity varchar(200),
	@Categories varchar(max),
	@Options varchar(max),
	@MOQ varchar(200),
	@ImageUrl varchar(max)
)

AS   

BEGIN

SET NOCOUNT ON;  

 IF EXISTS(SELECT TOP 1 1 FROM DBO.ProductMaster WITH (NOLOCK) WHERE ProductId=@ProductId)
 BEGIN
	UPDATE DBO.ProductMaster
		SET 
		Model=@Model,
		[Description]=@Description,
		EAN=@EAN,
		[Name]=@Name,
		Dimension=@Dimension,
		Price=@Price,
		DeliveryCode=@DeliveryCode,
		Quantity=@Quantity,
		Categories=@Categories,
		Options=@Options,
		MOQ=@MOQ,
		ImageUrl=@ImageUrl,
		UpdatedDate=GETDATE()
		WHERE ProductId=@ProductId
 END
 ELSE
 BEGIN
	INSERT INTO DBO.ProductMaster
	(
		ProductId,Model,EAN,[Name],[Description],Dimension,Price,DeliveryCode,Quantity,Categories,Options,MOQ,ImageUrl,CreatedDate
	) VALUES(@ProductId,@Model,@EAN,@Name,@Description,@Dimension,@Price,@DeliveryCode,@Quantity,@Categories,@Options,@MOQ,@ImageUrl,getdate())
 END


END

