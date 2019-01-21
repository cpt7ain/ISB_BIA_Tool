using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISB_BIA_IMPORT1.Model;

namespace ISB_BIA_IMPORT1.Services
{
    public class MyDesignTimeSharedResourceService : IMySharedResourceService
    {
        public bool ConstructionMode
        {
            get => false;
            set
            {
            }
        }

        public Current_Environment Current_Environment
        {
            get => Current_Environment.Local_Test;
            set
            {
            }
        }

        public bool Admin
        {
            get => true;
            set
            {
            }
        }

        public Login_Model User
        {
            get=> new Login_Model()
                {
                    Givenname = "TestUser",
                    Surname = "Test",
                    OE = "1.1",
                    UserGroup = UserGroups.CISO,
                    Username = "TEST"
                };
            set
            {
            }
        }



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
