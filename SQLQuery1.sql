Create Role DB_Zugriff_CISO
Create Role DB_Zugriff_Admin
Create Role DB_Zugriff_Normal_User
Create Role DB_Zugriff_SBA_User



EXEC sp_addrolemember 'DB_Zugriff_CISO','Captain'

Grant Select,Insert,Update,delete on dbo.ISB_BIA_Applikationen to DB_Zugriff_CISO
Grant Select,Insert,Update,delete on dbo.ISB_BIA_Delta_Analyse to DB_Zugriff_CISO
Grant Select,Insert,Update,delete on dbo.ISB_BIA_Informationssegmente to DB_Zugriff_CISO
Grant Select,Insert,Update,delete on dbo.ISB_BIA_Informationssegmente_Attribute to DB_Zugriff_CISO
Grant Select,Insert,Update,delete on dbo.ISB_BIA_Lock to DB_Zugriff_CISO
Grant Select,Insert,Update,delete on dbo.ISB_BIA_Log to DB_Zugriff_CISO
Grant Select,Insert,Update,delete on dbo.ISB_BIA_OEs to DB_Zugriff_CISO
Grant Select,Insert,Update,delete on dbo.ISB_BIA_Prozesse to DB_Zugriff_CISO
Grant Select,Insert,Update,delete on dbo.ISB_BIA_Prozesse_Applikationen to DB_Zugriff_CISO
Grant Select,Insert,Update,delete on dbo.ISB_BIA_Settings to DB_Zugriff_CISO

EXEC sp_addrolemember 'DB_Zugriff_Admin','Captain'

Grant Select,Insert,Update,delete on dbo.ISB_BIA_Applikationen to DB_Zugriff_Admin
Grant Select,Insert,Update,delete on dbo.ISB_BIA_Delta_Analyse to DB_Zugriff_Admin
Grant Select,Insert,Update,delete on dbo.ISB_BIA_Informationssegmente to DB_Zugriff_Admin
Grant Select,Insert,Update,delete on dbo.ISB_BIA_Informationssegmente_Attribute to DB_Zugriff_Admin
Grant Select,Insert,Update,delete on dbo.ISB_BIA_Lock to DB_Zugriff_Admin
Grant Select,Insert,Update,delete on dbo.ISB_BIA_Log to DB_Zugriff_Admin
Grant Select,Insert,Update,delete on dbo.ISB_BIA_OEs to DB_Zugriff_Admin
Grant Select,Insert,Update,delete on dbo.ISB_BIA_Prozesse to DB_Zugriff_Admin
Grant Select,Insert,Update,delete on dbo.ISB_BIA_Prozesse_Applikationen to DB_Zugriff_Admin
Grant Select,Insert,Update,delete on dbo.ISB_BIA_Settings to DB_Zugriff_Admin

EXEC sp_addrolemember 'DB_Zugriff_SBA_User','Captain'

Grant Select,Insert,Update,delete on dbo.ISB_BIA_Applikationen to DB_Zugriff_SBA_User
Grant Select,Insert,Update,delete on dbo.ISB_BIA_Delta_Analyse to DB_Zugriff_SBA_User
Grant Select,Insert,Update,delete on dbo.ISB_BIA_Informationssegmente to DB_Zugriff_SBA_User
Grant Select,Insert,Update,delete on dbo.ISB_BIA_Informationssegmente_Attribute to DB_Zugriff_SBA_User
Grant Select,Insert,Update,delete on dbo.ISB_BIA_Lock to DB_Zugriff_SBA_User
Grant Select,Insert,Update,delete on dbo.ISB_BIA_Log to DB_Zugriff_SBA_User
Grant Select,Insert,Update,delete on dbo.ISB_BIA_OEs to DB_Zugriff_SBA_User
Grant Select,Insert,Update,delete on dbo.ISB_BIA_Prozesse to DB_Zugriff_SBA_User
Grant Select,Insert,Update,delete on dbo.ISB_BIA_Prozesse_Applikationen to DB_Zugriff_SBA_User
Grant Select,Insert,Update,delete on dbo.ISB_BIA_Settings to DB_Zugriff_SBA_User

EXEC sp_addrolemember 'DB_Zugriff_Normal_User','Captain'

Grant Select,Insert,Update,delete on dbo.ISB_BIA_Applikationen to DB_Zugriff_Normal_User
Grant Select,Insert,Update,delete on dbo.ISB_BIA_Delta_Analyse to DB_Zugriff_Normal_User
Grant Select,Insert,Update,delete on dbo.ISB_BIA_Informationssegmente to DB_Zugriff_Normal_User
Grant Select,Insert,Update,delete on dbo.ISB_BIA_Informationssegmente_Attribute to DB_Zugriff_Normal_User
Grant Select,Insert,Update,delete on dbo.ISB_BIA_Lock to DB_Zugriff_Normal_User
Grant Select,Insert,Update,delete on dbo.ISB_BIA_Log to DB_Zugriff_Normal_User
Grant Select,Insert,Update,delete on dbo.ISB_BIA_OEs to DB_Zugriff_Normal_User
Grant Select,Insert,Update,delete on dbo.ISB_BIA_Prozesse to DB_Zugriff_Normal_User
Grant Select,Insert,Update,delete on dbo.ISB_BIA_Prozesse_Applikationen to DB_Zugriff_Normal_User
Grant Select,Insert,Update,delete on dbo.ISB_BIA_Settings to DB_Zugriff_Normal_User