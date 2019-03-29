using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using ISB_BIA_IMPORT1.Model;
using ISB_BIA_IMPORT1.LINQ2SQL;
using System.Collections.ObjectModel;
using System.Windows;
using ISB_BIA_IMPORT1.Helpers;
using ISB_BIA_IMPORT1.Services.Interfaces;

namespace ISB_BIA_IMPORT1.ViewModel
{
    /// <summary>
    /// VM zur Darstellung der Informationssegment-Attribut-Übersicht
    /// </summary>
    public class Attributes_ViewModel : ViewModelBase
    {
        #region Backing Fields
        private ObservableCollection<Attributes_Model> _list_CurrentAttributes;
        private ISISAttributeMode _mode;
        private Visibility _vis_NewSecurityGoals;
        #endregion

        /// <summary>
        /// Aktuelle Liste der Attribute, welche angezeigt und bearbeitet werden kann
        /// </summary>
        public ObservableCollection<Attributes_Model> List_CurrentAttributes
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
                if (_myAtt.Insert_Attribute(List_CurrentAttributes))
                {
                    Cleanup();
                    _myNavi.NavigateBack();
                    _myLock.Unlock_Object(Table_Lock_Flags.Attributes, 0);
                }
            });
        }

        #region Services
        private readonly INavigationService _myNavi;
        private readonly IDialogService _myDia;
        private readonly IDataService_Attribute _myAtt;
        private readonly IDataService_Setting _mySett;
        private readonly ILockService _myLock;

        #endregion

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="myDia"></param>
        /// <param name="myNavi"></param>
        /// <param name="myAtt"></param>
        public Attributes_ViewModel(IDialogService myDia, INavigationService myNavi, 
            IDataService_Attribute myAtt, ILockService myLock, IDataService_Setting mySett)
        {
            #region Services
            _myDia = myDia;
            _myLock = myLock;
            _mySett = mySett;
            _myNavi = myNavi;
            _myAtt = myAtt;
            #endregion
            //Messenger Registrierung für Bestimmung des Ansichtsmodus
            MessengerInstance.Register<NotificationMessage<ISISAttributeMode>>(this, message => 
            {
                if (!(message.Sender is INavigationService)) return;
                Mode = message.Content;
            });
            //Messenger Registrierung für Benachrichtigungen bei Fehlerhafter Eingabe
            MessengerInstance.Register<NotificationMessage<string>>(this, MessageToken.ISAttributValidationError, message =>
            {
                if (!(message.Sender is Attributes_Model)) return;
                _myDia.ShowWarning(message.Content);
            });

            #region Aktuelle Einstellungen abrufen
            Setting = _mySett.Get_List_Settings();
            Vis_NewSecurityGoals = (Setting.Neue_Schutzziele_aktiviert == "Ja") ? Visibility.Visible : Visibility.Collapsed;
            #endregion
            //Abrufen der Aktuellen Attributliste
            List_CurrentAttributes = GetAttributeList();
        }

        /// <summary>
        /// Liste der 8-10 Attribute (Abhängig van Einstellungen für Attribut 9/10) abrufen (Mapping von DB zu Model im Model Konstruktor inklusive)
        /// </summary>
        /// <returns></returns>
        private ObservableCollection<Attributes_Model> GetAttributeList()
        {
            ObservableCollection<Attributes_Model> result = new ObservableCollection<Attributes_Model>();
            //Die ersten 8 Attribute abrufen
            for (int i=1; i <= 8; i++)
            {
                Attributes_Model item = _myAtt.Get_Model_FromDB(i);
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
                Attributes_Model item = _myAtt.Get_Model_FromDB(9);
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
                Attributes_Model item = _myAtt.Get_Model_FromDB(10);
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
