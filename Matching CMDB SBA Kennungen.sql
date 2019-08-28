--Drop Table Test_App
--SELECT * INTO [dbo].[Test_App] 
--FROM [dbo].[ISB_BIA_Applikationen]
--go
--Alter table Test_App 
--add Kennung varchar(40)
--update Test_App set Kennung = "a" where 

--UPDATE Test_App t1
--INNER JOIN Test_Relation as t2 ON t1.App_ID = t2.App_ID
--SET t1.Kennung = t2.Kennung

--Befüllen der Applikationstabelle mit den Kennungen der CMDB über die in Excel erfassten Relationen (ISB...original.xlsx)
UPDATE t1
SET t1.Kennung = t2.Kennung
from Test_App as t1
INNER JOIN Test_Relation as t2 ON t1.Applikation_ID = t2.App_ID
