IF DATABASE_PRINCIPAL_ID('DB_Zugriff_CISO') IS NULL 
Create Role DB_Zugriff_CISO
go
IF DATABASE_PRINCIPAL_ID('DB_Zugriff_CISO') IS NULL 
Create Role DB_Zugriff_Admin
go
IF DATABASE_PRINCIPAL_ID('DB_Zugriff_CISO') IS NULL 
Create Role DB_Zugriff_Normal_User
go
IF DATABASE_PRINCIPAL_ID('DB_Zugriff_CISO') IS NULL 
Create Role DB_Zugriff_SBA_User
go

Declare @domName varchar(200);
set @domName = (Select DEFAULT_DOMAIN()[DomainName]);

Declare @Gruppe varchar(250);

print 'Zugriff für P.ABT...CISO_PROD'
Set @Gruppe = 'Captain' --später durch AD-Gruppe ersetzen

EXEC sp_addrolemember 'DB_Zugriff_CISO', @Gruppe

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

print 'Zugriff für P.ABT...Admin_PROD'
Set @Gruppe = 'Captain' --später durch AD-Gruppe ersetzen

EXEC sp_addrolemember 'DB_Zugriff_Admin',@Gruppe

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

print 'Zugriff für P.ABT...SBA_User_PROD'
Set @Gruppe = 'Captain' --später durch AD-Gruppe ersetzen

EXEC sp_addrolemember 'DB_Zugriff_SBA_User',@Gruppe

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

print 'Zugriff für P.ABT...Normal_User_PROD'
Set @Gruppe = 'Captain' --später durch AD-Gruppe ersetzen

EXEC sp_addrolemember 'DB_Zugriff_Normal_User',@Gruppe

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