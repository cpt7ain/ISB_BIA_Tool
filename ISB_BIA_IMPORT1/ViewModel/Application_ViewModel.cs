using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using ISB_BIA_IMPORT1.Model;
using ISB_BIA_IMPORT1.LINQ2SQL;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;
using ISB_BIA_IMPORT1.Helpers;
using ISB_BIA_IMPORT1.Services.Interfaces;

namespace ISB_BIA_IMPORT1.ViewModel
{
    /// <summary>
    /// VM zur Darstellung der Applikationsansicht
    /// </summary>
    public class Application_ViewModel : ViewModelBase
    {
        #region Backing-Fields
        private Application_Model _applicationCurrent;
        private Application_Model _oldCurrentApplication;

        private string _app_Rechenzentrum = "";
        private string _app_RechenzentrumText = "";
        private bool _app_RechenzentrumCheck;
        private string _app_Server = "";
        private string _app_Server_Text = "";
        private bool _app_Server_Check;
        private string _app_Virtuelle_Maschine = "";
        private string _app_Virtuelle_Maschine_Text = "";
        private bool _app_Virtuelle_Maschine_Check;
        private string _app_Typ = "";
        private string _app_Typ_Text = "";
        private bool _app_Typ_Check;
        private string _app_IT_Betriebsart = "";
        private string _app_IT_Betriebsart_Text = "";
        private bool _app_IT_Betriebsart_Check;
        private ObservableCollection<string> _list_Rechenzentrum;
        private ObservableCollection<string> _list_Server;
        private ObservableCollection<string> _list_Virtuelle_Maschine;
        private ObservableCollection<string> _list_Typ;
        private ObservableCollection<string> _list_IT_Betriebsart;
        private ProcAppMode _mode;
        private int _currentTab;
        private MyRelayCommand _cmd_NextTab;
        private MyRelayCommand _cmd_PrevTab;
        private Visibility _vis_SaveButton;
        private MyRelayCommand _cmd_ExportApplicationHistory;
        private MyRelayCommand<string> _cmd_Info;
        private MyRelayCommand _cmd_ResetValues;
        #endregion

        /// <summary>
        /// Aktuell geöffnete Applikation
        /// </summary>
        public Application_Model ApplicationCurrent
        {
            get => _applicationCurrent;
            set => Set(() => ApplicationCurrent, ref _applicationCurrent, value);
        }

