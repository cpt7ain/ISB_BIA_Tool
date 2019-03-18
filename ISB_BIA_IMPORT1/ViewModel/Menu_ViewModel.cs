using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using ISB_BIA_IMPORT1.Model;
using ISB_BIA_IMPORT1.LINQ2SQL;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;
using ISB_BIA_IMPORT1.Helpers;
using ISB_BIA_IMPORT1.Services.Interfaces;

namespace ISB_BIA_IMPORT1.ViewModel
{
    /// <summary>
    /// VM zur Darstellung des Menüs
    /// </summary>
    public class Menu_ViewModel : ViewModelBase
    {
        #region Backing Fields
        private MyRelayCommand _cmd_NavToNewProcess;
        private MyRelayCommand<ProcAppListMode> _cmd_NavToProcessView;
        private MyRelayCommand _cmd_NavToNewApplication;
        private MyRelayCommand<ProcAppListMode> _cmd_NavToApplicationView;
        private MyRelayCommand _cmd_NavToSBAView;
        private MyRelayCommand _cmd_NavToInformationSegmentView;
        private MyRelayCommand _cmd_NavToAttributesView;
        private MyRelayCommand _cmd_NavToOESettings;
        private MyRelayCommand _cmd_NavToSettings;
        private MyRelayCommand _cmd_NavToDataModelSetting;
        private MyRelayCommand _cmd_NavToLogView;
        private MyRelayCommand _cmd_RemoveLocks;
        private MyRelayCommand<string> _cmd_NavToInfoView;
        private Visibility _vis_AppMenu;
        private Visibility _vis_SBaMenu;
        private Visibility _vis_SettingsMenu;
        private Visibility _vis_DeltaBorder;
        private Visibility _vis_ProcessBorder;
        private DateTime _pickerDate;
        private MyRelayCommand _cmd_NavToDeltaDate;
        private MyRelayCommand _cmd_NavToLastDelta;
        private MyRelayCommand _cmd_ExportProcessList;
        private int _globalFontSize = 14;
        private int _count_AllProcesses;
        private int _count_EditProcesses;
        private MyRelayCommand _cmd_ChangeTextSize;
        #endregion

        #region Nav-Commands
        /// <summary>
        /// Navigiert zum ProcessVM um einen neuen Prozess anzulegen
        /// </summary>
        public MyRelayCommand Cmd_NavToNewProcess
        {
            get => _cmd_NavToNewProcess
                  ?? (_cmd_NavToNewProcess = new MyRelayCommand(() =>
                  {
                      _myNavi.NavigateTo<Process_ViewModel>(0, ProcAppMode.New);
                  }));
        }

        /// <summary>
        /// Navigiert zum ProcessViewVM um einen vorhandenen Prozess zu bearbeiten oder zu löschen
        /// </summary>
        public MyRelayCommand<ProcAppListMode> Cmd_NavToProcessView
        {
            get => _cmd_NavToProcessView
                  ?? (_cmd_NavToProcessView = new MyRelayCommand<ProcAppListMode>((m) =>
                  {
                      _myNavi.NavigateTo<ProcessView_ViewModel>(m);
                  }));
        }

        /// <summary>
        /// Navigiert zum ApplicationVM um eine neue Anwendung anzulegen
        /// </summary>
        public MyRelayCommand Cmd_NavToNewApplication
        {
            get => _cmd_NavToNewApplication
                  ?? (_cmd_NavToNewApplication = new MyRelayCommand(() =>
                  {
                      _myNavi.NavigateTo<Application_ViewModel>(0, ProcAppMode.New);
                  }));
        }

        /// <summary>
        /// Navigiert zum ApplicationViewVM um eine vorhandenen Anwendung zu bearbeiten oder zu löschen
        /// </summary>
        public MyRelayCommand<ProcAppListMode> Cmd_NavToApplicationView
        {
            get => _cmd_NavToApplicationView
                  ?? (_cmd_NavToApplicationView = new MyRelayCommand<ProcAppListMode>((mode) =>
                  {
                      _myNavi.NavigateTo<ApplicationView_ViewModel>(mode);
                  }));
        }

        /// <summary>
        /// Navigiert zum ApplicationVM um eine vorhandenen Anwendung zu bearbeiten
        /// </summary>
        public MyRelayCommand Cmd_NavToSBAView
        {
            get => _cmd_NavToSBAView
                  ?? (_cmd_NavToSBAView = new MyRelayCommand(() =>
                  {
                      _myNavi.NavigateTo<SBA_View_ViewModel>();
                  }));
        }

