using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using ISB_BIA_IMPORT1.Model;
using ISB_BIA_IMPORT1.LinqDataContext;
using ISB_BIA_IMPORT1.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;

namespace ISB_BIA_IMPORT1.ViewModel
{
    /// <summary>
    /// VM zur Darstellung des Menüs
    /// </summary>
    public class Menu_ViewModel : ViewModelBase
    {
        #region Backing Fields
        private MyRelayCommand _newProcess;
        private MyRelayCommand<ProcAppListMode> _setProcessView;
        private MyRelayCommand _newApplication;
        private MyRelayCommand<ProcAppListMode> _setApplicationView;
        private MyRelayCommand _setSBAView;
        private MyRelayCommand _setInformationsSegmentView;
        private MyRelayCommand _setInformationsSegmentsAttributes;
        private MyRelayCommand _setOESettings;
        private MyRelayCommand _setSettings;
        private MyRelayCommand _setDataModel;
        private MyRelayCommand _setLog;
        private MyRelayCommand _removeLocks;
        private MyRelayCommand<string> _info;
        private Visibility _appMenuVisible;
        private Visibility _sBAMenuVisible;
        private Visibility _settingsMenuVisible;
        private Visibility _deltaBorderVisible;
        private Visibility _processBorderVisible;
        private DateTime _pickerDate;
        private MyRelayCommand _navToDeltaDate;
        private MyRelayCommand _navToLastDelta;
        private MyRelayCommand _exportProcessList;
        private int _myFontSize = 14;
        private int _processCount;
        private int _editProcessCount;
        private MyRelayCommand _changeTextSize;
        #endregion

        #region Nav-Commands
        /// <summary>
        /// Navigiert zum ProcessVM um einen neuen Prozess anzulegen
        /// </summary>
        public MyRelayCommand NewProcess
        {
            get => _newProcess
                  ?? (_newProcess = new MyRelayCommand(() =>
                  {
                      myNavi.NavigateTo<Process_ViewModel>(0, ProcAppMode.New);
                  }));
        }

        /// <summary>
        /// Navigiert zum ProcessViewVM um einen vorhandenen Prozess zu bearbeiten oder zu löschen
        /// </summary>
        public MyRelayCommand<ProcAppListMode> SetProcessView
        {
            get => _setProcessView
                  ?? (_setProcessView = new MyRelayCommand<ProcAppListMode>((m) =>
                  {
                      myNavi.NavigateTo<ProcessView_ViewModel>(m);
                  }));
        }

        /// <summary>
        /// Navigiert zum ApplicationVM um eine neue Anwendung anzulegen
        /// </summary>
        public MyRelayCommand NewApplication
        {
            get => _newApplication
                  ?? (_newApplication = new MyRelayCommand(() =>
                  {
                      myNavi.NavigateTo<Application_ViewModel>(0, ProcAppMode.New);
                  }));
        }

        /// <summary>
        /// Navigiert zum ApplicationViewVM um eine vorhandenen Anwendung zu bearbeiten oder zu löschen
        /// </summary>
        public MyRelayCommand<ProcAppListMode> SetApplicationView
        {
            get => _setApplicationView
                  ?? (_setApplicationView = new MyRelayCommand<ProcAppListMode>((mode) =>
                  {
                      myNavi.NavigateTo<ApplicationView_ViewModel>(mode);
                  }));
        }

        /// <summary>
        /// Navigiert zum ApplicationVM um eine vorhandenen Anwendung zu bearbeiten
        /// </summary>
        public MyRelayCommand SetSBAView
        {
            get => _setSBAView
                  ?? (_setSBAView = new MyRelayCommand(() =>
                  {
                      myNavi.NavigateTo<SBA_View_ViewModel>();
                  }));
        }

        /// <summary>
        /// Navigiert zum InformationSegmentViewVM um ein Informationssegment zu bearbeiten oder zu betrachten
        /// </summary>
        public MyRelayCommand SetInformationsSegmentView
        {
            get => _setInformationsSegmentView
                  ?? (_setInformationsSegmentView = new MyRelayCommand(() =>
                  {
                      ISISAttributeMode m;
                      if (User.UserGroup == UserGroups.CISO)
                          m = ISISAttributeMode.Edit;
                      else
                          m = ISISAttributeMode.View;
                      myNavi.NavigateTo<InformationSegmentsView_ViewModel>(m);
                  }));
        }

