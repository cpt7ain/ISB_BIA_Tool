using ISB_BIA_IMPORT1.Model;
using ISB_BIA_IMPORT1.ViewModel;
using ISB_BIA_IMPORT1.LINQ2SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using ISB_BIA_IMPORT1.Services.Interfaces;

namespace ISB_BIA_IMPORT1.Services
{
    public class MyDesignTimeDataService_Setting : IMyDataService_Setting
    {
        private List<ISB_BIA_Settings> SettingsDummyList;

        public MyDesignTimeDataService_Setting()
        {
            SettingsDummyList = new List<ISB_BIA_Settings>() {GetDummySettings(), GetDummySettings()};
        }

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
                Multi_Speichern = "Nein",
                Datum = DateTime.Now,
                Benutzer = Environment.UserName
            };
        }
        public Settings_Model Get_ModelFromDB()
        {
            ISB_BIA_Settings linqSettings = SettingsDummyList.First();
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
        public ISB_BIA_Settings Map_ModelToDB(Settings_Model s)
        {
            return null;
        }
        public ISB_BIA_Settings Get_Settings()
        {
            return SettingsDummyList.First();
        }
        public bool Insert_Settings(ISB_BIA_Settings newSettings, ISB_BIA_Settings oldSettings)
        {
            return true;
        }
        public List<ISB_BIA_Settings> Get_History_Settings()
        {
            return SettingsDummyList;
        }
        #endregion
    }
}
