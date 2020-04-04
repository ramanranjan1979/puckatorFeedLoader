 
CREATE PROCEDURE [dbo].[GetSetting]   

AS   

BEGIN

SET NOCOUNT ON;  

 SELECT ID , KeyName,KeyValue FROM DBO.Setting WITH (NOLOCK)  WHERE IsActive=1 order by ID


END




