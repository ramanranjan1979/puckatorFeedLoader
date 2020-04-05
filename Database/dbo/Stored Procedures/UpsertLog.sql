  
CREATE PROCEDURE [dbo].[UpsertLog]   
(
    @Id INT,   
    @LogType INT,
	@RequestData varchar(Max),
	@ResponseData varchar(Max),
	@ErrorCode varchar(50)	
)

AS   

BEGIN

SET NOCOUNT ON;  

Declare @inserted table 
(
	[Id] INT, 
    [Logtype] INT NOT NULL, 
    [RequestData] VARCHAR(MAX) NULL, 
    [ResponseData] VARCHAR(MAX) NULL, 
    [ErrorCode] VARCHAR(50) NULL, 
    [CreatedDate] DATETIME NOT NULL, 
    [CreatedBy] VARCHAR(50) NULL, 
    [UpdatedDate] DATETIME NULL
)

 IF EXISTS(SELECT TOP 1 1 FROM DBO.[Log] WITH (NOLOCK) WHERE Id=@Id)
 BEGIN
	UPDATE DBO.[Log]
		SET 
		Logtype=@LogType,
		RequestData=@RequestData,
		ResponseData=@ResponseData,
		ErrorCode=@ErrorCode,
		UpdatedDate=getdate()
		WHERE ID=@Id
 END
 ELSE
 BEGIN
	INSERT INTO DBO.[log](Logtype,RequestData,ResponseData,ErrorCode,CreatedDate,CreatedBy,UpdatedDate) 
	OUTPUT 
		inserted.Id,
		inserted.Logtype, 
		inserted.RequestData,
		inserted.ResponseData,
		inserted.ErrorCode,
		inserted.CreatedDate,
		inserted.CreatedBy,
		inserted.UpdatedDate
	INTO @inserted
	VALUES(@LogType,@RequestData,@ResponseData,@ErrorCode,Getdate(),null,null)
 END
 
 SELECT ID from @inserted

END