        // Eigenschaften, welche gebraucht werden um Korrekte Übernahme der vom User gewollten Werte zu sichern,
        // da Werte an jeweils mehrere Controls gebunden sind (TextBox & Dropdown, gesteuert durch CheckBox)
        // Wenn Check-Property sich ändert, wird der jeweilige Wert aus Dropdown oder Textfeld übernommen
        #region Hilfs-Properties
        /// <summary>
        /// Rechenzentrum (DropDown)
        /// </summary>
        public string App_Rechenzentrum
        {
            get => _app_Rechenzentrum;
            set
            {
                Set(() => App_Rechenzentrum, ref _app_Rechenzentrum, value);
                if (App_Rechenzentrum_Check)
                    ApplicationCurrent.Rechenzentrum = App_Rechenzentrum_Text;
                else
                    ApplicationCurrent.Rechenzentrum = App_Rechenzentrum;
            }
        }
        /// <summary>
        /// Rechenzentrum (TextBox)
        /// </summary>
        public string App_Rechenzentrum_Text
        {
            get => _app_RechenzentrumText;
            set
            {
                Set(() => App_Rechenzentrum_Text, ref _app_RechenzentrumText, value);
                if (App_Rechenzentrum_Check)
                    ApplicationCurrent.Rechenzentrum = App_Rechenzentrum_Text;
                else
                    ApplicationCurrent.Rechenzentrum = App_Rechenzentrum;
            }
        }
        /// <summary>
        /// Rechenzentrum (CheckBox Switch))
        /// </summary>
        public bool App_Rechenzentrum_Check
        {
            get => _app_RechenzentrumCheck;
            set
            {
                Set(() => App_Rechenzentrum_Check, ref _app_RechenzentrumCheck, value);
                if (value)
                    ApplicationCurrent.Rechenzentrum = App_Rechenzentrum_Text;
                else
                    ApplicationCurrent.Rechenzentrum = App_Rechenzentrum;
            }
        }
        /// <summary>
        /// Server (DropDown)
        /// </summary>
        public string App_Server
        {
            get => _app_Server;
            set
            {
                Set(() => App_Server, ref _app_Server, value);
                if (App_Server_Check)
                    ApplicationCurrent.Rechenzentrum = _app_Server_Text;
                else
                    ApplicationCurrent.Rechenzentrum = App_Server;
            }
        }
        /// <summary>
        /// Server (TextBox)
        /// </summary>
        public string App_Server_Text
        {
            get => _app_Server_Text;
            set
            {
                Set(() => App_Server_Text, ref _app_Server_Text, value);
                if (App_Server_Check)
                    ApplicationCurrent.Rechenzentrum = App_Server_Text;
                else
                    ApplicationCurrent.Rechenzentrum = App_Server;
            }
        }
        /// <summary>
        /// Server (CheckBox Switch))
        /// </summary>
        public bool App_Server_Check
        {
            get => _app_Server_Check;
            set
            {
                Set(() => App_Server_Check, ref _app_Server_Check, value);
                if (value)
                    ApplicationCurrent.Server = App_Rechenzentrum_Text;
                else
                    ApplicationCurrent.Server = App_Rechenzentrum;
            }
        }
        /// <summary>
        /// Virtuelle_Maschine (DropDown)
        /// </summary>
        public string App_Virtuelle_Maschine
        {
            get => _app_Virtuelle_Maschine;
            set
            {
                Set(() => App_Virtuelle_Maschine, ref _app_Virtuelle_Maschine, value);
                if (App_Virtuelle_Maschine_Check)
                    ApplicationCurrent.Virtuelle_Maschine = App_Virtuelle_Maschine_Text;
                else
                    ApplicationCurrent.Virtuelle_Maschine = App_Virtuelle_Maschine;
            }
        }
        /// <summary>
        /// Virtuelle_Maschine (TextBox)
        /// </summary>
        public string App_Virtuelle_Maschine_Text
        {
            get => _app_Virtuelle_Maschine_Text;
            set
            {
                Set(() => App_Virtuelle_Maschine_Text, ref _app_Virtuelle_Maschine_Text, value);
                if (App_Virtuelle_Maschine_Check)
                    ApplicationCurrent.Virtuelle_Maschine = App_Virtuelle_Maschine_Text;
                else
                    ApplicationCurrent.Virtuelle_Maschine = App_Virtuelle_Maschine;
            }
        }
        /// <summary>
        /// Virtuelle_Maschine (CheckBox Switch))
        /// </summary>
        public bool App_Virtuelle_Maschine_Check
        {
            get => _app_Virtuelle_Maschine_Check;
            set
            {
                Set(() => App_Virtuelle_Maschine_Check, ref _app_Virtuelle_Maschine_Check, value);
                if (value)
                    ApplicationCurrent.Virtuelle_Maschine = App_Virtuelle_Maschine_Text;
                else
                    ApplicationCurrent.Virtuelle_Maschine = App_Virtuelle_Maschine;
            }
        }
        /// <summary>
        /// Typ (DropDown)
        /// </summary>
        public string App_Typ
        {
            get => _app_Typ;
            set
            {
                Set(() => App_Typ, ref _app_Typ, value);
                if (App_Typ_Check)
                    ApplicationCurrent.Typ = App_Typ_Text;
                else
                    ApplicationCurrent.Typ = App_Typ;
            }
        }
        /// <summary>
        /// Typ (TextBox)
        /// </summary>
        public string App_Typ_Text
        {
            get => _app_Typ_Text;
            set
            {
                Set(() => App_Typ_Text, ref _app_Typ_Text, value);
                if (App_Typ_Check)
                    ApplicationCurrent.Typ = App_Typ_Text;
                else
                    ApplicationCurrent.Typ = App_Typ;
            }
        }
        /// <summary>
        /// Typ (CheckBox Switch))
        /// </summary>
        public bool App_Typ_Check
        {
            get => _app_Typ_Check;
            set
            {
                Set(() => App_Typ_Check, ref _app_Typ_Check, value);
                if (value)
                    ApplicationCurrent.Typ = App_Typ_Text;
                else
                    ApplicationCurrent.Typ = App_Typ;
            }
        }
        /// <summary>
        /// IT_Betriebsart (DropDown)
        /// </summary>
        public string App_IT_Betriebsart
        {
            get => _app_IT_Betriebsart;
            set
            {
                Set(() => App_IT_Betriebsart, ref _app_IT_Betriebsart, value);
                if (App_IT_Betriebsart_Check)
                    ApplicationCurrent.IT_Betriebsart = App_IT_Betriebsart_Text;
                else
                    ApplicationCurrent.IT_Betriebsart = App_IT_Betriebsart;
            }
        }
        /// <summary>
        /// IT_Betriebsart (TextBox)
        /// </summary>
        public string App_IT_Betriebsart_Text
        {
            get => _app_IT_Betriebsart_Text;
            set
            {
                Set(() => App_IT_Betriebsart_Text, ref _app_IT_Betriebsart_Text, value);
                if (App_IT_Betriebsart_Check)
                    ApplicationCurrent.IT_Betriebsart = App_IT_Betriebsart_Text;
                else
                    ApplicationCurrent.IT_Betriebsart = App_IT_Betriebsart;
            }
        }
        /// <summary>
        /// IT_Betriebsart (CheckBox Switch))
        /// </summary>
        public bool App_IT_Betriebsart_Check
        {
            get => _app_IT_Betriebsart_Check;
            set
            {
                Set(() => App_IT_Betriebsart_Check, ref _app_IT_Betriebsart_Check, value);
                if (ApplicationCurrent != null)
                {
                    if (value)
                        ApplicationCurrent.IT_Betriebsart = App_IT_Betriebsart_Text;
                    else
                        ApplicationCurrent.IT_Betriebsart = App_IT_Betriebsart;
                }
            }
        }
        #endregion

