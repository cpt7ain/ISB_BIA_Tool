using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using ISB_BIA_IMPORT1.Model;
using ISB_BIA_IMPORT1.LinqDataContext;
using ISB_BIA_IMPORT1.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace ISB_BIA_IMPORT1.ViewModel
{
    /// <summary>
    /// VM zur Darstellung der Prozessübersicht
    /// </summary>
    public class ProcessView_ViewModel : ViewModelBase
    {
        #region Backing-Fields
        private MyRelayCommand _navToProcess;
        private MyRelayCommand _deleteProc;
        private ObservableCollection<ISB_BIA_Prozesse> _selectedProcesses;
        private MyRelayCommand<object> _saveSelectedProcesses;
        private MyRelayCommand _exportProcessList;
        private ObservableCollection<ISB_BIA_Prozesse> _processList;
        private int _nonEditCount;
        private int _editCount;
        private object _selectedItem;
        private ProcAppListMode _processViewMode;
        private ISB_BIA_Settings _setting;

        #endregion

        /// <summary>
        /// Command zum Zurückkehren zum vorherigen VM
        /// </summary>
        public MyRelayCommand NavBack
        {
            get => new MyRelayCommand(() =>
                {
                    Cleanup();
                    myNavi.NavigateBack();
                });
        }
        /// <summary>
        /// Zu ausgewähltem Prozess <see cref="SelectedItem"/> navigieren falls dieser nicht gesperrt ist, und für andere User sperren
        /// </summary>
        public MyRelayCommand NavToProcess
        {
            get => _navToProcess
                    ?? (_navToProcess = new MyRelayCommand(() =>
                    {
                        if (SelectedItem == null) return;
                        ISB_BIA_Prozesse ProcessToChange = (ISB_BIA_Prozesse)SelectedItem;
                        string isLockedBy = myData.GetObjectLocked(Table_Lock_Flags.Process, ProcessToChange.Prozess_Id);
                        if (isLockedBy == "")
                        {
                            if(myData.LockObject(Table_Lock_Flags.Process, ProcessToChange.Prozess_Id))
                            myNavi.NavigateTo<Process_ViewModel>(ProcessToChange.Prozess_Id, ProcAppMode.Change);
                        }
                        else
                        {
                            myDia.ShowWarning("Dieser Prozess wird momentan durch einen anderen User bearbeitet und kann daher nicht geöffnet werden.\n\nBelegender Benutzer: "+isLockedBy+"\n\nSollte der User den Prozess nicht geöffnet haben, wenden Sie sich bitte an die IT.");
                        }
                    }, () => ProcessViewMode == ProcAppListMode.Change));
        }
        /// <summary>
        /// Prozess löschen, falls nicht bereits gelöscht (inaktiv)
        /// </summary>
        public MyRelayCommand DeleteProc
        {
            get => _deleteProc
                    ?? (_deleteProc = new MyRelayCommand(() =>
                    {
                        if (SelectedItem == null) return;
                        ISB_BIA_Prozesse ProcessToDelete = (ISB_BIA_Prozesse)SelectedItem;
                        string isLockedBy = myData.GetObjectLocked(Table_Lock_Flags.Process, ProcessToDelete.Prozess_Id);
                        if (isLockedBy == "")
                        {
                            if (ProcessToDelete.Aktiv != 0)
                            {
                                ISB_BIA_Prozesse NewProcessToDelete = myData.DeleteProcess(ProcessToDelete);
                                if (NewProcessToDelete != null)
                                {
                                    Refresh();
                                }
                            }
                            else
                            {
                                myDia.ShowWarning("Dieser Prozess wurde bereits gelöscht.");
                            }
                        }
                        else
                        {
                            myDia.ShowWarning("Dieser Prozess wird momentan durch einen anderen User bearbeitet und kann daher nicht gelöscht werden.\n\nBelegender Benutzer: " + isLockedBy + "\n\nSollte der User den Prozess nicht geöffnet haben, wenden Sie sich bitte an die IT.");
                        }

                    }, () => ProcessViewMode == ProcAppListMode.Delete));
        }
        /// <summary>
        /// Command welches je nach Modus <see cref="ProcessViewMode"/> definiert wird
        /// </summary>
        public MyRelayCommand RowDoubleClick { get; set; }
        /// <summary>
        /// Liste der ausgewählten Prozesse für das Gruppenspeichern <see cref="SaveAllProcesses(ObservableCollection{ISB_BIA_Prozesse})"/>
        /// </summary>
        public ObservableCollection<ISB_BIA_Prozesse> SelectedProcesses
        {
            get => _selectedProcesses;
            set => Set(()=>SelectedProcesses, ref _selectedProcesses, value);
        }
        /// <summary>
        /// Command für die Gruppenspeicherung. <see cref="SaveAllProcesses(ObservableCollection{ISB_BIA_Prozesse})"/>
        /// </summary>
        public MyRelayCommand<object> SaveSelectedProcesses
        {
            get
            {
                if (!IsInDesignMode)
                {
                     return _saveSelectedProcesses
                        ?? (_saveSelectedProcesses = new MyRelayCommand<object>((list) =>
                        {
                            System.Collections.IList items = (System.Collections.IList)list;
                            if(items.Count > 0)
                            {
                                var collection = items.Cast<ISB_BIA_Prozesse>();
                                SelectedProcesses = new ObservableCollection<ISB_BIA_Prozesse>(collection);
                                List<ISB_BIA_Prozesse> lockedList = new List<ISB_BIA_Prozesse>();
                                string pl = "";
                                foreach (ISB_BIA_Prozesse p in SelectedProcesses)
                                {
                                    string user = myData.GetObjectLocked(Table_Lock_Flags.Process, p.Prozess_Id);
                                    if (user != "")
                                    {
                                        lockedList.Add(p);
                                        pl = pl + p.Prozess + " (geöffnet von: " + user + ")\n";
                                    }
                                }
                                if (lockedList.Count == 0)
                                {
                                    if (myData.SaveAllProcesses(SelectedProcesses))
                                    {
                                        Refresh();
                                    }
                                }
                                else
                                {
                                    string msg = "In der Auswahl befinden sich Prozesse, die momentan durch andere User geöffnet sind und deshalb nicht gespeichert werden können.\nBitte warten Sie, bis die Bearbeitung beendet ist oder deselektieren Sie betroffene Prozesse.\n\n";
                                    myDia.ShowWarning(msg + pl);
                                }
                            }
                            else
                            {
                                myDia.ShowMessage("Bitte wählen Sie Prozesse aus der Übersicht aus, die Sie ohne Änderungen aktualisieren möchten.");
                            }

                        }, (list) => Setting.Multi_Save == "Ja"));
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// Exportieren der Liste der angezeigten Prozesse nach Excel
        /// </summary>
        public MyRelayCommand ExportProcessList
        {
            get => _exportProcessList
                    ?? (_exportProcessList = new MyRelayCommand(() =>
                    {
                        bool success = myExport.ExportProcesses(ProcessList);
                        if (success)
                        {
                            myDia.ShowInfo("Export erfolgreich");
                        }
                    }, () => ProcessViewMode == ProcAppListMode.Change));
        }

        /// <summary>
        /// Die Angezeigte Prozessliste
        /// </summary>
        public ObservableCollection<ISB_BIA_Prozesse> ProcessList
        {
            get => _processList;
            set => Set(() => ProcessList, ref _processList,value);
        }
        /// <summary>
        /// Anzahl nicht bearbeiteter Prozesse
        /// </summary>
        public int NonEditCount
        {
            get => _nonEditCount;
            set => Set(() => NonEditCount, ref _nonEditCount, value);   
        }
        /// <summary>
        /// Anzahl bearbeiteter Prozesse
        /// </summary>
        public int EditCount
        {
            get => _editCount;
            set => Set(() => EditCount, ref _editCount, value);
        }

        /// <summary>
        /// Ausgewähltes Objekt
        /// </summary>
        public object SelectedItem
        {
            get => _selectedItem;
            set => Set(() => SelectedItem, ref _selectedItem, value);
        }

        #region Modi-Abhängige Properties (Ändern/Löschen)
        /// <summary>
        /// Modus, welcher Funktionen und Darstellungen /Anweisungen bestimmt
        /// </summary>
        public ProcAppListMode ProcessViewMode
        {
            get => _processViewMode;
            set
            {
                if (value == ProcAppListMode.Change)
                {
                    ButtonVis = Visibility.Visible;
                    Instruction = "Doppelklick auf einen Prozess, den Sie ändern möchten, oder Prozesse anhaken, welche ohne Änderungen gespeichert werden sollen.";
                    Header = "Prozesse bearbeiten";
                    RowDoubleClick = NavToProcess;
                }
                else if (value == ProcAppListMode.Delete)
                {
                    ButtonVis = Visibility.Collapsed;
                    Instruction = "Doppelklick auf einen Prozess, den Sie löschen möchten";
                    Header = "Prozesse löschen";
                    RowDoubleClick = DeleteProc;

                }
                Set(() => ProcessViewMode, ref _processViewMode, value);
            }
        }
        /// <summary>
        /// Sichtbarkeit des Multi-Save und Export Buttons je nach Bearbeitungsmodus
        /// </summary>
        public Visibility ButtonVis { get; set; }
        /// <summary>
        /// Anweisung
        /// </summary>
        public string Instruction { get; set; }
        /// <summary>
        /// Überschrift
        /// </summary>
        public string Header { get; set; }
        #endregion

        /// <summary>
        /// Aktuelle Einstellungen
        /// </summary>
        public ISB_BIA_Settings Setting
        {
            get => _setting;
            set => Set(() => Setting, ref _setting, value);
        }

        #region Sevices
        IMyNavigationService myNavi;
        IMyDialogService myDia;
        IMyExportService myExport;
        IMyDataService myData;
        IMySharedResourceService myShared;
        IMyMailNotificationService myMail;
        #endregion

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="myDialogService"></param>
        /// <param name="myNavigationService"></param>
        /// <param name="myExportService"></param>
        /// <param name="myDataService"></param>
        /// <param name="mySharedResourceService"></param>
        /// <param name="myMailNotificationService"></param>
        public ProcessView_ViewModel(IMyDialogService myDialogService, IMyNavigationService myNavigationService, IMyExportService myExportService, IMyDataService myDataService, IMySharedResourceService mySharedResourceService, IMyMailNotificationService myMailNotificationService)
        {
            #region Services
            myDia = myDialogService;
            myNavi = myNavigationService;
            myExport = myExportService;
            myData = myDataService;
            myShared = mySharedResourceService;
            myMail = myMailNotificationService;
            #endregion

            if (IsInDesignMode)
            {
                ProcessList = myData.GetProcesses();
                EditCount = ProcessList.Where(x => x.Datum.Year == DateTime.Now.Year).ToList().Count;
                NonEditCount = ProcessList.Where(x => x.Datum.Year != DateTime.Now.Year).ToList().Count;
                
                Header = "TestHeader";
                Instruction = "TestInstruction";
            }
            else
            {
                Messenger.Default.Register<ProcAppListMode>(this, p => { ProcessViewMode = p; });
                Messenger.Default.Register<string>(this, MessageToken.RefreshData, s => { Refresh(); });
                Setting = myData.GetSettings();
                //Laden der Daten
                Refresh();
            }
        }

        /// <summary>
        /// Aktualisieren der Daten
        /// </summary>
        public void Refresh()
        {
            ObservableCollection<string> RelevantOEsList = myData.GetOEsForUser(myShared.User.OE);
            if(myShared.User.UserGroup == UserGroups.Admin || myShared.User.UserGroup == UserGroups.CISO)
            {
                ProcessList = myData.GetProcesses();
            }
            else
            {
                ProcessList = myData.GetProcessesByOE(RelevantOEsList);
            }
            if (ProcessList == null || RelevantOEsList == null)
            {
                Cleanup();
                myNavi.NavigateBack();
                myDia.ShowError("Es ist ein Fehler beim Laden der Daten aufgetreten");
            }
            EditCount = ProcessList.Where(x => x.Datum.Year == DateTime.Now.Year).ToList().Count;
            NonEditCount = ProcessList.Where(x => x.Datum.Year != DateTime.Now.Year).ToList().Count;
            SelectedProcesses = new ObservableCollection<ISB_BIA_Prozesse>();
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
