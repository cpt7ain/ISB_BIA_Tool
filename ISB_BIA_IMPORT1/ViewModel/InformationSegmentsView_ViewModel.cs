using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using ISB_BIA_IMPORT1.LINQ2SQL;
using ISB_BIA_IMPORT1.Services;
using System.Collections.ObjectModel;
using System.Windows;
using ISB_BIA_IMPORT1.Helpers;
using ISB_BIA_IMPORT1.Services.Interfaces;

namespace ISB_BIA_IMPORT1.ViewModel
{
    /// <summary>
    /// VM zur Darstellung der Informationssegment-Übersicht
    /// </summary>
    public class InformationSegmentsView_ViewModel : ViewModelBase
    {
        #region Backing Fields
        private MyRelayCommand _cmd_NavToIS;
        private ObservableCollection<ISB_BIA_Informationssegmente> _list_InformationSegment;
        private ObservableCollection<string> _attributeColumnHeaderText;
        private object _selectedItem;
        private ISISAttributeMode _mode;
        #endregion

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
        /// Zu dem ausgewählten Informationssegment <see cref="SelectedItem"/> navigieren.
        /// Wenn EditMode dann Objekt zusätzlich sperren.
        /// </summary>
        public MyRelayCommand Cmd_NavToIS
        {
            get => _cmd_NavToIS
                    ?? (_cmd_NavToIS = new MyRelayCommand(() =>
                    {
                        if (SelectedItem == null) return;
                        ISB_BIA_Informationssegmente iSToChange = (ISB_BIA_Informationssegmente)SelectedItem;
                        if (Mode == ISISAttributeMode.Edit)
                        {
                            string user = _myLock.Get_ObjectIsLocked(Table_Lock_Flags.Segment, iSToChange.Informationssegment_Id);
                            if (user == "")
                            {
                                if (_myLock.Lock_Object(Table_Lock_Flags.Segment, iSToChange.Informationssegment_Id))
                                    _myNavi.NavigateTo<InformationSegment_ViewModel>(iSToChange.Informationssegment_Id, Mode);
                            }
                            else
                            {
                                _myDia.ShowWarning("Die Attribute werden momentan durch einen anderen CISO bearbeitet und können daher nicht geöffnet werden.\n\nBelegender Benutzer: " + user + "\n\nSollte kein CISO die Attribut-Liste geöffnet haben, wenden Sie sich bitte IT.");
                            }
                        }
                        else
                        {
                            _myNavi.NavigateTo<InformationSegment_ViewModel>(iSToChange.Informationssegment_Id, Mode);
                        }
                    }));
        }
        /// <summary>
        /// Command zum Exportieren der Segment und Attributübersicht inklusive historischer Daten
        /// </summary>
        public MyRelayCommand Cmd_ExportSegmentAndAttributes
        {
            get => new MyRelayCommand(() =>
            {
                _myExp.IS_Attr_ExportSegmentAndAttributeHistory();
            });
        }
        /// <summary>
        /// Liste der angezeigten Informationssegmente
        /// </summary>
        public ObservableCollection<ISB_BIA_Informationssegmente> List_InformationSegment
        {
            get => _list_InformationSegment;
            set => Set(() => List_InformationSegment, ref _list_InformationSegment, value);
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
        public Visibility Vis_Attribut9
        {
            get => (Settings.Attribut9_aktiviert == "Ja") ? Visibility.Visible : Visibility.Collapsed;
        }
        /// <summary>
        /// Sichtbarkeit von Attribut 10
        /// </summary>
        public Visibility Vis_Attribut10
        {
            get => (Settings.Attribut10_aktiviert == "Ja") ? Visibility.Visible : Visibility.Collapsed;
        }
        #endregion

        /// <summary>
        /// Modus, welcher angezeigte Segmente sowie Anweisung bestimmt
        /// </summary>
        public ISISAttributeMode Mode
        {
            get => _mode;
            set
            {
                Set(() => Mode, ref _mode, value);
                if (value == ISISAttributeMode.Edit)
                {
                    Str_Instruction = "Doppelklick auf ein Segment, das Sie ändern oder betrachten möchten.";
                    List_InformationSegment = _myIS.Get_Segments_All();
                }
                else if (value == ISISAttributeMode.View)
                {
                    Str_Instruction = "Doppelklick auf ein Segment, das Sie betrachten möchten.";
                    List_InformationSegment = _myIS.Get_Segments_Enabled();
                }
            }
        }

        /// <summary>
        /// Anweisung
        /// </summary>
        public string Str_Instruction { get; set; }

        #region Services
        private readonly IMyNavigationService _myNavi;
        private readonly IMyDialogService _myDia;
        private readonly IMyDataService_IS_Attribute _myIS;
        private readonly IMyDataService_Lock _myLock;
        private readonly IMyExportService _myExp;
        private readonly IMyDataService_Setting _mySett;
        #endregion

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="myDia"></param>
        /// <param name="myNavi"></param>
        /// <param name="myIS"></param>
        /// <param name="myExp"></param>
        public InformationSegmentsView_ViewModel(IMyDialogService myDia, IMyNavigationService myNavi, 
            IMyDataService_IS_Attribute myIS, IMyExportService myExp, IMyDataService_Lock myLock,
            IMyDataService_Setting mySett)
        {
            #region Services
            _myDia = myDia;
            _myNavi = myNavi;
            _myLock = myLock;
            _mySett = mySett;
            _myIS = myIS;
            _myExp = myExp;
            #endregion

            if (IsInDesignMode)
            {
                AttributeColumnHeaderText = _myIS.Get_List_AttributeNamesForHeader();
                List_InformationSegment = _myIS.Get_Segments_All();
                Settings = _mySett.Get_Settings();
            }
            else
            {
                //Messenger Registierung für Nachrichten die den Ansichtsmodus bestimmen
                MessengerInstance.Register<NotificationMessage<ISISAttributeMode>>(this, n =>
                {
                    if (!(n.Sender is IMyNavigationService)) return;
                    Mode = n.Content;
                });
                MessengerInstance.Register<NotificationMessage<string>>(this, MessageToken.RefreshData, s =>
                {
                    if (!(s.Sender is IMyNavigationService)) return;
                    Refresh();
                });
                //Abrufen der Headernamen
                AttributeColumnHeaderText = _myIS.Get_List_AttributeNamesForHeader();
                //Einstellungen abrufen
                Settings = _mySett.Get_Settings();
            }
        }

        /// <summary>
        /// Aktualisieren der Liste der angezeigten Segmente
        /// </summary>
        public void Refresh()
        {
            if (Mode == ISISAttributeMode.Edit)
            {
                List_InformationSegment = _myIS.Get_Segments_All();
            }
            else if (Mode == ISISAttributeMode.View)
            {
                List_InformationSegment = _myIS.Get_Segments_Enabled();
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
