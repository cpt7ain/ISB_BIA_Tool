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
    public class MyDesignTimeDataService_Process : IMyDataService_Process
    {
        public ObservableCollection<ISB_BIA_Prozesse> ProcessDummyList;
        readonly IMyDialogService _myDia;
        readonly IMySharedResourceService _myShared;
        readonly IMyDataService_OE _myOE;
        readonly IMyDataService_IS_Attribute _myIS;

        public MyDesignTimeDataService_Process(IMyDialogService myDia, IMySharedResourceService myShared, 
             IMyDataService_OE myOE, IMyDataService_IS_Attribute myIS)
        {
            this._myDia = myDia;
            this._myShared = myShared;
            this._myOE = myOE;
            this._myIS = myIS;
            ProcessDummyList = GetDummyProcesses();
        }

        #region Process
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
                         Benutzer = "TestUser" + r.Next(1, 10)
                     };
            var p2 = from n in Enumerable.Range(1, 10)
                     select new ISB_BIA_Prozesse
                     {
                         Prozess_Id = n + 10,
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
        public Process_Model Proc_Get_ProcessModelFromDB(int id)
        {
            ISB_BIA_Prozesse linqProc = ProcessDummyList.FirstOrDefault();
            ObservableCollection<ISB_BIA_Applikationen> linqApps = new ObservableCollection<ISB_BIA_Applikationen>();
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
        public ISB_BIA_Prozesse Proc_Map_ProcessModelToDB(Process_Model p)
        {
            return new ISB_BIA_Prozesse();
        }
        public Dictionary<string, string> IS_Get_ISDropDownList()
        {
            List<ISB_BIA_Informationssegmente> listIS = _myIS.Get_Segments_All().ToList();
            listIS.Insert(0, new ISB_BIA_Informationssegmente() { Name = "", Segment = "<leer>" });
            return new Dictionary<string, string>(listIS.Select(t => new { t.Name, t.Segment }).ToDictionary(t => t.Name, t => t.Segment));
        }
        public ObservableCollection<ISB_BIA_Prozesse> Proc_Get_ProcessesByOE(ObservableCollection<string> listOE)
        {
            return ProcessDummyList;
        }
        public ObservableCollection<ISB_BIA_Prozesse> Proc_Get_AllProcesses(DateTime? d = null)
        {
            return ProcessDummyList;
        }
        public ObservableCollection<ISB_BIA_Prozesse> Proc_Get_ActiveProcesses()
        {
            return new ObservableCollection<ISB_BIA_Prozesse>(ProcessDummyList.Where(x => x.Aktiv == 1).ToList());
        }
        public ObservableCollection<string> Proc_Get_ListProcessOwner()
        {
            return new ObservableCollection<string>(ProcessDummyList.Where(n => n.Prozessverantwortlicher != "" && n.Prozessverantwortlicher != " ").Select(p => p.Prozessverantwortlicher).Distinct());
        }
        public ObservableCollection<string> Proc_Get_ListOEsForUser(string userOE)
        {
            return new ObservableCollection<string>() { "4.4" };
        }
        public ObservableCollection<string> Proc_Get_ListOEs()
        {
            return new ObservableCollection<string>(_myOE.Get_OENames().Select(p => p.OE_Name).Distinct());
        }
        public ObservableCollection<string> Proc_Get_ListPreProcesses()
        {
            return new ObservableCollection<string>(ProcessDummyList.Select(p => p.Vorgelagerte_Prozesse).Distinct());
        }
        public ObservableCollection<string> Proc_Get_ListPostProcesses()
        {
            return new ObservableCollection<string>(ProcessDummyList.Select(p => p.Nachgelagerte_Prozesse).Distinct());
        }
        public ObservableCollection<ISB_BIA_Prozesse> Proc_Get_ProcessHistory(int process_id)
        {
            return new ObservableCollection<ISB_BIA_Prozesse>();
        }
        public bool Proc_Insert_ProcessAndRelations(Process_Model p, ProcAppMode mode, ObservableCollection<ISB_BIA_Applikationen> add, ObservableCollection<ISB_BIA_Applikationen> remove)
        {
            return true;
        }
        public ISB_BIA_Prozesse Proc_Delete_Process(ISB_BIA_Prozesse p)
        {
            return p;
        }
        public ISB_BIA_Prozesse TryDeleteProcess(ISB_BIA_Prozesse toDelete)
        {
            return Proc_Get_AllProcesses().FirstOrDefault();
        }
        public bool Proc_Insert_AllProcesses(ObservableCollection<ISB_BIA_Prozesse> pList)
        {
            return true;
        }
        public ObservableCollection<ISB_BIA_Delta_Analyse> Get_History_ProcAppRelations(int id)
        {
            return new ObservableCollection<ISB_BIA_Delta_Analyse>();
        }
        public ObservableCollection<string> Get_List_AppCategories()
        {
            return new ObservableCollection<string>();
        }

        public Process_Model Get_ModelFromDB(int id)
        {
            throw new NotImplementedException();
        }

        public ISB_BIA_Prozesse Map_ModelToDB(Process_Model p)
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<ISB_BIA_Prozesse> Get_Processes_ByOE(ObservableCollection<string> listOE)
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<ISB_BIA_Prozesse> Get_Processes_All(DateTime? d = null)
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<ISB_BIA_Prozesse> Get_Processes_Active()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> Get_List_ISDictionary()
        {
            throw new NotImplementedException();
        }

        public ISB_BIA_Informationssegmente Get_IS_ByName(string ISName)
        {
            throw new NotImplementedException();
        }

        public List<ISB_BIA_Informationssegmente> Get_Segments_5ForCalculation(Process_Model process)
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<string> Get_List_ProcessOwner()
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<string> Get_List_OEsForUser(string userOE)
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<string> Get_List_OEs_All()
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<string> Get_List_PreProcesses()
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<string> Get_List_PostProcesses()
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<ISB_BIA_Prozesse> Get_History_Process(int process_id)
        {
            throw new NotImplementedException();
        }

        public bool Insert_ProcessAndRelations(Process_Model p, ProcAppMode mode, ObservableCollection<ISB_BIA_Applikationen> add, ObservableCollection<ISB_BIA_Applikationen> remove)
        {
            return true;
        }

        public ISB_BIA_Prozesse Delete_Process(ISB_BIA_Prozesse p)
        {
            return new ISB_BIA_Prozesse();
        }

        public bool Insert_Processes_All(ObservableCollection<ISB_BIA_Prozesse> pList)
        {
            return true;
        }

        public ObservableCollection<ISB_BIA_Applikationen> Get_Applications_Active()
        {
            return new ObservableCollection<ISB_BIA_Applikationen>();
        }
        #endregion

    }
}
