using ISB_BIA_IMPORT1.Model;
using ISB_BIA_IMPORT1.ViewModel;
using ISB_BIA_IMPORT1.LinqEntityContext;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Data;
using System.Data.SqlClient;

namespace ISB_BIA_IMPORT1.Services
{
    public class MyDesignTimeDataService : IMyDataService
    {
        public MyDesignTimeDataService()
        {
            ProcessDummyList = GetDummyProcesses();
        }

        public bool CheckDBConnection()
        {
            return true;
        }

        public bool CreateDataModel(DataTable dt_Processes, DataTable dt_Applications, DataTable dt_Relation, DataTable dt_InformationSegments, DataTable dt_InformationSegmentAttributes)
        {
            return true;
        }

        public void SQLBulkCopy(string tableName, SqlConnection con, DataTable dt)
        {
            
        }

        #region Datensatz-Lock Operationen
        public string GetObjectLocked(Table_Lock_Flags table_Flag, int id)
        {
            return "";
        }
        public bool LockObject(Table_Lock_Flags table_Flag, int id)
        {
            return true;
        }
        public bool UnlockObject(Table_Lock_Flags table_Flag, int id)
        {
            return true;
        }
        public bool UnlockAllObjectsForUser()
        {
            return true;
        }
        public bool UnlockAllObjects()
        {
            return true;
        }

        #endregion

