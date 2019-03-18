using ISB_BIA_IMPORT1.Model;
using ISB_BIA_IMPORT1.Services.Interfaces;

namespace ISB_BIA_IMPORT1.Services
{
    public class DesignTimeSharedResourceService : ISharedResourceService
    {
        public bool Conf_ConstructionMode { get; set; } = false;

        public Current_Environment Conf_CurrentEnvironment { get; set; } = Current_Environment.Local_Test;

        public bool Conf_Admin { get; set; } = true;

        public Login_Model User { get; set; }
            = new Login_Model(){
                    Givenname = "TestUser",
                    Surname = "Test",
                    OE = "1.1",
                    UserGroup = UserGroups.CISO,
                    Username = "TEST"
                };

        public string Conf_TargetMail { get; set; } = "";

        #region Standard Dateipfad für einzulesende Quelldatei
        public string Conf_ConnectionString
        {
            get => "";
            set { }
        }
        public string Dir_InitialDirectory
        {
            get => "";
            set { }
        }
        public string Dir_Source
        {
            get => "";
            set { }
        }
        public string Conf_AD_Group_CISO { get; set; }


        public string Conf_AD_Group_Admin { get; set; }


        public string Conf_AD_Group_SBA { get; set; }


        public string Conf_AD_Group_Normal { get; set; }

        public string Tbl_Prozesse
        {
            get => "";
            set { }
        }
        public string Tbl_Proz_App
        {
            get => "";
            set { }
        }
        public string Tbl_Delta
        {
            get => "";
            set { }
        }
        public string Tbl_IS
        {
            get => "";
            set { }
        }
        public string Tbl_IS_Attribute
        {
            get => "";
            set { }
        }
        public string Tbl_Applikationen
        {
            get => "";
            set { }
        }
        public string Tbl_Log
        {
            get => "";
            set { }
        }
        public string Tbl_OEs
        {
            get => "";
            set { }
        }
        public string Tbl_Settings
        {
            get => "";
            set { }
        }
        public string Tbl_Lock
        {
            get => "";
            set { }
        }
        #endregion
    }
}
