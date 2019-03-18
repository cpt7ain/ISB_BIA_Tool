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
    public class DesignTimeDataService_Application : IDataService_Application
    {
        public ObservableCollection<ISB_BIA_Applikationen> ApplicationDummyList;

        public DesignTimeDataService_Application()
        {
            ApplicationDummyList = GetDummyApplications();
        }

        #region Application
        public ObservableCollection<ISB_BIA_Applikationen> GetDummyApplications()
        {
            Random r = new Random();
            List<int> k = new List<int>() { 1, 5, 10 };
            var people = from n in Enumerable.Range(1, 100)
                         select new ISB_BIA_Applikationen
                         {
                             Applikation_Id = n,
                             IT_Anwendung_System = "TestAnwendung" + n,
                             IT_Betriebsart = "TestKategorie" + n,
                             Rechenzentrum = "TestRechenzentrum",
                             Server = "TestServer",
                             Virtuelle_Maschine = "TestVM",
                             Typ = "TestTyp",
                             Wichtiges_Anwendungssystem = "x",
                             SZ_1 = r.Next(0, 5),
                             SZ_2 = r.Next(0, 5),
                             SZ_3 = r.Next(0, 5),
                             SZ_4 = r.Next(0, 5),
                             SZ_5 = r.Next(0, 5),
                             SZ_6 = r.Next(0, 5),
                             Aktiv = r.Next(0, 2),
                             Datum = DateTime.Now,
                             Benutzer = "TestUser"
                         };
            return new ObservableCollection<ISB_BIA_Applikationen>(people.ToList());
        }

        public Tuple<List<ISB_BIA_Informationssegmente>, List<ISB_BIA_Informationssegmente>, List<ISB_BIA_Informationssegmente_Attribute>, List<ISB_BIA_Informationssegmente_Attribute>> IS_Attr_Get_ISAndISAttForExport()
        {
            return null;
        }
        public ObservableCollection<ISB_BIA_Applikationen> Get_List_Applications_Active()
        {
            return new ObservableCollection<ISB_BIA_Applikationen>(
                ApplicationDummyList.GroupBy(y => y.Applikation_Id).
                    Select(z => z.OrderByDescending(q => q.Datum).FirstOrDefault()).
                    Where(x => x.Aktiv == 1).OrderBy(k => k.Applikation_Id).ToList());
        }
        public Application_Model Get_Model_FromDB(int id)
        {
            ISB_BIA_Applikationen linqApp = ApplicationDummyList.FirstOrDefault();
            ObservableCollection<ISB_BIA_Prozesse> linqProcs = new ObservableCollection<ISB_BIA_Prozesse>();
            return new Application_Model
            {
                Applikation_Id = linqApp.Applikation_Id,
                Rechenzentrum = linqApp.Rechenzentrum,
                Server = linqApp.Server,
                Virtuelle_Maschine = linqApp.Virtuelle_Maschine,
                Typ = linqApp.Typ,
                IT_Betriebsart = linqApp.IT_Betriebsart,
                IT_Anwendung_System = linqApp.IT_Anwendung_System,
                Wichtiges_Anwendungssystem = (linqApp.Wichtiges_Anwendungssystem == "") ? false : true,
                SZ_1 = (SZ_Values)linqApp.SZ_1,
                SZ_2 = (SZ_Values)linqApp.SZ_2,
                SZ_3 = (SZ_Values)linqApp.SZ_3,
                SZ_4 = (SZ_Values)linqApp.SZ_4,
                SZ_5 = (SZ_Values)linqApp.SZ_5,
                SZ_6 = (SZ_Values)linqApp.SZ_6,
                Aktiv = linqApp.Aktiv,
                ProcessList = linqProcs
            };
        }
        public ISB_BIA_Applikationen Map_Model_ToDB(Application_Model a)
        {
            return null;
        }
        public ObservableCollection<string> Get_StringList_Rechenzentrum()
        {
            return new ObservableCollection<string>(ApplicationDummyList.Select(p => p.Rechenzentrum).Distinct());
        }
        public ObservableCollection<string> Get_StringList_Server()
        {
            return new ObservableCollection<string>(ApplicationDummyList.Select(p => p.Server).Distinct());
        }
        public ObservableCollection<string> Get_StringList_Virtuelle_Maschine()
        {
            return new ObservableCollection<string>(ApplicationDummyList.Select(p => p.Virtuelle_Maschine).Distinct());
        }
        public ObservableCollection<string> Get_StringList_Types()
        {
            return new ObservableCollection<string>(ApplicationDummyList.Select(p => p.Typ).Distinct());
        }
        public ObservableCollection<string> Get_StringList_Betriebsart()
        {
            return new ObservableCollection<string>(ApplicationDummyList.Select(p => p.IT_Betriebsart).Distinct());
        }
        public ObservableCollection<ISB_BIA_Applikationen> Get_List_Applications_All(DateTime? date = null)
        {
            return new ObservableCollection<ISB_BIA_Applikationen>(
                ApplicationDummyList.GroupBy(a => a.Applikation_Id)
                .Select(g => g.OrderByDescending(p => p.Datum).FirstOrDefault()).
                OrderBy(x => x.Applikation_Id).ToList());
        }
        public ObservableCollection<ISB_BIA_Applikationen> Get_History_Application(int applikation_Id)
        {
            return new ObservableCollection<ISB_BIA_Applikationen>(
                ApplicationDummyList.Where(a => a.Applikation_Id == applikation_Id).
                OrderByDescending(p => p.Datum).ToList());
        }
        public bool Insert_Application(Application_Model a, ProcAppMode mode)
        {
            return true;
        }
        public ISB_BIA_Applikationen Delete_Application(ISB_BIA_Applikationen a)
        {
            return Get_List_Applications_All().FirstOrDefault();
        }
        public ISB_BIA_Applikationen TryDeleteApplication(ISB_BIA_Applikationen toDelete)
        {
            return Get_List_Applications_All().FirstOrDefault();
        }
        public bool Insert_Applications_All(ObservableCollection<ISB_BIA_Applikationen> aList)
        {
            return true;
        }
        public ObservableCollection<ISB_BIA_Delta_Analyse> Get_History_ProcAppRelations(int id)
        {
            return null;
        }
        #endregion

    }
}
