using ISB_BIA_IMPORT1.Model;
using ISB_BIA_IMPORT1.ViewModel;
using ISB_BIA_IMPORT1.LINQ2SQL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Data;
using ISB_BIA_IMPORT1.Services.Interfaces;

namespace ISB_BIA_IMPORT1.Services
{
    class MyDataService_Process : IMyDataService_Process
    {
        readonly IMyDialogService _myDia;
        readonly IMySharedResourceService _myShared;
        readonly IMyMailNotificationService _myMail;
        readonly IMyDataService_Lock _myLock;
        readonly IMyDataService_IS_Attribute _myIS;

        public MyDataService_Process(IMyDialogService myDia, IMySharedResourceService myShared, 
            IMyMailNotificationService myMail, IMyDataService_Lock myLock, IMyDataService_IS_Attribute myIS)
        {
            this._myDia = myDia;
            this._myShared = myShared;
            this._myMail = myMail;
            this._myLock = myLock;
            this._myIS = myIS;
        }

        #region Process
        public Process_Model Get_ModelFromDB(int id)
        {
            try
            {
                //Letzte Version des Prozesses abrufen
                ISB_BIA_Prozesse linqProc;
                ObservableCollection<ISB_BIA_Applikationen> linqApps= new ObservableCollection<ISB_BIA_Applikationen>();
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    //Prozess
                    linqProc = db.ISB_BIA_Prozesse.Where(c => c.Prozess_Id == id).OrderByDescending(p => p.Datum).FirstOrDefault();
                    //Applikationsliste
                    List<ISB_BIA_Prozesse_Applikationen> proc_AppCurrent = db.ISB_BIA_Prozesse_Applikationen.Where(x => x.Prozess_Id == id).GroupBy(y => y.Applikation_Id).Select(z => z.OrderByDescending(q => q.Datum).FirstOrDefault()).ToList();
                    List<int> listIdCurrentApplications = proc_AppCurrent.Where(x => x.Relation == 1).Select(y => y.Applikation_Id).ToList();
                    if (listIdCurrentApplications.Count == 0)
                        linqApps = new ObservableCollection<ISB_BIA_Applikationen>();
                    else
                        linqApps = new ObservableCollection<ISB_BIA_Applikationen>(db.ISB_BIA_Applikationen.Where(x => listIdCurrentApplications.Contains(x.Applikation_Id)).GroupBy(y => y.Applikation_Id).Select(z => z.OrderByDescending(q => q.Datum).FirstOrDefault()).OrderBy(k => k.Applikation_Id).ToList());

                }
                //Falls existiert
                if (linqProc != null && linqApps != null)
                {
                    Process_Model result = new Process_Model()
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
                    return result;
                }
                else
                {
                    _myDia.ShowError("Prozessdaten konnten nicht abgerufen werden");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Prozessdaten konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public ISB_BIA_Prozesse Map_ModelToDB(Process_Model p)
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext())
                {
                    List<ISB_BIA_Informationssegmente> ISListForDates = db.ISB_BIA_Informationssegmente
                        .GroupBy(x => x.Name)
                        .Select(c => c.OrderByDescending(v => v.Datum).FirstOrDefault()).ToList();
                    ISB_BIA_Prozesse res = new ISB_BIA_Prozesse()
                    {
                        Prozess_Id = p.Prozess_Id,
                        OE_Filter = p.OE_Filter,
                        Prozess = p.Prozess,
                        Sub_Prozess = p.Sub_Prozess,
                        Prozessverantwortlicher = p.Prozessverantwortlicher,
                        Kritikalität_des_Prozesses = p.Kritikalität_des_Prozesses,
                        Kritischer_Prozess = (p.Kritischer_Prozess == "Ja") ? "x" : "",
                        Reifegrad_des_Prozesses = p.Reifegrad_des_Prozesses,
                        Regulatorisch = (p.Regulatorisch) ? "x" : "",
                        Reputatorisch = (p.Reputatorisch) ? "x" : "",
                        Finanziell = (p.Finanziell) ? "x" : "",
                        SZ_1 = (int) p.SZ_1,
                        SZ_2 = (int) p.SZ_2,
                        SZ_3 = (int) p.SZ_3,
                        SZ_4 = (int) p.SZ_4,
                        SZ_5 = (int) p.SZ_5,
                        SZ_6 = (int) p.SZ_6,
                        Vorgelagerte_Prozesse = p.Vorgelagerte_Prozesse,
                        Nachgelagerte_Prozesse = p.Nachgelagerte_Prozesse,
                        Servicezeit_Helpdesk = p.Servicezeit_Helpdesk,
                        RPO_Datenverlustzeit_Recovery_Point_Objective = p.RPO_Datenverlustzeit_Recovery_Point_Objective,
                        RTO_Wiederanlaufzeit_Recovery_Time_Objective = p.RTO_Wiederanlaufzeit_Recovery_Time_Objective,
                        RTO_Wiederanlaufzeit_Recovery_Time_Objective_Notfall =
                            p.RTO_Wiederanlaufzeit_Recovery_Time_Objective_Notfall,
                        Relevantes_IS_1 = p.Relevantes_IS_1,
                        Relevantes_IS_2 = p.Relevantes_IS_2,
                        Relevantes_IS_3 = p.Relevantes_IS_3,
                        Relevantes_IS_4 = p.Relevantes_IS_4,
                        Relevantes_IS_5 = p.Relevantes_IS_5,
                        Aktiv = p.Aktiv,
                        Datum = p.Datum,
                        Benutzer = p.Benutzer
                    };
                    return res;
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Fehler beim Mappen des Prozesses", ex);
                return null;
            }
        }
        public Dictionary<string, string> Get_List_ISDictionary()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    List<ISB_BIA_Informationssegmente> listIS = db.ISB_BIA_Informationssegmente.Where(x => x.Segment != "Lorem ipsum").GroupBy(y => y.Informationssegment_Id).Select(c => c.OrderByDescending(d => d.Datum).FirstOrDefault()).ToList();
                    listIS.Insert(0, new ISB_BIA_Informationssegmente() { Name = "", Segment = "<leer>" });
                    return new Dictionary<string, string>(listIS.Select(t => new { t.Name, t.Segment }).ToDictionary(t => t.Name, t => t.Segment));
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Liste der Segmente konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public ISB_BIA_Informationssegmente Get_IS_ByName(string iSName)
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    return db.ISB_BIA_Informationssegmente.Where(y => y.Name == iSName).
                        GroupBy(a => a.Name).Select(g => g.OrderByDescending(p => p.Datum).FirstOrDefault()).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Informationssegment konnten nicht abgerufen werden.", ex);
                return null;
            }
        }
        public List<ISB_BIA_Informationssegmente> Get_Segments_5ForCalculation(Process_Model process)
        {
            using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
            {
                //Zutreffende Segmente auswählen
                return _myIS.Get_Segments_Enabled().Where(x =>
                    x.Name == process.Relevantes_IS_1 ||
                    x.Name == process.Relevantes_IS_2 ||
                    x.Name == process.Relevantes_IS_3 ||
                    x.Name == process.Relevantes_IS_4 ||
                    x.Name == process.Relevantes_IS_5).ToList();
            }
        }
        public ObservableCollection<ISB_BIA_Prozesse> Get_Processes_ByOE(ObservableCollection<string> listOE)
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    return new ObservableCollection<ISB_BIA_Prozesse>(
                        db.ISB_BIA_Prozesse.Where(n => listOE.Contains(n.OE_Filter)).GroupBy(p => p.Prozess_Id)
                        .Select(g => g.OrderByDescending(p => p.Datum).FirstOrDefault()).Where(v => v.Aktiv == 1).OrderBy(x => x.Prozess_Id).ToList());
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Prozesse konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public ObservableCollection<ISB_BIA_Prozesse> Get_Processes_All(DateTime? date = null)
        {
            try
            {
                if (!date.HasValue) date = DateTime.Now;

                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    return new ObservableCollection<ISB_BIA_Prozesse>(
                        db.ISB_BIA_Prozesse.Where(d => d.Datum <= date).GroupBy(p => p.Prozess_Id)
                        .Select(g => g.OrderByDescending(p => p.Datum).FirstOrDefault()).
                        OrderBy(x => x.Prozess_Id).ToList());
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Prozesse konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public ObservableCollection<ISB_BIA_Prozesse> Get_Processes_Active()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    return new ObservableCollection<ISB_BIA_Prozesse>(
                        db.ISB_BIA_Prozesse.GroupBy(p => p.Prozess_Id)
                        .Select(g => g.OrderByDescending(p => p.Datum).FirstOrDefault()).Where(v => v.Aktiv == 1).
                        OrderBy(x => x.Prozess_Id).ToList());
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Prozesse konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public ObservableCollection<string> Get_List_ProcessOwner()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    return new ObservableCollection<string>(db.ISB_BIA_Prozesse.Where(n => n.Prozessverantwortlicher != "" && n.Prozessverantwortlicher != " ").Select(p => p.Prozessverantwortlicher).Distinct());
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Prozesseigentümer konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public ObservableCollection<string> Get_List_OEsForUser(string userOE)
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    return new ObservableCollection<string>(db.ISB_BIA_OEs.Where(x => x.OE_Nummer.StartsWith(userOE)).Select(p => p.OE_Name).Distinct());
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("OE's konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public ObservableCollection<string> Get_List_OEs_All()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    return new ObservableCollection<string>(db.ISB_BIA_OEs.Select(p => p.OE_Name).Distinct());
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("OE's konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public ObservableCollection<string> Get_List_PreProcesses()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    return new ObservableCollection<string>(db.ISB_BIA_Prozesse.Select(p => p.Vorgelagerte_Prozesse).ToList().Distinct(StringComparer.Ordinal).OrderBy(a => a));
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Vorgelagerte Prozesse konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public ObservableCollection<string> Get_List_PostProcesses()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    return new ObservableCollection<string>(db.ISB_BIA_Prozesse.Select(p => p.Nachgelagerte_Prozesse).ToList().Distinct(StringComparer.Ordinal).OrderBy(a => a));
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Nachgelagerte Prozesse konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public ObservableCollection<ISB_BIA_Prozesse> Get_History_Process(int process_id)
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    return new ObservableCollection<ISB_BIA_Prozesse>(
                        db.ISB_BIA_Prozesse.Where(p => p.Prozess_Id == process_id)
                        .OrderByDescending(p => p.Datum).ToList());
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Prozesse konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public bool Insert_ProcessAndRelations(Process_Model p, ProcAppMode mode, ObservableCollection<ISB_BIA_Applikationen> add, ObservableCollection<ISB_BIA_Applikationen> remove)
        {
            if (p.IsValid)
            {
                try
                {
                    //In Datenbank schreiben
                    using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                    {
                        int duplicateCount = Get_Processes_All().Where(x => x.Prozess == p.Prozess && x.Sub_Prozess == p.Sub_Prozess && x.OE_Filter == p.OE_Filter).Count();
                        if (duplicateCount > 0)
                        {
                            _myDia.ShowMessage("Es existiert(e) bereits ein Prozess mit diesem Prozess- und Sub-Prozess Namen in dieser OE.\nFalls es sich um einen gelöschten Prozess handelt, den Sie reaktivieren möchten, wenden Sie sich bitte an die IT.\nAndernfalls wählen Sie bitte andere Bezeichnungen für diesen Prozess.");
                            return false;
                        }
                        //Bei Bearbeitung eines inaktiven Prozesses (durch Admin oder CISO) beim Speichern fragen, ob er auf aktiv gesetzt werden soll
                        if (p.Aktiv == 0)
                        {
                            p.Aktiv = (_myDia.ShowQuestion("Der Prozess ist momentan auf inaktiv gesetzt. Möchten Sie den Prozess auf aktiv setzen?", "Prozess aktivieren")) ? 1 : 0;
                        }
                        if (mode == ProcAppMode.New)
                        {
                            p.Prozess_Id = db.ISB_BIA_Prozesse.Max(x => x.Prozess_Id) + 1;
                        }
                        DateTime d = DateTime.Now;
                        //Bei Neuanlage neue ID berechnen

                        p.Benutzer = Environment.UserName;
                        p.Datum = d;
                        //Nach DB-Format Mappen und einfügen
                        db.ISB_BIA_Prozesse.InsertOnSubmit(Map_ModelToDB(p));
                        //Logeintrag erzeugen
                        ISB_BIA_Log logEntry = new ISB_BIA_Log
                        {
                            Aktion = "Ändern/Erstellen eines Prozesses",
                            Tabelle = _myShared.Tbl_Prozesse,
                            Details = "Id = " + p.Prozess_Id + ", Name = '" + p.Prozess + "', Sub-Name = '" + p.Sub_Prozess + "'",
                            Id_1 = p.Prozess_Id,
                            Id_2 = 0,
                            Datum = d,
                            Benutzer = p.Benutzer
                        };
                        db.ISB_BIA_Log.InsertOnSubmit(logEntry);

                        #region relation
                        int k = add.Count;
                        int i = 0;
                        List<ISB_BIA_Applikationen> result = add.Concat(remove).ToList();
                        foreach (ISB_BIA_Applikationen a in result)
                        {
                            i++;
                            ISB_BIA_Prozesse_Applikationen proc_app = new ISB_BIA_Prozesse_Applikationen
                            {
                                Applikation_Id = a.Applikation_Id,
                                Datum_Applikation = a.Datum,
                                Prozess_Id = p.Prozess_Id,
                                Datum_Prozess = d,
                                Relation = (i <= k) ? 1 : 0,
                                Datum = d,
                                Benutzer = Environment.UserName
                            };

                            //Schreiben in Datenbank
                            db.ISB_BIA_Prozesse_Applikationen.InsertOnSubmit(proc_app);

                            string s = (proc_app.Relation == 1) ? "Verknüpfung" : "Trennung";

                            ISB_BIA_Log logEntryRelation = new ISB_BIA_Log
                            {
                                Aktion = "Ändern einer Prozesses-Applikations-Relation: " + s,
                                Tabelle = _myShared.Tbl_Proz_App,
                                Details = "Prozess Id = " + proc_app.Prozess_Id + ", App. Id = " + proc_app.Applikation_Id + ", App. Name = '" + a.IT_Anwendung_System + "'",
                                Id_1 = proc_app.Prozess_Id,
                                Id_2 = proc_app.Applikation_Id,
                                Datum = d,
                                Benutzer = p.Benutzer,
                            };
                            db.ISB_BIA_Log.InsertOnSubmit(logEntryRelation);
                        }
                        #endregion

                        #region Notification-Mail senden
                        string subject;
                        if (mode == ProcAppMode.Change)
                        {
                            subject = "BIA-Tool Auto-Notification: Prozess bearbeitet";
                        }
                        else
                        {
                            subject = "BIA-Tool Auto-Notification: Prozess erstellt";
                        }
                        string body = "Prozess-ID: " + p.Prozess_Id + Environment.NewLine +
                            "Prozess Name: " + p.Prozess + Environment.NewLine +
                            "OE: " + p.OE_Filter + Environment.NewLine +
                            "Prozessverantwortlicher: " + p.Prozessverantwortlicher + Environment.NewLine +
                            "Datum: " + d;
                        _myMail.Send_NotificationMail(subject, body, _myShared.CurrentEnvironment);
                        #endregion
                        db.SubmitChanges();
                    }
                    _myDia.ShowInfo("Prozess gespeichert");
                    return true;
                }
                catch (Exception ex1)
                {
                    //LogEntry bei Fehler erstellen + Schreiben in Datenbank
                    try
                    {
                        using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                        {
                            ISB_BIA_Log logEntry = new ISB_BIA_Log
                            {
                                Aktion = "Fehler: Ändern/Erstellen eines Prozesses",
                                Tabelle = _myShared.Tbl_Prozesse,
                                Details = ex1.Message,
                                Id_1 = p.Prozess_Id,
                                Id_2 = 0,
                                Datum = DateTime.Now,
                                Benutzer = Environment.UserName
                            };

                            db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                            db.SubmitChanges();
                            _myDia.ShowError("Fehler beim Speichern des Prozesses!\nEin Log Eintrag wurde erzeugt.", ex1);
                            return false;
                        }
                    }
                    catch (Exception ex2)
                    {
                        _myDia.ShowError("Fehler beim Speichern des Prozesses!\nEin Log Eintrag konnte ebenfalls nicht erzeugt werden.", ex2);
                        return false;
                    }
                }
            }
            else
            {
                _myDia.ShowWarning("Bitte füllen Sie alle Pflichtfelder aus.\nPflichtfelder sind fett markiert und werden für Sie nun rot gekennzeichnet.");
                return false;
            }
        }
        #region Prozess löschen (Setzen des Aktiv Flags in der Datenbank auf 0)
        public ISB_BIA_Prozesse Delete_Process(ISB_BIA_Prozesse p)
        {
            ISB_BIA_Prozesse toDelete = new ISB_BIA_Prozesse
            {
                Prozess_Id = p.Prozess_Id,
                OE_Filter = p.OE_Filter,
                Prozess = p.Prozess,
                Sub_Prozess = p.Sub_Prozess,
                Prozessverantwortlicher = p.Prozessverantwortlicher,
                Kritischer_Prozess = p.Kritischer_Prozess,
                Kritikalität_des_Prozesses = p.Kritikalität_des_Prozesses,
                Reifegrad_des_Prozesses = p.Reifegrad_des_Prozesses,
                Regulatorisch = p.Regulatorisch,
                Reputatorisch = p.Reputatorisch,
                Finanziell = p.Finanziell,
                SZ_1 = p.SZ_1,
                SZ_2 = p.SZ_2,
                SZ_3 = p.SZ_3,
                SZ_4 = p.SZ_4,
                SZ_5 = p.SZ_5,
                SZ_6 = p.SZ_6,
                Vorgelagerte_Prozesse = p.Vorgelagerte_Prozesse,
                Nachgelagerte_Prozesse = p.Nachgelagerte_Prozesse,
                Servicezeit_Helpdesk = p.Servicezeit_Helpdesk,
                RPO_Datenverlustzeit_Recovery_Point_Objective = p.RPO_Datenverlustzeit_Recovery_Point_Objective,
                RTO_Wiederanlaufzeit_Recovery_Time_Objective = p.RTO_Wiederanlaufzeit_Recovery_Time_Objective,
                RTO_Wiederanlaufzeit_Recovery_Time_Objective_Notfall = p.RTO_Wiederanlaufzeit_Recovery_Time_Objective_Notfall,
                Relevantes_IS_1 = p.Relevantes_IS_1,
                Relevantes_IS_2 = p.Relevantes_IS_2,
                Relevantes_IS_3 = p.Relevantes_IS_3,
                Relevantes_IS_4 = p.Relevantes_IS_4,
                Relevantes_IS_5 = p.Relevantes_IS_5,
                Datum = DateTime.Now,
                Benutzer = Environment.UserName,
                Aktiv = 0
            };
            bool res = _myDia.ShowQuestion("Möchten Sie den Prozess wirklich löschen?", "Prozess löschen");
            if (!res) return null;
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    toDelete.Datum = DateTime.Now;
                    db.ISB_BIA_Prozesse.InsertOnSubmit(toDelete);
                    //Logeintrag erzeugen
                    ISB_BIA_Log logEntry = new ISB_BIA_Log
                    {
                        Aktion = "Löschen eines Prozesses (Setzen auf inaktiv)",
                        Tabelle = _myShared.Tbl_Prozesse,
                        Details = "Id = " + toDelete.Prozess_Id + ", Name = '" + toDelete.Prozess + "', Sub-Name = '" + toDelete.Sub_Prozess + "'",
                        Id_1 = toDelete.Prozess_Id,
                        Id_2 = 0,
                        Datum = toDelete.Datum,
                        Benutzer = Environment.UserName
                    };
                    db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                    db.SubmitChanges();
                    #region Mail Notification senden
                    string subject = "BIA-Tool Auto-Notification: Prozess gelöscht";
                    string body = "Prozess-ID: " + toDelete.Prozess_Id + Environment.NewLine +
                        "Prozess Name: " + toDelete.Prozess + Environment.NewLine +
                        "OE: " + toDelete.OE_Filter + Environment.NewLine +
                        "Prozessverantwortlicher: " + toDelete.Prozessverantwortlicher + Environment.NewLine +
                        "Datum: " + toDelete.Datum;
                    _myMail.Send_NotificationMail(subject, body, _myShared.CurrentEnvironment);
                    #endregion
                }
                _myDia.ShowMessage("Prozess gelöscht");
                return toDelete;
            }
            catch (Exception ex1)
            {
                //Logeintrag bei Fehler
                try
                {
                    using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                    {
                        ISB_BIA_Log logEntry = new ISB_BIA_Log
                        {
                            Aktion = "Fehler: Löschen eines Prozesses",
                            Tabelle = _myShared.Tbl_Prozesse,
                            Details = ex1.Message,
                            Id_1 = toDelete.Prozess_Id,
                            Id_2 = 0,
                            Datum = toDelete.Datum,
                            Benutzer = Environment.UserName
                        };
                        db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                        db.SubmitChanges();
                        _myDia.ShowError("Prozess konnte nicht gelöscht werden.\n", ex1);
                        return null;
                    }
                }
                catch (Exception ex2)
                {
                    _myDia.ShowError("Prozess konnte nicht gelöscht werden!\nEin Log Eintrag konnte ebenfalls nicht erzeugt werden.\n", ex2);
                    return null;
                }
            }
        }
        #endregion
        public bool Insert_Processes_All(ObservableCollection<ISB_BIA_Prozesse> pList)
        {
            List<ISB_BIA_Prozesse> refreshedProcesses = new List<ISB_BIA_Prozesse>();

            bool res = _myDia.ShowQuestion("Möchten Sie die ausgewählten Prozesse wirklich ohne Änderungen aktualisieren?", "Auswahl bestätigen");
            if (res)
            {
                //Liste der gesperrten Prozesse
                List<ISB_BIA_Prozesse> lockedList = new List<ISB_BIA_Prozesse>();
                //String für die Nachricht welcher Prozess durch welchen User gesperrt ist
                string lockedListStringMsg = "";
                foreach (ISB_BIA_Prozesse p in pList)
                {
                    string user = _myLock.Get_ObjectIsLocked(Table_Lock_Flags.Process, p.Prozess_Id);
                    if (user != "")
                    {
                        //Wenn Prozess gesperrt zur Liste hinzufügen
                        lockedList.Add(p);
                        lockedListStringMsg = lockedListStringMsg + p.Prozess + " (geöffnet von: " + user + ")\n";
                    }
                }
                //Falls kein Prozess gesperrt ist
                if (lockedList.Count == 0)
                {
                    //Schreiben in DB
                    try
                    {
                        using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                        {
                            foreach (ISB_BIA_Prozesse process_old in pList)
                            {
                                //zu speichernden Prozess in neues Objekt "kopieren"
                                ISB_BIA_Prozesse process_refresh = new ISB_BIA_Prozesse
                                {
                                    Prozess_Id = process_old.Prozess_Id,
                                    Aktiv = process_old.Aktiv,
                                    OE_Filter = process_old.OE_Filter,
                                    Prozess = process_old.Prozess,
                                    Sub_Prozess = process_old.Sub_Prozess,
                                    Prozessverantwortlicher = process_old.Prozessverantwortlicher,
                                    Kritikalität_des_Prozesses = process_old.Kritikalität_des_Prozesses,
                                    Kritischer_Prozess = process_old.Kritischer_Prozess,
                                    Reifegrad_des_Prozesses = process_old.Reifegrad_des_Prozesses,
                                    Regulatorisch = process_old.Regulatorisch,
                                    Reputatorisch = process_old.Reputatorisch,
                                    Finanziell = process_old.Finanziell,
                                    SZ_1 = process_old.SZ_1,
                                    SZ_2 = process_old.SZ_2,
                                    SZ_3 = process_old.SZ_3,
                                    SZ_4 = process_old.SZ_4,
                                    SZ_5 = process_old.SZ_5,
                                    SZ_6 = process_old.SZ_6,
                                    Vorgelagerte_Prozesse = process_old.Vorgelagerte_Prozesse,
                                    Nachgelagerte_Prozesse = process_old.Nachgelagerte_Prozesse,
                                    Servicezeit_Helpdesk = process_old.Servicezeit_Helpdesk,
                                    RPO_Datenverlustzeit_Recovery_Point_Objective = process_old.RPO_Datenverlustzeit_Recovery_Point_Objective,
                                    RTO_Wiederanlaufzeit_Recovery_Time_Objective = process_old.RTO_Wiederanlaufzeit_Recovery_Time_Objective,
                                    RTO_Wiederanlaufzeit_Recovery_Time_Objective_Notfall = process_old.RTO_Wiederanlaufzeit_Recovery_Time_Objective_Notfall,
                                    Relevantes_IS_1 = process_old.Relevantes_IS_1,
                                    Relevantes_IS_2 = process_old.Relevantes_IS_2,
                                    Relevantes_IS_3 = process_old.Relevantes_IS_3,
                                    Relevantes_IS_4 = process_old.Relevantes_IS_4,
                                    Relevantes_IS_5 = process_old.Relevantes_IS_5,
                                };
                                process_refresh.Benutzer = Environment.UserName;
                                process_refresh.Datum = DateTime.Now;
                                db.ISB_BIA_Prozesse.InsertOnSubmit(process_refresh);
                                //Logeintrag erzeugen
                                ISB_BIA_Log logEntry = new ISB_BIA_Log
                                {
                                    Datum = process_refresh.Datum,
                                    Aktion = "Aktualisieren eines Prozesses ohne Änderungen",
                                    Tabelle = _myShared.Tbl_Prozesse,
                                    Details = "Id = " + process_refresh.Prozess_Id + ", Name = '" + process_refresh.Prozess + "', Sub-Name = '" + process_refresh.Sub_Prozess + "'",
                                    Id_1 = process_refresh.Prozess_Id,
                                    Id_2 = 0,
                                    Benutzer = Environment.UserName
                                };
                                db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                                refreshedProcesses.Add(process_refresh);
                            }
                            db.SubmitChanges();
                            #region Mail Notification senden
                            string body = "Benutzer: " + Environment.UserName + "\nErfolgreich: " + refreshedProcesses.Count + "\nFolgende Prozesse wurden ohne Änderungen aktualisiert:";
                            string subject = "BIA-Tool Auto-Notification: Aktualisieren von Prozessen";
                            body += Environment.NewLine;
                            foreach (ISB_BIA_Prozesse h in refreshedProcesses)
                            {
                                body += Environment.NewLine;
                                body += h.Prozess_Id + ", " + h.Prozess + ", " + h.Sub_Prozess;
                            }
                            _myMail.Send_NotificationMail(subject, body, _myShared.CurrentEnvironment);
                            #endregion
                            _myDia.ShowInfo("Prozesse erfolgreich gespeichert.");
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        //Bei Fehler Logeintrag
                        try
                        {
                            using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                            {
                                ISB_BIA_Log logEntry = new ISB_BIA_Log
                                {
                                    Datum = DateTime.Now,
                                    Aktion = "Fehler: Aktualisieren von Prozessen ohne Änderungen",
                                    Tabelle = _myShared.Tbl_Prozesse,
                                    Details = ex.Message,
                                    Id_1 = 0,
                                    Id_2 = 0,
                                    Benutzer = Environment.UserName
                                };
                                db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                                db.SubmitChanges();
                            }
                            _myDia.ShowError("Die ausgewählten Prozesse konnten nicht gespeichert werden.", ex);
                            return false;
                        }
                        catch (Exception ex1)
                        {
                            _myDia.ShowError("Die ausgewählten Prozesse konnten nicht gespeichert werden.", ex1);
                            return false;
                        }
                    }
                }
                else
                {
                    string msg = "In der Auswahl befinden sich Prozesse, die momentan durch andere User geöffnet sind und deshalb nicht gespeichert werden können.\nBitte warten Sie, bis die Bearbeitung beendet ist oder deselektieren Sie betroffene Prozesse.\n\n";
                    _myDia.ShowWarning(msg + lockedListStringMsg);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public ObservableCollection<ISB_BIA_Applikationen> Get_Applications_Active()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    return new ObservableCollection<ISB_BIA_Applikationen>(
                        db.ISB_BIA_Applikationen.GroupBy(y => y.Applikation_Id).
                        Select(z => z.OrderByDescending(q => q.Datum).FirstOrDefault()).
                        Where(x => x.Aktiv == 1).OrderBy(k => k.Applikation_Id).ToList());

                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Applikationen konnten nicht abgerufen werden.", ex);
                return null;
            }
        }
        public ObservableCollection<string> Get_List_AppCategories()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    List<ISB_BIA_Applikationen> ocCategories = db.ISB_BIA_Applikationen.GroupBy(y => y.Applikation_Id).Select(z => z.OrderByDescending(p => p.Datum).FirstOrDefault()).Where(u => u.Aktiv == 1).GroupBy(x => x.IT_Betriebsart).Select(group => group.FirstOrDefault()).ToList();
                    //Für späteres Filtern: Eintrag für alle Kategorien
                    ocCategories.Insert(0, new ISB_BIA_Applikationen() { Applikation_Id = 0, IT_Betriebsart = "<Alle>" });
                    return new ObservableCollection<string>(ocCategories.Select(x => x.IT_Betriebsart));

                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Anwendungskategorien konnten nicht abgerufen werden.", ex);
                return null;
            }
        }
        public ObservableCollection<ISB_BIA_Delta_Analyse> Get_History_ProcAppRelations(int id)
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    ObservableCollection<ISB_BIA_Delta_Analyse> result = new ObservableCollection<ISB_BIA_Delta_Analyse>();
                    List<ISB_BIA_Prozesse_Applikationen> procAppList = db.ISB_BIA_Prozesse_Applikationen.Where(x => x.Prozess_Id == id).OrderByDescending(c => c.Datum).ToList();
                    List<ISB_BIA_Prozesse_Applikationen> procAppListCurrent = procAppList.GroupBy(x => x.Applikation_Id)
                        .Select(c => c.OrderByDescending(v => v.Datum).FirstOrDefault()).ToList();
                    List<ISB_BIA_Prozesse> processes = db.ISB_BIA_Prozesse.ToList();
                    List<ISB_BIA_Applikationen> applications = db.ISB_BIA_Applikationen.ToList();
                    foreach (ISB_BIA_Prozesse_Applikationen k in procAppList)
                    {
                        ISB_BIA_Prozesse p = processes.Where(x => x.Prozess_Id == k.Prozess_Id).GroupBy(l => l.Prozess_Id).Select(g => g.OrderByDescending(d => d.Datum).FirstOrDefault()).FirstOrDefault();
                        ISB_BIA_Applikationen a = applications.Where(x => x.Applikation_Id == k.Applikation_Id).GroupBy(l => l.Applikation_Id).Select(g => g.OrderByDescending(d => d.Datum).FirstOrDefault()).FirstOrDefault();

                        ISB_BIA_Delta_Analyse n = new ISB_BIA_Delta_Analyse()
                        {
                            Prozess_Id = p.Prozess_Id,
                            Prozess = p.Prozess,
                            Sub_Prozess = p.Sub_Prozess,
                            Applikation_Id = a.Applikation_Id,
                            Applikation = a.IT_Anwendung_System,
                            SZ_1 = k.Relation,
                            SZ_2 = (procAppListCurrent.Count(x => x.Applikation_Id == k.Applikation_Id && x.Prozess_Id == k.Prozess_Id && x.Datum == k.Datum) > 0) ? 1 : 0,
                            Datum = k.Datum
                        };
                        result.Add(n);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Anwendungskategorien konnten nicht abgerufen werden.", ex);
                return null;
            }
        }
        #endregion
        
    }
}