        #region Process
        public ObservableCollection<ISB_BIA_Prozesse> ProcessDummyList;
        public ObservableCollection<ISB_BIA_Prozesse> GetDummyProcesses()
        {
            Random r = new Random();
            List<int> k = new List<int>() { 1, 5, 10 };
            var p1 = from n in Enumerable.Range(1, 10)
                         select new ISB_BIA_Prozesse
                         {
                             Prozess_Id = n,
                             Prozess = "Prozess" + n,
                             Sub_Prozess = "Sub" + n,
                             OE_Filter = "IT",
                             Prozessverantwortlicher = "Hr. Mustermann",
                             Kritikalität_des_Prozesses = (r.Next(0, 2) == 0) ? "Hoch" : "Sehr hoch",
                             Reifegrad_des_Prozesses = "1 - Initial",
                             Regulatorisch = (r.Next(0, 2) == 0) ? "x" : "",
                             Reputatorisch = (r.Next(0, 2) == 0) ? "x" : "",
                             Finanziell = (r.Next(0, 2) == 0) ? "x" : "",
                             SZ_1 = 1,
                             SZ_2 = 2,
                             SZ_3 = 3,
                             SZ_4 = 4,
                             SZ_5 = 0,
                             SZ_6 = 0,
                             Vorgelagerte_Prozesse = (r.Next(0, 2) == 0) ? "v1" : "v2",
                             Nachgelagerte_Prozesse = (r.Next(0, 2) == 0) ? "n1" : "n2",
                             Servicezeit_Helpdesk = (r.Next(0, 2) == 0) ? "Mo 6-8" : "Di 7-8",
                             RPO_Datenverlustzeit_Recovery_Point_Objective = k[r.Next(k.Count)],
                             RTO_Wiederanlaufzeit_Recovery_Time_Objective = k[r.Next(k.Count)],
                             RTO_Wiederanlaufzeit_Recovery_Time_Objective_Notfall = k[r.Next(k.Count)],
                             Relevantes_IS_1 = "IS" + r.Next(1, 10),
                             Relevantes_IS_2 = "IS" + r.Next(1, 10),
                             Relevantes_IS_3 = "IS" + r.Next(1, 10),
                             Relevantes_IS_4 = "IS" + r.Next(1, 10),
                             Relevantes_IS_5 = "IS" + r.Next(1, 10),
                             Aktiv = 1,
                             Datum = DateTime.Now,
                             Benutzer = "TestUser"+ r.Next(1, 10)
                         };
            var p2 = from n in Enumerable.Range(1, 10)
                         select new ISB_BIA_Prozesse
                         {
                             Prozess_Id = n+10,
                             Prozess = "Prozess" + n + 10,
                             Sub_Prozess = "Sub" + n + 10,
                             OE_Filter = "I",
                             Prozessverantwortlicher = "Hr. Mustermann",
                             Kritikalität_des_Prozesses = (r.Next(0, 2) == 0) ? "Hoch" : "Sehr hoch",
                             Reifegrad_des_Prozesses = "1 - Initial",
                             Regulatorisch = (r.Next(0, 2) == 0) ? "x" : "",
                             Reputatorisch = (r.Next(0, 2) == 0) ? "x" : "",
                             Finanziell = (r.Next(0, 2) == 0) ? "x" : "",
                             SZ_1 = 3,
                             SZ_2 = 3,
                             SZ_3 = 3,
                             SZ_4 = 3,
                             SZ_5 = 3,
                             SZ_6 = 3,
                             Vorgelagerte_Prozesse = (r.Next(0, 2) == 0) ? "v1" : "v2",
                             Nachgelagerte_Prozesse = (r.Next(0, 2) == 0) ? "n1" : "n2",
                             Servicezeit_Helpdesk = (r.Next(0, 2) == 0) ? "Mo 6-8" : "Di 7-8",
                             RPO_Datenverlustzeit_Recovery_Point_Objective = k[r.Next(k.Count)],
                             RTO_Wiederanlaufzeit_Recovery_Time_Objective = k[r.Next(k.Count)],
                             RTO_Wiederanlaufzeit_Recovery_Time_Objective_Notfall = k[r.Next(k.Count)],
                             Relevantes_IS_1 = "IS" + r.Next(1, 10),
                             Relevantes_IS_2 = "IS" + r.Next(1, 10),
                             Relevantes_IS_3 = "IS" + r.Next(1, 10),
                             Relevantes_IS_4 = "IS" + r.Next(1, 10),
                             Relevantes_IS_5 = "IS" + r.Next(1, 10),
                             Aktiv = 0,
                             Datum = DateTime.Now,
                             Benutzer = "TestUser" + r.Next(1, 10)
                         };
            ObservableCollection<ISB_BIA_Prozesse> res = new ObservableCollection<ISB_BIA_Prozesse>(p1.ToList().Concat<ISB_BIA_Prozesse>(p2));
            return res;
        }
        public Process_Model GetProcessModelFromDB(int id)
        {
            ISB_BIA_Prozesse linqProc = ProcessDummyList.FirstOrDefault();
            ObservableCollection<ISB_BIA_Applikationen> linqApps = new ObservableCollection<ISB_BIA_Applikationen>(GetDummyApplications().Take(15));
            return new Process_Model()
            {
                //"Kritischer Prozess" im Process_Model initial auf "Nein" gesetzt, wird berechnet in den Settern der anderen Eigenschaften
                //Kritischer_Prozess = (linqProc.Kritischer_Prozess == "x") ? "Ja" : "Nein",
                Prozess_Id = linqProc.Prozess_Id,
                OE_Filter = linqProc.OE_Filter,
                Prozess = linqProc.Prozess,
                Sub_Prozess = linqProc.Sub_Prozess,
                Prozessverantwortlicher = linqProc.Prozessverantwortlicher,
                Kritikalität_des_Prozesses = linqProc.Kritikalität_des_Prozesses,
                Reifegrad_des_Prozesses = linqProc.Reifegrad_des_Prozesses,
                Regulatorisch = (linqProc.Regulatorisch == "x") ? true : false,
                Reputatorisch = (linqProc.Reputatorisch == "x") ? true : false,
                Finanziell = (linqProc.Finanziell == "x") ? true : false,
                SZ_1 = (SZ_Values)linqProc.SZ_1,
                SZ_2 = (SZ_Values)linqProc.SZ_2,
                SZ_3 = (SZ_Values)linqProc.SZ_3,
                SZ_4 = (SZ_Values)linqProc.SZ_4,
                SZ_5 = (SZ_Values)linqProc.SZ_5,
                SZ_6 = (SZ_Values)linqProc.SZ_6,
                Vorgelagerte_Prozesse = linqProc.Vorgelagerte_Prozesse,
                Nachgelagerte_Prozesse = linqProc.Nachgelagerte_Prozesse,
                Servicezeit_Helpdesk = linqProc.Servicezeit_Helpdesk,
                RPO_Datenverlustzeit_Recovery_Point_Objective = linqProc.RPO_Datenverlustzeit_Recovery_Point_Objective,
                RTO_Wiederanlaufzeit_Recovery_Time_Objective = linqProc.RTO_Wiederanlaufzeit_Recovery_Time_Objective,
                RTO_Wiederanlaufzeit_Recovery_Time_Objective_Notfall = linqProc.RTO_Wiederanlaufzeit_Recovery_Time_Objective_Notfall,
                Relevantes_IS_1 = linqProc.Relevantes_IS_1,
                Relevantes_IS_2 = linqProc.Relevantes_IS_2,
                Relevantes_IS_3 = linqProc.Relevantes_IS_3,
                Relevantes_IS_4 = linqProc.Relevantes_IS_4,
                Relevantes_IS_5 = linqProc.Relevantes_IS_5,
                Aktiv = linqProc.Aktiv,
                Datum = linqProc.Datum,
                Benutzer = linqProc.Benutzer,
                ApplicationList = linqApps,
            };
        }
        public ISB_BIA_Prozesse MapProcessModelToDB(Process_Model p)
        {
            return null;
        }
        public Dictionary<string, string> GetISList()
        {
            List<ISB_BIA_Informationssegmente> listIS = GetDummySegments().ToList();
            listIS.Insert(0, new ISB_BIA_Informationssegmente() { Name = "", Segment = "<leer>" });
            return new Dictionary<string, string>(listIS.Select(t => new { t.Name, t.Segment }).ToDictionary(t => t.Name, t => t.Segment));
        }
        public ObservableCollection<ISB_BIA_Prozesse> GetProcessesByOE(ObservableCollection<string> listOE)
        {
            return ProcessDummyList;
        }
        public ObservableCollection<ISB_BIA_Prozesse> GetProcesses(DateTime? d = null)
        {
            return ProcessDummyList;
        }
        public ObservableCollection<ISB_BIA_Prozesse> GetActiveProcesses()
        {
            return new ObservableCollection<ISB_BIA_Prozesse>(ProcessDummyList.Where(x => x.Aktiv==1).ToList());
        }
        public ObservableCollection<string> GetProcessOwner()
        {
            return new ObservableCollection<string>(ProcessDummyList.Where(n => n.Prozessverantwortlicher != "" && n.Prozessverantwortlicher != " ").Select(p => p.Prozessverantwortlicher).Distinct());
        }
        public ObservableCollection<string> GetOEsForUser(string userOE)
        {
            return new ObservableCollection<string>(){"4.4"};
        }
        public ObservableCollection<string> GetOEs()
        {
            return new ObservableCollection<string>(GetDummyOEs().Select(p => p.OE_Name).Distinct());
        }
        public ObservableCollection<string> GetPreProcesses()
        {
            return new ObservableCollection<string>(ProcessDummyList.Select(p => p.Vorgelagerte_Prozesse).Distinct());
        }
        public ObservableCollection<string> GetPostProcesses()
        {
            return new ObservableCollection<string>(ProcessDummyList.Select(p => p.Nachgelagerte_Prozesse).Distinct());
        }
        public ObservableCollection<ISB_BIA_Prozesse> GetProcessHistory(int process_id)
        {
            return null;
        }
        public bool InsertProcessAndRelations(Process_Model p, ProcAppMode mode, ObservableCollection<ISB_BIA_Applikationen> add, ObservableCollection<ISB_BIA_Applikationen> remove)
        {
            return true;
        }
        public ISB_BIA_Prozesse DeleteProcess(ISB_BIA_Prozesse p)
        {
            return p;
        }
        public ISB_BIA_Prozesse TryInsert(ISB_BIA_Prozesse toDelete)
        {
            return GetProcesses().FirstOrDefault();
        }
        public bool SaveAllProcesses(ObservableCollection<ISB_BIA_Prozesse> pList)
        {
            return true;
        }
        #endregion

