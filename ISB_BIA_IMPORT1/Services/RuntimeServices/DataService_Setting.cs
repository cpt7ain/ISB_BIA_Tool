using ISB_BIA_IMPORT1.Model;
using ISB_BIA_IMPORT1.LINQ2SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using ISB_BIA_IMPORT1.Services.Interfaces;

namespace ISB_BIA_IMPORT1.Services
{
    class DataService_Setting : IDataService_Setting
    {
        readonly IDialogService _myDia;
        readonly ISharedResourceService _myShared;

        public DataService_Setting(IDialogService myDia, ISharedResourceService myShared)
        {
            this._myDia = myDia;
            this._myShared = myShared;
        }

        #region Einstellungen
        public Settings_Model Get_Model_FromDB()
        {
            try
            {
                ISB_BIA_Settings linqSettings;
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    linqSettings = db.ISB_BIA_Settings
                        .OrderByDescending(p => p.Datum).FirstOrDefault();
                }
                if (linqSettings != null)
                {
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
                        Multi_Speichern = (linqSettings.Multi_Speichern == "Ja") ? true : false,
                        Datum = linqSettings.Datum,
                        Benutzer = linqSettings.Benutzer
                    };
                    return result;
                }
                else
                {
                    _myDia.ShowError("Einstellungen konnten nicht abgerufen werden");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Einstellungen konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public ISB_BIA_Settings Map_Model_ToDB(Settings_Model s)
        {
            try
            {
                return new ISB_BIA_Settings()
                {
                    SZ_1_Name = s.SZ_1_Name,
                    SZ_2_Name = s.SZ_2_Name,
                    SZ_3_Name = s.SZ_3_Name,
                    SZ_4_Name = s.SZ_4_Name,
                    SZ_5_Name = s.SZ_5_Name,
                    SZ_6_Name = s.SZ_6_Name,
                    Neue_Schutzziele_aktiviert = (s.Neue_Schutzziele_aktiviert) ? "Ja" : "Nein",
                    BIA_abgeschlossen = (s.BIA_abgeschlossen) ? "Ja" : "Nein",
                    SBA_abgeschlossen = (s.SBA_abgeschlossen) ? "Ja" : "Nein",
                    Delta_abgeschlossen = (s.Delta_abgeschlossen) ? "Ja" : "Nein",
                    Attribut9_aktiviert = (s.Attribut9_aktiviert) ? "Ja" : "Nein",
                    Attribut10_aktiviert = (s.Attribut10_aktiviert) ? "Ja" : "Nein",
                    Multi_Speichern = (s.Multi_Speichern) ? "Ja" : "Nein",
                    Datum = s.Datum,
                    Benutzer = s.Benutzer
                };
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Fehler beim Mappen der Einstellungen", ex);
                return null;
            }
        }
        public ISB_BIA_Settings Get_List_Settings()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    ISB_BIA_Settings res = db.ISB_BIA_Settings.OrderByDescending(d => d.Datum).FirstOrDefault();
                    if (res == null) throw new Exception();
                    else return res;
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Einstellungen konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public List<ISB_BIA_Settings> Get_History_Settings()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    return db.ISB_BIA_Settings.OrderByDescending(d => d.Datum).ToList();
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Einstellungshistorie konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public bool Insert_Settings(Settings_Model newSettings, Settings_Model oldSettings)
        {
            try
            {
                if (newSettings.SZ_1_Name != oldSettings.SZ_1_Name
                    || newSettings.SZ_2_Name != oldSettings.SZ_2_Name
                    || newSettings.SZ_3_Name != oldSettings.SZ_3_Name
                    || newSettings.SZ_4_Name != oldSettings.SZ_4_Name
                    || newSettings.SZ_5_Name != oldSettings.SZ_5_Name
                    || newSettings.SZ_6_Name != oldSettings.SZ_6_Name
                    || newSettings.Neue_Schutzziele_aktiviert != oldSettings.Neue_Schutzziele_aktiviert
                    || newSettings.BIA_abgeschlossen != oldSettings.BIA_abgeschlossen
                    || newSettings.SBA_abgeschlossen != oldSettings.SBA_abgeschlossen
                    || newSettings.Delta_abgeschlossen != oldSettings.Delta_abgeschlossen
                    || newSettings.Attribut9_aktiviert != oldSettings.Attribut9_aktiviert
                    || newSettings.Attribut10_aktiviert != oldSettings.Attribut10_aktiviert
                    || newSettings.Multi_Speichern != oldSettings.Multi_Speichern)
                {
                    using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                    {
                        newSettings.Datum = DateTime.Now;
                        newSettings.Benutzer =  _myShared.User.WholeName;
                        ISB_BIA_Settings resNew = Map_Model_ToDB(newSettings);
                        if (resNew == null) return false;
                        db.ISB_BIA_Settings.InsertOnSubmit(resNew);

                        //Logeintrag erzeugen
                        ISB_BIA_Log logEntry = new ISB_BIA_Log
                        {
                            Aktion = "Ändern der Einstellungen",
                            Tabelle = _myShared.Tbl_Settings,
                            Details = "Für Details exportieren Sie die Einstellungshistorie",
                            Id_1 = 0,
                            Id_2 = 0,
                            Datum = DateTime.Now,
                            Benutzer = _myShared.User.WholeName,
                        };
                        db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                        db.SubmitChanges();
                    }
                    _myDia.ShowMessage("Einstellungen gespeichert.");
                    return true;
                }
                else
                {
                    _myDia.ShowMessage("Keine Änderungen entdeckt.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Fehler: Einstellungen konnten nicht gespeichert werden.", ex);
                return false;
            }
        }
        #endregion
    }
}
