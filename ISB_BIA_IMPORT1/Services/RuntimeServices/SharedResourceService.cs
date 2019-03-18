using GalaSoft.MvvmLight;
using ISB_BIA_IMPORT1.Model;
using System.Configuration;
using System.IO;
using ISB_BIA_IMPORT1.Services.Interfaces;

namespace ISB_BIA_IMPORT1.Services
{
    class SharedResourceService : ObservableObject, ISharedResourceService
    {
        private Login_Model _user;

        public SharedResourceService(IDialogService myDia)
        {
            if(ConfigurationManager.AppSettings["Current_Environment"] == "local")
            {
                Conf_CurrentEnvironment = Current_Environment.Local_Test;
                try
                {
                    Conf_ConnectionString = ConfigurationManager.ConnectionStrings["LOCAL_TEST_DataConnectionString"].ConnectionString;
                }
                catch
                {
                    myDia.ShowError("Ungültige Konfiguration. Bitte ändern Sie 'Current_Environment' oder definieren Sie 'LOCAL_TEST_DataConnectionString'.");
                }
            }
            else if(ConfigurationManager.AppSettings["Current_Environment"] == "test")
            {
                Conf_CurrentEnvironment = Current_Environment.Test;
                try
                {
                    Conf_ConnectionString = ConfigurationManager.ConnectionStrings["TEST_DataConnectionString"].ConnectionString;
                    Conf_AD_Group_CISO = ConfigurationManager.AppSettings["AD_Group_CISO_TEST"].ToString();
                    Conf_AD_Group_SBA = ConfigurationManager.AppSettings["AD_Group_SBA_User_TEST"].ToString();
                    Conf_AD_Group_Admin = ConfigurationManager.AppSettings["AD_Group_Admin_TEST"].ToString();
                    Conf_AD_Group_Normal = ConfigurationManager.AppSettings["AD_Group_Normal_User_TEST"].ToString();
                }
                catch
                {
                    myDia.ShowError("Ungültige Konfiguration.");
                }
            }
            else if (ConfigurationManager.AppSettings["Current_Environment"] == "prod")
            {
                Conf_CurrentEnvironment = Current_Environment.Prod;
                try
                {
                    Conf_ConnectionString = ConfigurationManager.ConnectionStrings["PROD_DataConnectionString"].ConnectionString;
                    Conf_AD_Group_CISO = ConfigurationManager.AppSettings["AD_Group_CISO_PROD"].ToString();
                    Conf_AD_Group_SBA = ConfigurationManager.AppSettings["AD_Group_SBA_User_PROD"].ToString();
                    Conf_AD_Group_Admin = ConfigurationManager.AppSettings["AD_Group_Admin_PROD"].ToString();
                    Conf_AD_Group_Normal = ConfigurationManager.AppSettings["AD_Group_Normal_User_PROD"].ToString();
                }
                catch
                {
                    myDia.ShowError("Ungültige Konfiguration.");
                }
            }
            else
            {
                myDia.ShowError("Konfigurationsdatei ungültig.\n'Current_Environment' muss einen der folgenden Werte besitzen: 'prod','test',local'");
            }

            try
            {
                Conf_ConstructionMode = (ConfigurationManager.AppSettings["MODE_Construction"] == "true") ? true : false;
                Conf_Admin = (ConfigurationManager.AppSettings["MODE_Admin"] == "true") ? true : false;
                Conf_TargetMail = ConfigurationManager.AppSettings["Target_Mail"];
                Dir_InitialDirectory = Directory.GetDirectories(Directory.GetCurrentDirectory(), "Data")[0];
                Dir_Source = Dir_InitialDirectory + @"\ISB_BIA-SBA.xlsx";
            }
            catch
            {
                myDia.ShowError("Ungültige Konfigurationsdatei.");
            }

            Tbl_Prozesse = "ISB_BIA_Prozesse";
            Tbl_Proz_App = "ISB_BIA_Prozesse_Applikationen";
            Tbl_Delta = "ISB_BIA_Delta_Analyse";
            Tbl_IS = "ISB_BIA_Informationssegmente";
            Tbl_IS_Attribute = "ISB_BIA_Informationssegmente_Attribute";
            Tbl_Applikationen = "ISB_BIA_Applikationen";
            Tbl_Log = "ISB_BIA_Log";
            Tbl_OEs = "ISB_BIA_OEs";
            Tbl_Settings = "ISB_BIA_Settings";
            Tbl_Lock = "ISB_BIA_Lock";
        }

        public Login_Model User
        {
            get => _user;
            set => Set(() => User, ref _user, value);
        }
        public bool Conf_ConstructionMode { get; set; }
        public Current_Environment Conf_CurrentEnvironment { get; set; }
        public bool Conf_Admin { get; set; }
        public string Conf_TargetMail { get; set; }

        #region Standard Dateipfad für einzulesende Quelldatei
        public string Conf_ConnectionString { get; set; }
        public string Dir_InitialDirectory { get; set; }
        public string Dir_Source { get; set; }
        public string Conf_AD_Group_CISO { get; set; }
        public string Conf_AD_Group_Admin { get; set; }
        public string Conf_AD_Group_SBA { get; set; }
        public string Conf_AD_Group_Normal { get; set; }
        public string Tbl_Prozesse { get; set; }
        public string Tbl_Proz_App { get; set; }
        public string Tbl_Delta { get; set; }
        public string Tbl_IS { get; set; }
        public string Tbl_IS_Attribute { get; set; }
        public string Tbl_Applikationen { get; set; }
        public string Tbl_Log { get; set; }
        public string Tbl_OEs { get; set; }
        public string Tbl_Settings { get; set; }
        public string Tbl_Lock { get; set; }
        #endregion
    }
}
