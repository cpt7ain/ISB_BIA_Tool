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
    public class MyDesignTimeDataService_OE : IMyDataService_OE
    {
        #region OE
        public ObservableCollection<ISB_BIA_OEs> GetDummyOEs()
        {
            Random r = new Random();
            var people = from n in Enumerable.Range(1, 100)
                         select new ISB_BIA_OEs
                         {
                             Id = n,
                             OE_Name = "OE" + n,
                             OE_Nummer = r.Next(1, 4) + "." + r.Next(1, 3),
                             Datum = DateTime.Now,
                             Benutzer = "TestUser" + n
                         };
            return new ObservableCollection<ISB_BIA_OEs>(people.ToList());
        }

        public ObservableCollection<ISB_BIA_OEs> Get_OENames()
        {
            List<ISB_BIA_OEs> queryName = GetDummyOEs().Where(c => c.OE_Name != "").
                GroupBy(x => x.OE_Name).Select(g => g.OrderBy(p => p.Datum).FirstOrDefault()).ToList();
            return new ObservableCollection<ISB_BIA_OEs>(queryName);
        }
        public ObservableCollection<ISB_BIA_OEs> Get_OENumbers()
        {
            List<ISB_BIA_OEs> queryKennung = GetDummyOEs().Where(y => y.OE_Nummer != null && y.OE_Nummer != "").GroupBy(x => x.OE_Nummer).Select(g => g.OrderBy(p => p.Datum).FirstOrDefault()).ToList();
            return new ObservableCollection<ISB_BIA_OEs>(queryKennung);
        }
        public ObservableCollection<ISB_BIA_OEs> Get_OELinks()
        {
            List<ISB_BIA_OEs> queryLink = GetDummyOEs().Where(x => x.OE_Nummer != "").OrderBy(c => c.OE_Name).ToList();
            return new ObservableCollection<ISB_BIA_OEs>(queryLink);
        }

        public ISB_BIA_OEs Insert_OEName_New(string name)
        {
            return GetDummyOEs().FirstOrDefault();
        }

        public bool Insert_OEName_Edit(string name, string oldName)
        {
            return true;
        }

        public bool Delete_OEName(string oeName)
        {
            return true;
        }

        public bool Delete_OELink(string oeName, string oeNumber)
        {
            return true;
        }

        public bool Delete_OENumber(string oeNumber)
        {
            return true;
        }

        public ISB_BIA_OEs Insert_OELink(ISB_BIA_OEs name, ISB_BIA_OEs number)
        {
            return GetDummyOEs().FirstOrDefault();
        }

        public ISB_BIA_OEs Insert_OENumber_New(string number, ISB_BIA_OEs name)
        {
            return GetDummyOEs().FirstOrDefault();
        }

        public bool Insert_OENumber_Edit(string number, string oldNumber)
        {
            return true;
        }
        #endregion

    }
}