        /// <summary>
        /// Navigiert zum InformationSegmentsAttributesVM um Attribute zu bearbeiten oder zu betrachten
        /// Im Berbeitungsmodus wird die Liste für andere Bearbeitungen zusätzlich gesperrt
        /// </summary>
        public MyRelayCommand SetInformationsSegmentsAttributes
        {
            get => _setInformationsSegmentsAttributes
                  ?? (_setInformationsSegmentsAttributes = new MyRelayCommand(() =>
                  {
                      ISISAttributeMode mode;
                      if (User.UserGroup == UserGroups.CISO)
                      {
                          mode = ISISAttributeMode.Edit;
                          string user = myData.GetObjectLocked(Table_Lock_Flags.Attributes, 0);
                          if (user == "")
                          {
                              if (myData.LockObject(Table_Lock_Flags.Attributes, 0))
                                  myNavi.NavigateTo<InformationSegmentsAttributes_ViewModel>(mode);
                          }
                          else
                          {
                              myDia.ShowWarning("Die Attribute werden momentan durch einen anderen CISO bearbeitet und können daher nicht geöffnet werden.\n\nBelegender Benutzer: " + user + "\n\nSollte kein CISO die Attribut-Liste geöffnet haben, wenden Sie sich bitte IT.");
                          }
                      }
                      else
                      {
                          mode = ISISAttributeMode.View;
                          myNavi.NavigateTo<InformationSegmentsAttributes_ViewModel>(mode);
                      }
                  }));
        }

        /// <summary>
        /// Navigiert zum OE_AssignmentViewVM um OE's und OE-Gruppen zu verwalten und einander zuzuordnen, zu bearbeiten oder zu betrachten
        /// </summary>
        public MyRelayCommand SetOESettings
        {
            get => _setOESettings
                  ?? (_setOESettings = new MyRelayCommand(() =>
                  {
                      string user = myData.GetObjectLocked(Table_Lock_Flags.OEs, 0);
                      if (user == "")
                      {
                          if (myData.LockObject(Table_Lock_Flags.OEs, 0))
                              myNavi.NavigateTo<OE_AssignmentView_ViewModel>();
                      }
                      else
                      {
                          myDia.ShowWarning("Die OE-Einstellungen werden momentan durch einen anderen User bearbeitet und können daher nicht geöffnet werden.\n\nBelegender Benutzer: " + user);
                      }
                  }));
        }

        /// <summary>
        /// Navigiert zum SettingsVM um Einstellungen der Anwendung zu ändern
        /// </summary>
        public MyRelayCommand SetSettings
        {
            get => _setSettings
                  ?? (_setSettings = new MyRelayCommand(() =>
                  {
                      string user = myData.GetObjectLocked(Table_Lock_Flags.Settings, 0);
                      if (user == "")
                      {
                          if (myData.LockObject(Table_Lock_Flags.Settings, 0))
                              myNavi.NavigateTo<Settings_ViewModel>();
                      }
                      else
                      {
                          myDia.ShowWarning("Die Einstellungen werden momentan durch einen anderen User bearbeitet und können daher nicht geöffnet werden.\n\nBelegender Benutzer: " + user);
                      }
                  }));
        }

        /// <summary>
        /// Navigiert zum DataModelVM um das Datenmodell komplett neu zu erstellen (Funktion soll gesperrt werden)
        /// </summary>
        public MyRelayCommand SetDataModel
        {
            get => _setDataModel
                  ?? (_setDataModel = new MyRelayCommand(() =>
                  {
                      myNavi.NavigateTo<DataModel_ViewModel>();
                  }));
        }

        /// <summary>
        /// Navigiert zum LogVM um das Anwendungslog zu betrachten oder zu exportieren 
        /// </summary>
        public MyRelayCommand SetLog
        {
            get => _setLog
                  ?? (_setLog = new MyRelayCommand(() =>
                  {
                      myNavi.NavigateTo<LogView_ViewModel>();
                  }));
        }

        /// <summary>
        /// Command, um alle bestehenden Locks aus der DB zu entfernen (Im Fehlerfall, falls Locks nicht korrekt entfernt wurden)
        /// </summary>
        public MyRelayCommand RemoveLocks
        {
            get => _removeLocks
                  ?? (_removeLocks = new MyRelayCommand(() =>
                  {
                      if (myDia.ShowQuestion("Möchten Sie wirklich alle Datensatz-Locks entfernen?\nBitte stellen Sie sicher, dass kein User Datensätze geöfnnet hat, da es sonst zu einer gleichzeitigen Bearbeitung der Datensätze kommen kann.", "Locks entfernen"))
                      {
                          myData.UnlockAllObjects();
                      }
                  }));
        }