        /// <summary>
        /// Navigiert zum InformationSegmentViewVM um ein Informationssegment zu bearbeiten oder zu betrachten
        /// </summary>
        public MyRelayCommand Cmd_NavToInformationSegmentView
        {
            get => _cmd_NavToInformationSegmentView
                  ?? (_cmd_NavToInformationSegmentView = new MyRelayCommand(() =>
                  {
                      ISISAttributeMode m;
                      if (User.UserGroup == UserGroups.CISO)
                          m = ISISAttributeMode.Edit;
                      else
                          m = ISISAttributeMode.View;
                      _myNavi.NavigateTo<SegmentsView_ViewModel>(m);
                  }));
        }

        /// <summary>
        /// Navigiert zum InformationSegmentsAttributesVM um Attribute zu bearbeiten oder zu betrachten
        /// Im Berbeitungsmodus wird die Liste für andere Bearbeitungen zusätzlich gesperrt
        /// </summary>
        public MyRelayCommand Cmd_NavToAttributesView
        {
            get => _cmd_NavToAttributesView
                  ?? (_cmd_NavToAttributesView = new MyRelayCommand(() =>
                  {
                      ISISAttributeMode mode;
                      if (User.UserGroup == UserGroups.CISO)
                      {
                          mode = ISISAttributeMode.Edit;
                          string user = _myLock.Get_ObjectIsLocked(Table_Lock_Flags.Attributes, 0);
                          if (user == "")
                          {
                              if (_myLock.Lock_Object(Table_Lock_Flags.Attributes, 0))
                                  _myNavi.NavigateTo<Attributes_ViewModel>(mode);
                          }
                          else
                          {
                              _myDia.ShowWarning("Die Attribute werden momentan durch einen anderen CISO bearbeitet und können daher nicht geöffnet werden.\n\nBelegender Benutzer: " + user + "\n\nSollte kein CISO die Attribut-Liste geöffnet haben, wenden Sie sich bitte IT.");
                          }
                      }
                      else
                      {
                          mode = ISISAttributeMode.View;
                          _myNavi.NavigateTo<Attributes_ViewModel>(mode);
                      }
                  }));
        }

        /// <summary>
        /// Navigiert zum OE_AssignmentViewVM um OE's und OE-Gruppen zu verwalten und einander zuzuordnen, zu bearbeiten oder zu betrachten
        /// </summary>
        public MyRelayCommand Cmd_NavToOESettings
        {
            get => _cmd_NavToOESettings
                  ?? (_cmd_NavToOESettings = new MyRelayCommand(() =>
                  {
                      string user = _myLock.Get_ObjectIsLocked(Table_Lock_Flags.OEs, 0);
                      if (user == "")
                      {
                          if (_myLock.Lock_Object(Table_Lock_Flags.OEs, 0))
                              _myNavi.NavigateTo<OE_ViewModel>();
                      }
                      else
                      {
                          _myDia.ShowWarning("Die OE-Einstellungen werden momentan durch einen anderen User bearbeitet und können daher nicht geöffnet werden.\n\nBelegender Benutzer: " + user);
                      }
                  }));
        }

        /// <summary>
        /// Navigiert zum SettingsVM um Einstellungen der Anwendung zu ändern
        /// </summary>
        public MyRelayCommand Cmd_NavToSettings
        {
            get => _cmd_NavToSettings
                  ?? (_cmd_NavToSettings = new MyRelayCommand(() =>
                  {
                      string user = _myLock.Get_ObjectIsLocked(Table_Lock_Flags.Settings, 0);
                      if (user == "")
                      {
                          if (_myLock.Lock_Object(Table_Lock_Flags.Settings, 0))
                              _myNavi.NavigateTo<Settings_ViewModel>();
                      }
                      else
                      {
                          _myDia.ShowWarning("Die Einstellungen werden momentan durch einen anderen User bearbeitet und können daher nicht geöffnet werden.\n\nBelegender Benutzer: " + user);
                      }
                  }));
        }

