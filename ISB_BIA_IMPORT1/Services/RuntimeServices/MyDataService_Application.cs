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
    class MyDataService_Application : IMyDataService_Application
    {
        readonly IMyDialogService _myDia;
        readonly IMySharedResourceService _myShared;
        readonly IMyDataService_Lock _myLock;

        public MyDataService_Application(IMyDialogService myDia, IMySharedResourceService myShared, IMyDataService_Lock myLock)
        {
            this._myDia = myDia;
            this._myShared = myShared;
            this._myLock = myLock;
        }
 
        #region Application
        public Application_Model Get_ModelFromDB(int id)
        {
            try
            {
                ISB_BIA_Applikationen linqApp;
                ObservableCollection<ISB_BIA_Prozesse> linqProcs;
                //Letzte Version der Anwendung abrufen
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    linqApp = db.ISB_BIA_Applikationen.Where(c => c.Applikation_Id == id)
                        .OrderByDescending(p => p.Datum).FirstOrDefault();

                    List<ISB_BIA_Prozesse_Applikationen> proc_AppCurrent =
                        db.ISB_BIA_Prozesse_Applikationen.Where(x => x.Applikation_Id == id).
                            GroupBy(y => y.Prozess_Id).Select(z => z.OrderByDescending(q => q.Datum).FirstOrDefault()).ToList();
                    //Aktive Relationen filtern
                    List<int> listIdCurrentProcesses =
                        proc_AppCurrent.Where(x => x.Relation == 1).Select(y => y.Prozess_Id).ToList();
                    if (listIdCurrentProcesses.Count == 0)
                        linqProcs = new ObservableCollection<ISB_BIA_Prozesse>();
                    else
                        //
                        linqProcs = new ObservableCollection<ISB_BIA_Prozesse>(db.ISB_BIA_Prozesse.Where(x => listIdCurrentProcesses.Contains(x.Prozess_Id)).GroupBy(y => y.Prozess_Id).Select(z => z.OrderByDescending(q => q.Datum).FirstOrDefault()).OrderBy(k => k.Prozess_Id).ToList());
                }
                //Falls existiert
                if (linqApp != null)
                {
                    Application_Model result = new Application_Model
                    {
                        Applikation_Id = linqApp.Applikation_Id,
                        Rechenzentrum = linqApp.Rechenzentrum,
                        Server = linqApp.Server,
                        Virtuelle_Maschine = linqApp.Virtuelle_Maschine,
                        Typ = linqApp.Typ,
                        IT_Betriebsart = linqApp.IT_Betriebsart,
                        IT_Anwendung_System = linqApp.IT_Anwendung_System,
                        Wichtiges_Anwendungssystem = (linqApp.Wichtiges_Anwendungssystem != ""),
                        SZ_1 = (SZ_Values)linqApp.SZ_1,
                        SZ_2 = (SZ_Values)linqApp.SZ_2,
                        SZ_3 = (SZ_Values)linqApp.SZ_3,
                        SZ_4 = (SZ_Values)linqApp.SZ_4,
                        SZ_5 = (SZ_Values)linqApp.SZ_5,
                        SZ_6 = (SZ_Values)linqApp.SZ_6,
                        Aktiv = linqApp.Aktiv,
                        ProcessList = linqProcs
                    };
                    return result;
                }
                else
                {
                    _myDia.ShowError("Anwendungsdaten konnten nicht abgerufen werden");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Anwendungsdaten konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public ISB_BIA_Applikationen Map_ModelToDB(Application_Model a)
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
                Benutzer = a.Benutzer
            };
        }
        public ObservableCollection<string> Get_List_Rechenzentrum()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    return new ObservableCollection<string>(db.ISB_BIA_Applikationen.Select(p => p.Rechenzentrum).Distinct());
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Rechenzentren konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public ObservableCollection<string> Get_List_Server()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    return new ObservableCollection<string>(db.ISB_BIA_Applikationen.Select(p => p.Server).Distinct());
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Server konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public ObservableCollection<string> Get_List_Virtuelle_Maschine()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    return new ObservableCollection<string>(db.ISB_BIA_Applikationen.Select(p => p.Virtuelle_Maschine).Distinct());
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Virtuelle Maschinen konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public ObservableCollection<string> Get_List_Types()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    return new ObservableCollection<string>(db.ISB_BIA_Applikationen.Select(p => p.Typ).Distinct());
                }
            }
            catch
            {
                return null;
            }
        }
        public ObservableCollection<string> Get_List_Betriebsart()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    return new ObservableCollection<string>(db.ISB_BIA_Applikationen.Select(p => p.IT_Betriebsart).Distinct());
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Kategorien konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public ObservableCollection<ISB_BIA_Applikationen> Get_Applications_All(DateTime? date = null)
        {
            try
            {
                if (!date.HasValue) date = DateTime.Now;
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    return new ObservableCollection<ISB_BIA_Applikationen>(
                        db.ISB_BIA_Applikationen.Where(d => d.Datum <= date).GroupBy(a => a.Applikation_Id)
                        .Select(g => g.OrderByDescending(p => p.Datum).FirstOrDefault()).
                        OrderBy(x => x.Applikation_Id).ToList());
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Anwendungen konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public ObservableCollection<ISB_BIA_Applikationen> Get_History_Application(int applikation_Id)
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    return new ObservableCollection<ISB_BIA_Applikationen>(
                        db.ISB_BIA_Applikationen.Where(a => a.Applikation_Id == applikation_Id).
                        OrderByDescending(p => p.Datum).ToList());
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Anwendungen konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public bool Insert_Application(Application_Model a, ProcAppMode mode)
        {
            //Pflichtfelder (Anwendungsname & Kategorie)
            if (String.IsNullOrWhiteSpace(a.IT_Anwendung_System) || String.IsNullOrWhiteSpace(a.IT_Betriebsart))
            {
                _myDia.ShowInfo("Bitte Alle Pflichtfelder korrekt ausfüllen.\nPflichtfelder sind fett markiert.");
                return false;
            }
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    int duplicateCount = Get_Applications_All().Where(x => x.IT_Anwendung_System == a.IT_Anwendung_System && x.IT_Betriebsart == a.IT_Betriebsart).Count();
                    if (duplicateCount > 0)
                    {
                        _myDia.ShowMessage("Es existiert(e) bereits eine Anwendung/System mit diesem Namen und Kategorie.\nWählen Sie bitte andere Bezeichnungen für diese Anwendung/System.");
                        return false;
                    }                    //Wenn Anwendung momentan inaktiv ist => Frage bzgl. Aktivierung
                    if (a.Aktiv == 0)
                    {
                        a.Aktiv = (_myDia.ShowQuestion("Die Anwendung ist momentan auf inaktiv gesetzt. Möchten Sie die Anwendung auf aktiv setzen?", "Anwendung aktivieren")) ? 1 : 0;
                    }
                    //neue Anwendungs ID erzeugen im Falle eines neuen Prozesses
                    if (mode == ProcAppMode.New)
                    {
                        a.Applikation_Id = db.ISB_BIA_Applikationen.Select(x => x.Applikation_Id).Max() + 1;
                    }               
                    a.Benutzer = Environment.UserName;
                    a.Datum = DateTime.Now;
                    //Mappen und Einfügen
                    db.ISB_BIA_Applikationen.InsertOnSubmit(Map_ModelToDB(a));
                    //Logeintrag erstellen
                    ISB_BIA_Log logEntry = new ISB_BIA_Log
                    {
                        Aktion = "Ändern/Erstellen einer Anwendung",
                        Tabelle = _myShared.Tbl_Prozesse,
                        Details = "Id = " + a.Applikation_Id + ", Name = '" + a.IT_Anwendung_System + "'",
                        Id_1 = a.Applikation_Id,
                        Id_2 = 0,
                        Datum = DateTime.Now,
                        Benutzer = a.Benutzer
                    };
                    db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                    db.SubmitChanges();
                }
                _myDia.ShowMessage("Anwendung eingefügt");
                return true;
            }
            catch (Exception ex1)
            {
                //Logeintrag bei Fehler erstellen
                try
                {
                    using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                    {
                        ISB_BIA_Log logEntry = new ISB_BIA_Log
                        {
                            Aktion = "Fehler: Ändern/Erstellen einer Anwendung",
                            Tabelle = _myShared.Tbl_Prozesse,
                            Details = ex1.Message,
                            Id_1 = a.Applikation_Id,
                            Id_2 = 0,
                            Datum = DateTime.Now,
                            Benutzer = Environment.UserName
                        };

                        db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                        db.SubmitChanges();
                        _myDia.ShowError("Fehler beim Speichern der Anwendung!\nEin Log Eintrag wurde erzeugt.", ex1);
                        return false;
                    }
                }
                catch (Exception ex2)
                {
                    _myDia.ShowError("Fehler beim Speichern der Anwendung!\nEin Log Eintrag konnte ebenfalls nicht erzeugt werden.", ex2);
                    return false;
                }
            }
        }
        #region Applikation löschen (Setzen des Aktiv Flags in der Datenbank auf 0)
        public ISB_BIA_Applikationen Delete_Application(ISB_BIA_Applikationen a)
        {
            ISB_BIA_Applikationen toDelete = new ISB_BIA_Applikationen
            {
                Applikation_Id = a.Applikation_Id,
                IT_Anwendung_System = a.IT_Anwendung_System,
                IT_Betriebsart = a.IT_Betriebsart,
                Wichtiges_Anwendungssystem = a.Wichtiges_Anwendungssystem,
                Rechenzentrum = a.Rechenzentrum,
                Server = a.Server,
                Virtuelle_Maschine = a.Virtuelle_Maschine,
                Typ = a.Typ,
                SZ_1 = a.SZ_1,
                SZ_2 = a.SZ_2,
                SZ_3 = a.SZ_3,
                SZ_4 = a.SZ_4,
                SZ_5 = a.SZ_5,
                SZ_6 = a.SZ_6,
                Benutzer = Environment.UserName,
                Aktiv = 0
            };
            bool res = _myDia.ShowQuestion("Möchten Sie die Anwendung wirklich löschen (auf inaktiv setzen)?", "Anwendung löschen");
            if (!res) return null;
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    toDelete.Datum = DateTime.Now;
                    db.ISB_BIA_Applikationen.InsertOnSubmit(toDelete);
                    ISB_BIA_Log logEntry = new ISB_BIA_Log
                    {
                        Aktion = "Löschen einer Anwendung (Setzen auf inaktiv)",
                        Tabelle = _myShared.Tbl_Applikationen,
                        Details = "Id = " + toDelete.Applikation_Id + ", Name = '" + toDelete.IT_Anwendung_System + "'",
                        Id_1 = toDelete.Applikation_Id,
                        Id_2 = 0,
                        Datum = toDelete.Datum,
                        Benutzer = Environment.UserName
                    };
                    db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                    db.SubmitChanges();
                }
                _myDia.ShowMessage("Anwendung gelöscht");
                return toDelete;
            }
            catch (Exception ex1)
            {
                try
                {
                    using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                    {
                        ISB_BIA_Log logEntry = new ISB_BIA_Log
                        {
                            Aktion = "Fehler: Löschen einer Anwendung",
                            Tabelle = _myShared.Tbl_Applikationen,
                            Details = ex1.Message,
                            Id_1 = toDelete.Applikation_Id,
                            Id_2 = 0,
                            Datum = toDelete.Datum,
                            Benutzer = Environment.UserName
                        };
                        db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                        db.SubmitChanges();
                        _myDia.ShowError("Applikation konnte nicht gelöscht werden.(1)\n", ex1);
                        return null;
                    }
                }
                catch (Exception ex2)
                {
                    _myDia.ShowError("Applikation konnte nicht gelöscht werden!\nEin Log Eintrag konnte ebenfalls nicht erzeugt werden.(2)\n", ex2);
                    return null;
                }
            }
        }
        #endregion
        public bool Insert_AllApplications(ObservableCollection<ISB_BIA_Applikationen> aList)
        {
            bool res = _myDia.ShowQuestion("Möchten Sie die ausgewählten Anwendungen wirklich ohne Änderungen aktualisieren?", "Auswahl bestätigen");
            if (!res) return false;

            //Liste der gesperrten Anwendungen
            List<ISB_BIA_Applikationen> lockedList = new List<ISB_BIA_Applikationen>();
            //String für die Nachricht welche Anwendung durch welchen User gesperrt ist
            string lockedListStringMsg = "";
            foreach (ISB_BIA_Applikationen a in aList)
            {
                string user = _myLock.Get_ObjectIsLocked(Table_Lock_Flags.Application, a.Applikation_Id);
                if (user != "")
                {
                    //Wenn Anwendung gesperrt zur Liste hinzufügen
                    lockedList.Add(a);
                    lockedListStringMsg = lockedListStringMsg + a.IT_Anwendung_System + " (geöffnet von: " + user + ")\n";
                }
            }
            //Falls keine Anwendung gesperrt ist
            if (lockedList.Count > 0)
            {
                string msg = "In der Auswahl befinden sich Anwendungen, die momentan durch andere User geöffnet sind und deshalb nicht gespeichert werden können.\nBitte warten Sie, bis die Bearbeitung beendet ist oder deselektieren Sie betroffene Anwendungen.\n\n";
                _myDia.ShowWarning(msg + lockedListStringMsg);
                return false;
            }
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    foreach (ISB_BIA_Applikationen application_old in aList)
                    {
                        //zu speichernde Anwendung in neues Objekt "kopieren"
                        ISB_BIA_Applikationen application_refresh = new ISB_BIA_Applikationen
                        {
                            Applikation_Id = application_old.Applikation_Id,
                            Aktiv = application_old.Aktiv,
                            Rechenzentrum = application_old.Rechenzentrum,
                            Server = application_old.Server,
                            Virtuelle_Maschine = application_old.Virtuelle_Maschine,
                            Typ = application_old.Typ,
                            IT_Betriebsart = application_old.IT_Betriebsart,
                            IT_Anwendung_System = application_old.IT_Anwendung_System,
                            Wichtiges_Anwendungssystem = application_old.Wichtiges_Anwendungssystem,
                            SZ_1 = application_old.SZ_1,
                            SZ_2 = application_old.SZ_2,
                            SZ_3 = application_old.SZ_3,
                            SZ_4 = application_old.SZ_4,
                            SZ_5 = application_old.SZ_5,
                            SZ_6 = application_old.SZ_6,
                        };
                        application_refresh.Benutzer = Environment.UserName;
                        application_refresh.Datum = DateTime.Now;
                        db.ISB_BIA_Applikationen.InsertOnSubmit(application_refresh);
                        //Logeintrag erzeugen
                        ISB_BIA_Log logEntry = new ISB_BIA_Log
                        {
                            Datum = application_refresh.Datum,
                            Aktion = "Aktualisieren einer Anwendung ohne Änderungen",
                            Tabelle = _myShared.Tbl_Applikationen,
                            Details = "Id = " + application_refresh.Applikation_Id + ", Name = '" + application_refresh.IT_Anwendung_System + "', Sub-Name = '" + application_refresh.IT_Betriebsart + "'",
                            Id_1 = application_refresh.Applikation_Id,
                            Id_2 = 0,
                            Benutzer = Environment.UserName
                        };
                        db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                    }
                    db.SubmitChanges();
                    _myDia.ShowInfo("Anwendungen erfolgreich gespeichert.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                //Bei Fehler Logeintrag
                try
                {
                    using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                    {
                        ISB_BIA_Log logEntry = new ISB_BIA_Log
                        {
                            Datum = DateTime.Now,
                            Aktion = "Fehler: Aktualisieren von Anwendungen ohne Änderungen",
                            Tabelle = _myShared.Tbl_Applikationen,
                            Details = ex.Message,
                            Id_1 = 0,
                            Id_2 = 0,
                            Benutzer = Environment.UserName
                        };
                        db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                        db.SubmitChanges();
                    }
                    _myDia.ShowError("Die ausgewählten Anwendungen konnten nicht gespeichert werden.", ex);
                    return false;
                }
                catch (Exception ex1)
                {
                    _myDia.ShowError("Die ausgewählten Anwendungen konnten nicht gespeichert werden.", ex1);
                    return false;
                }
            }
        }
        public ObservableCollection<ISB_BIA_Applikationen> Get_Applications_Active()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
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
        public ObservableCollection<ISB_BIA_Delta_Analyse> Get_History_ProcAppRelations(int id)
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    ObservableCollection<ISB_BIA_Delta_Analyse> result = new ObservableCollection<ISB_BIA_Delta_Analyse>();
                    //Alle Einträge für Prozess
                    List<ISB_BIA_Prozesse_Applikationen> procAppList = db.ISB_BIA_Prozesse_Applikationen.Where(x => x.Applikation_Id == id).OrderByDescending(c => c.Datum).ToList();
                    //Alle aktuellen Einträge für Prozess (später gekennzeichnet)
                    List<ISB_BIA_Prozesse_Applikationen> procAppListCurrent = procAppList.GroupBy(x => x.Prozess_Id)
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
