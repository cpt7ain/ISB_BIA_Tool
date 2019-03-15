using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using ISB_BIA_IMPORT1.Model;
using ISB_BIA_IMPORT1.LINQ2SQL;
using ISB_BIA_IMPORT1.Services;
using System.Collections.ObjectModel;
using System.Windows;
using ISB_BIA_IMPORT1.Helpers;
using ISB_BIA_IMPORT1.Services.Interfaces;

namespace ISB_BIA_IMPORT1.ViewModel
{
    /// <summary>
    /// VM zur Darstellung der Informationssegment-Attribut-Übersicht
    /// </summary>
    public class InformationSegmentsAttributes_ViewModel : ViewModelBase
    {
        #region Backing Fields
        private ObservableCollection<InformationSegmentAttribute_Model> _list_CurrentAttributes;
        private ISISAttributeMode _mode;
        private Visibility _vis_NewSecurityGoals;
        #endregion

        /// <summary>
        /// Aktuelle Liste der Attribute, welche angezeigt und bearbeitet werden kann
        /// </summary>
        public ObservableCollection<InformationSegmentAttribute_Model> List_CurrentAttributes
        {
            get => _list_CurrentAttributes;
            set => Set(() => List_CurrentAttributes, ref _list_CurrentAttributes, value);
        }

        #region Properties für Einstellungen bezüglich Bearbeitung durch Admin/CISO oder betrachten durch Normalen User
        /// <summary>
        /// Property für Einstellungen bezüglich Bearbeitung durch CISO oder betrachten durch andere User
        /// </summary>
        public ISISAttributeMode Mode
        {
            get => _mode;
            set
            {
                Set(() => Mode, ref _mode, value);
                if (value == ISISAttributeMode.Edit)
                {
                    EditMode = true;
                    Str_Instruction = "Ändern Sie hier den Attributnamen sowie die zugehörigen Mindesteinstufungen der Schutzziele";
                }
                else
                {
                    EditMode = false;
                    Str_Instruction = "Hier sehen Sie eine Übersicht der Informationssegment-Attribute und mit welchen Einstufungen diese für das jeweilige Schutzziel versehen sind.";
                }
            }
        }
        /// <summary>
        /// Property für Einstellungen bezüglich Bearbeitung durch CISO oder betrachten durch andere User
        /// </summary>
        public bool EditMode { get; set; }
        /// <summary>
        /// Anweisung-String
        /// </summary>
        public string Str_Instruction { get; set; }
        #endregion

        /// <summary>
        /// Command zum Zurückkehren zum vorherigen VM
        /// </summary>
        public MyRelayCommand Cmd_NavBack
        {
            get => new MyRelayCommand(() =>
                {
            if (Mode == ISISAttributeMode.View)
            {
                Cleanup();
                _myNavi.NavigateBack();
            }
            else if (_myDia.CancelDecision())
            {
                Cleanup();
                _myNavi.NavigateBack();
                _myLock.Unlock_Object(Table_Lock_Flags.Attributes, 0);
            }
        });
        }

        /// <summary>
        /// Sichtbarkeit der neuen Schutzziele
        /// </summary>
        public Visibility Vis_NewSecurityGoals
        {
            get => _vis_NewSecurityGoals;
            set => Set(()=>Vis_NewSecurityGoals, ref _vis_NewSecurityGoals, value);
        }

        /// <summary>
        /// Aktuelle Einstellungen
        /// </summary>
        public ISB_BIA_Settings Setting { get; set; }

        /// <summary>
        /// Speichern der Attributliste, entsperren der Liste und zuruückkehren zum vorherigen VM
        /// </summary>
        public MyRelayCommand Cmd_Save
        {
            get => new MyRelayCommand(() =>
            {
                if (_myIS.Insert_Attribute(List_CurrentAttributes))
                {
                    Cleanup();
                    _myNavi.NavigateBack();
                    _myLock.Unlock_Object(Table_Lock_Flags.Attributes, 0);
                }
            });
        }

        #region Services
        private readonly IMyNavigationService _myNavi;
        private readonly IMyDialogService _myDia;
        private readonly IMyDataService_IS_Attribute _myIS;
        private readonly IMyDataService_Setting _mySett;
        private readonly IMyDataService_Lock _myLock;

        #endregion

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="myDialogService"></param>
        /// <param name="myNavigationService"></param>
        /// <param name="myIS"></param>
        public InformationSegmentsAttributes_ViewModel(IMyDialogService myDialogService, 
            IMyNavigationService myNavigationService, IMyDataService_IS_Attribute myIS,
            IMyDataService_Lock myLock, IMyDataService_Setting mySett)
        {
            #region Services
            _myDia = myDialogService;
            _myLock = myLock;
            _mySett = mySett;
            _myNavi = myNavigationService;
            _myIS = myIS;
            #endregion
            //Messenger Registrierung für Bestimmung des Ansichtsmodus
            MessengerInstance.Register<NotificationMessage<ISISAttributeMode>>(this, message => 
            {
                if (!(message.Sender is IMyNavigationService)) return;
                Mode = message.Content;
            });
            //Messenger Registrierung für Benachrichtigungen bei Fehlerhafter Eingabe
            MessengerInstance.Register<NotificationMessage<string>>(this, MessageToken.ISAttributValidationError, message =>
            {
                if (!(message.Sender is InformationSegmentAttribute_Model)) return;
                _myDia.ShowWarning(message.Content);
            });

            #region Aktuelle Einstellungen abrufen
            Setting = _mySett.Get_Settings();
            Vis_NewSecurityGoals = (Setting.Neue_Schutzziele_aktiviert == "Ja") ? Visibility.Visible : Visibility.Collapsed;
            #endregion
            //Abrufen der Aktuellen Attributliste
            List_CurrentAttributes = GetAttributeList();
        }

        /// <summary>
        /// Liste der 8-10 Attribute (Abhängig van Einstellungen für Attribut 9/10) abrufen (Mapping von DB zu Model im Model Konstruktor inklusive)
        /// </summary>
        /// <returns></returns>
        private ObservableCollection<InformationSegmentAttribute_Model> GetAttributeList()
        {
            ObservableCollection<InformationSegmentAttribute_Model> result = new ObservableCollection<InformationSegmentAttribute_Model>();
            //Die ersten 8 Attribute abrufen
            for (int i=1; i <= 8; i++)
            {
                InformationSegmentAttribute_Model item = _myIS.Get_AttributeModelFromDB(i);
                if (item == null)
                {
                    _myDia.ShowError("Fehler beim Laden der Daten.");
                    Cleanup();
                    _myNavi.NavigateBack();
                }
                result.Add(item);
            }
            //Attribut 9
            if (Setting.Attribut9_aktiviert=="Ja")
            {
                InformationSegmentAttribute_Model item = _myIS.Get_AttributeModelFromDB(9);
                if (item == null)
                {
                    _myDia.ShowError("Fehler beim Laden der Daten.");
                    Cleanup();
                    _myNavi.NavigateBack();
                }
                result.Add(item);
            }
            //Attribut 10
            if (Setting.Attribut10_aktiviert == "Ja")
            {
                InformationSegmentAttribute_Model item = _myIS.Get_AttributeModelFromDB(10);
                if (item == null)
                {
                    _myDia.ShowError("Fehler beim Laden der Daten.");
                    Cleanup();
                    _myNavi.NavigateBack();
                }
                result.Add(item);
            }
            return result;
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