        /// <summary>
        /// Navigiert zum DataModelVM um das Datenmodell komplett neu zu erstellen (Funktion soll gesperrt werden)
        /// </summary>
        public MyRelayCommand Cmd_NavToDataModelSetting
        {
            get => _cmd_NavToDataModelSetting
                  ?? (_cmd_NavToDataModelSetting = new MyRelayCommand(() =>
                  {
                      _myNavi.NavigateTo<DataModel_ViewModel>();
                  }));
        }

        /// <summary>
        /// Navigiert zum LogVM um das Anwendungslog zu betrachten oder zu exportieren 
        /// </summary>
        public MyRelayCommand Cmd_NavToLogView
        {
            get => _cmd_NavToLogView
                  ?? (_cmd_NavToLogView = new MyRelayCommand(() =>
                  {
                      _myNavi.NavigateTo<LogView_ViewModel>();
                  }));
        }

        /// <summary>
        /// Command, um alle bestehenden Locks aus der DB zu entfernen (Im Fehlerfall, falls Locks nicht korrekt entfernt wurden)
        /// </summary>
        public MyRelayCommand Cmd_RemoveLocks
        {
            get => _cmd_RemoveLocks
                  ?? (_cmd_RemoveLocks = new MyRelayCommand(() =>
                  {
                      if (_myDia.ShowQuestion("Möchten Sie wirklich alle Datensatz-Locks entfernen?\nBitte stellen Sie sicher, dass kein User Datensätze geöfnnet hat, da es sonst zu einer gleichzeitigen Bearbeitung der Datensätze kommen kann.", "Locks entfernen"))
                      {
                          _myLock.Unlock_AllObjects();
                      }
                  }));
        }

