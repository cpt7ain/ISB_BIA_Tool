using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using ISB_BIA_IMPORT1.Model;
using ISB_BIA_IMPORT1.LinqDataContext;
using ISB_BIA_IMPORT1.Services;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;

namespace ISB_BIA_IMPORT1.ViewModel
{
    /// <summary>
    /// VM zur Darstellung der Applikationsansicht
    /// </summary>
    public class Application_ViewModel : ViewModelBase
    {
        #region Backing-Fields
        private Application_Model _currentApplication;
        private string _rechenzentrum = "";
        private string _rechenzentrum_Text = "";
        private bool _rechenzentrum_Check;
        private string _server = "";
        private string _server_Text = "";
        private bool _server_Check;
        private string _virtuelle_Maschine = "";
        private string _virtuelle_Maschine_Text = "";
        private bool _virtuelle_Maschine_Check;
        private string _typ = "";
        private string _typ_Text = "";
        private bool _typ_Check;
        private string _iT_Betriebsart = "";
        private string _iT_Betriebsart_Text = "";
        private bool _iT_Betriebsart_Check;
        private ObservableCollection<string> _rechenzentrumList;
        private ObservableCollection<string> _serverList;
        private ObservableCollection<string> _virtuelle_MaschineList;
        private ObservableCollection<string> _typeList;
        private ObservableCollection<string> _iT_BetriebsartList;
        private ProcAppMode _mode;
        private int _currentTab;
        private MyRelayCommand _nextTab;
        private MyRelayCommand _prevTab;
        private Visibility _buttonSaveVisibility;
        private MyRelayCommand _exportApplicationHistory;
        private MyRelayCommand<string> _info;
        #endregion

        /// <summary>
        /// Aktuell geöffnete Applikation
        /// </summary>
        public Application_Model CurrentApplication
        {
            get => _currentApplication;
            set => Set(() => CurrentApplication, ref _currentApplication, value);
        }

