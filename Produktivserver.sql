
--Für PROD
IF DATABASE_PRINCIPAL_ID('DB_Zugriff_CISO') IS NULL 
CREATE ROLE DB_Zugriff_CISO
GO
IF DATABASE_PRINCIPAL_ID('DB_Zugriff_CISO') IS NULL 
CREATE ROLE DB_Zugriff_Admin
GO
IF DATABASE_PRINCIPAL_ID('DB_Zugriff_CISO') IS NULL 
CREATE ROLE DB_Zugriff_Normal_User
GO
IF DATABASE_PRINCIPAL_ID('DB_Zugriff_CISO') IS NULL 
CREATE ROLE DB_Zugriff_SBA_User
GO

Declare @domName varchar(200);
SET @domName = (SELECT DEFAULT_DOMAIN()[DomainName]);

Declare @Gruppe varchar(250);

print 'Zugriff für P.ABT.BIA_Tool_CISO_PROD'
SET @Gruppe = @domName+'\P.ABT.BIA_Tool_CISO_PROD' 

EXEC sp_addrolemember 'DB_Zugriff_CISO', @Gruppe

GRANT SELECT,INSERT,UPDATE ON [dbo].[ISB_BIA_Applikationen] TO DB_Zugriff_CISO
GRANT SELECT,INSERT,UPDATE,DELETE ON [dbo].[ISB_BIA_Delta_Analyse] TO DB_Zugriff_CISO
GRANT SELECT,INSERT,UPDATE ON [dbo].[ISB_BIA_Informationssegmente] TO DB_Zugriff_CISO
GRANT SELECT,INSERT,UPDATE ON [dbo].[ISB_BIA_Informationssegmente_Attribute] TO DB_Zugriff_CISO
GRANT SELECT,INSERT,UPDATE,DELETE ON [dbo].[ISB_BIA_Lock] TO DB_Zugriff_CISO
GRANT SELECT,INSERT,UPDATE ON [dbo].[ISB_BIA_Log] TO DB_Zugriff_CISO
GRANT SELECT,INSERT,UPDATE,DELETE ON [dbo].[ISB_BIA_OEs] TO DB_Zugriff_CISO
GRANT SELECT,INSERT,UPDATE ON [dbo].[ISB_BIA_Prozesse] TO DB_Zugriff_CISO
GRANT SELECT,INSERT,UPDATE ON [dbo].[ISB_BIA_Prozesse_Applikationen] TO DB_Zugriff_CISO
GRANT SELECT,INSERT,UPDATE ON [dbo].[ISB_BIA_Settings] TO DB_Zugriff_CISO

print 'Zugriff für P.ABT.BIA_Tool_Admin_PROD'
SET @Gruppe = @domName+'\P.ABT.BIA_Tool_Admin_PROD' 

EXEC sp_addrolemember 'DB_Zugriff_Admin',@Gruppe

GRANT SELECT,INSERT,UPDATE ON [dbo].[ISB_BIA_Applikationen] TO DB_Zugriff_Admin
GRANT SELECT,INSERT,UPDATE,DELETE ON [dbo].[ISB_BIA_Delta_Analyse] TO DB_Zugriff_Admin
GRANT SELECT ON [dbo].[ISB_BIA_Informationssegmente] TO DB_Zugriff_Admin
GRANT SELECT ON [dbo].[ISB_BIA_Informationssegmente_Attribute] TO DB_Zugriff_Admin
GRANT SELECT,INSERT,UPDATE,DELETE ON [dbo].[ISB_BIA_Lock] TO DB_Zugriff_Admin
GRANT SELECT,INSERT,UPDATE ON [dbo].[ISB_BIA_Log] TO DB_Zugriff_Admin
GRANT SELECT,INSERT,UPDATE,DELETE ON [dbo].[ISB_BIA_OEs] TO DB_Zugriff_Admin
GRANT SELECT,INSERT,UPDATE ON [dbo].[ISB_BIA_Prozesse] TO DB_Zugriff_Admin
GRANT SELECT,INSERT,UPDATE ON [dbo].[ISB_BIA_Prozesse_Applikationen] TO DB_Zugriff_Admin
GRANT SELECT,INSERT,UPDATE ON [dbo].[ISB_BIA_Settings] TO DB_Zugriff_Admin

print 'Zugriff für P.ABT.BIA_Tool_SBA_User_PROD'
SET @Gruppe = @domName+'\P.ABT.BIA_Tool_SBA_User_PROD' 

EXEC sp_addrolemember 'DB_Zugriff_SBA_User',@Gruppe

GRANT SELECT,INSERT,UPDATE ON [dbo].[ISB_BIA_Applikationen] TO DB_Zugriff_SBA_User
GRANT SELECT,INSERT,UPDATE,DELETE ON [dbo].[ISB_BIA_Delta_Analyse] TO DB_Zugriff_SBA_User
GRANT SELECT ON [dbo].[ISB_BIA_Informationssegmente] TO DB_Zugriff_SBA_User
GRANT SELECT ON [dbo].[ISB_BIA_Informationssegmente_Attribute] TO DB_Zugriff_SBA_User
GRANT SELECT,INSERT,UPDATE,DELETE ON [dbo].[ISB_BIA_Lock] TO DB_Zugriff_SBA_User
GRANT SELECT,INSERT,UPDATE ON [dbo].[ISB_BIA_Log] TO DB_Zugriff_SBA_User
GRANT SELECT ON [dbo].[ISB_BIA_OEs] TO DB_Zugriff_SBA_User
GRANT SELECT,INSERT,UPDATE ON [dbo].[ISB_BIA_Prozesse] TO DB_Zugriff_SBA_User
GRANT SELECT,INSERT,UPDATE ON [dbo].[ISB_BIA_Prozesse_Applikationen] TO DB_Zugriff_SBA_User
GRANT SELECT ON [dbo].[ISB_BIA_Settings] TO DB_Zugriff_SBA_User

print 'Zugriff für P.ABT.BIA_Tool_Normal_User_PROD'
SET @Gruppe = @domName+'\P.ABT.BIA_Tool_Normal_User_PROD' 

EXEC sp_addrolemember 'DB_Zugriff_Normal_User',@Gruppe

GRANT SELECT ON [dbo].[ISB_BIA_Applikationen] TO DB_Zugriff_Normal_User
GRANT SELECT ON [dbo].[ISB_BIA_Delta_Analyse] TO DB_Zugriff_Normal_User
GRANT SELECT ON [dbo].[ISB_BIA_Informationssegmente] TO DB_Zugriff_Normal_User
GRANT SELECT ON [dbo].[ISB_BIA_Informationssegmente_Attribute] TO DB_Zugriff_Normal_User
GRANT SELECT,INSERT,UPDATE,DELETE ON [dbo].[ISB_BIA_Lock] TO DB_Zugriff_Normal_User
GRANT SELECT,INSERT,UPDATE ON [dbo].[ISB_BIA_Log] TO DB_Zugriff_Normal_User
GRANT SELECT ON [dbo].[ISB_BIA_OEs] TO DB_Zugriff_Normal_User
GRANT SELECT,INSERT,UPDATE ON [dbo].[ISB_BIA_Prozesse] TO DB_Zugriff_Normal_User
GRANT SELECT,INSERT,UPDATE ON [dbo].[ISB_BIA_Prozesse_Applikationen] TO DB_Zugriff_Normal_User
GRANT SELECT ON [dbo].[ISB_BIA_Settings] TO DB_Zugriff_Normal_User