        /// <summary>
        /// Navigiert zum InfoVM diverse externe Hilfe/Infodateien innerhalb der Anwendung zu betrachten 
        /// </summary>
        public MyRelayCommand<string> Cmd_NavToInfoView
        {
            get => _cmd_NavToInfoView
                  ?? (_cmd_NavToInfoView = new MyRelayCommand<string>((name) =>
                  {
                      try
                      {
                          if(name == "Hilfe")
                          {
                              if (_myShared.User.UserGroup == UserGroups.Normal_User) name = "ISB-BIA-Tool_Hilfe";
                              //else if (myShared.User.UserGroup == UserGroup.SBA) name = "ISB-BIA-Tool_Hilfe - SBA";
                              //else if (myShared.User.UserGroup == UserGroup.Admin) name = "ISB-BIA-Tool_Hilfe - Admin";
                              else name = "ISB-BIA-Tool_Hilfe - CISO";
                          }
                          string file1 = _myShared.Dir_InitialDirectory + @"\" + name + ".xps";
                          string file2 = _myShared.Dir_InitialDirectory + @"\" + name + ".pdf";
                          if (File.Exists(file1))
                          {
                              XpsDocument xpsDocument = new XpsDocument(file1, FileAccess.Read);
                              FixedDocumentSequence fds = xpsDocument.GetFixedDocumentSequence();
                              _myNavi.NavigateTo<DocumentView_ViewModel>();
                              //XPS Document an Viewmodel senden +pdf Pfad für externes Öffnen
                              MessengerInstance.Send(new NotificationMessage<FixedDocumentSequence>(this,fds, file2));
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
        #endregion

        /// <summary>
        /// Sichtbarkeit des Anwedungsreiters im Menü
        /// </summary>
        public Visibility Vis_AppMenu
        {
            get=> _vis_AppMenu;
            set => Set(()=> Vis_AppMenu, ref _vis_AppMenu, value);
        }

        /// <summary>
        /// Sichtbarkeit des Schutzbedarfsanalysereiters im Menü
        /// </summary>
        public Visibility Vis_SBAMenu
        {
            get => _vis_SBaMenu;
            set => Set(() => Vis_SBAMenu, ref _vis_SBaMenu, value);
        }

        /// <summary>
        /// Sichtbarkeit des Einstellungsreiters im Menü
        /// </summary>
        public Visibility Vis_SettingsMenu
        {
            get => _vis_SettingsMenu;
            set => Set(() => Vis_SettingsMenu, ref _vis_SettingsMenu, value);
        }

        /// <summary>
        /// Sichtbarkeit des Deltaanalysebereichs
        /// </summary>
        public Visibility Vis_DeltaBorder
        {
            get => _vis_DeltaBorder;
            set => Set(() => Vis_DeltaBorder, ref _vis_DeltaBorder, value);
        }

        /// <summary>
        /// Sichtbarkeit des BIA-Prozessbereichs
        /// </summary>
        public Visibility Vis_ProcessBorder
        {
            get => _vis_ProcessBorder;
            set => Set(() => Vis_ProcessBorder, ref _vis_ProcessBorder, value);
        }

        /// <summary>
        /// Datum des Datepickers für die Deltaanalyse
        /// </summary>
        public DateTime PickerDate
        {
            get => _pickerDate;
            set => Set(()=> PickerDate, ref _pickerDate, value);
        }

        /// <summary>
        /// Navigiert zum DeltaAnalysisVM um eine Deltaanalyse für das gegebene Datum zu erstellen und zu betrachten
        /// </summary>
        public MyRelayCommand Cmd_NavToDeltaDate
        {
            get => _cmd_NavToDeltaDate
                  ?? (_cmd_NavToDeltaDate = new MyRelayCommand(() =>
                  {
                      ObservableCollection<ISB_BIA_Delta_Analyse> list = _myDelta.Create_DeltaAnalysis(PickerDate);
                      if (list != null)
                      {
                          _myNavi.NavigateTo<DeltaAnalysis_ViewModel>();
                          MessengerInstance.Send(new NotificationMessage<ObservableCollection<ISB_BIA_Delta_Analyse>>(this,list,null));
                      }
                  }));
        }

        /// <summary>
        /// Navigiert zum DeltaAnalysisVM um die letzte gespeicherte Deltaanalyse zu betrachten
        /// </summary>
        public MyRelayCommand Cmd_NavToLastDelta
        {
            get => _cmd_NavToLastDelta
                  ?? (_cmd_NavToLastDelta = new MyRelayCommand(() =>
                  {
                      ObservableCollection<ISB_BIA_Delta_Analyse> list = _myDelta.Get_List_Delta();
                      if (list != null && list.Count > 0)
                      {
                          _myNavi.NavigateTo<DeltaAnalysis_ViewModel>();
                          MessengerInstance.Send(new NotificationMessage<ObservableCollection<ISB_BIA_Delta_Analyse>>(this, list, null));
                      }
                      else
                      {
                          _myDia.ShowInfo("Keine Daten vorhanden.");
                      }
                  }));
        }

        /// <summary>
        /// Exportieren der Liste aller Prozesse nach Excel
        /// </summary>
        public MyRelayCommand Cmd_ExportProcessList
        {
            get => _cmd_ExportProcessList
                    ?? (_cmd_ExportProcessList = new MyRelayCommand(() =>
                    {
                        bool success = _myExport.Export_Processes_Active();
                        if (success)
                        {
                            _myDia.ShowInfo("Export erfolgreich");
                        }
                    }));           
        }

        /// <summary>
        /// Ändern der allgemeinen Schriftgröße
        /// </summary>
        public MyRelayCommand Cmd_ChangeTextSize
        {
            get => _cmd_ChangeTextSize
                    ?? (_cmd_ChangeTextSize = new MyRelayCommand(() =>
                    {
                        GlobalFontSize = (GlobalFontSize == 14) ? 18 : 14;
                        MessengerInstance.Send(new NotificationMessage<int>(this,GlobalFontSize,null), MessageToken.ChangeTextSize);
                    }));
        }

        /// <summary>
        /// allgemeine Schriftgröße (für Menü)
        /// </summary>
        public int GlobalFontSize
        {
            get => _globalFontSize;
            set => Set(() => GlobalFontSize, ref _globalFontSize, value);
        }

        /// <summary>
        /// Anzahl aller Prozesse
        /// </summary>
        public int Count_AllProcesses
        {
            get => _count_AllProcesses;
            set => Set(() => Count_AllProcesses, ref _count_AllProcesses, value);
        }

        /// <summary>
        /// Anzahl aller bearbeiteten Prozesse
        /// </summary>
        public int Count_EditProcesses
        {
            get => _count_EditProcesses;
            set => Set(() => Count_EditProcesses, ref _count_EditProcesses, value);
        }

        /// <summary>
        /// Anweisung im MenüVM
        /// </summary>
        public string[] Instructions { get; set; }
        
        /// <summary>
        /// Angemeldeter User
        /// </summary>
        public Login_Model User
        {
            get => _myShared.User;
        }

        #region Services
        private readonly INavigationService _myNavi;
        private readonly IDialogService _myDia;
        private readonly IExportService _myExport;
        private readonly IDataService_Process _myProc;
        private readonly IDataService_Delta _myDelta;
        private readonly ISharedResourceService _myShared;
        private readonly ILockService _myLock;
        #endregion

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="myDialogService"></param>
        /// <param name="myNavi"></param>
        /// <param name="myExp"></param>
        /// <param name="myProc"></param>
        /// <param name="myShared"></param>
        public Menu_ViewModel(IDialogService myDialogService, INavigationService myNavi, IExportService myExp, 
            IDataService_Process myProc, IDataService_Delta myDelta, ISharedResourceService myShared, 
            ILockService myLock)
        {
            #region Services
            _myDia = myDialogService;
            _myNavi = myNavi;
            _myExport = myExp;
            _myDelta = myDelta;
            _myLock = myLock;
            _myProc = myProc;
            _myShared = myShared;
            #endregion

            PickerDate = DateTime.Now;

            #region Prozessanzahl berechnen (komplett, bearbeitet)
            //Datenbankabfragen nicht ausführen wenn ConstructionMode (da Datenmodell evtl nicht auf neustem Stand)
            if (!_myShared.Conf_ConstructionMode)
            {
                ObservableCollection<ISB_BIA_Prozesse> processes = _myProc.Get_List_Processes_Active();
                Count_AllProcesses = processes.Count;
                Count_EditProcesses = processes.Where(x => x.Datum.Year == DateTime.Now.Year).ToList().Count;
            }
            #endregion

            //Sichtbarkeiten und Anweisungen für den jeweiligen Usermodus definieren
            Instructions = new string[3];
            Vis_AppMenu = Visibility.Visible;
            Vis_SBAMenu = Visibility.Visible;
            Vis_SettingsMenu = Visibility.Visible;
            Vis_ProcessBorder = Visibility.Visible;
            Vis_DeltaBorder = Visibility.Visible;
            Instructions[1] = "Informationssegmente anzeigen";
            Instructions[2] = "Informationssegment-Attribute anzeigen";
            switch (_myShared.User.UserGroup)
            {
                case UserGroups.CISO:
                    Instructions[0] = "Bitte starten Sie im Sinne der Business Impact Analysis mit Ihrer Bearbeitung (Neuanlage, Löschen) von Prozessen über den Menüpunkt 'Prozesse' in der Menüleiste oben.\nBitte füllen Sie mindestens alle Felder mit fettgedrucktem Feldnamen aus, da diese als Pflichtfelder definiert sind.";
                    Instructions[1] = "Informationssegmente anzeigen und bearbeiten";
                    Instructions[2] = "Informationssegment-Attribute anzeigen und bearbeiten";
                    break;
                case UserGroups.Admin:
                    Instructions[0] = "Bitte starten Sie im Sinne der Business Impact Analysis mit Ihrer Bearbeitung (Neuanlage, Löschen) von Prozessen über den Menüpunkt 'Prozesse' in der Menüleiste oben.\nBitte füllen Sie mindestens alle Felder mit fettgedrucktem Feldnamen aus, da diese als Pflichtfelder definiert sind.";
                    break;
                case UserGroups.SBA_User:
                    Vis_SettingsMenu = Visibility.Collapsed;
                    Instructions[0] = "Bitte starten Sie im Sinne der Schutzbedarfsanalyse mit Ihrer Bearbeitung (Neuanlage, Löschen) von Anwendungen über den Menüpunkt 'Schutzbedarfsanalyse' in der Menüleiste oben.\nBitte füllen Sie mindestens alle Felder mit fettgedrucktem Feldnamen aus, da diese als Pflichtfelder definiert sind.";
                    break;
                case UserGroups.Normal_User:
                    Vis_AppMenu = Visibility.Collapsed;
                    Vis_SBAMenu = Visibility.Collapsed;
                    Vis_SettingsMenu = Visibility.Collapsed;
                    Vis_ProcessBorder = Visibility.Collapsed;
                    Vis_DeltaBorder = Visibility.Collapsed;
                    Instructions[0] = "Bitte starten Sie im Sinne der Business Impact Analysis mit Ihrer Bearbeitung (Neuanlage, Löschen) von Prozessen über den Menüpunkt 'Prozesse' in der Menüleiste oben.\nBitte füllen Sie mindestens alle Felder mit fettgedrucktem Feldnamen aus, da diese als Pflichtfelder definiert sind.";
                    break;
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
