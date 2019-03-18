using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using ISB_BIA_IMPORT1.LINQ2SQL;
using ISB_BIA_IMPORT1.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using ISB_BIA_IMPORT1.Helpers;
using ISB_BIA_IMPORT1.Services.Interfaces;

namespace ISB_BIA_IMPORT1.ViewModel
{
    /// <summary>
    /// VM zur Darstellung der DeltaAnsicht
    /// </summary>
    public class DeltaAnalysis_ViewModel : ViewModelBase
    {
        #region Backing-Fields
        private ObservableCollection<ISB_BIA_Delta_Analyse> _list_Complete;
        private ObservableCollection<ISB_BIA_Delta_Analyse> _list_Delta;
        private bool _mode;
        private string _str_Instruction;
        private MyRelayCommand _cmd_ExportDeltaList;
        private CollectionView _filterView;
        private static string _str_FilterText;
        #endregion

        /// <summary>
        /// Delta-Analysis-Liste (komplett) ->ListView
        /// Erzeugt im Setter zursätzlich die gefilterte Liste <see cref="List_Delta"/>
        /// </summary>
        public ObservableCollection<ISB_BIA_Delta_Analyse> List_Complete
        {
            get => _list_Complete;
            set
            {
                Set(() =>  List_Complete, ref _list_Complete, value);
                List_Delta = FilterForDeltaList(value);
            }
        }

        /// <summary>
        /// elta-Analysis-Liste (nur delta) ->DataGrid
        /// </summary>
        public ObservableCollection<ISB_BIA_Delta_Analyse> List_Delta
        {
            get => _list_Delta;
            set => Set(() => List_Delta, ref _list_Delta, value);
        }