        // Eigenschaften, welche gebraucht werden um Korrekte Übernahme der vom User gewollten Werte zu sichern,
        // da Werte an jeweils mehrere Controls gebunden sind (TextBox & Dropdown, gesteuert durch CheckBox)
        // Wenn Check-Property sich ändert, wird der jeweilige Wert aus Dropdown oder Textfeld übernommen
        #region Hilfs-Properties
        /// <summary>
        /// Rechenzentrum (DropDown)
        /// </summary>
        public string Rechenzentrum
        {
            get => _rechenzentrum;
            set
            {
                Set(() => Rechenzentrum, ref _rechenzentrum, value);
                if (Rechenzentrum_Check)
                    CurrentApplication.Rechenzentrum = Rechenzentrum_Text;
                else
                    CurrentApplication.Rechenzentrum = Rechenzentrum;
            }
        }
        /// <summary>
        /// Rechenzentrum (TextBox)
        /// </summary>
        public string Rechenzentrum_Text
        {
            get => _rechenzentrum_Text;
            set
            {
                Set(() => Rechenzentrum_Text, ref _rechenzentrum_Text, value);
                if (Rechenzentrum_Check)
                    CurrentApplication.Rechenzentrum = Rechenzentrum_Text;
                else
                    CurrentApplication.Rechenzentrum = Rechenzentrum;
            }
        }
        /// <summary>
        /// Rechenzentrum (CheckBox Switch))
        /// </summary>
        public bool Rechenzentrum_Check
        {
            get => _rechenzentrum_Check;
            set
            {
                Set(() => Rechenzentrum_Check, ref _rechenzentrum_Check, value);
                if (value)
                    CurrentApplication.Rechenzentrum = Rechenzentrum_Text;
                else
                    CurrentApplication.Rechenzentrum = Rechenzentrum;
            }
        }
        /// <summary>
        /// Server (DropDown)
        /// </summary>
        public string Server
        {
            get => _server;
            set
            {
                Set(() => Server, ref _server, value);
                if (Server_Check)
                    CurrentApplication.Rechenzentrum = _server_Text;
                else
                    CurrentApplication.Rechenzentrum = Server;
            }
        }
        /// <summary>
        /// Server (TextBox)
        /// </summary>
        public string Server_Text
        {
            get => _server_Text;
            set
            {
                Set(() => Server_Text, ref _server_Text, value);
                if (Server_Check)
                    CurrentApplication.Rechenzentrum = Server_Text;
                else
                    CurrentApplication.Rechenzentrum = Server;
            }
        }
        /// <summary>
        /// Server (CheckBox Switch))
        /// </summary>
        public bool Server_Check
        {
            get => _server_Check;
            set
            {
                Set(() => Server_Check, ref _server_Check, value);
                if (value)
                    CurrentApplication.Server = Rechenzentrum_Text;
                else
                    CurrentApplication.Server = Rechenzentrum;
            }
        }
        /// <summary>
        /// Virtuelle_Maschine (DropDown)
        /// </summary>
        public string Virtuelle_Maschine
        {
            get => _virtuelle_Maschine;
            set
            {
                Set(() => Virtuelle_Maschine, ref _virtuelle_Maschine, value);
                if (Virtuelle_Maschine_Check)
                    CurrentApplication.Virtuelle_Maschine = Virtuelle_Maschine_Text;
                else
                    CurrentApplication.Virtuelle_Maschine = Virtuelle_Maschine;
            }
        }
        /// <summary>
        /// Virtuelle_Maschine (TextBox)
        /// </summary>
        public string Virtuelle_Maschine_Text
        {
            get => _virtuelle_Maschine_Text;
            set
            {
                Set(() => Virtuelle_Maschine_Text, ref _virtuelle_Maschine_Text, value);
                if (Virtuelle_Maschine_Check)
                    CurrentApplication.Virtuelle_Maschine = Virtuelle_Maschine_Text;
                else
                    CurrentApplication.Virtuelle_Maschine = Virtuelle_Maschine;
            }
        }
        /// <summary>
        /// Virtuelle_Maschine (CheckBox Switch))
        /// </summary>
        public bool Virtuelle_Maschine_Check
        {
            get => _virtuelle_Maschine_Check;
            set
            {
                Set(() => Virtuelle_Maschine_Check, ref _virtuelle_Maschine_Check, value);
                if (value)
                    CurrentApplication.Virtuelle_Maschine = Virtuelle_Maschine_Text;
                else
                    CurrentApplication.Virtuelle_Maschine = Virtuelle_Maschine;
            }
        }
        /// <summary>
        /// Typ (DropDown)
        /// </summary>
        public string Typ
        {
            get => _typ;
            set
            {
                Set(() => Typ, ref _typ, value);
                if (Typ_Check)
                    CurrentApplication.Typ = Typ_Text;
                else
                    CurrentApplication.Typ = Typ;
            }
        }
        /// <summary>
        /// Typ (TextBox)
        /// </summary>
        public string Typ_Text
        {
            get => _typ_Text;
            set
            {
                Set(() => Typ_Text, ref _typ_Text, value);
                if (Typ_Check)
                    CurrentApplication.Typ = Typ_Text;
                else
                    CurrentApplication.Typ = Typ;
            }
        }
        /// <summary>
        /// Typ (CheckBox Switch))
        /// </summary>
        public bool Typ_Check
        {
            get => _typ_Check;
            set
            {
                Set(() => Typ_Check, ref _typ_Check, value);
                if (value)
                    CurrentApplication.Typ = Typ_Text;
                else
                    CurrentApplication.Typ = Typ;
            }
        }
        /// <summary>
        /// IT_Betriebsart (DropDown)
        /// </summary>
        public string IT_Betriebsart
        {
            get => _iT_Betriebsart;
            set
            {
                Set(() => IT_Betriebsart, ref _iT_Betriebsart, value);
                if (IT_Betriebsart_Check)
                    CurrentApplication.IT_Betriebsart = IT_Betriebsart_Text;
                else
                    CurrentApplication.IT_Betriebsart = IT_Betriebsart;
            }
        }
        /// <summary>
        /// IT_Betriebsart (TextBox)
        /// </summary>
        public string IT_Betriebsart_Text
        {
            get => _iT_Betriebsart_Text;
            set
            {
                Set(() => IT_Betriebsart_Text, ref _iT_Betriebsart_Text, value);
                if (IT_Betriebsart_Check)
                    CurrentApplication.IT_Betriebsart = IT_Betriebsart_Text;
                else
                    CurrentApplication.IT_Betriebsart = IT_Betriebsart;
            }
        }
        /// <summary>
        /// IT_Betriebsart (CheckBox Switch))
        /// </summary>
        public bool IT_Betriebsart_Check
        {
            get => _iT_Betriebsart_Check;
            set
            {
                Set(() => IT_Betriebsart_Check, ref _iT_Betriebsart_Check, value);
                if (CurrentApplication != null)
                {
                    if (value)
                        CurrentApplication.IT_Betriebsart = IT_Betriebsart_Text;
                    else
                        CurrentApplication.IT_Betriebsart = IT_Betriebsart;
                }
            }
        }
        #endregion

