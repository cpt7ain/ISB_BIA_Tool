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
        public ObservableCollection<Application_Model> Get_List_ApplicationModels_Active(bool additional = false)
        {
            ObservableCollection <ISB_BIA_Applikationen> a= new ObservableCollection<ISB_BIA_Applikationen>(
                ApplicationDummyList.GroupBy(y => y.Applikation_Id).
                    Select(z => z.OrderByDescending(q => q.Datum).FirstOrDefault()).
                    Where(x => x.Aktiv == 1).OrderBy(k => k.Applikation_Id).ToList());
            ObservableCollection<Application_Model> res = new ObservableCollection<Application_Model>();
            foreach (ISB_BIA_Applikationen b in a)
            {
                res.Add(Map_DB_ToModel(b));
            }
            return res;
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
            return new ISB_BIA_Applikationen()
            {
                Applikation_Id = a.Applikation_Id,
                Rechenzentrum = a.Rechenzentrum,
                Server = a.Server,
                Virtuelle_Maschine = a.Virtuelle_Maschine,
                Typ = a.Typ,
                IT_Betriebsart = a.IT_Betriebsart,
                IT_Anwendung_System = a.IT_Anwendung_System,
                Wichtiges_Anwendungssystem = (a.Wichtiges_Anwendungssystem) ? "x" : "",
                SZ_1 = (int)a.SZ_1,
                SZ_2 = (int)a.SZ_2,
                SZ_3 = (int)a.SZ_3,
                SZ_4 = (int)a.SZ_4,
                SZ_5 = (int)a.SZ_5,
                SZ_6 = (int)a.SZ_6,
                Aktiv = a.Aktiv,
                Datum = a.Datum,
                Benutzer = a.Benutzer,
                Erstanlage = a.Erstanlage,
            };
        }
        public Application_Model Map_DB_ToModel(ISB_BIA_Applikationen a)
        {
            return new Application_Model()
            {
                Applikation_Id = a.Applikation_Id,
                Rechenzentrum = a.Rechenzentrum,
                Server = a.Server,
                Virtuelle_Maschine = a.Virtuelle_Maschine,
                Typ = a.Typ,
                IT_Betriebsart = a.IT_Betriebsart,
                IT_Anwendung_System = a.IT_Anwendung_System,
                Wichtiges_Anwendungssystem = (a.Wichtiges_Anwendungssystem == "x") ? true : false,
                SZ_1 = (SZ_Values)a.SZ_1,
                SZ_2 = (SZ_Values)a.SZ_2,
                SZ_3 = (SZ_Values)a.SZ_3,
                SZ_4 = (SZ_Values)a.SZ_4,
                SZ_5 = (SZ_Values)a.SZ_5,
                SZ_6 = (SZ_Values)a.SZ_6,
                Aktiv = a.Aktiv,
                Datum = a.Datum,
                Benutzer = a.Benutzer,
                Erstanlage = a.Erstanlage,
            };
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
        public ObservableCollection<Application_Model> Get_List_ApplicationModels_All(DateTime? date = null, bool additional = false)
        {
            ObservableCollection<ISB_BIA_Applikationen> a = new ObservableCollection<ISB_BIA_Applikationen>(
                ApplicationDummyList.GroupBy(r => r.Applikation_Id)
                .Select(g => g.OrderByDescending(p => p.Datum).FirstOrDefault()).
                OrderBy(x => x.Applikation_Id).ToList());
            ObservableCollection<Application_Model> res = new ObservableCollection<Application_Model>();
            foreach (ISB_BIA_Applikationen b in a)
            {
                res.Add(Map_DB_ToModel(b));
            }
            return res;
        }
        public ObservableCollection<Application_Model> Get_History_Application(int applikation_Id)
        {
            ObservableCollection<ISB_BIA_Applikationen> a = new ObservableCollection<ISB_BIA_Applikationen>(
                ApplicationDummyList.Where(r => r.Applikation_Id == applikation_Id).
                OrderByDescending(p => p.Datum).ToList());
            ObservableCollection<Application_Model> res = new ObservableCollection<Application_Model>();
            foreach (ISB_BIA_Applikationen b in a)
            {
                res.Add(Map_DB_ToModel(b));
            }
            return res;
        }
        public bool Insert_Application(Application_Model a, ProcAppMode mode)
        {
            return true;
        }
        public ISB_BIA_Applikationen Delete_Application(Application_Model a)
        {
            return null;
        }
        public bool Insert_Applications_All(ObservableCollection<Application_Model> aList)
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
