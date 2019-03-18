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
    public class DesignTimeDataService_Log : IDataService_Log
    {
        #region Log
        public ObservableCollection<ISB_BIA_Log> GetDummyLog()
        {
            Random r = new Random();
            var people = from n in Enumerable.Range(1, 100)
                select new ISB_BIA_Log
                {
                    Id = n,
                    Aktion = "Aktion" + n,
                    Tabelle = "T" + r.Next(1, 5),
                    Details = "Detail" + n,
                    Id_1 = r.Next(1, 400),
                    Id_2 = r.Next(1, 400),
                    Datum = DateTime.Now,
                    Benutzer = "TestUser" + n
                };
            return new ObservableCollection<ISB_BIA_Log>(people.ToList());
        }

        public ObservableCollection<ISB_BIA_Log> Get_List_Log()
        {
            return GetDummyLog();
        }
        #endregion
    }
}