        #region Dropdown Listen
        /// <summary>
        /// Liste der Rechenzentren
        /// </summary>
        public ObservableCollection<string> List_Rechenzentrum
        {
            get => _list_Rechenzentrum;
            set => Set(() => List_Rechenzentrum, ref _list_Rechenzentrum, value);

        }
        /// <summary>
        /// Liste der Server
        /// </summary>
        public ObservableCollection<string> List_Server
        {
            get => _list_Server;
            set => Set(() => List_Server, ref _list_Server, value);

        }
        /// <summary>
        /// Liste der Virtuellen Maschinen
        /// </summary>
        public ObservableCollection<string> List_Virtuelle_Maschine
        {
            get => _list_Virtuelle_Maschine;
            set => Set(() => List_Virtuelle_Maschine, ref _list_Virtuelle_Maschine, value);

        }
        /// <summary>
        /// Liste der Typen
        /// </summary>
        public ObservableCollection<string> List_Typ
        {
            get => _list_Typ;
            set => Set(() => List_Typ, ref _list_Typ, value);
        }
        /// <summary>
        /// Liste der Kategorien
        /// </summary>
        public ObservableCollection<string> List_IT_Betriebsart
        {
            get => _list_IT_Betriebsart;
            set => Set(() => List_IT_Betriebsart, ref _list_IT_Betriebsart, value);

        }
        #endregion