        #region Dropdown Listen
        /// <summary>
        /// Liste der Rechenzentren
        /// </summary>
        public ObservableCollection<string> RechenzentrumList
        {
            get => _rechenzentrumList;
            set => Set(() => RechenzentrumList, ref _rechenzentrumList, value);

        }
        /// <summary>
        /// Liste der Server
        /// </summary>
        public ObservableCollection<string> ServerList
        {
            get => _serverList;
            set => Set(() => ServerList, ref _serverList, value);

        }
        /// <summary>
        /// Liste der Virtuellen Maschinen
        /// </summary>
        public ObservableCollection<string> Virtuelle_MaschineList
        {
            get => _virtuelle_MaschineList;
            set => Set(() => Virtuelle_MaschineList, ref _virtuelle_MaschineList, value);

        }
        /// <summary>
        /// Liste der Typen
        /// </summary>
        public ObservableCollection<string> TypeList
        {
            get => _typeList;
            set => Set(() => TypeList, ref _typeList, value);
        }
        /// <summary>
        /// Liste der Kategorien
        /// </summary>
        public ObservableCollection<string> IT_BetriebsartList
        {
            get => _iT_BetriebsartList;
            set => Set(() => IT_BetriebsartList, ref _iT_BetriebsartList, value);

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
        public bool SchutzzielVisible { get; set; }
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
                ButtonSaveVisibility = (_currentTab == 3)? Visibility.Visible: Visibility.Hidden;               
            }
        }
        /// <summary>
        /// Command, um zum nächsten Tab zu navigieren
        /// </summary>
        public MyRelayCommand NextTab
        {
            get => _nextTab
                    ?? (_nextTab = new MyRelayCommand(() =>
                    {
                        CurrentTab += 1;
                    }, () => { return CurrentTab != 3; }));
        }
        /// <summary>
        /// Command, um zum vorherigen Tab zu navigieren
        /// </summary>
        public MyRelayCommand PrevTab
        {
            get => _prevTab
                    ?? (_prevTab = new MyRelayCommand(() =>
                    {
                        CurrentTab -= 1;
                    }, () => { return CurrentTab != 0; }));
        }
        /// <summary>
        /// Sichtbarkeit des Speicherbuttons (wenn letzter Tab aktiv)
        /// </summary>
        public Visibility ButtonSaveVisibility
        {
            get => _buttonSaveVisibility;
            set => Set(() => ButtonSaveVisibility, ref _buttonSaveVisibility, value);

        }
        #endregion

        #region Navigations Commands, welche Änderungen des Viewmodels ausführen (bei Abbruch der Aktion oder erfolgreichem Beenden)
        /// <summary>
        /// Zurück Navigieren (Vorgang abbbrechen)
        /// </summary>
        public MyRelayCommand NavBack
        {
            get => new MyRelayCommand(() =>
            {
                if (myDia.CancelDecision())
                {
                    Cleanup();
                    myNavi.NavigateBack(true);
                    myData.UnlockObject(Table_Lock_Flags.Application, CurrentApplication.Applikation_Id);
                }
            });
        }

        /// <summary>
        /// Speichern und zurückkeheren. 
        /// Wenn <see cref="ProcAppMode"/> == Edit, dann Nachricht an ApplicationViewVM zur Aktualisierung der Anwendungsliste
        /// </summary>
        public MyRelayCommand SaveAndContinue
        {
            get => new MyRelayCommand(() =>
            {
                if (myData.InsertApplication(CurrentApplication, Mode))
                {
                    bool refreshMsg = (Mode == ProcAppMode.Change) ? true : false;
                    Cleanup();
                    myNavi.NavigateBack(refreshMsg);
                    myData.UnlockObject(Table_Lock_Flags.Application, CurrentApplication.Applikation_Id);
                }
            }, () => { return CurrentTab == 3; });
        }
        #endregion