        /// <summary>
        /// Navigiert zum InfoVM diverse externe Hilfe/Infodateien innerhalb der Anwendung zu betrachten 
        /// </summary>
        public MyRelayCommand<string> Info
        {
            get => _info
                  ?? (_info = new MyRelayCommand<string>((name) =>
                  {
                      try
                      {
                          if(name == "Hilfe")
                          {
                              if (myShared.User.UserGroup == UserGroups.Normal_User) name = "ISB-BIA-Tool_Hilfe";
                              //else if (myShared.User.UserGroup == UserGroup.SBA) name = "ISB-BIA-Tool_Hilfe - SBA";
                              //else if (myShared.User.UserGroup == UserGroup.Admin) name = "ISB-BIA-Tool_Hilfe - Admin";
                              else name = "ISB-BIA-Tool_Hilfe - CISO";
                          }
                          string file = myShared.InitialDirectory + @"\" + name + ".xps";
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
        #endregion

        /// <summary>
        /// Sichtbarkeit des Anwedungsreiters im Menü
        /// </summary>
        public Visibility AppMenuVisible
        {
            get=> _appMenuVisible;
            set => Set(()=> AppMenuVisible, ref _appMenuVisible, value);
        }

        /// <summary>
        /// Sichtbarkeit des Schutzbedarfsanalysereiters im Menü
        /// </summary>
        public Visibility SBAMenuVisible
        {
            get => _sBAMenuVisible;
            set => Set(() => SBAMenuVisible, ref _sBAMenuVisible, value);
        }

        /// <summary>
        /// Sichtbarkeit des Einstellungsreiters im Menü
        /// </summary>
        public Visibility SettingsMenuVisible
        {
            get => _settingsMenuVisible;
            set => Set(() => SettingsMenuVisible, ref _settingsMenuVisible, value);
        }

        /// <summary>
        /// Sichtbarkeit des Deltaanalysebereichs
        /// </summary>
        public Visibility DeltaBorderVisible
        {
            get => _deltaBorderVisible;
            set => Set(() => DeltaBorderVisible, ref _deltaBorderVisible, value);
        }

        /// <summary>
        /// Sichtbarkeit des BIA-Prozessbereichs
        /// </summary>
        public Visibility ProcessBorderVisible
        {
            get => _processBorderVisible;
            set => Set(() => ProcessBorderVisible, ref _processBorderVisible, value);
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
        public MyRelayCommand NavToDeltaDate
        {
            get => _navToDeltaDate
                  ?? (_navToDeltaDate = new MyRelayCommand(() =>
                  {
                      ObservableCollection<ISB_BIA_Delta_Analyse> list = myData.InitiateDeltaAnalysis(PickerDate);
                      if (list != null)
                      {
                          myNavi.NavigateTo<DeltaAnalysis_ViewModel>();
                          MessengerInstance.Send<ObservableCollection<ISB_BIA_Delta_Analyse>>(list);
                      }
                  }));
        }

        /// <summary>
        /// Navigiert zum DeltaAnalysisVM um die letzte gespeicherte Deltaanalyse zu betrachten
        /// </summary>
        public MyRelayCommand NavToLastDelta
        {
            get => _navToLastDelta
                  ?? (_navToLastDelta = new MyRelayCommand(() =>
                  {
                      ObservableCollection<ISB_BIA_Delta_Analyse> list = myData.GetDeltaAnalysis();
                      if (list != null && list.Count > 0)
                      {
                          myNavi.NavigateTo<DeltaAnalysis_ViewModel>();
                          MessengerInstance.Send<ObservableCollection<ISB_BIA_Delta_Analyse>>(list);
                      }
                      else
                      {
                          myDia.ShowInfo("Keine Daten vorhanden.");
                      }
                  }));
        }

        /// <summary>
        /// Exportieren der Liste aller Prozesse nach Excel
        /// </summary>
        public MyRelayCommand ExportProcessList
        {
            get => _exportProcessList
                    ?? (_exportProcessList = new MyRelayCommand(() =>
                    {
                        bool success = myExport.AllActiveProcessesExport();
                        if (success)
                        {
                            myDia.ShowInfo("Export erfolgreich");
                        }
                    }));           
        }

        /// <summary>
        /// Ändern der allgemeinen Schriftgröße
        /// </summary>
        public MyRelayCommand ChangeTextSize
        {
            get => _changeTextSize
                    ?? (_changeTextSize = new MyRelayCommand(() =>
                    {
                        MyFontSize = (MyFontSize == 14) ? 18 : 14;
                        Messenger.Default.Send<int>(MyFontSize, MessageToken.ChangeTextSize);
                    }));
        }

        /// <summary>
        /// allgemeine Schriftgröße (für Menü)
        /// </summary>
        public int MyFontSize
        {
            get => _myFontSize;
            set => Set(() => MyFontSize, ref _myFontSize, value);
        }

        /// <summary>
        /// Anzahl aller Prozesse
        /// </summary>
        public int ProcessCount
        {
            get => _processCount;
            set => Set(() => ProcessCount, ref _processCount, value);
        }

        /// <summary>
        /// Anzahl aller bearbeiteten Prozesse
        /// </summary>
        public int EditProcessCount
        {
            get => _editProcessCount;
            set => Set(() => EditProcessCount, ref _editProcessCount, value);
        }

        /// <summary>
        /// Anweisung im MenüVM
        /// </summary>
        public string Instruction { get; set; }
        
        /// <summary>
        /// Angemeldeter User
        /// </summary>
        public Login_Model User
        {
            get => myShared.User;
        }

        #region Services
        IMyNavigationService myNavi;
        IMyDialogService myDia;
        IMyExportService myExport;
        IMyDataService myData;
        IMySharedResourceService myShared;
        #endregion

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="myDialogService"></param>
        /// <param name="myNavigationService"></param>
        /// <param name="myExportService"></param>
        /// <param name="myDataService"></param>
        /// <param name="mySharedResourceService"></param>
        public Menu_ViewModel(IMyDialogService myDialogService, IMyNavigationService myNavigationService, IMyExportService myExportService, IMyDataService myDataService, IMySharedResourceService mySharedResourceService)
        {
            #region Services
            myDia = myDialogService;
            myNavi = myNavigationService;
            myExport = myExportService;
            myData = myDataService;
            myShared = mySharedResourceService;
            #endregion

            PickerDate = DateTime.Now;

            #region Prozessanzahl berechnen (komplett, bearbeitet)
            //Datenbankabfragen nicht ausführen wenn ConstructionMode (da Datenmodell evtl nicht auf neustem Stand)
            if (!myShared.ConstructionMode)
            {
                ObservableCollection<ISB_BIA_Prozesse> processes = myData.GetActiveProcesses();
                ProcessCount = processes.Count;
                EditProcessCount = processes.Where(x => x.Datum.Year == DateTime.Now.Year).ToList().Count;
            }
            #endregion

            //Sichtbarkeiten und Anweisungen für den jeweiligen Usermodus definieren
            switch (myShared.User.UserGroup)
            {
                case UserGroups.CISO:
                case UserGroups.Admin:
                    AppMenuVisible = Visibility.Visible;
                    SBAMenuVisible = Visibility.Visible;
                    SettingsMenuVisible = Visibility.Visible;
                    ProcessBorderVisible = Visibility.Visible;
                    DeltaBorderVisible = Visibility.Visible;
                    Instruction = "Bitte starten Sie im Sinne der Business Impact Analysis mit Ihrer Bearbeitung (Neuanlage, Löschen) von Prozessen über den Menüpunkt 'Prozesse' in der Menüleiste oben.\nBitte füllen Sie mindestens alle Felder mit fettgedrucktem Feldnamen aus, da diese als Pflichtfelder definiert sind.";
                    break;
                case UserGroups.SBA_User:
                    AppMenuVisible = Visibility.Visible;
                    SBAMenuVisible = Visibility.Visible;
                    SettingsMenuVisible = Visibility.Collapsed;
                    ProcessBorderVisible = Visibility.Visible;
                    DeltaBorderVisible = Visibility.Visible;
                    Instruction = "Bitte starten Sie im Sinne der Schutzbedarfsanalyse mit Ihrer Bearbeitung (Neuanlage, Löschen) von Anwendungen über den Menüpunkt 'Schutzbedarfsanalyse' in der Menüleiste oben.\nBitte füllen Sie mindestens alle Felder mit fettgedrucktem Feldnamen aus, da diese als Pflichtfelder definiert sind.";
                    break;
                case UserGroups.Normal_User:
                    AppMenuVisible = Visibility.Collapsed;
                    SBAMenuVisible = Visibility.Collapsed;
                    SettingsMenuVisible = Visibility.Collapsed;
                    ProcessBorderVisible = Visibility.Collapsed;
                    DeltaBorderVisible = Visibility.Collapsed;
                    Instruction = "Bitte starten Sie im Sinne der Business Impact Analysis mit Ihrer Bearbeitung (Neuanlage, Löschen) von Prozessen über den Menüpunkt 'Prozesse' in der Menüleiste oben.\nBitte füllen Sie mindestens alle Felder mit fettgedrucktem Feldnamen aus, da diese als Pflichtfelder definiert sind.";
                    break;
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
