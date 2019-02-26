using GalaSoft.MvvmLight;
using System;

namespace ISB_BIA_IMPORT1.Model
{
    /// <summary>
    /// Model für die Settings. Enthält Methoden zum Konvertieren von Daten von und zur Datenbankdarstellung
    /// </summary>
    public class Settings_Model: ObservableObject
    {
        #region Backing-Fields
        private string _sZ_1_Name="";
        private string _sZ_2_Name="";
        private string _sZ_3_Name="";
        private string _sZ_4_Name="";
        private string _sZ_5_Name="";
        private string _sZ_6_Name="";
        private bool _neue_Schutzziele_aktiviert;
        private bool _bIA_abgeschlossen;
        private bool _sBA_abgeschlossen;
        private bool _delta_abgeschlossen;
        private bool _attribut9_aktiviert;
        private bool _attribut10_aktiviert;
        private bool _multi_Save;
        private DateTime _datum;
        private string _benutzer="";
        #endregion

        /// <summary>
        /// Name Schutzziel 1
        /// </summary>
        public string SZ_1_Name
        {
            get => _sZ_1_Name;
            set => Set(() => SZ_1_Name, ref _sZ_1_Name, value);
        }
        /// <summary>
        /// Name Schutzziel 2
        /// </summary>
        public string SZ_2_Name
        {
            get => _sZ_2_Name;
            set => Set(() => SZ_2_Name, ref _sZ_2_Name, value);
        }
        /// <summary>
        /// Name Schutzziel 3
        /// </summary>
        public string SZ_3_Name
        {
            get => _sZ_3_Name;
            set => Set(() => SZ_3_Name, ref _sZ_3_Name, value);
        }
        /// <summary>
        /// Name Schutzziel 4
        /// </summary>
        public string SZ_4_Name
        {
            get => _sZ_4_Name;
            set => Set(() => SZ_4_Name, ref _sZ_4_Name, value);
        }
        /// <summary>
        /// Name Schutzziel 5
        /// </summary>
        public string SZ_5_Name
        {
            get => _sZ_5_Name;
            set => Set(() => SZ_5_Name, ref _sZ_5_Name, value);
        }
        /// <summary>
        /// Name Schutzziel 6
        /// </summary>
        public string SZ_6_Name
        {
            get => _sZ_6_Name;
            set => Set(() => SZ_6_Name, ref _sZ_6_Name, value);
        }
        /// <summary>
        /// Schutzziele 5 + 6 ausblenden.
        /// </summary>
        public bool Neue_Schutzziele_aktiviert
        {
            get => _neue_Schutzziele_aktiviert;
            set => Set(() => Neue_Schutzziele_aktiviert, ref _neue_Schutzziele_aktiviert, value);
        }
        /// <summary>
        /// Bisher keine Funktion
        /// </summary>
        public bool BIA_abgeschlossen
        {
            get => _bIA_abgeschlossen;
            set => Set(() => BIA_abgeschlossen, ref _bIA_abgeschlossen, value);
        }
        /// <summary>
        /// Bisher keine Funktion
        /// </summary>
        public bool SBA_abgeschlossen
        {
            get => _sBA_abgeschlossen;
            set => Set(() => SBA_abgeschlossen, ref _sBA_abgeschlossen, value);
        }
        /// <summary>
        /// Bisher keine Funktion
        /// </summary>
        public bool Delta_abgeschlossen
        {
            get => _delta_abgeschlossen;
            set => Set(() => Delta_abgeschlossen, ref _delta_abgeschlossen, value);
        }
        /// <summary>
        /// EInstellung für Aktivierung des Attribut 9 für Informationssegmente
        /// </summary>
        public bool Attribut9_aktiviert
        {
            get => _attribut9_aktiviert;
            set => Set(() => Attribut9_aktiviert, ref _attribut9_aktiviert, value);
        }
        /// <summary>
        /// EInstellung für Aktivierung des Attribut 10 für Informationssegmente
        /// </summary>
        public bool Attribut10_aktiviert
        {
            get => _attribut10_aktiviert;
            set => Set(() => Attribut10_aktiviert, ref _attribut10_aktiviert, value);
        }
        /// <summary>
        /// Einstellung zum aktivieren der Speicherung mehrerer Objekte (Prozesse, Anwendung) in der Listenansicht
        /// </summary>
        public bool Multi_Save
        {
            get => _multi_Save;
            set => Set(() => Multi_Save, ref _multi_Save, value);
        }
        /// <summary>
        /// Bearbeitungsdatum des Datensatzes
        /// </summary>
        public DateTime Datum
        {
            get => _datum;
            set => Set(() => Datum, ref _datum, value);
        }
        /// <summary>
        /// Bearbeitender Nutzer des Datensatzes
        /// </summary>
        public string Benutzer
        {
            get => _benutzer;
            set => Set(() => Benutzer, ref _benutzer, value);
        }
        
    }
}
