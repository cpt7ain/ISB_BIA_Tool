using GalaSoft.MvvmLight;
using ISB_BIA_IMPORT1.Model;
using System.Configuration;
using System.IO;
using ISB_BIA_IMPORT1.Services.Interfaces;

namespace ISB_BIA_IMPORT1.Services
{
    class MySharedResourceService : ObservableObject, IMySharedResourceService
    {
        private bool _constructionMode;
        private Current_Environment _current_Environment;
        private bool _admin;
        private Login_Model _user;
        private string _targetMail;

        private string _connectionString;
        private string _initialDirectory;
        private string _source;

        #region Tabellennamen in Datenbank
        public string _tbl_Prozesse;
        public string _tbl_Proz_App;
        public string _tbl_Delta;
        public string _tbl_IS;
        public string _tbl_IS_Attribute;
        public string _tbl_Applikationen;
        public string _tbl_Log;
        public string _tbl_OEs;
        public string _tbl_Settings;
        public string _tbl_Lock;
        #endregion

        public MySharedResourceService(IMyDialogService myDia)
        {
            ConstructionMode = (ConfigurationManager.AppSettings["MODE_Construction"] == "true") ? true : false;
            if(ConfigurationManager.AppSettings["Current_Environment"] == "local")
            {
                CurrentEnvironment = Current_Environment.Local_Test;
                try
                {
                    ConnectionString = ConfigurationManager.ConnectionStrings["LOCAL_TEST_DataConnectionString"].ConnectionString;
                }
                catch
                {
                    myDia.ShowError("Ungültige Konfiguration. Bitte ändern Sie 'Current_Environment' oder definieren Sie 'LOCAL_TEST_DataConnectionString'.");
                }
            }
            else if(ConfigurationManager.AppSettings["Current_Environment"] == "test")
            {
                CurrentEnvironment = Current_Environment.Test;
                ConnectionString = ConfigurationManager.ConnectionStrings["TEST_DataConnectionString"].ConnectionString;

            }
            else if (ConfigurationManager.AppSettings["Current_Environment"] == "prod")
            {
                CurrentEnvironment = Current_Environment.Prod;
                ConnectionString = ConfigurationManager.ConnectionStrings["PROD_DataConnectionString"].ConnectionString;
            }
            else
            {
                myDia.ShowError("Konfigurationsdatei ungültig. Bitte prüfen Sie 'Current_Environment' sowie den passenden ConnectionString");
            }

            Admin = (ConfigurationManager.AppSettings["MODE_Admin"] == "true") ? true : false;
            TargetMail = ConfigurationManager.AppSettings["Target_Mail"];

            InitialDirectory = Directory.GetDirectories(Directory.GetCurrentDirectory(), "Data")[0];
            Source = _initialDirectory + @"\ISB_BIA-SBA.xlsx";

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

        public bool ConstructionMode
        {
            get => _constructionMode;
            set => Set(() => ConstructionMode, ref _constructionMode, value);
        }

        public Current_Environment CurrentEnvironment
        {
            get => _current_Environment;
            set => Set(()=> CurrentEnvironment, ref _current_Environment, value);
        }

        public bool Admin
        {
            get => _admin;
            set => Set(() => Admin, ref _admin, value);
        }

        public Login_Model User
        {
            get => _user;
            set => Set(() => User, ref _user, value);
        }

        public string TargetMail
        {
            get => _targetMail;
            set => Set(() => TargetMail, ref _targetMail, value);
        }

        #region Standard Dateipfad für einzulesende Quelldatei
        public string ConnectionString
        {
            get => _connectionString;
            set => Set(() => ConnectionString, ref _connectionString, value);
        }
        public string InitialDirectory
        {
            get => _initialDirectory;
            set => Set(() => InitialDirectory, ref _initialDirectory, value);
        }
        public string Source
        {
            get => _source;
            set => Set(() => Source, ref _source, value);
        }
        public string Tbl_Prozesse
        {
            get => _tbl_Prozesse;
            set => Set(() => Tbl_Prozesse, ref _tbl_Prozesse, value);
        }
        public string Tbl_Proz_App
        {
            get => _tbl_Proz_App;
            set => Set(() => Tbl_Proz_App, ref _tbl_Proz_App, value);
        }
        public string Tbl_Delta
        {
            get => _tbl_Delta;
            set => Set(() => Tbl_Delta, ref _tbl_Delta, value);
        }
        public string Tbl_IS
        {
            get => _tbl_IS;
            set => Set(() => Tbl_IS, ref _tbl_IS, value);
        }
        public string Tbl_IS_Attribute
        {
            get => _tbl_IS_Attribute;
            set => Set(() => Tbl_IS_Attribute, ref _tbl_IS_Attribute, value);
        }
        public string Tbl_Applikationen
        {
            get => _tbl_Applikationen;
            set => Set(() => Tbl_Applikationen, ref _tbl_Applikationen, value);
        }
        public string Tbl_Log
        {
            get => _tbl_Log;
            set => Set(() => Tbl_Log, ref _tbl_Log, value);
        }
        public string Tbl_OEs
        {
            get => _tbl_OEs;
            set => Set(() => Tbl_OEs, ref _tbl_OEs, value);
        }
        public string Tbl_Settings
        {
            get => _tbl_Settings;
            set => Set(() => Tbl_Settings, ref _tbl_Settings, value);
        }
        public string Tbl_Lock
        {
            get => _tbl_Lock;
            set => Set(() => Tbl_Lock, ref _tbl_Lock, value);
        }
        #endregion
    }
}