        #region Sonstige Properties
        /// <summary>
        /// Bestimmt Modus, in dem sich der Anwender befindet
        /// und das als nächstes aktivierte Viewmodel, sowie den Header
        /// </summary>
        public ProcAppMode Mode
        {
            get => _mode;
            set
            {
                Set(() => Mode, ref _mode, value);
                CurrentTab = 0;
                if (value == ProcAppMode.Change)
                {
                    Header = "Anwendung bearbeiten";
                }
                else if (value == ProcAppMode.New)
                {
                    Header = "Neue Anwendung anlegen";
                }
            }
        }
        /// <summary>
        /// Überschrift des Fensters
        /// </summary>
        public string Header { get; set; }
        /// <summary>
        /// Bestimmt, ob die neuen Schutzziele angezeigt werden oder nicht
        /// </summary>
        public bool Vis_NewSecurityGoals { get; set; }
        #endregion

        #region Tabcontrol-Navigations-Properties (für lokale Navigation zwischen den Tabs in der Anwendungsansicht)
        /// <summary>
        /// Aktiver Tab
        /// </summary>
        public int CurrentTab
        {
            get => _currentTab;
            set
            {
                Set(()=> CurrentTab, ref _currentTab, value);
                Vis_SaveButton = (_currentTab == 3)? Visibility.Visible: Visibility.Hidden;               
            }
        }
        /// <summary>
        /// Command, um zum nächsten Tab zu navigieren
        /// </summary>
        public MyRelayCommand Cmd_NextTab
        {
            get => _cmd_NextTab
                    ?? (_cmd_NextTab = new MyRelayCommand(() =>
                    {
                        CurrentTab += 1;
                    }, () => { return CurrentTab != 3; }));
        }
        /// <summary>
        /// Command, um zum vorherigen Tab zu navigieren
        /// </summary>
        public MyRelayCommand Cmd_PrevTab
        {
            get => _cmd_PrevTab
                    ?? (_cmd_PrevTab = new MyRelayCommand(() =>
                    {
                        CurrentTab -= 1;
                    }, () => { return CurrentTab != 0; }));
        }
        /// <summary>
        /// Sichtbarkeit des Speicherbuttons (wenn letzter Tab aktiv)
        /// </summary>
        public Visibility Vis_SaveButton
        {
            get => _vis_SaveButton;
            set => Set(() => Vis_SaveButton, ref _vis_SaveButton, value);

        }
        #endregion

        #region Navigations Commands, welche Änderungen des Viewmodels ausführen (bei Abbruch der Aktion oder erfolgreichem Beenden)
        /// <summary>
        /// Zurück Navigieren (Vorgang abbbrechen)
        /// </summary>
        public MyRelayCommand Cmd_NavBack
        {
            get => new MyRelayCommand(() =>
            {
                if (_myDia.CancelDecision())
                {
                    Cleanup();
                    _myNavi.NavigateBack(true);
                    _myLock.Unlock_Object(Table_Lock_Flags.Application, ApplicationCurrent.Applikation_Id);
                }
            });
        }

        /// <summary>
        /// Speichern und zurückkeheren. 
        /// Wenn <see cref="ProcAppMode"/> == Edit, dann Nachricht an ApplicationViewVM zur Aktualisierung der Anwendungsliste
        /// </summary>
        public MyRelayCommand Cmd_SaveAndContinue
        {
            get => new MyRelayCommand(() =>
            {
                if (_myApp.Insert_Application(ApplicationCurrent, Mode))
                {
                    bool refreshMsg = (Mode == ProcAppMode.Change);
                    Cleanup();
                    _myNavi.NavigateBack(refreshMsg);
                    _myLock.Unlock_Object(Table_Lock_Flags.Application, ApplicationCurrent.Applikation_Id);
                }
            }, () => { return CurrentTab == 3; });
        }
        #endregion

