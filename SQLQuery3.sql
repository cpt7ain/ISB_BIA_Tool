EXEC sp_addrolemember 'DB_Zugriff','ISB_BIA_Tool_CISO'
EXEC sp_addrolemember 'DB_Zugriff','ISB_BIA_Tool_ADMIN'
EXEC sp_addrolemember 'DB_Zugriff','ISB_BIA_Tool_NORMAL'
EXEC sp_addrolemember 'DB_Zugriff','ISB_BIA_Tool_SBAs'


Grant Select,Insert,Update,delete on dbo.ISB_BIA_Applikationen to DB_Zugriff
Grant Select,Insert,Update,delete on dbo.ISB_BIA_Delta_Analyse to DB_Zugriff
Grant Select,Insert,Update,delete on dbo.ISB_BIA_Informationssegmente to DB_Zugriff
Grant Select,Insert,Update,delete on dbo.ISB_BIA_Informationssegmente_Attribute to DB_Zugriff
Grant Select,Insert,Update,delete on dbo.ISB_BIA_Lock to DB_Zugriff
Grant Select,Insert,Update,delete on dbo.ISB_BIA_Log to DB_Zugriff
Grant Select,Insert,Update,delete on dbo.ISB_BIA_OEs to DB_Zugriff
Grant Select,Insert,Update,delete on dbo.ISB_BIA_Prozesse to DB_Zugriff
Grant Select,Insert,Update,delete on dbo.ISB_BIA_Prozesse_Applikationen to DB_Zugriff
Grant Select,Insert,Update,delete on dbo.ISB_BIA_Settings to DB_Zugriff