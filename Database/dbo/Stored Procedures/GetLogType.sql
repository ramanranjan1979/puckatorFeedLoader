 
CREATE PROCEDURE [dbo].[GetLogType]   

AS   

BEGIN

SET NOCOUNT ON;  

 SELECT ID , [Name] FROM DBO.LogType WITH (NOLOCK) order by ID


END