        /// <summary>
        /// Zeigt Definitionen für Schutzziele an
        /// </summary>
        public MyRelayCommand<string> Cmd_Info
        {
            get => _cmd_Info
                    ?? (_cmd_Info = new MyRelayCommand<string>((name) =>
                    {
                        try
                        {
                            string file = _myShared.Dir_InitialDirectory + @"\" + name + "_Info.xps";
                            if (File.Exists(file))
                            {
                                XpsDocument xpsDocument = new XpsDocument(file, FileAccess.Read);
                                FixedDocumentSequence fds = xpsDocument.GetFixedDocumentSequence();
                                _myNavi.NavigateTo<DocumentView_ViewModel>();
                                MessengerInstance.Send(new NotificationMessage<FixedDocumentSequence>(this,fds,null));
                            }
                            else
                            {
                                _myDia.ShowInfo("Keine Beschreibung verfügbar.");
                            }
                        }
                        catch (Exception ex)
                        {
                            _myDia.ShowError("Keine Beschreibung verfügbar.", ex);
                        }
                    }));
        }
        /// <summary>
        /// Exportiere Bearbeitungs-Historie der Anwendungen
        /// </summary>
        public MyRelayCommand Cmd_ExportApplicationHistory
        {
            get => _cmd_ExportApplicationHistory
                    ?? (_cmd_ExportApplicationHistory = new MyRelayCommand(() =>
                    {
                        _myExp.Export_Applications(_myApp.Get_History_Application(ApplicationCurrent.Applikation_Id), "", ApplicationCurrent.Applikation_Id);
                    }, () => Mode == ProcAppMode.Change));
        }
        /// <summary>
        /// Zurücksetzen der Daten der aktuellen Maske (Nur bei Anwendungsbearbeitung, nicht Neuanlage)
        /// </summary>
        public MyRelayCommand Cmd_ResetValues
        {
            get => _cmd_ResetValues
                   ?? (_cmd_ResetValues = new MyRelayCommand(() =>
                   {
                       switch (CurrentTab)
                       {
                           case 0:
                               ApplicationCurrent.IT_Anwendung_System = _oldCurrentApplication.IT_Anwendung_System;
                               App_IT_Betriebsart = _oldCurrentApplication.IT_Betriebsart;
                               App_IT_Betriebsart_Text = "";
                               ApplicationCurrent.Wichtiges_Anwendungssystem = _oldCurrentApplication.Wichtiges_Anwendungssystem;
                               break;
                           case 1:
                               App_Rechenzentrum = _oldCurrentApplication.Rechenzentrum;
                               App_Rechenzentrum_Text = "";
                               App_Server = _oldCurrentApplication.Server;
                               App_Server_Text = "";
                               App_Virtuelle_Maschine = _oldCurrentApplication.Virtuelle_Maschine;
                               App_Virtuelle_Maschine_Text = "";
                               App_Typ = _oldCurrentApplication.Typ;
                               App_Typ_Text = "";
                               break;
                           case 2:
                               ApplicationCurrent.SZ_1 = _oldCurrentApplication.SZ_1;
                               ApplicationCurrent.SZ_2 = _oldCurrentApplication.SZ_2;
                               ApplicationCurrent.SZ_3 = _oldCurrentApplication.SZ_3;
                               ApplicationCurrent.SZ_4 = _oldCurrentApplication.SZ_4;
                               ApplicationCurrent.SZ_5 = _oldCurrentApplication.SZ_5;
                               ApplicationCurrent.SZ_6 = _oldCurrentApplication.SZ_6;
                               break;
                       }
                   }));
        }
        /// <summary>
        /// Einstellungen (aus DB abgerufen)
        /// </summary>
        public ISB_BIA_Settings Setting { get; set; }

