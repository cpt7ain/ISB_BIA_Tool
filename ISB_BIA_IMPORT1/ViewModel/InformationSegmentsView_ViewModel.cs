using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using ISB_BIA_IMPORT1.LinqDataContext;
using ISB_BIA_IMPORT1.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace ISB_BIA_IMPORT1.ViewModel
{
    /// <summary>
    /// VM zur Darstellung der Informationssegment-Übersicht
    /// </summary>
    public class InformationSegmentsView_ViewModel : ViewModelBase
    {
        #region Backing Fields
        private MyRelayCommand _navToIS;
        private ObservableCollection<ISB_BIA_Informationssegmente> _iSList;
        private ObservableCollection<string> _attributeColumnHeaderText;
        private object _selectedItem;
        private ISISAttributeMode _iSMode;
        #endregion

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
        /// Zu dem ausgewählten Informationssegment <see cref="SelectedItem"/> navigieren.
        /// Wenn EditMode dann Objekt zusätzlich sperren.
        /// </summary>
        public MyRelayCommand NavToIS
        {
            get => _navToIS
                    ?? (_navToIS = new MyRelayCommand(() =>
                    {
                        if (SelectedItem == null) return;
                        ISB_BIA_Informationssegmente iSToChange = (ISB_BIA_Informationssegmente)SelectedItem;
                        if (ISMode == ISISAttributeMode.Edit)
                        {
                            string user = _myData.GetObjectLocked(Table_Lock_Flags.Segment, iSToChange.Informationssegment_Id);
                            if (user == "")
                            {
                                if (_myData.LockObject(Table_Lock_Flags.Segment, iSToChange.Informationssegment_Id))
                                    _myNavi.NavigateTo<InformationSegment_ViewModel>(iSToChange.Informationssegment_Id, ISMode);
                            }
                            else
                            {
                                _myDia.ShowWarning("Die Attribute werden momentan durch einen anderen CISO bearbeitet und können daher nicht geöffnet werden.\n\nBelegender Benutzer: " + user + "\n\nSollte kein CISO die Attribut-Liste geöffnet haben, wenden Sie sich bitte IT.");
                            }
                        }
                        else
                        {
                            _myNavi.NavigateTo<InformationSegment_ViewModel>(iSToChange.Informationssegment_Id, ISMode);
                        }
                    }));
        }

        /// <summary>
        /// Liste der angezeigten Informationssegmente
        /// </summary>
        public ObservableCollection<ISB_BIA_Informationssegmente> ISList
        {
            get => _iSList;
            set => Set(() => ISList, ref _iSList, value);
        }

        /// <summary>
        /// Liste der Attribut-Namen im Spaltenheader je nach aktueller Benennung des jeweiligen Attributs
        /// </summary>
        public ObservableCollection<string> AttributeColumnHeaderText
        {
            get => _attributeColumnHeaderText;
            set => Set(() => AttributeColumnHeaderText, ref _attributeColumnHeaderText, value);
        }

        /// <summary>
        /// Ausgewähltes Element
        /// </summary>
        public object SelectedItem
        {
            get => _selectedItem;
            set => Set(() => SelectedItem, ref _selectedItem, value);
        }

        #region Einstellungen zur darstellung der Spalten der zusätzlichen Attribute
        /// <summary>
        /// Aktuelle Einstellungen
        /// </summary>
        ISB_BIA_Settings Settings { get; set; }
        /// <summary>
        /// Sichtbarkeit von Attribut 9
        /// </summary>
        public Visibility Attribute_9_Vis
        {
            get => (Settings.Attribut9_aktiviert == "Ja") ? Visibility.Visible : Visibility.Collapsed;
        }
        /// <summary>
        /// Sichtbarkeit von Attribut 10
        /// </summary>
        public Visibility Attribute_10_Vis
        {
            get => (Settings.Attribut10_aktiviert == "Ja") ? Visibility.Visible : Visibility.Collapsed;
        }
        #endregion

        /// <summary>
        /// Modus, welcher angezeigte Segmente sowie Anweisung bestimmt
        /// </summary>
        public ISISAttributeMode ISMode
        {
            get => _iSMode;
            set
            {
                Set(() => ISMode, ref _iSMode, value);
                if (value == ISISAttributeMode.Edit)
                {
                    Instruction = "Doppelklick auf ein Segment, das Sie ändern oder betrachten möchten.";
                    ISList = _myData.GetAllSegments();
                }
                else if (value == ISISAttributeMode.View)
                {
                    Instruction = "Doppelklick auf ein Segment, das Sie betrachten möchten.";
                    ISList = _myData.GetEnabledSegments();
                }
            }
        }

        /// <summary>
        /// Anweisung
        /// </summary>
        public string Instruction { get; set; }

        #region Services
        IMyNavigationService _myNavi;
        IMyDialogService _myDia;
        IMyDataService _myData;
        #endregion

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="myDialogService"></param>
        /// <param name="myNavigationService"></param>
        /// <param name="myDataService"></param>
        public InformationSegmentsView_ViewModel(IMyDialogService myDialogService, IMyNavigationService myNavigationService, IMyDataService myDataService)
        {
            #region Services
            _myDia = myDialogService;
            _myNavi = myNavigationService;
            _myData = myDataService;
            #endregion

            if (IsInDesignMode)
            {
                AttributeColumnHeaderText = _myData.GetAttributeNamesForHeader();
                ISList = _myData.GetAllSegments();
                Settings = _myData.GetSettings();
            }
            else
            {
                //Messenger Registierung für Nachrichten die den Ansichtsmodus bestimmen
                Messenger.Default.Register<ISISAttributeMode>(this, n => { ISMode = n; });
                //Messenger Registierung für Nachrichten die eine Aktualisierung der ISListe auslösen
                Messenger.Default.Register<string>(this, MessageToken.RefreshData, p => { Refresh(); });
                //Abrufen der Headernamen
                AttributeColumnHeaderText = _myData.GetAttributeNamesForHeader();
                //Einstellungen abrufen
                Settings = _myData.GetSettings();
            }
        }

        /// <summary>
        /// Aktualisieren der Liste der angezeigten Segmente
        /// </summary>
        public void Refresh()
        {
            if (ISMode == ISISAttributeMode.Edit)
            {
                ISList = _myData.GetAllSegments();
            }
            else if (ISMode == ISISAttributeMode.View)
            {
                ISList = _myData.GetEnabledSegments();
            }
        }

        /// <summary>
        /// Bereinigt das Viewmodel
        /// </summary>
        override public void Cleanup()
        {
            Messenger.Default.Unregister(this);
            SimpleIoc.Default.Unregister(this);
            base.Cleanup();
        }
    }
}