        /// <summary>
        /// Zeigt Definitionen für Schutzziele an
        /// </summary>
        public MyRelayCommand<string> Info
        {
            get => _info
                    ?? (_info = new MyRelayCommand<string>((name) =>
                    {
                        try
                        {
                            string file = myShared.InitialDirectory + @"\" + name + "_Info.xps";
                            if (File.Exists(file))
                            {
                                XpsDocument xpsDocument = new XpsDocument(file, FileAccess.Read);
                                FixedDocumentSequence fds = xpsDocument.GetFixedDocumentSequence();
                                myNavi.NavigateTo<DocumentView_ViewModel>();
                                Messenger.Default.Send<FixedDocumentSequence>(fds);
                            }
                            else
                            {
                                myDia.ShowInfo("Keine Beschreibung verfügbar.");
                            }
                        }
                        catch (Exception ex)
                        {
                            myDia.ShowError("Keine Beschreibung verfügbar.", ex);
                        }
                    }));
        }
        /// <summary>
        /// Exportiere Bearbeitungs-Historie der Anwendungen
        /// </summary>
        public MyRelayCommand ExportApplicationHistory
        {
            get => _exportApplicationHistory
                    ?? (_exportApplicationHistory = new MyRelayCommand(() =>
                    {
                        bool success = myExport.ExportApplications(myData.GetApplicationHistory(CurrentApplication.Applikation_Id), "", CurrentApplication.Applikation_Id.ToString());
                        if (success)
                        {
                            myDia.ShowInfo("Export erfolgreich");
                        }
                    }, () => Mode == ProcAppMode.Change));
        }
        /// <summary>
        /// Einstellungen (aus DB abgerufen)
        /// </summary>
        public ISB_BIA_Settings Setting { get; set; }

        #region Services
        IMyNavigationService myNavi;
        IMyDialogService myDia;
        IMyDataService myData;
        IMyExportService myExport;
        IMySharedResourceService myShared;
        #endregion

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="myDialogService"></param>
        /// <param name="myNavigationService"></param>
        /// <param name="myDataService"></param>
        /// <param name="myExportService"></param>
        /// <param name="mySharedResourceService"></param>
        public Application_ViewModel(IMyDialogService myDialogService, IMyNavigationService myNavigationService, IMyDataService myDataService, IMyExportService myExportService, IMySharedResourceService mySharedResourceService)
        {
            #region Services
            myNavi = myNavigationService;
            myDia = myDialogService;
            myData = myDataService;
            myExport = myExportService;
            myShared = mySharedResourceService;
            #endregion

            if (IsInDesignMode)
            {
                CurrentApplication = myData.GetApplicationModelFromDB(1);
                Header = "TestHeader";
                RechenzentrumList = myData.GetRechenzentrum();
                ServerList = myData.GetServer();
                Virtuelle_MaschineList = myData.GetVirtuelle_Maschine();
                TypeList = myData.GetTypes();
                IT_BetriebsartList = myData.GetBetriebsart();
                Setting = myData.GetSettings();
            }
            else
            {
                #region Messenger Registrierungen für 2 Modi (Anwendung bearbeiten, neue Anwendung anlegen)
                MessengerInstance.Register<int>(this, ProcAppMode.Change, p => {
                    Mode = ProcAppMode.Change;
                    CurrentApplication = myData.GetApplicationModelFromDB(p);
                    //Wenn Daten Fehlerhaft dann zurückkehren
                    if (CurrentApplication == null)
                    {
                        myDia.ShowError("Fehler beim Laden der Daten.");
                        Cleanup();
                        myNavi.NavigateBack();
                    }
                    else
                    {
                        //Setzen der Properties der Textfelder der Control-abhängigen Infos 
                        Rechenzentrum = CurrentApplication.Rechenzentrum;
                        Server = CurrentApplication.Server;
                        Virtuelle_Maschine = CurrentApplication.Virtuelle_Maschine;
                        Typ = CurrentApplication.Typ;
                        IT_Betriebsart = CurrentApplication.IT_Betriebsart;
                    }
                });
                MessengerInstance.Register<int>(this, ProcAppMode.New, p => {
                    Mode = ProcAppMode.New;
                    CurrentApplication = new Application_Model();
                });
                #endregion
                #region Abrufen der Listendaten für die Dropdown-Felder
                RechenzentrumList = myData.GetRechenzentrum();
                ServerList = myData.GetServer();
                Virtuelle_MaschineList = myData.GetVirtuelle_Maschine();
                TypeList = myData.GetTypes();
                IT_BetriebsartList = myData.GetBetriebsart();
                #endregion
                #region Aus Settings-Tabelle Abfragen ob neue Schutzziele (5+6) aktiviert sind (angezeigt werden)
                Setting = myData.GetSettings();
                SchutzzielVisible = (Setting.Neue_Schutzziele_aktiviert == "Ja") ? true : false;
                #endregion
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
