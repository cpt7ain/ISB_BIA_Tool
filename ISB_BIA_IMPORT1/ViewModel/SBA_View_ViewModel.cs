using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using ISB_BIA_IMPORT1.LinqDataContext;
using ISB_BIA_IMPORT1.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ISB_BIA_IMPORT1.ViewModel
{
    /// <summary>
    /// VM zur Darstellung der Schutzbedarfsanalyse-Anwendungsübersicht
    /// </summary>
    public class SBA_View_ViewModel : ViewModelBase
    {
        #region Backing-Fields
        private MyRelayCommand _navToApp;
        private ObservableCollection<ISB_BIA_Applikationen> _selectedApplications;
        private MyRelayCommand<object> _saveSelectedApplications;
        private MyRelayCommand _exportProcessList;
        private MyRelayCommand _exportApplicationList;
        private ObservableCollection<ISB_BIA_Applikationen> _applicationList;
        private object _selectedItem;
        private int _nonEditCount;
        private int _editCount;
        private int _processCount;
        private int _editProcessCount;

        #endregion

        #region Navigations-Commands die Viewmodel ändern und ggf. Daten daran versenden
        /// <summary>
        /// Command zum Zurückkehren zum vorherigen VM
        /// </summary>
        public MyRelayCommand NavBack
        {
            get
            {
                return new MyRelayCommand(() =>
                {
                    Cleanup();
                    myNavi.NavigateBack();
                });
            }
        }
        /// <summary>
        /// Zu ausgewählter Applikation <see cref="SelectedItem"/> navigieren falls diese nicht gesperrt ist, und für andere User sperren
        /// </summary>
        public MyRelayCommand NavToApp
        {
            get
            {
                return _navToApp
                    ?? (_navToApp = new MyRelayCommand(() =>
                    {
                        if (SelectedItem == null) return;
                        ISB_BIA_Applikationen ApplicationToChange = (ISB_BIA_Applikationen)SelectedItem;
                        string user = myData.GetObjectLocked(Table_Lock_Flags.Application, ApplicationToChange.Applikation_Id);
                        if (user == "")
                        {
                            if (myData.LockObject(Table_Lock_Flags.Application, ApplicationToChange.Applikation_Id))
                                myNavi.NavigateTo<Application_ViewModel>(ApplicationToChange.Applikation_Id, ProcAppMode.Change);
                        }
                        else
                        {
                            myDia.ShowWarning("Diese Anwendung wird momentan durch einen anderen User bearbeitet und kann daher nicht geöffnet werden.\n\nBelegender Benutzer: " + user + "\n\nSollte der User die Anwendung nicht geöffnet haben, wenden Sie sich bitte an die IT.");
                        }
                    }));
            }
        }
        #endregion

        /// <summary>
        /// Liste der ausgewählten Anwendungen für das Gruppenspeichern <see cref="SaveAllApplications(ObservableCollection{ISB_BIA_Applikationen})"/>
        /// </summary>
        public ObservableCollection<ISB_BIA_Applikationen> SelectedApplications
        {
            get => _selectedApplications;
            set => Set(() => SelectedApplications, ref _selectedApplications, value);
        }
        /// <summary>
        /// Command für die Gruppenspeicherung. <see cref="SaveAllApplications(ObservableCollection{ISB_BIA_Applikationen})"/>
        /// </summary>
        public MyRelayCommand<object> SaveSelectedApplications
        {
            get => _saveSelectedApplications
                        ?? (_saveSelectedApplications = new MyRelayCommand<object>((list) =>
                        {
                            System.Collections.IList items = (System.Collections.IList)list;
                            if(items.Count > 0)
                            {
                                var collection = items.Cast<ISB_BIA_Applikationen>();
                                SelectedApplications = new ObservableCollection<ISB_BIA_Applikationen>(collection);
                                List<ISB_BIA_Applikationen> lockedList = new List<ISB_BIA_Applikationen>();
                                string al = "";
                                foreach (ISB_BIA_Applikationen a in SelectedApplications)
                                {
                                    string user = myData.GetObjectLocked(Table_Lock_Flags.Application, a.Applikation_Id);
                                    if (user != "")
                                    {
                                        lockedList.Add(a);
                                        al = al + a.IT_Anwendung_System + " (geöffnet von: " + user + ")\n";
                                    }
                                }
                                if (lockedList.Count == 0)
                                {
                                    if (myData.SaveAllApplications(SelectedApplications))
                                    {
                                        Refresh();
                                    }
                                }
                                else
                                {
                                    string msg = "In der Auswahl befinden sich Anwendungen, die momentan durch andere User geöffnet sind und deshalb nicht gespeichert werden können.\nBitte warten Sie, bis die Bearbeitung beendet ist oder deselektieren Sie betroffene Prozesse.\n\n";
                                    myDia.ShowWarning(msg + al);
                                }
                            }
                            else
                            {
                                myDia.ShowMessage("Bitte wählen Sie Anwendungen aus der Übersicht aus, die Sie ohne Änderungen aktualisieren möchten.");
                            }
                        }, (list) => Setting.Multi_Save == "Ja"));
        }
        /// <summary>
        /// Exportieren der Liste der angezeigten Prozesse nach Excel
        /// </summary>
        public MyRelayCommand ExportProcessList
        {
            get
            {
                return _exportProcessList
                    ?? (_exportProcessList = new MyRelayCommand(() =>
                    {
                        bool success = myExport.AllActiveProcessesExport();
                        if (success)
                        {
                            myDia.ShowInfo("Export erfolgreich");
                        }
                    }));
            }
        }
        /// <summary>
        /// Exportieren der Liste aller aktiven Applikationen nach Excel
        /// </summary>
        public MyRelayCommand ExportApplicationList
        {
            get
            {
                return _exportApplicationList
                    ?? (_exportApplicationList = new MyRelayCommand(() =>
                    {
                        if (myExport.AllActiveApplicationsExport())
                        {
                            myDia.ShowInfo("Export erfolgreich");
                        }
                    }));
            }
        }
        /// <summary>
        /// Liste der angezeigten Applikationen
        /// </summary>
        public ObservableCollection<ISB_BIA_Applikationen> ApplicationList
        {
            get => _applicationList;
            set => Set(() => ApplicationList, ref _applicationList, value);

        }
        /// <summary>
        /// Ausgewähltes Objekt
        /// </summary>
        public object SelectedItem
        {
            get => _selectedItem;
            set => Set(() => SelectedItem, ref _selectedItem, value);

        }
        /// <summary>
        /// Anzahl nicht bearbeiteter Anwendungen
        /// </summary>
        public int NonEditCount
        {
            get => _nonEditCount;
            set => Set(() => NonEditCount, ref _nonEditCount, value);

        }
        /// <summary>
        /// Anzahl bearbeiteter Anwendungen
        /// </summary>
        public int EditCount
        {
            get => _editCount;
            set => Set(() => EditCount, ref _editCount, value);

        }
        /// <summary>
        /// Anzahl aller angezeigten Anwendungen
        /// </summary>
        public int AllCount
        {
            get => _nonEditCount + _editCount;
        }
        /// <summary>
        /// Anzahl aller Prozesse der BIA
        /// </summary>
        public int ProcessCount
        {
            get => _processCount;
            set => Set(()=>ProcessCount, ref _processCount, value);

        }
        /// <summary>
        /// Anzahl bearbeiteter Prozesse der BIA
        /// </summary>
        public int EditProcessCount
        {
            get => _editProcessCount;
            set => Set(() => EditProcessCount, ref _editProcessCount, value);
        }
        /// <summary>
        /// Anweisung
        /// </summary>
        public string Instruction
        {
            get => "Doppelklick auf eine Applikation, den Sie bearbeiten möchten.";
        }
        /// <summary>
        /// Überschrift
        /// </summary>
        public string Header
        {
            get => "Anwendungen bearbeiten";
        }
        /// <summary>
        /// Aktuelle Einstellungen
        /// </summary>
        public ISB_BIA_Settings Setting { get; set; }

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
        public SBA_View_ViewModel(IMyDialogService myDialogService, IMyNavigationService myNavigationService, IMyExportService myExportService, IMyDataService myDataService, IMySharedResourceService mySharedResourceService)
        {
            myNavi = myNavigationService;
            myDia = myDialogService;
            myExport = myExportService;
            myData = myDataService;
            myShared = mySharedResourceService;
            Messenger.Default.Register<string>(this, MessageToken.RefreshData, s => { Refresh(); });
            Setting = myData.GetSettings();
            Refresh();
        }

        /// <summary>
        /// Aktualisieren der Daten
        /// </summary>
        public void Refresh()
        {
            ApplicationList = myData.GetActiveApplications();
            if (ApplicationList == null)
            {
                Cleanup();
                myNavi.NavigateBack();
                myDia.ShowError("Es konnten keine Applikationen abgerufen werden.");
            }
            EditCount = ApplicationList.Where(x => x.Datum.Year == DateTime.Now.Year).ToList().Count;
            NonEditCount = ApplicationList.Where(x => x.Datum.Year != DateTime.Now.Year).ToList().Count;
            ObservableCollection<ISB_BIA_Prozesse> processes = myData.GetActiveProcesses();
            ProcessCount = processes.Count;
            EditProcessCount = processes.Where(x => x.Datum.Year == DateTime.Now.Year).ToList().Count;
            SelectedApplications = new ObservableCollection<ISB_BIA_Applikationen>();
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
