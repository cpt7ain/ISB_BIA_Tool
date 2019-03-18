using ISB_BIA_IMPORT1.Model;
using ISB_BIA_IMPORT1.ViewModel;
using ISB_BIA_IMPORT1.LINQ2SQL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Net;
using ISB_BIA_IMPORT1.Services.Interfaces;

namespace ISB_BIA_IMPORT1.Services
{
    public class DesignTimeDataService_Lock : ILockService
    {
        public bool CheckDBConnection()
        {
            return true;
        }
        #region Datensatz-Lock Operationen
        public string Get_ObjectIsLocked(Table_Lock_Flags table_Flag, int id)
        {
            return "";
        }
        public bool Lock_Object(Table_Lock_Flags table_Flag, int id)
        {
            return true;
        }
        public bool Unlock_Object(Table_Lock_Flags table_Flag, int id)
        {
            return true;
        }
        public bool Unlock_AllObjectsForUserOnMachine()
        {
            return true;
        }
        public bool Unlock_AllObjects()
        {
            return true;
        }

        #endregion
    }
}