        #region Application
        public ObservableCollection<ISB_BIA_Applikationen> GetDummyApplications()
        {
            Random r = new Random();
            List<int> k = new List<int>() { 1, 5, 10 };
            var people = from n in Enumerable.Range(1, 100)
                         select new ISB_BIA_Applikationen
                         {
                             Applikation_Id = n,
                             IT_Anwendung_System = "TestAnwendung"+n,
                             IT_Betriebsart = "TestKategorie"+n,
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
        public Application_Model GetApplicationModelFromDB(int id)
        {
            ISB_BIA_Applikationen linqApp = GetDummyApplications().FirstOrDefault();
            ObservableCollection<ISB_BIA_Prozesse> linqProcs = new ObservableCollection<ISB_BIA_Prozesse>(ProcessDummyList.Take(15));
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
        public ISB_BIA_Applikationen MapApplicationModelToDB(Application_Model a)
        {
            return null;
        }
        public ObservableCollection<string> GetRechenzentrum()
        {
            return new ObservableCollection<string>(GetDummyApplications().Select(p => p.Rechenzentrum).Distinct());
        }
        public ObservableCollection<string> GetServer()
        {
            return new ObservableCollection<string>(GetDummyApplications().Select(p => p.Server).Distinct());
        }
        public ObservableCollection<string> GetVirtuelle_Maschine()
        {
            return new ObservableCollection<string>(GetDummyApplications().Select(p => p.Virtuelle_Maschine).Distinct());
        }
        public ObservableCollection<string> GetTypes()
        {
            return new ObservableCollection<string>(GetDummyApplications().Select(p => p.Typ).Distinct());
        }
        public ObservableCollection<string> GetBetriebsart()
        {
            return new ObservableCollection<string>(GetDummyApplications().Select(p => p.IT_Betriebsart).Distinct());
        }
        public ObservableCollection<ISB_BIA_Applikationen> GetApplications(DateTime? date = null)
        {
            return new ObservableCollection<ISB_BIA_Applikationen>(
                GetDummyApplications().GroupBy(a => a.Applikation_Id)
                .Select(g => g.OrderByDescending(p => p.Datum).FirstOrDefault()).
                OrderBy(x => x.Applikation_Id).ToList());
        }
        public ObservableCollection<ISB_BIA_Applikationen> GetApplicationHistory(int applikation_Id)
        {
            return new ObservableCollection<ISB_BIA_Applikationen>(
                GetDummyApplications().Where(a => a.Applikation_Id == applikation_Id).
                OrderByDescending(p => p.Datum).ToList());
        }
        public bool InsertApplication(Application_Model a, ProcAppMode mode)
        {
            return true;
        }
        public ISB_BIA_Applikationen DeleteApplication(ISB_BIA_Applikationen a)
        {
            return GetApplications().FirstOrDefault();
        }
        public ISB_BIA_Applikationen TryInsert(ISB_BIA_Applikationen toDelete)
        {
            return GetApplications().FirstOrDefault();
        }
        public bool SaveAllApplications(ObservableCollection<ISB_BIA_Applikationen> aList)
        {
            return true;
        }
        #endregion

        #region Einstellungen
        public ISB_BIA_Settings GetDummySettings()
        {
            return new ISB_BIA_Settings()
            {
                Id = 1,
                SZ_1_Name = "Verfügbarkeit",
                SZ_2_Name = "Integrität",
                SZ_3_Name = "Vertraulichkeit",
                SZ_4_Name = "Authentizität",
                SZ_5_Name = "Verbindlichkeit",
                SZ_6_Name = "Zurechenbarkeit",
                Neue_Schutzziele_aktiviert = "Ja",
                BIA_abgeschlossen = "Nein",
                SBA_abgeschlossen = "Nein",
                Delta_abgeschlossen = "Nein",
                Attribut9_aktiviert = "Nein",
                Attribut10_aktiviert = "Nein",
                Multi_Save = "Nein",
                Datum = DateTime.Now,
                Benutzer = Environment.UserName
            };
        }

        public Settings_Model GetSettingsModelFromDB()
        {
            ISB_BIA_Settings linqSettings = GetDummySettings();
            Settings_Model result = new Settings_Model()
            {
                SZ_1_Name = linqSettings.SZ_1_Name,
                SZ_2_Name = linqSettings.SZ_2_Name,
                SZ_3_Name = linqSettings.SZ_3_Name,
                SZ_4_Name = linqSettings.SZ_4_Name,
                SZ_5_Name = linqSettings.SZ_5_Name,
                SZ_6_Name = linqSettings.SZ_6_Name,

                Neue_Schutzziele_aktiviert = (linqSettings.Neue_Schutzziele_aktiviert == "Ja") ? true : false,
                BIA_abgeschlossen = (linqSettings.BIA_abgeschlossen == "Ja") ? true : false,
                SBA_abgeschlossen = (linqSettings.SBA_abgeschlossen == "Ja") ? true : false,
                Delta_abgeschlossen = (linqSettings.Delta_abgeschlossen == "Ja") ? true : false,
                Attribut9_aktiviert = (linqSettings.Attribut9_aktiviert == "Ja") ? true : false,
                Attribut10_aktiviert = (linqSettings.Attribut10_aktiviert == "Ja") ? true : false,
                Multi_Save = (linqSettings.Multi_Save == "Ja") ? true : false,
                Datum = linqSettings.Datum,
                Benutzer = linqSettings.Benutzer
            };
            return result;
        }
        public ISB_BIA_Settings MapSettingsModelToDB(Settings_Model s)
        {
            return null;
        }
        public ISB_BIA_Settings GetSettings()
        {
            return GetDummySettings();
        }

        public bool InsertSettings(ISB_BIA_Settings newSettings, ISB_BIA_Settings oldSettings)
        {
            return true;
        }
        #endregion

        #region Informationssegmente
        public ObservableCollection<ISB_BIA_Informationssegmente> GetDummySegments()
        {
            Random r = new Random();
            var people = from n in Enumerable.Range(1, 25)
                         select new ISB_BIA_Informationssegmente
                         {
                             Informationssegment_Id = n,
                             Name = "IS" + n,
                             Segment = "Segment" + n,
                             Beschreibung = "Beschreibung " + n,
                             Mögliche_Segmentinhalte = "Inhalt " + n,
                             Attribut_1 = "P",
                             Attribut_2 = "O",
                             Attribut_3 = "O",
                             Attribut_4 = "O",
                             Attribut_5 = "P",
                             Attribut_6 = "P" ,
                             Attribut_7 = "O",
                             Attribut_8 = "O",
                             Attribut_9 = "O",
                             Attribut_10 = "O",
                             Datum = DateTime.Now,
                             Benutzer = "Test"
                         };
            return new ObservableCollection<ISB_BIA_Informationssegmente>(people.ToList());
        }
        public ObservableCollection<ISB_BIA_Informationssegmente_Attribute> GetDummyAttributes()
        {
            Random r = new Random();
            var people = from n in Enumerable.Range(1, 100)
                         select new ISB_BIA_Informationssegmente_Attribute
                         {
                             Attribut_Id = n,
                             Name = "Att" + n,
                             Info = "Info " + n,
                             SZ_1 = r.Next(0, 5),
                             SZ_2 = r.Next(0, 5),
                             SZ_3 = r.Next(0, 5),
                             SZ_4 = r.Next(0, 5),
                             SZ_5 = r.Next(0, 5),
                             SZ_6 = r.Next(0, 5),
                             Datum = DateTime.Now,
                             Benutzer = "Test"
                         };
            return new ObservableCollection<ISB_BIA_Informationssegmente_Attribute>(people.ToList());
        }
        public InformationSegment_Model GetSegmentModelFromDB(int id)
        {
            ISB_BIA_Informationssegmente linqIS = GetDummySegments().FirstOrDefault();

            InformationSegment_Model result = new InformationSegment_Model()
            {
                Informationssegment_Id = linqIS.Informationssegment_Id,
                Name = linqIS.Name,
                Segment = linqIS.Segment,
                Beschreibung = linqIS.Beschreibung,
                Mögliche_Segmentinhalte = linqIS.Mögliche_Segmentinhalte,
                Attribut1 = (linqIS.Attribut_1 == "P") ? true : false,
                Attribut2 = (linqIS.Attribut_2 == "P") ? true : false,
                Attribut3 = (linqIS.Attribut_3 == "P") ? true : false,
                Attribut4 = (linqIS.Attribut_4 == "P") ? true : false,
                Attribut5 = (linqIS.Attribut_5 == "P") ? true : false,
                Attribut6 = (linqIS.Attribut_6 == "P") ? true : false,
                Attribut7 = (linqIS.Attribut_7 == "P") ? true : false,
                Attribut8 = (linqIS.Attribut_8 == "P") ? true : false,
                Attribut9 = (linqIS.Attribut_9 == "P") ? true : false,
                Attribut10 = (linqIS.Attribut_10 == "P") ? true : false
            };
            return result;
        }
        public ISB_BIA_Informationssegmente MapSegmentModelToDB(InformationSegment_Model i)
        {
            return null;
        }
        public InformationSegmentAttribute_Model GetAttributeModelFromDB(int id)
        {
            ISB_BIA_Informationssegmente_Attribute linqAttribute = GetDummyAttributes()[id-1];

            InformationSegmentAttribute_Model result = new InformationSegmentAttribute_Model()
            {
                Attribut_Id = linqAttribute.Attribut_Id,
                Name = linqAttribute.Name,
                Info = linqAttribute.Info,
                SZ_1 = linqAttribute.SZ_1.ToString(),
                SZ_2 = linqAttribute.SZ_2.ToString(),
                SZ_3 = linqAttribute.SZ_3.ToString(),
                SZ_4 = linqAttribute.SZ_4.ToString(),
                SZ_5 = linqAttribute.SZ_5.ToString(),
                SZ_6 = linqAttribute.SZ_6.ToString()
            };
            return result;
        }
        public ISB_BIA_Informationssegmente_Attribute MapAttributeModelToDB(InformationSegmentAttribute_Model ia)
        {
            return null;
        }
        public ObservableCollection<ISB_BIA_Informationssegmente> GetAllSegments()
        {
            return new ObservableCollection<ISB_BIA_Informationssegmente>(
                GetDummySegments().GroupBy(a => a.Informationssegment_Id)
                .Select(g => g.OrderByDescending(p => p.Datum).FirstOrDefault()).OrderBy(x => x.Informationssegment_Id).ToList());
        }
        public ObservableCollection<ISB_BIA_Informationssegmente> GetEnabledSegments()
        {
            return new ObservableCollection<ISB_BIA_Informationssegmente>(
                GetDummySegments().Where(x => x.Segment != "Lorem ipsum").GroupBy(a => a.Informationssegment_Id)
                .Select(g => g.OrderByDescending(p => p.Datum).FirstOrDefault()).OrderBy(x => x.Informationssegment_Id).ToList());
        }
        public List<ISB_BIA_Informationssegmente> Get5SegmentsForCalculation(Process_Model process)
        {
            process = new Process_Model { Relevantes_IS_1 = GetDummySegments()[0].Name, Relevantes_IS_2 = GetDummySegments()[1].Name };
                //Zutreffende Segmente auswählen
                return GetEnabledSegments().Where(x =>
                x.Name == process.Relevantes_IS_1 ||
                x.Name == process.Relevantes_IS_2 ||
                x.Name == process.Relevantes_IS_3 ||
                x.Name == process.Relevantes_IS_4 ||
                x.Name == process.Relevantes_IS_5).ToList();           
        }
        public ISB_BIA_Informationssegmente GetISByISName(string iSName)
        {
            return GetDummySegments().Where(y => y.Name == iSName).
                GroupBy(a => a.Informationssegment_Id).Select(g => g.OrderByDescending(p => p.Datum).FirstOrDefault()).FirstOrDefault();
        }
        public ObservableCollection<ISB_BIA_Informationssegmente_Attribute> GetAttributes()
        {
            return new ObservableCollection<ISB_BIA_Informationssegmente_Attribute>(
                GetDummyAttributes().GroupBy(a => a.Attribut_Id)
                .Select(g => g.OrderByDescending(p => p.Datum).FirstOrDefault()).OrderBy(x => x.Attribut_Id).ToList());
        }
        public ObservableCollection<string> GetAttributeNamesAndInfoForIS()
        {
            return new ObservableCollection<string>(
                GetDummyAttributes().GroupBy(x => x.Attribut_Id).
                Select(h => h.OrderByDescending(g => g.Datum).FirstOrDefault()).
                ToList().OrderBy(b => b.Attribut_Id).Select(s => String.Concat(s.Name, " ", s.Info)));
        }
        public ObservableCollection<string> GetAttributeNamesForHeader()
        {
            return new ObservableCollection<string>(
                GetDummyAttributes().GroupBy(x => x.Attribut_Id).
                Select(h => h.OrderByDescending(g => g.Datum).FirstOrDefault()).
                ToList().OrderBy(b => b.Attribut_Id).Select(s => s.Name));
        }
        public bool InsertIS(InformationSegment_Model newIS, InformationSegment_Model oldIS)
        {
            return true;
        }
        public bool InsertISAtt(ObservableCollection<InformationSegmentAttribute_Model> newAttributeList)
        {
            return true;
        }
        #endregion

        #region Prozess<-->Applikation
        public ObservableCollection<ISB_BIA_Applikationen> GetActiveApplications()
        {
                    return new ObservableCollection<ISB_BIA_Applikationen>(
                        GetDummyApplications().GroupBy(y => y.Applikation_Id).
                        Select(z => z.OrderByDescending(q => q.Datum).FirstOrDefault()).
                        Where(x => x.Aktiv == 1).OrderBy(k => k.Applikation_Id).ToList());
        }
        public ObservableCollection<string> GetApplicationCategories()
        {
                    List<ISB_BIA_Applikationen> ocCategories = GetDummyApplications().GroupBy(y => y.Applikation_Id).Select(z => z.OrderByDescending(p => p.Datum).FirstOrDefault()).Where(u => u.Aktiv == 1).GroupBy(x => x.IT_Betriebsart).Select(group => group.FirstOrDefault()).ToList();
                    //Für späteres Filtern: Eintrag für alle Kategorien
                    ocCategories.Insert(0, new ISB_BIA_Applikationen() { Applikation_Id = 0, IT_Betriebsart = "<Alle>" });
                    return new ObservableCollection<string>(ocCategories.Select(x => x.IT_Betriebsart));
        }
        public ObservableCollection<ISB_BIA_Delta_Analyse> GetProcessApplicationHistoryForProcess(int id)
        {
            return null;
        }
        public ObservableCollection<ISB_BIA_Delta_Analyse> GetProcessApplicationHistoryForApplication(int id)
        {
            return null;
        }
        #endregion

        #region Delta
        public ObservableCollection<ISB_BIA_Delta_Analyse> GetDummyDelta()
        {
            Random r = new Random();
            var people = from n in Enumerable.Range(1, 20)
                         select new ISB_BIA_Delta_Analyse
                         {
                             Prozess_Id = n,
                             Prozess = "Pro" + n,
                             Sub_Prozess = "Sub " + n,
                             Datum_Prozess = DateTime.Now,
                             Applikation_Id = n,
                             Applikation = "App"+n,
                             Datum_Applikation = DateTime.Now,
                             SZ_1 = 3,
                             SZ_2 = 3,
                             SZ_3 = 3,
                             SZ_4 = 3,
                             SZ_5 = 3,
                             SZ_6 = 3,
                             Datum = DateTime.Now,
                         };
            return new ObservableCollection<ISB_BIA_Delta_Analyse>(people.ToList());
        }

        public ObservableCollection<ISB_BIA_Delta_Analyse> GetDeltaAnalysis()
        {
            return new ObservableCollection<ISB_BIA_Delta_Analyse>(
                GetDummyDelta().OrderBy(x => x.Prozess_Id).ToList());
        }

        public ObservableCollection<ISB_BIA_Delta_Analyse> InitiateDeltaAnalysis(DateTime d)
        {
            return GetDummyDelta();
        }

        public ObservableCollection<ISB_BIA_Delta_Analyse> DateDeltaAnalysis(DateTime date, bool toDB, List<ISB_BIA_Prozesse_Applikationen> proc_App)
        {
            return GetDummyDelta();
        }
        #endregion

        #region OE
        public ObservableCollection<ISB_BIA_OEs> GetDummyOEs()
        {
            Random r = new Random();
            var people = from n in Enumerable.Range(1, 100)
                         select new ISB_BIA_OEs
                         {
                                Id = n,
                                OE_Name = "OE"+n,
                                OE_Nummer = r.Next(1,4)+"."+r.Next(1,3),
                                Datum = DateTime.Now,
                                Benutzer = "TestUser"+n
                         };
            return new ObservableCollection<ISB_BIA_OEs>(people.ToList());
        }

        public ObservableCollection<ISB_BIA_OEs> GetOENames()
        {
                    List<ISB_BIA_OEs> queryName = GetDummyOEs().Where(c => c.OE_Name != "").
                        GroupBy(x => x.OE_Name).Select(g => g.OrderBy(p => p.Datum).FirstOrDefault()).ToList();
                    return new ObservableCollection<ISB_BIA_OEs>(queryName);
        }
        public ObservableCollection<ISB_BIA_OEs> GetOENumbers()
        {
                    List<ISB_BIA_OEs> queryKennung = GetDummyOEs().Where(y => y.OE_Nummer != null && y.OE_Nummer != "").GroupBy(x => x.OE_Nummer).Select(g => g.OrderBy(p => p.Datum).FirstOrDefault()).ToList();
                    return new ObservableCollection<ISB_BIA_OEs>(queryKennung);
        }
        public ObservableCollection<ISB_BIA_OEs> GetOELinks()
        {
                    List<ISB_BIA_OEs> queryLink = GetDummyOEs().Where(x => x.OE_Nummer != "").OrderBy(c => c.OE_Name).ToList();
                    return new ObservableCollection<ISB_BIA_OEs>(queryLink);
        }

        public ISB_BIA_OEs InsertOEName(string name)
        {
            return GetDummyOEs().FirstOrDefault();
        }

        public bool EditOEName(string name, string oldName)
        {
            return true;
        }

        public bool DeleteOEName(string oeName)
        {
            throw new NotImplementedException();
        }

        public bool DeleteOELink(string oeName, string oeNumber)
        {
            return true;
        }

        public bool DeleteOENumber(string oeNumber)
        {
            return true;
        }

        public ISB_BIA_OEs InsertOELink(ISB_BIA_OEs name, ISB_BIA_OEs number)
        {
            return GetDummyOEs().FirstOrDefault();
        }

        public ISB_BIA_OEs InsertOENumber(string number, ISB_BIA_OEs name)
        {
            return GetDummyOEs().FirstOrDefault();
        }

        public bool EditOENumber(string number, string oldNumber)
        {
            return true;
        }
        #endregion

        #region Log
        public ObservableCollection<ISB_BIA_Log> GetDummyLog()
        {
            Random r = new Random();
            var people = from n in Enumerable.Range(1, 100)
                         select new ISB_BIA_Log
                         {
                             Id = n,
                             Action = "Aktion" + n,
                             Tabelle = "T"+r.Next(1,5),
                             Details = "Detail"+n,
                             Id_1 = r.Next(1, 400),
                             Id_2 = r.Next(1, 400),
                             Datum = DateTime.Now,
                             Benutzer = "TestUser" + n
                         };
            return new ObservableCollection<ISB_BIA_Log>(people.ToList());
        }

        public ObservableCollection<ISB_BIA_Log> GetLog()
        {
            return GetDummyLog();
        }

        public List<ISB_BIA_Settings> GetSettingsHistory()
        {
            throw new NotImplementedException();
        }



        #endregion
    }
}