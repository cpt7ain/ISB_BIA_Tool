using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using ISB_BIA_IMPORT1.LinqEntityContext;
using ISB_BIA_IMPORT1.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace ISB_BIA_IMPORT1.ViewModel
{
    /// <summary>
    /// VM zur Darstellung der DeltaAnsicht
    /// </summary>
    public class DeltaAnalysis_ViewModel : ViewModelBase
    {
        #region Backing-Fields
        private ObservableCollection<ISB_BIA_Delta_Analyse> _list;
        private ObservableCollection<ISB_BIA_Delta_Analyse> _deltaList;
        private bool _mode;
        private string _instruction;
        private MyRelayCommand _exportDeltaList;
        private CollectionView _view;
        private static string _filterText;
        #endregion

        /// <summary>
        /// Delta-Analysis-Liste (komplett) ->ListView
        /// Erzeugt im Setter zursätzlich die gefilterte Liste <see cref="DeltaList"/>
        /// </summary>
        public ObservableCollection<ISB_BIA_Delta_Analyse> List
        {
            get => _list;
            set
            {
                Set(() =>  List, ref _list, value);
                DeltaList = FilterForDeltaList(value);
            }
        }

        /// <summary>
        /// elta-Analysis-Liste (nur delta) ->DataGrid
        /// </summary>
        public ObservableCollection<ISB_BIA_Delta_Analyse> DeltaList
        {
            get => _deltaList;
            set => Set(() => DeltaList, ref _deltaList, value);
        }

        /// <summary>
        /// Command zum Zurückkehren zum vorherigen VM
        /// </summary>
        public MyRelayCommand NavBack
        {
            get => new MyRelayCommand(() =>
                {
                    Cleanup();
                    _myNavi.NavigateBack();
                });         
        }

        /// <summary>
        /// Modus zum Wechsel zwischen ListView+Filter Ansicht und DataGrid+Export Ansicht
        /// </summary>
        public bool Mode
        {
            get => _mode;
            set
            {
                Set(() =>  Mode, ref _mode, value);
                if (!value) Instruction = "Ansicht ändern (=> nur Delta-Einträge anzeigen)";
                else Instruction = "Ansicht ändern (=> alle Einträge anzeigen)";           
            }
        }

        /// <summary>
        /// Beschriftung der Checkbox, welche den Ansichtsmodus bestimmt
        /// </summary>
        public string Instruction
        {
            get => _instruction;
            set => Set(() => Instruction, ref _instruction, value);
        }

        #region Einstellungen zur Darstellung der Spalten der neuen Schutzziele
        /// <summary>
        /// Sichtbarkeit der neuen Schutzziele im Datagrid
        /// </summary>
        public Visibility NewSecurityGoals { get; set; }
        /// <summary>
        /// Sichtbarkeit der neuen Schutzziele im ListView
        /// </summary>
        public int NewSecurityGoalsWidth { get; set; }
        /// <summary>
        /// Aktuelle Anwendungseinstellungen
        /// </summary>
        public ISB_BIA_Settings Setting { get; set; }
        #endregion

        /// <summary>
        /// Exportieren der Delta-Analysis-Liste (nur delta) nach Excel
        /// </summary>
        public MyRelayCommand ExportDeltaList
        {
            get => _exportDeltaList
                  ?? (_exportDeltaList = new MyRelayCommand(() =>
                  {
                      _myExport.ExportDeltaAnalysis(DeltaList);
                  }));          
        }

        /// <summary>
        /// Ansicht für das Sortieren und Filtern der Delta-Liste
        /// </summary>
        public CollectionView View
        {
            get => _view;
            set => Set(() => View, ref _view, value);
        }

        /// <summary>
        /// Text, nach dem die Delta-Liste gefiltert werden soll
        /// </summary>
        public string FilterText
        {
            get => _filterText;
            set
            {
                Set(() =>  FilterText, ref _filterText, value);
                View.Refresh();
            }
        }

        #region Services
        private readonly IMyNavigationService _myNavi;
        private readonly IMyExportService _myExport;
        private readonly IMyDataService _myData;
        #endregion

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="myNavigationService"></param>
        /// <param name="myExportService"></param>
        /// <param name="myDataService"></param>
        public DeltaAnalysis_ViewModel(IMyNavigationService myNavigationService, IMyExportService myExportService, IMyDataService myDataService)
        {
            #region Services
            _myNavi = myNavigationService;
            _myExport = myExportService;
            _myData = myDataService;
            #endregion

            if (IsInDesignMode)
            {
                Setting = _myData.GetSettings();
                List = _myData.GetDeltaAnalysis();
                DeltaList = new ObservableCollection<ISB_BIA_Delta_Analyse>(List);
                //DeltaList = _myData.GetDeltaAnalysis();
                View = (CollectionView)CollectionViewSource.GetDefaultView(List);
                View.Filter = DeltaFilter;
                NewSecurityGoalsWidth = (Setting.Neue_Schutzziele_aktiviert == "Ja") ? 110 : 0;
                NewSecurityGoals = (Setting.Neue_Schutzziele_aktiviert == "Ja") ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                //Starten mit Datagrid-Ansicht
                Mode = true;
                #region Delta-Analyse Abrufen
                List = new ObservableCollection<ISB_BIA_Delta_Analyse>();
                DeltaList = new ObservableCollection<ISB_BIA_Delta_Analyse>();
                #endregion
                #region Registrierung für Messages um DeltaAnalyse zu erhalten
                MessengerInstance.Register<NotificationMessage<ObservableCollection<ISB_BIA_Delta_Analyse>>>(this, a => 
                {
                    if (!(a.Sender is Menu_ViewModel)) return;
                    List = a.Content;
                    View = (CollectionView)CollectionViewSource.GetDefaultView(List);
                    View.Filter = DeltaFilter;
                });
                #endregion
                #region Einstellungen abrufen
                Setting = _myData.GetSettings();
                NewSecurityGoalsWidth = (Setting.Neue_Schutzziele_aktiviert == "Ja") ? 110 : 0;
                NewSecurityGoals = (Setting.Neue_Schutzziele_aktiviert == "Ja") ? Visibility.Visible : Visibility.Collapsed;
                #endregion
            }
        }

        /// <summary>
        /// Filtert die Delta-Analyse für relevante Einträge (Einträge mit Delta)
        /// </summary>
        /// <param name="all"> Liste der Kompletten Delta-Analyse </param>
        /// <returns> Liste der relevanten Einträge </returns>
        public ObservableCollection<ISB_BIA_Delta_Analyse> FilterForDeltaList(ObservableCollection<ISB_BIA_Delta_Analyse> all)
        {
            return new ObservableCollection<ISB_BIA_Delta_Analyse>(
                all.Where(d => d.SZ_1 < 0 || d.SZ_2 < 0 ||
                          d.SZ_3 < 0 || d.SZ_4 < 0
                          || d.SZ_5 < 0 || d.SZ_6 < 0).ToList());
        }

        /// <summary>
        /// Definition des Delta-Filters
        /// </summary>
        /// <param name="item"> Filter Item </param>
        /// <returns> Wahrheitswert, ob Item gefiltert wird oder nicht </returns>
        public static bool DeltaFilter(object item)
        {
            if (String.IsNullOrEmpty(_filterText))
                return true;
            else
            {
                ISB_BIA_Delta_Analyse deltaItem = (ISB_BIA_Delta_Analyse)item;
                return (
                    (deltaItem.Prozess.IndexOf(_filterText, StringComparison.OrdinalIgnoreCase) >= 0)
                 || (deltaItem.Applikation.IndexOf(_filterText, StringComparison.OrdinalIgnoreCase) >= 0)
                 || (deltaItem.Applikation_Id.ToString().IndexOf(_filterText, StringComparison.OrdinalIgnoreCase) >= 0)
                 || (deltaItem.Prozess_Id.ToString().IndexOf(_filterText, StringComparison.OrdinalIgnoreCase) >= 0)
                 || (deltaItem.Sub_Prozess.IndexOf(_filterText, StringComparison.OrdinalIgnoreCase) >= 0)
                 || (deltaItem.Datum.ToString().IndexOf(_filterText, StringComparison.OrdinalIgnoreCase) >= 0)
                 || (deltaItem.Datum_Prozess.ToString().IndexOf(_filterText, StringComparison.OrdinalIgnoreCase) >= 0)
                 || (deltaItem.Datum_Applikation.ToString().IndexOf(_filterText, StringComparison.OrdinalIgnoreCase) >= 0)
                 || (deltaItem.SZ_1.ToString().IndexOf(_filterText, StringComparison.OrdinalIgnoreCase) >= 0)
                 || (deltaItem.SZ_2.ToString().IndexOf(_filterText, StringComparison.OrdinalIgnoreCase) >= 0)
                 || (deltaItem.SZ_3.ToString().IndexOf(_filterText, StringComparison.OrdinalIgnoreCase) >= 0)
                 || (deltaItem.SZ_4.ToString().IndexOf(_filterText, StringComparison.OrdinalIgnoreCase) >= 0)
                 || (deltaItem.SZ_5.ToString().IndexOf(_filterText, StringComparison.OrdinalIgnoreCase) >= 0)
                 || (deltaItem.SZ_6.ToString().IndexOf(_filterText, StringComparison.OrdinalIgnoreCase) >= 0)
                 );
            }
        }

        /// <summary>
        /// Bereinigt das Viewmodel
        /// </summary>
        override public void Cleanup()
        {
            SimpleIoc.Default.Unregister(this);
            base.Cleanup();
        }
    }
}