        /// <summary>
        /// Command zum Zurückkehren zum vorherigen VM
        /// </summary>
        public MyRelayCommand Cmd_NavBack
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
                if (!value) Str_Instruction = "Ansicht ändern (=> nur Delta-Einträge anzeigen)";
                else Str_Instruction = "Ansicht ändern (=> alle Einträge anzeigen)";           
            }
        }

        /// <summary>
        /// Beschriftung der Checkbox, welche den Ansichtsmodus bestimmt
        /// </summary>
        public string Str_Instruction
        {
            get => _str_Instruction;
            set => Set(() => Str_Instruction, ref _str_Instruction, value);
        }

        #region Einstellungen zur Darstellung der Spalten der neuen Schutzziele
        /// <summary>
        /// Sichtbarkeit der neuen Schutzziele im Datagrid
        /// </summary>
        public Visibility Vis_NewSecurityGoals { get; set; }
        /// <summary>
        /// Sichtbarkeit der neuen Schutzziele im ListView
        /// </summary>
        public int Width_NewSecurityGoals { get; set; }
        /// <summary>
        /// Aktuelle Anwendungseinstellungen
        /// </summary>
        public ISB_BIA_Settings Setting { get; set; }
        #endregion

        /// <summary>
        /// Exportieren der Delta-Analysis-Liste (nur delta) nach Excel
        /// </summary>
        public MyRelayCommand Cmd_ExportDeltaList
        {
            get => _cmd_ExportDeltaList
                  ?? (_cmd_ExportDeltaList = new MyRelayCommand(() =>
                  {
                      _myExport.Export_DeltaAnalysis(List_Delta);
                  }));          
        }

        /// <summary>
        /// Ansicht für das Sortieren und Filtern der Delta-Liste
        /// </summary>
        public CollectionView FilterView
        {
            get => _filterView;
            set => Set(() => FilterView, ref _filterView, value);
        }

        /// <summary>
        /// Text, nach dem die Delta-Liste gefiltert werden soll
        /// </summary>
        public string Str_FilterText
        {
            get => _str_FilterText;
            set
            {
                Set(() =>  Str_FilterText, ref _str_FilterText, value);
                FilterView.Refresh();
            }
        }

        #region Services
        private readonly INavigationService _myNavi;
        private readonly IExportService _myExport;
        private readonly IDataService_Delta _myDelta;
        private readonly IDataService_Setting _mySett;
        #endregion

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="myNavi"></param>
        /// <param name="myExp"></param>
        /// <param name="myDelta"></param>
        public DeltaAnalysis_ViewModel(INavigationService myNavi, IExportService myExp, 
            IDataService_Delta myDelta, IDataService_Setting mySett)
        {
            #region Services
            _myNavi = myNavi;
            _myExport = myExp;
            _myDelta = myDelta;
            _mySett = mySett;
            #endregion

            if (IsInDesignMode)
            {
                Setting = _mySett.Get_List_Settings();
                List_Complete = _myDelta.Get_List_Delta();
                List_Delta = new ObservableCollection<ISB_BIA_Delta_Analyse>(List_Complete);
                //DeltaList = _myData.GetDeltaAnalysis();
                FilterView = (CollectionView)CollectionViewSource.GetDefaultView(List_Complete);
                FilterView.Filter = DeltaFilter;
                Width_NewSecurityGoals = (Setting.Neue_Schutzziele_aktiviert == "Ja") ? 110 : 0;
                Vis_NewSecurityGoals = (Setting.Neue_Schutzziele_aktiviert == "Ja") ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                //Starten mit Datagrid-Ansicht
                Mode = true;
                #region Delta-Analyse Abrufen
                List_Complete = new ObservableCollection<ISB_BIA_Delta_Analyse>();
                List_Delta = new ObservableCollection<ISB_BIA_Delta_Analyse>();
                #endregion
                #region Registrierung für Messages um DeltaAnalyse zu erhalten
                MessengerInstance.Register<NotificationMessage<ObservableCollection<ISB_BIA_Delta_Analyse>>>(this, a => 
                {
                    if (!(a.Sender is Menu_ViewModel)) return;
                    List_Complete = a.Content;
                    FilterView = (CollectionView)CollectionViewSource.GetDefaultView(List_Complete);
                    FilterView.Filter = DeltaFilter;
                });
                #endregion
                #region Einstellungen abrufen
                Setting = _mySett.Get_List_Settings();
                Width_NewSecurityGoals = (Setting.Neue_Schutzziele_aktiviert == "Ja") ? 110 : 0;
                Vis_NewSecurityGoals = (Setting.Neue_Schutzziele_aktiviert == "Ja") ? Visibility.Visible : Visibility.Collapsed;
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
            if (String.IsNullOrEmpty(_str_FilterText))
                return true;
            else
            {
                ISB_BIA_Delta_Analyse deltaItem = (ISB_BIA_Delta_Analyse)item;
                return (
                    (deltaItem.Prozess.IndexOf(_str_FilterText, StringComparison.OrdinalIgnoreCase) >= 0)
                 || (deltaItem.Applikation.IndexOf(_str_FilterText, StringComparison.OrdinalIgnoreCase) >= 0)
                 || (deltaItem.Applikation_Id.ToString().IndexOf(_str_FilterText, StringComparison.OrdinalIgnoreCase) >= 0)
                 || (deltaItem.Prozess_Id.ToString().IndexOf(_str_FilterText, StringComparison.OrdinalIgnoreCase) >= 0)
                 || (deltaItem.Sub_Prozess.IndexOf(_str_FilterText, StringComparison.OrdinalIgnoreCase) >= 0)
                 || (deltaItem.Datum.ToString().IndexOf(_str_FilterText, StringComparison.OrdinalIgnoreCase) >= 0)
                 || (deltaItem.Datum_Prozess.ToString().IndexOf(_str_FilterText, StringComparison.OrdinalIgnoreCase) >= 0)
                 || (deltaItem.Datum_Applikation.ToString().IndexOf(_str_FilterText, StringComparison.OrdinalIgnoreCase) >= 0)
                 || (deltaItem.SZ_1.ToString().IndexOf(_str_FilterText, StringComparison.OrdinalIgnoreCase) >= 0)
                 || (deltaItem.SZ_2.ToString().IndexOf(_str_FilterText, StringComparison.OrdinalIgnoreCase) >= 0)
                 || (deltaItem.SZ_3.ToString().IndexOf(_str_FilterText, StringComparison.OrdinalIgnoreCase) >= 0)
                 || (deltaItem.SZ_4.ToString().IndexOf(_str_FilterText, StringComparison.OrdinalIgnoreCase) >= 0)
                 || (deltaItem.SZ_5.ToString().IndexOf(_str_FilterText, StringComparison.OrdinalIgnoreCase) >= 0)
                 || (deltaItem.SZ_6.ToString().IndexOf(_str_FilterText, StringComparison.OrdinalIgnoreCase) >= 0)
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
