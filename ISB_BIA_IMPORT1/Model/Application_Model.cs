using GalaSoft.MvvmLight;
using ISB_BIA_IMPORT1.LINQ2SQL;
using ISB_BIA_IMPORT1.ViewModel;
using System;
using System.Collections.ObjectModel;

namespace ISB_BIA_IMPORT1.Model
{
    /// <summary>
    /// Model einer Anwendung
    /// </summary>
    public class Application_Model : ObservableObject
    {
        #region Backing-Fields
        private ObservableCollection<ISB_BIA_Prozesse> _processList = new ObservableCollection<ISB_BIA_Prozesse>();
        private int _applikation_Id;
        private string _iT_Anwendung_System = "";
        private string _iT_Betriebsart="";
        private string _typ="";
        private string _rechenzentrum="";
        private string _server="";
        private string _virtuelle_Maschine="";
        private bool _wichtiges_Anwendungssystem= false;
        private SZ_Values _sZ_1 = 0;
        private SZ_Values _sZ_2 = 0;
        private SZ_Values _sZ_3 = 0;
        private SZ_Values _sZ_4 = 0;
        private SZ_Values _sZ_5 = 0;
        private SZ_Values _sZ_6 = 0;
        private int _verknüpfte_Prozesse;
        private string _benutzer="";
        private DateTime _datum;
        private int _aktiv=1;
        private string _erstanlage;
        #endregion

        /// <summary>
        /// Methode, um Daten einer Anwendungsinstanz auf eine neue Instanz zu kopieren
        /// </summary>
        /// <returns> Kopie des Objektes </returns>
        public Application_Model Copy()
        {
            Application_Model copy = (Application_Model)MemberwiseClone();
            copy.ProcessList = new ObservableCollection<ISB_BIA_Prozesse>(ProcessList);
            return copy;
        }

        #region Properties der aktuellen Anwendung für Darstellung im View(XAML) anhand von DataBinding
        /// <summary>
        /// Liste der diesem Prozess zugeordneten Anwendungen
        /// </summary>
        public ObservableCollection<ISB_BIA_Prozesse> ProcessList
        {
            get => _processList;
            set
            {
                _processList = value;
                Set(() => ProcessList, ref _processList, value);
            }
        }
        /// <summary>
        /// Anwendungs Id der Anwendung
        /// </summary>
        public int Applikation_Id
        {
            get => _applikation_Id;
            set => Set(() => Applikation_Id, ref _applikation_Id, value);

        }
        /// <summary>
        /// Name der Anwendung
        /// </summary>
        public string IT_Anwendung_System
        {
            get => _iT_Anwendung_System;
            set => Set(() => IT_Anwendung_System, ref _iT_Anwendung_System, value);

        }
        /// <summary>
        /// Indikator ob Anwendung wichtig oder nicht
        /// </summary>
        public bool Wichtiges_Anwendungssystem
        {
            get => _wichtiges_Anwendungssystem;
            set => Set(() => Wichtiges_Anwendungssystem, ref _wichtiges_Anwendungssystem, value);

        }
        /// <summary>
        /// Rechenzentrum der Anwendung
        /// </summary>
        public string Rechenzentrum
        {
            get => _rechenzentrum;
            set => Set(() => Rechenzentrum, ref _rechenzentrum, value);

        }
        /// <summary>
        /// Server der Anwendung
        /// </summary>
        public string Server
        {
            get => _server;
            set => Set(() => Server, ref _server, value);

        }
        /// <summary>
        /// Virtuelle Maschine der Anwendung
        /// </summary>
        public string Virtuelle_Maschine
        {
            get => _virtuelle_Maschine;
            set => Set(() => Virtuelle_Maschine, ref _virtuelle_Maschine, value);

        }
        /// <summary>
        /// Typ der Anwendung
        /// </summary>
        public string Typ
        {
            get => _typ;
            set => Set(() => Typ, ref _typ, value);

        }
        /// <summary>
        /// Kategorie der Anwendung
        /// </summary>
        public string IT_Betriebsart
        {
            get => _iT_Betriebsart;
            set => Set(() => IT_Betriebsart, ref _iT_Betriebsart, value);

        }
        /// <summary>
        /// Schutzziel-Wert, ( 0(nicht relevant) bis 4(Sehr Hoch))
        /// </summary>
        public SZ_Values SZ_1
        {
            get => _sZ_1;
            set => Set(() => SZ_1, ref _sZ_1, value);

        }
        /// <summary>
        /// Schutzziel-Wert, ( 0(nicht relevant) bis 4(Sehr Hoch))
        /// </summary>
        public SZ_Values SZ_2
        {
            get => _sZ_2;
            set => Set(() => SZ_2, ref _sZ_2, value);

        }
        /// <summary>
        /// Schutzziel-Wert, ( 0(nicht relevant) bis 4(Sehr Hoch))
        /// </summary>
        public SZ_Values SZ_3
        {
            get => _sZ_3;
            set => Set(() => SZ_3, ref _sZ_3, value);

        }
        /// <summary>
        /// Schutzziel-Wert, ( 0(nicht relevant) bis 4(Sehr Hoch))
        /// </summary>
        public SZ_Values SZ_4
        {
            get => _sZ_4;
            set => Set(() => SZ_4, ref _sZ_4, value);

        }
        /// <summary>
        /// Schutzziel-Wert, ( 0(nicht relevant) bis 4(Sehr Hoch))
        /// </summary>
        public SZ_Values SZ_5
        {
            get => _sZ_5;
            set => Set(() => SZ_5, ref _sZ_5, value);

        }
        /// <summary>
        /// Schutzziel-Wert, ( 0(nicht relevant) bis 4(Sehr Hoch))
        /// </summary>
        public SZ_Values SZ_6
        {
            get => _sZ_6;
            set => Set(() =>  SZ_6, ref _sZ_6, value);

        }
        /// <summary>
        /// Anzahl verknüpfter Prozesse
        /// </summary>
        public int Verknüpfte_Prozesse
        {
            get => _verknüpfte_Prozesse;
            set => Set(() => Verknüpfte_Prozesse, ref _verknüpfte_Prozesse, value);
        }
        /// <summary>
        /// Bearbeitender Nutzer des Datensatzes
        /// </summary>
        public string Benutzer
        {
            get => _benutzer;
            set => Set(() => Benutzer, ref _benutzer, value);

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
        /// gibt an, ob Applikation aktiv oder inaktiv ist
        /// </summary>
        public int Aktiv
        {
            get => _aktiv;
            set => Set(() => Aktiv, ref _aktiv, value);

        }
        /// <summary>
        /// gibt an, ob Applikation neu angelegt wurde (für SBA)
        /// </summary>
        public string Erstanlage
        {
            get => _erstanlage;
            set => Set(() => Erstanlage, ref _erstanlage, value);

        }
        #endregion
    }
}
