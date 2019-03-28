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
    public class DataService_OE : IDataService_OE
    {
        readonly IDialogService _myDia;
        readonly ISharedResourceService _myShared;
        readonly ILockService _myLock;

        public DataService_OE(IDialogService myDia, ISharedResourceService myShared, ILockService myLock)
        {
            this._myDia = myDia;
            this._myShared = myShared;
            this._myLock = myLock;
        }

        #region OE
        public ObservableCollection<ISB_BIA_OEs> Get_List_OENames()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    List<ISB_BIA_OEs> queryName = db.ISB_BIA_OEs.Where(c => c.OE_Name != "").
                        GroupBy(x => x.OE_Name).Select(g => g.OrderBy(p => p.Datum).FirstOrDefault()).ToList();
                    return new ObservableCollection<ISB_BIA_OEs>(queryName);
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("OE-Gruppierungen konnten nicht abgerufen werden.", ex);
                return null;
            }
        }
        public ObservableCollection<ISB_BIA_OEs> Get_List_OENumbers()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    List<ISB_BIA_OEs> queryKennung = db.ISB_BIA_OEs.Where(y => y.OE_Nummer != null && y.OE_Nummer != "").GroupBy(x => x.OE_Nummer).Select(g => g.OrderBy(p => p.Datum).FirstOrDefault()).ToList();
                    return new ObservableCollection<ISB_BIA_OEs>(queryKennung);
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("OE-Nummern konnten nicht abgerufen werden.", ex);
                return null;
            }
        }
        public ObservableCollection<ISB_BIA_OEs> Get_List_OERelation()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    List<ISB_BIA_OEs> queryLink = db.ISB_BIA_OEs.Where(x => x.OE_Nummer != "").OrderBy(c => c.OE_Name).ToList();
                    return new ObservableCollection<ISB_BIA_OEs>(queryLink);
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("OE-Relationen konnten nicht abgerufen werden.", ex);
                return null;
            }
        }
        public ISB_BIA_OEs Insert_OEName_New(string name)
        {
            try
            {
                //neu erstellen: OE Gruppe
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    //Indikator ob Name bereits existiert
                    int already_exists = db.ISB_BIA_OEs.Where(x => x.OE_Name == name).ToList().Count;
                    if (String.IsNullOrWhiteSpace(name) || already_exists > 0)
                    {
                        _myDia.ShowWarning("Bitte geben Sie einen gültigen Namen für eine OE ein, welche nicht bereits existiert.");
                        return null;
                    }
                    //Bei erstellung einer neuen Gruppe wird OE_Nummer leer gelassen
                    ISB_BIA_OEs new_Link = new ISB_BIA_OEs
                    {
                        OE_Name = name,
                        OE_Nummer = "",
                        Benutzer = _myShared.User.Username,
                        Datum = DateTime.Now
                    };
                    db.ISB_BIA_OEs.InsertOnSubmit(new_Link);
                    //Erstellen eines LogEntries und schreiben in Datenbank
                    ISB_BIA_Log logEntry = new ISB_BIA_Log
                    {
                        Aktion = "ADMIN/CISO: Erstellen einer OE",
                        Tabelle = _myShared.Tbl_OEs,
                        Details = "Name = '" + name + "'",
                        Id_1 = 0,
                        Id_2 = 0,
                        Datum = DateTime.Now,
                        Benutzer = _myShared.User.Username
                    };
                    db.ISB_BIA_Log.InsertOnSubmit(logEntry);

                    db.SubmitChanges();
                    _myDia.ShowInfo("OE wurde erstellt.");
                    return new_Link;

                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("OE-Gruppierung konnten nicht abgerufen werden.", ex);
                return null;
            }
        }
        public bool Insert_OEName_Edit(string name, string oldName)
        {
            //Ändern: OE_Name
            if (String.IsNullOrWhiteSpace(name) || name == oldName)
            {
                _myDia.ShowWarning("Bitte gültige Bezeichnung eingeben.");
                return false;
            }
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    //Indikator ob Name bereits existiert
                    int already_exists = db.ISB_BIA_OEs.Where(x => x.OE_Name == name).ToList().Count;
                    if (String.IsNullOrWhiteSpace(name) || already_exists > 0)
                    {
                        _myDia.ShowWarning("Bezeichnung existiert bereits");
                        return false;
                    }
                    //Liste aller Prozesse, der die OE-Gruppierung zugeordnet ist
                    List<ISB_BIA_Prozesse> oldProcessList = db.ISB_BIA_Prozesse.Where(c => c.OE_Filter == oldName).GroupBy(x => x.Prozess_Id).Select(y => y.OrderByDescending(z => z.Datum).FirstOrDefault()).ToList();
                    //Prüfen ob ein Prozess gesperrt ist (es darf kein Prozess geöffnet sein, da sonst veraltete Daten verwendet werden könnten)
                    var lockedProcesses = db.ISB_BIA_Lock.Where(x => x.Tabellen_Kennzeichen == (int)Table_Lock_Flags.Process).ToList();
                    string pl = "Prozesse sind durch folgende User geöffnet";
                    if (lockedProcesses.Count != 0)
                    {
                        foreach (ISB_BIA_Lock p in lockedProcesses)
                        {
                            string user = _myLock.Get_ObjectIsLocked(Table_Lock_Flags.Process, p.Objekt_Id);
                            if (user != "")
                            {
                                pl = pl + "\n" + user + "\n";
                            }
                        }
                        string msg = "OE-Namen können nur geändert werden, wenn momentan kein Prozess zur Bearbeitung geöffnet ist.\nBitte warten Sie, bis die Bearbeitung durch den entsprechenden User beendet ist.\n\n";
                        _myDia.ShowWarning(msg + pl);
                        return false;
                    }

                    //Alle OE-Einträge ändern (hier keine Historisierung)
                    db.ISB_BIA_OEs.Where(n => n.OE_Name == oldName).ToList().ForEach(x => { x.OE_Name = name; });

                    //Die OE's all dieser Prozesse ändern
                    foreach (ISB_BIA_Prozesse process_old in oldProcessList)
                    {
                        ISB_BIA_Prozesse process_new = new ISB_BIA_Prozesse()
                        {
                            OE_Filter = name,
                            Datum = DateTime.Now,
                            Benutzer = _myShared.User.Username,

                            Prozess_Id = process_old.Prozess_Id,
                            Aktiv = process_old.Aktiv,
                            Prozess = process_old.Prozess,
                            Sub_Prozess = process_old.Sub_Prozess,
                            Prozessverantwortlicher = process_old.Prozessverantwortlicher,
                            Kritischer_Prozess = process_old.Kritischer_Prozess,
                            Kritikalität_des_Prozesses = process_old.Kritikalität_des_Prozesses,
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
                            RPO_Datenverlustzeit_Recovery_Point_Objective =
                                process_old.RPO_Datenverlustzeit_Recovery_Point_Objective,
                            RTO_Wiederanlaufzeit_Recovery_Time_Objective =
                                process_old.RTO_Wiederanlaufzeit_Recovery_Time_Objective,
                            RTO_Wiederanlaufzeit_Recovery_Time_Objective_Notfall =
                                process_old.RTO_Wiederanlaufzeit_Recovery_Time_Objective_Notfall,
                            Relevantes_IS_1 = process_old.Relevantes_IS_1,
                            Relevantes_IS_2 = process_old.Relevantes_IS_2,
                            Relevantes_IS_3 = process_old.Relevantes_IS_3,
                            Relevantes_IS_4 = process_old.Relevantes_IS_4,
                            Relevantes_IS_5 = process_old.Relevantes_IS_5
                        };
                        db.ISB_BIA_Prozesse.InsertOnSubmit(process_new);
                    }

                    //Erstellen eines LogEntries und schreiben in Datenbank
                    ISB_BIA_Log logEntry = new ISB_BIA_Log
                    {
                        Aktion = "ADMIN/CISO: Ändern einer OE",
                        Tabelle = _myShared.Tbl_Prozesse + ", " + _myShared.Tbl_OEs,
                        Details = "Von '" + oldName + "' zu '" + name + "'",
                        Id_1 = 0,
                        Id_2 = 0,
                        Datum = DateTime.Now,
                        Benutzer = _myShared.User.Username
                    };
                    db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                    db.SubmitChanges();
                    _myDia.ShowInfo("OE-Bezeichnung geändert!");
                    return true;
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Fehler beim Ändern der OE-Namen.\nDies betrifft auch alle Prozesse, denen diese OE zugeordnet war.", ex);
                return false;
            }
        }
        public bool Delete_OEName(string oeName)
        {
            bool res = _myDia.ShowQuestion("Möchten Sie diesen OE-Filter wirklich löschen?\nAlle Zuordnungen werden ebenfalls gelöscht.", "OE-Filter löschen");
            if (res) return false;
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    //OE Gruppe kann nur gelöscht werden, wenn kein Prozess mehr dieser Gruppe zugeordnet ist
                    List<ISB_BIA_Prozesse> list1 = db.ISB_BIA_Prozesse.Where(x => x.OE_Filter == oeName).ToList();
                    if (list1.Count > 0)
                    {
                        _myDia.ShowWarning("Diesem OE-Namen sind noch Prozesse zugeordnet.\nOrdnen Sie diese Prozesse zunächst einer anderen OE zu.");
                        return false;
                    }

                    //Prüfen ob ein Prozess gesperrt ist (es darf kein Prozess geöffnet sein, da sonst veraltete Daten verwendet werden könnten)
                    var lockedProcesses = db.ISB_BIA_Lock.Where(x => x.Tabellen_Kennzeichen == (int)Table_Lock_Flags.Process).ToList();
                    string pl = "Prozesse sind durch folgende User geöffnet";
                    if (lockedProcesses.Count != 0)
                    {
                        foreach (ISB_BIA_Lock p in lockedProcesses)
                        {
                            string user = _myLock.Get_ObjectIsLocked(Table_Lock_Flags.Process, p.Objekt_Id);
                            if (user != "")
                            {
                                pl = pl + "\n" + user + "\n";
                            }
                        }
                        string msg = "OE's können nur gelöscht werden, wenn momentan kein Prozess zur Bearbeitung geöffnet ist.\nBitte warten Sie, bis die Bearbeitung durch den entsprechenden User beendet ist.\n\n";
                        _myDia.ShowWarning(msg + pl);
                        return false;
                    }

                    try
                    {
                        //Löschen aller Zuordnungen mit dieser Gruppe
                        db.ISB_BIA_OEs.DeleteAllOnSubmit(db.ISB_BIA_OEs.Where(x => x.OE_Name == oeName));
                        ISB_BIA_Log logEntry = new ISB_BIA_Log
                        {
                            Aktion = "Löschen einer OE",
                            Details = "Name = '" + oeName + "'",
                            Tabelle = _myShared.Tbl_OEs,
                            Id_1 = 0,
                            Id_2 = 0,
                            Datum = DateTime.Now,
                            Benutzer = _myShared.User.Username
                        };
                        db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                        db.SubmitChanges();
                        _myDia.ShowInfo("OE wurde gelöscht.");
                        return true;
                    }
                    catch
                    {
                        _myDia.ShowError("OE konnte nicht gelöscht werden.");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Fehler beim Löschen.", ex);
                return false;
            }
        }
        public bool Delete_OERelation(string oeName, string oeNumber)
        {
            bool res = _myDia.ShowQuestion("Möchten Sie diesen diese OE-Zuordnung wirklich löschen?", "OE-Zuordnung löschen");
            if (res) return false;
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    //Löschen der angegebenen Zuordnung
                    db.ISB_BIA_OEs.DeleteAllOnSubmit(db.ISB_BIA_OEs.Where(x => x.OE_Nummer == oeNumber && x.OE_Name == oeName));
                    ISB_BIA_Log logEntry = new ISB_BIA_Log
                    {
                        Aktion = "Löschen einer OE-Zuordnung",
                        Tabelle = _myShared.Tbl_OEs,
                        Details = "Zuordnung = '" + oeName + "' <-> " + oeNumber,
                        Id_1 = 0,
                        Id_2 = 0,
                        Datum = DateTime.Now,
                        Benutzer = _myShared.User.Username
                    };
                    db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                    db.SubmitChanges();
                    _myDia.ShowInfo("OE-Zuordnung wurde gelöscht.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("OE-Zuordnung konnte nicht gelöscht werden.", ex);
                return false;

            }
        }
        public bool Delete_OENumber(string oeNumber)
        {
            bool res = _myDia.ShowQuestion("Möchten Sie diesen diese OE-Kennung wirklich löschen?\nAlle Zuordnungen werden ebenfalls gelöscht.", "OE-Kennung löschen");
            if (!res) return false;
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    //Löschen aller Zuordnungen mit dieser OE-Nummer
                    db.ISB_BIA_OEs.DeleteAllOnSubmit(db.ISB_BIA_OEs.Where(x => x.OE_Nummer == oeNumber));
                    ISB_BIA_Log logEntry = new ISB_BIA_Log
                    {
                        Aktion = "Löschen einer OE-Nummer",
                        Tabelle = _myShared.Tbl_OEs,
                        Details = "Nummer = " + oeNumber,
                        Id_1 = 0,
                        Id_2 = 0,
                        Datum = DateTime.Now,
                        Benutzer = _myShared.User.Username
                    };
                    db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                    db.SubmitChanges();
                    _myDia.ShowInfo("OE-Nummer wurde gelöscht.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("OE-Nummer konnte nicht gelöscht werden.", ex);
                return false;
            }
        }
        public ISB_BIA_OEs Insert_OERelation(ISB_BIA_OEs name, ISB_BIA_OEs number)
        {
            if (name == null || number == null)
            {
                _myDia.ShowWarning("Bitte geben Sie einen gültigen Namen ein.");
                return null;
            }
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    //Prüfen ob Zuordnung bereits vorhanden
                    if (db.ISB_BIA_OEs.Where(x => x.OE_Name == name.OE_Name && x.OE_Nummer == number.OE_Nummer).ToList().Count > 0)
                    {
                        _myDia.ShowWarning("Diese Zuordnung existiert bereits.");
                        return null;
                    }
                    //Neue Zuordnung erstellen
                    ISB_BIA_OEs new_Link = new ISB_BIA_OEs
                    {
                        OE_Name = name.OE_Name,
                        OE_Nummer = number.OE_Nummer,
                        Benutzer = _myShared.User.Username,
                        Datum = DateTime.Now
                    };
                    db.ISB_BIA_OEs.InsertOnSubmit(new_Link);
                    //Erstellen eines LogEntries und schreiben in Datenbank
                    ISB_BIA_Log logEntry = new ISB_BIA_Log
                    {
                        Aktion = "ADMIN/CISO: Erstellen einer OE-Zuordnung",
                        Tabelle = _myShared.Tbl_OEs,
                        Details = "Zuordnung = '" + name.OE_Name + "' <-> " + number.OE_Nummer,
                        Id_1 = 0,
                        Id_2 = 0,
                        Datum = DateTime.Now,
                        Benutzer = _myShared.User.Username
                    };
                    db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                    db.SubmitChanges();
                    _myDia.ShowInfo("OE-Zuordnung wurde erstellt.");
                    return new_Link;
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("OE-Zuordnung konnte nicht erstellt werden.", ex);
                return null;
            }
        }
        public ISB_BIA_OEs Insert_OENumber_New(string number, ISB_BIA_OEs name)
        {
            if (name == null || String.IsNullOrWhiteSpace(number))
            {
                _myDia.ShowWarning("Bitte geben Sie einen gültigen Namen ein und wählen Sie eine OE aus.");
                return null;
            }
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    //Prüfen ob Zuordnung bereits existiert
                    if (db.ISB_BIA_OEs.Where(x => x.OE_Name == name.OE_Name && x.OE_Nummer == number).ToList().Count > 0)
                    {
                        _myDia.ShowWarning("Diese Zuordnung existiert bereits.");
                        return null;
                    }
                    //Neue Zuordnung erstellen
                    ISB_BIA_OEs new_Number = new ISB_BIA_OEs
                    {
                        OE_Name = name.OE_Name,
                        OE_Nummer = number,
                        Benutzer = _myShared.User.Username,
                        Datum = DateTime.Now
                    };
                    db.ISB_BIA_OEs.InsertOnSubmit(new_Number);
                    //Erstellen eines LogEntries und schreiben in Datenbank
                    ISB_BIA_Log logEntry = new ISB_BIA_Log
                    {
                        Aktion = "ADMIN/CISO: Erstellen einer OE-Nummer",
                        Tabelle = _myShared.Tbl_OEs,
                        Details = "Nummer = " + new_Number.OE_Nummer,
                        Id_1 = 0,
                        Id_2 = 0,
                        Datum = DateTime.Now,
                        Benutzer = _myShared.User.Username
                    };
                    db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                    db.SubmitChanges();
                    _myDia.ShowInfo("OE-Nummer wurde erstellt.");
                    return new_Number;
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("OE-Nummer konnte nicht erstellt werden.", ex);
                return null;
            }
        }
        public bool Insert_OENumber_Edit(string number, string oldNumber)
        {
            if (String.IsNullOrWhiteSpace(number) || number == oldNumber)
            {
                _myDia.ShowWarning("Bitte geben Sie einen gültigen Namen ein.");
                return false;
            }
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    //Prüfen ob Nummer bereits vorhanden
                    if (db.ISB_BIA_OEs.Where(x => x.OE_Nummer == number).ToList().Count > 0)
                    {
                        _myDia.ShowWarning("OE-Nummer existiert bereits.\nÄndern Sie ggf. zunächst die vorhandene Nummer.");
                        return false;
                    }
                    //Ändern der Nummern aller Zuordnungen, deren Nummer momentan der angegebenen entspricht
                    List<ISB_BIA_OEs> numbers = db.ISB_BIA_OEs.Where(x => x.OE_Nummer == oldNumber).ToList();
                    foreach (ISB_BIA_OEs x in numbers)
                    {
                        ISB_BIA_OEs newNumber = new ISB_BIA_OEs()
                        {
                            OE_Name = x.OE_Name,
                            OE_Nummer = number,
                            Benutzer = _myShared.User.Username,
                            Datum = DateTime.Now,
                        };
                        //Zuordnung mit neuer nummer einfügen
                        db.ISB_BIA_OEs.InsertOnSubmit(newNumber);
                    }
                    //Alte Zuordnungen löschen
                    db.ISB_BIA_OEs.DeleteAllOnSubmit(numbers);

                    //Erstellen eines LogEntries und schreiben in Datenbank
                    ISB_BIA_Log logEntry = new ISB_BIA_Log
                    {
                        Aktion = "ADMIN/CISO: Ändern einer OE-Nummer",
                        Tabelle = _myShared.Tbl_OEs,
                        Details = "Von '" + number + "' zu '" + oldNumber + "'",
                        Id_1 = 0,
                        Id_2 = 0,
                        Datum = DateTime.Now,
                        Benutzer = _myShared.User.Username
                    };
                    db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                    db.SubmitChanges();
                    _myDia.ShowInfo("OE-Nummer wurde geändert.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("OE-Nummer konnte nicht geändert werden.", ex);
                return false;
            }
        }
        #endregion
    }
}