        #region Services
        private readonly INavigationService _myNavi;
        private readonly IDialogService _myDia;
        private readonly IDataService_Application _myApp;
        private readonly ILockService _myLock;
        private readonly IDataService_Setting _mySett;
        private readonly IExportService _myExp;
        private readonly ISharedResourceService _myShared;
        #endregion

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="myDia"></param>
        /// <param name="myNavi"></param>
        /// <param name="myApp"></param>
        /// <param name="myExp"></param>
        /// <param name="myShared"></param>
        public Application_ViewModel(IDialogService myDia, INavigationService myNavi, 
            IDataService_Application myApp, IExportService myExp, 
            ISharedResourceService myShared, ILockService myLock, IDataService_Setting mySett)
        {
            #region Services
            _myNavi = myNavi;
            _myDia = myDia;
            _myApp = myApp;
            _mySett = mySett;
            _myLock = myLock;
            _myExp = myExp;
            _myShared = myShared;
            #endregion

            if (IsInDesignMode)
            {
                ApplicationCurrent = _myApp.Get_Model_FromDB(1);
                Header = "TestHeader";
                List_Rechenzentrum = _myApp.Get_StringList_Rechenzentrum();
                List_Server = _myApp.Get_StringList_Server();
                List_Virtuelle_Maschine = _myApp.Get_StringList_Virtuelle_Maschine();
                List_Typ = _myApp.Get_StringList_Types();
                List_IT_Betriebsart = _myApp.Get_StringList_Betriebsart();
                Setting = _mySett.Get_List_Settings();
            }
            else
            {
                #region Messenger Registrierungen für 2 Modi (Anwendung bearbeiten, neue Anwendung anlegen)
                //NotificationMessage<int> a = new NotificationMessage<int>(4,"");
                MessengerInstance.Register<NotificationMessage<int>>(this, ProcAppMode.Change, message => {
                    if (!(message.Sender is INavigationService)) return;
                    Mode = ProcAppMode.Change;
                    ApplicationCurrent = _myApp.Get_Model_FromDB(message.Content);
                    //Wenn Daten Fehlerhaft dann zurückkehren
                    if (ApplicationCurrent == null)
                    {
                        _myDia.ShowError("Fehler beim Laden der Daten.");
                        Cleanup();
                        _myNavi.NavigateBack();
                    }
                    else
                    {
                        _oldCurrentApplication = ApplicationCurrent.Copy();
                        //Setzen der Properties der Textfelder der Control-abhängigen Infos 
                        App_Rechenzentrum = ApplicationCurrent.Rechenzentrum;
                        App_Server = ApplicationCurrent.Server;
                        App_Virtuelle_Maschine = ApplicationCurrent.Virtuelle_Maschine;
                        App_Typ = ApplicationCurrent.Typ;
                        App_IT_Betriebsart = ApplicationCurrent.IT_Betriebsart;
                    }
                });
                MessengerInstance.Register<NotificationMessage<int>>(this, ProcAppMode.New, message => {
                    if (!(message.Sender is INavigationService)) return;
                    Mode = ProcAppMode.New;
                    ApplicationCurrent = new Application_Model();
                    _oldCurrentApplication = new Application_Model();
                });
                #endregion
                #region Abrufen der Listendaten für die Dropdown-Felder
                List_Rechenzentrum = _myApp.Get_StringList_Rechenzentrum();
                List_Server = _myApp.Get_StringList_Server();
                List_Virtuelle_Maschine = _myApp.Get_StringList_Virtuelle_Maschine();
                List_Typ = _myApp.Get_StringList_Types();
                List_IT_Betriebsart = _myApp.Get_StringList_Betriebsart();
                #endregion
                #region Aus Settings-Tabelle Abfragen ob neue Schutzziele (5+6) aktiviert sind (angezeigt werden)
                Setting = _mySett.Get_List_Settings();
                Vis_NewSecurityGoals = (Setting.Neue_Schutzziele_aktiviert == "Ja");
                #endregion
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
