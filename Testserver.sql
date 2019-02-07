--Für Test
IF DATABASE_PRINCIPAL_ID('DB_Zugriff_CISO') IS NULL 
CREATE ROLE DB_Zugriff_CISO
GO
IF DATABASE_PRINCIPAL_ID('DB_Zugriff_Admin') IS NULL 
CREATE ROLE DB_Zugriff_Admin
GO
IF DATABASE_PRINCIPAL_ID('DB_Zugriff_Normal_User') IS NULL 
CREATE ROLE DB_Zugriff_Normal_User
GO
IF DATABASE_PRINCIPAL_ID('DB_Zugriff_SBA_User') IS NULL 
CREATE ROLE DB_Zugriff_SBA_User
GO

print 'Datenbankberechtigungen an alle Gruppen verteilen'
GRANT SELECT,INSERT,UPDATE,DELETE TO DB_Zugriff_CISO
GRANT SELECT,INSERT,UPDATE,DELETE TO DB_Zugriff_Admin
GRANT SELECT,INSERT,UPDATE,DELETE TO DB_Zugriff_SBA_User
GRANT SELECT,INSERT,UPDATE,DELETE TO DB_Zugriff_Normal_User


Declare @Gruppe varchar(250);

print 'Zugriff auf einzelne Tabellen für P.ABT.BIA_Tool_CISO_TEST'
SET @Gruppe = 'ADP\P.ABT.BIA_Tool_CISO_TEST' 

print 'Login erzeugen für CISO (AD-Gruppen anhand von Windows-Athentifizierung)'
CREATE LOGIN [AD_ISB_BIA_CISO] FROM WINDOWS WITH DEFAULT_DATABASE=[BIA];

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

print 'Zugriff auf einzelne Tabellen für P.ABT.BIA_Tool_Admin_TEST'
SET @Gruppe = 'ADP\P.ABT.BIA_Tool_Admin_TEST' 

print 'Login erzeugen für Admin (AD-Gruppen anhand von Windows-Athentifizierung)'
CREATE LOGIN [AD_ISB_BIA_Admin] FROM WINDOWS WITH DEFAULT_DATABASE=[BIA];

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

print 'Zugriff auf einzelne Tabellen für P.ABT.BIA_Tool_SBA_User_TEST'
SET @Gruppe = 'ADP\P.ABT.BIA_Tool_SBA_User_TEST' 

print 'Login erzeugen für Normalen User (AD-Gruppen anhand von Windows-Athentifizierung)'
CREATE LOGIN [AD_ISB_BIA_Normal_User] FROM WINDOWS WITH DEFAULT_DATABASE=[BIA];

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

print 'Zugriff auf einzelne Tabellen für P.ABT.BIA_Tool_Normal_User_TEST'
SET @Gruppe = 'ADP\P.ABT.BIA_Tool_Normal_User_TEST' 

print 'Login erzeugen für CISO (AD-Gruppen anhand von Windows-Athentifizierung)'
CREATE LOGIN [AD_ISB_BIA_SBA_User] FROM WINDOWS WITH DEFAULT_DATABASE=[BIA];

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