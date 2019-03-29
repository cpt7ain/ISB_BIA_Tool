using ISB_BIA_IMPORT1.LINQ2SQL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Data;
using ISB_BIA_IMPORT1.Services.Interfaces;

namespace ISB_BIA_IMPORT1.Services
{
    class DataService_Delta : IDataService_Delta
    {
        readonly IDialogService _myDia;
        readonly ISharedResourceService _myShared;
        readonly IDataService_Process _myDataProcess;
        readonly IDataService_Application _myDataApp;

        public DataService_Delta(IDialogService myDia, ISharedResourceService myShared, IDataService_Process myDataProcess, IDataService_Application myDataApp)
        {
            this._myDia = myDia;
            this._myShared = myShared;
            this._myDataProcess = myDataProcess;
            this._myDataApp = myDataApp;
        }
        #region Delta
        public ObservableCollection<ISB_BIA_Delta_Analyse> Get_List_Delta()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    return new ObservableCollection<ISB_BIA_Delta_Analyse>(
                        db.ISB_BIA_Delta_Analyse.OrderBy(x => x.Prozess_Id).ToList());
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Fehler beim Abrufen der Delta Analyse.\n", ex);
                return null;
            }
        }
        public ObservableCollection<ISB_BIA_Delta_Analyse> Create_DeltaAnalysis(DateTime d)
        {
            try
            {
                List<ISB_BIA_Prozesse_Applikationen> proc_App;
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    //einen Tag aufaddieren da immer zeit 00:00 benutzt wird & sonst Fehler für spätere Einträge des Tages auftreten
                    d = d.AddDays(1);
                    //Nur aktuelle 1er Relationen bis zu dem bestimmten Datum aus Relationentabelle abfragen
                    proc_App = db.ISB_BIA_Prozesse_Applikationen.Where(q => q.Datum <= d && q.Relation == 1)
                        .GroupBy(x => new { x.Prozess_Id, x.Applikation_Id })
                        .Select(z => z.OrderBy(q => q.Prozess_Id).FirstOrDefault()).ToList();
                }
                if (proc_App.Count != 0)
                {
                    //Dialog ob Analyse in Datenbank gespeichert werden soll oder nicht
                    bool res = _myDia.ShowQuestion("Möchten Sie die letzte gespeicherte Deltaanalyse in Datenbank überschreiben?", "Deltaanalyse");
                    if (res)
                        return ComputeAndGet_List_Delta_Date(d, true, proc_App);
                    else
                        return ComputeAndGet_List_Delta_Date(d, false, proc_App);
                }
                else
                {
                    _myDia.ShowInfo("Für den gewählten Zeitpunkt existieren keine Daten für eine Delta-Analyse.");
                    return null;
                }

            }
            catch (Exception ex)
            {
                _myDia.ShowError("Fehler beim Erstellen der Tabelle \n" + _myShared.Tbl_Delta + "\n", ex);
                return null;
            }
        }
        public ObservableCollection<ISB_BIA_Delta_Analyse> ComputeAndGet_List_Delta_Date(DateTime date, bool toDB, List<ISB_BIA_Prozesse_Applikationen> proc_App)
        {
            List<ISB_BIA_Delta_Analyse> DeltaList = new List<ISB_BIA_Delta_Analyse>();
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    //Erstelle Liste der Prozesse und Anwendungen mit dem zu dem gewählten Zeitpunkt aktuellsten Stand
                    ObservableCollection<ISB_BIA_Prozesse> processes = _myDataProcess.Get_List_Processes_All(date);
                    ObservableCollection<ISB_BIA_Applikationen> applications = _myDataApp.Get_List_Applications_All(date);
                    if (toDB)
                    {
                        db.ISB_BIA_Delta_Analyse.DeleteAllOnSubmit(db.ISB_BIA_Delta_Analyse.ToList());
                        db.SubmitChanges();
                    }
                    //Delta-Analyse für jede Prozess-Applikation Relation
                    foreach (ISB_BIA_Prozesse_Applikationen pa in proc_App)
                    {
                        ISB_BIA_Prozesse p = processes.Where(x => x.Prozess_Id == pa.Prozess_Id).FirstOrDefault();
                        ISB_BIA_Applikationen a = applications.Where(x => x.Applikation_Id == pa.Applikation_Id).FirstOrDefault();
                        //"Gelöschte" Prozesse / Anwendungen irrelevant
                        if (a.Aktiv == 0 || p.Aktiv == 0)
                        {
                            continue;
                        }

                        ISB_BIA_Delta_Analyse d = new ISB_BIA_Delta_Analyse
                        {
                            Prozess_Id = p.Prozess_Id,
                            Prozess = p.Prozess,
                            Sub_Prozess = p.Sub_Prozess,
                            Datum_Prozess = p.Datum,
                            Applikation_Id = a.Applikation_Id,
                            Applikation = a.IT_Anwendung_System,
                            Datum_Applikation = a.Datum,
                            SZ_1 = a.SZ_1 - p.SZ_1,
                            SZ_2 = a.SZ_2 - p.SZ_2,
                            SZ_3 = a.SZ_3 - p.SZ_3,
                            SZ_4 = a.SZ_4 - p.SZ_4,
                            SZ_5 = a.SZ_5 - p.SZ_5,
                            SZ_6 = a.SZ_6 - p.SZ_6,
                            Datum = date.Subtract(TimeSpan.FromDays(1))
                        };
                        DeltaList.Add(d);
                        if (toDB) db.ISB_BIA_Delta_Analyse.InsertOnSubmit(d);                        
                    }
                    if (toDB)
                    {
                        //Log Eintrag für erfolgreiches schreiben in Datenbank
                        ISB_BIA_Log logEntry = new ISB_BIA_Log
                        {
                            Aktion = "Erstellen der Delta-Analyse",
                            Tabelle = _myShared.Tbl_Delta,
                            Details = "-",
                            Id_1 = 0,
                            Id_2 = 0,
                            Datum = DateTime.Now,
                            Benutzer =  _myShared.User.Username
                        };
                        db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                        db.SubmitChanges();
                    }
                }
                //Rückgabe der Delta-Liste zum Anzeigen der Ergebnisse
                return new ObservableCollection<ISB_BIA_Delta_Analyse>(DeltaList);
            }
            catch (Exception ex)
            {
                try
                {
                    using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                    {
                        _myDia.ShowError("Fehler: Ertellen der Delta-Analyse.", ex);
                        ISB_BIA_Log logEntry = new ISB_BIA_Log
                        {
                            Aktion = "Fehler: Erstellen der Delta-Analyse",
                            Tabelle = _myShared.Tbl_Delta,
                            Details = ex.Message,
                            Id_1 = 0,
                            Id_2 = 0,
                            Datum = DateTime.Now,
                            Benutzer =  _myShared.User.Username
                        };
                        db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                        db.SubmitChanges();
                        _myDia.ShowError("Fehler beim Speichern der Delta-Analyse!\nEin Log Eintrag wurde erzeugt." + ex.ToString(), ex);
                        return null;
                    }
                }
                catch (Exception ex1)
                {
                    _myDia.ShowError("Fehler beim Speichern der Delta-Analyse!\nEin Log Eintrag konnte ebenfalls nicht erzeugt werden.\n", ex1);
                    return null;
                }
            }
        }
        #endregion
    }
}
