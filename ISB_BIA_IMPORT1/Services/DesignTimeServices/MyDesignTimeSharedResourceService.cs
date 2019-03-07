using ISB_BIA_IMPORT1.Model;
using ISB_BIA_IMPORT1.Services.Interfaces;

namespace ISB_BIA_IMPORT1.Services
{
    public class MyDesignTimeSharedResourceService : IMySharedResourceService
    {
        public bool ConstructionMode { get; set; } = false;

        public Current_Environment Current_Environment { get; set; } = Current_Environment.Local_Test;

        public bool Admin { get; set; } = true;

        public Login_Model User { get; set; }
            = new Login_Model(){
                    Givenname = "TestUser",
                    Surname = "Test",
                    OE = "1.1",
                    UserGroup = UserGroups.CISO,
                    Username = "TEST"
                };

        public string TargetMail { get; set; } = "";

        #region Standard Dateipfad für einzulesende Quelldatei
        public string ConnectionString
        {
            get => "";
            set { }
        }
        public string InitialDirectory
        {
            get => "";
            set { }
        }
        public string Source
        {
            get => "";
            set { }
        }
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
