using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using ISB_BIA_IMPORT1.Model;
using ISB_BIA_IMPORT1.LINQ2SQL;
using ISB_BIA_IMPORT1.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ISB_BIA_IMPORT1.Helpers;
using ISB_BIA_IMPORT1.Services.Interfaces;

namespace ISB_BIA_IMPORT1.ViewModel
{
    /// <summary>
    /// VM zur Darstellung der Prozessübersicht
    /// </summary>
    public class ProcessView_ViewModel : ViewModelBase
    {
        #region Backing-Fields
        private MyRelayCommand _cmd_NavToProcess;
        private MyRelayCommand _cmd_DeleteProc;
        private ObservableCollection<ISB_BIA_Prozesse> _list_SelectedProcesses;
        private MyRelayCommand<object> _cmd_SaveSelectedProcesses;
        private MyRelayCommand _cmd_ExportProcessList;
        private ObservableCollection<ISB_BIA_Prozesse> _list_Processes;
        private int _count_NonEdit;
        private int _count_Edit;
        private object _selectedItem;
        private ProcAppListMode _mode;
        private ISB_BIA_Settings _setting;
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
        /// Zu ausgewähltem Prozess <see cref="SelectedItem"/> navigieren falls dieser nicht gesperrt ist, und für andere User sperren
        /// </summary>
        public MyRelayCommand Cmd_NavToProcess
        {
            get => _cmd_NavToProcess
                    ?? (_cmd_NavToProcess = new MyRelayCommand(() =>
                    {
                        if (SelectedItem == null) return;
                        ISB_BIA_Prozesse processToChange = (ISB_BIA_Prozesse)SelectedItem;
                        string isLockedBy = _myLock.Get_ObjectIsLocked(Table_Lock_Flags.Process, processToChange.Prozess_Id);
                        if (isLockedBy == "")
                        {
                            if(_myLock.Lock_Object(Table_Lock_Flags.Process, processToChange.Prozess_Id))
                            _myNavi.NavigateTo<Process_ViewModel>(processToChange.Prozess_Id, ProcAppMode.Change);
                        }
                        else
                        {
                            _myDia.ShowWarning("Dieser Prozess wird momentan durch einen anderen User bearbeitet und kann daher nicht geöffnet werden.\n\nBelegender Benutzer: "+isLockedBy+"\n\nSollte der User den Prozess nicht geöffnet haben, wenden Sie sich bitte an die IT.");
                        }
                    }, () => Mode == ProcAppListMode.Change));
        }
        /// <summary>
        /// Prozess löschen, falls nicht bereits gelöscht (inaktiv)
        /// </summary>
        public MyRelayCommand Cmd_DeleteProc
        {
            get => _cmd_DeleteProc
                    ?? (_cmd_DeleteProc = new MyRelayCommand(() =>
                    {
                        if (SelectedItem == null) return;
                        ISB_BIA_Prozesse processToDelete = (ISB_BIA_Prozesse)SelectedItem;
                        string isLockedBy = _myLock.Get_ObjectIsLocked(Table_Lock_Flags.Process, processToDelete.Prozess_Id);
                        if (isLockedBy == "")
                        {
                            if (processToDelete.Aktiv != 0)
                            {
                                ISB_BIA_Prozesse newProcessToDelete = _myProc.Delete_Process(processToDelete);
                                if (newProcessToDelete != null)
                                {
                                    Refresh();
                                }
                            }
                            else
                            {
                                _myDia.ShowWarning("Dieser Prozess wurde bereits gelöscht.");
                            }
                        }
                        else
                        {
                            _myDia.ShowWarning("Dieser Prozess wird momentan durch einen anderen User bearbeitet und kann daher nicht gelöscht werden.\n\nBelegender Benutzer: " + isLockedBy + "\n\nSollte der User den Prozess nicht geöffnet haben, wenden Sie sich bitte an die IT.");
                        }

                    }, () => Mode == ProcAppListMode.Delete));
        }
        /// <summary>
        /// Command welches je nach Modus <see cref="Mode"/> definiert wird
        /// </summary>
        public MyRelayCommand Cmd_RowDoubleClick { get; set; }
        /// <summary>
        /// Liste der ausgewählten Prozesse für das Gruppenspeichern/>
        /// </summary>
        public ObservableCollection<ISB_BIA_Prozesse> List_SelectedProcesses
        {
            get => _list_SelectedProcesses;
            set => Set(()=>List_SelectedProcesses, ref _list_SelectedProcesses, value);
        }
        /// <summary>
        /// Command für die Gruppenspeicherung/>
        /// </summary>
        public MyRelayCommand<object> Cmd_SaveSelectedProcesses
        {
            get
            {
                if (!IsInDesignMode)
                {
                     return _cmd_SaveSelectedProcesses
                        ?? (_cmd_SaveSelectedProcesses = new MyRelayCommand<object>((list) =>
                        {
                            System.Collections.IList items = (System.Collections.IList)list;
                            if(items.Count > 0)
                            {
                                var collection = items.Cast<ISB_BIA_Prozesse>();
                                List_SelectedProcesses = new ObservableCollection<ISB_BIA_Prozesse>(collection);
                                List<ISB_BIA_Prozesse> lockedList = new List<ISB_BIA_Prozesse>();
                                string pl = "";
                                foreach (ISB_BIA_Prozesse p in List_SelectedProcesses)
                                {
                                    string user = _myLock.Get_ObjectIsLocked(Table_Lock_Flags.Process, p.Prozess_Id);
                                    if (user != "")
                                    {
                                        lockedList.Add(p);
                                        pl = pl + p.Prozess + " (geöffnet von: " + user + ")\n";
                                    }
                                }
                                if (lockedList.Count == 0)
                                {
                                    if (_myProc.Insert_Processes_All(List_SelectedProcesses))
                                    {
                                        Refresh();
                                    }
                                }
                                else
                                {
                                    string msg = "In der Auswahl befinden sich Prozesse, die momentan durch andere User geöffnet sind und deshalb nicht gespeichert werden können.\nBitte warten Sie, bis die Bearbeitung beendet ist oder deselektieren Sie betroffene Prozesse.\n\n";
                                    _myDia.ShowWarning(msg + pl);
                                }
                            }
                            else
                            {
                                _myDia.ShowMessage("Bitte wählen Sie Prozesse aus der Übersicht aus, die Sie ohne Änderungen aktualisieren möchten.");
                            }

                        }, (list) => Setting.Multi_Speichern == "Ja"));
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
        public MyRelayCommand Cmd_ExportProcessList
        {
            get => _cmd_ExportProcessList
                    ?? (_cmd_ExportProcessList = new MyRelayCommand(() =>
                    {
                        _myExport.Export_Processes(List_Processes);
                    }, () => Mode == ProcAppListMode.Change));
        }
        /// <summary>
        /// Die Angezeigte Prozessliste
        /// </summary>
        public ObservableCollection<ISB_BIA_Prozesse> List_Processes
        {
            get => _list_Processes;
            set => Set(() => List_Processes, ref _list_Processes,value);
        }
        /// <summary>
        /// Anzahl nicht bearbeiteter Prozesse
        /// </summary>
        public int Count_NonEdit
        {
            get => _count_NonEdit;
            set => Set(() => Count_NonEdit, ref _count_NonEdit, value);   
        }
        /// <summary>
        /// Anzahl bearbeiteter Prozesse
        /// </summary>
        public int Count_Edit
        {
            get => _count_Edit;
            set => Set(() => Count_Edit, ref _count_Edit, value);
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
        public ProcAppListMode Mode
        {
            get => _mode;
            set
            {
                if (value == ProcAppListMode.Change)
                {
                    SelectionMode = (Setting.Multi_Speichern == "Ja")? DataGridSelectionMode.Extended:DataGridSelectionMode.Single;
                    Vis_ButtonMultiSave = Visibility.Visible;
                    Str_Instruction = "Doppelklick auf einen Prozess, den Sie ändern möchten, oder Prozesse anhaken, welche ohne Änderungen gespeichert werden sollen.";
                    Str_Header = "Prozesse bearbeiten";
                    Cmd_RowDoubleClick = Cmd_NavToProcess;
                }
                else if (value == ProcAppListMode.Delete)
                {
                    SelectionMode = DataGridSelectionMode.Single;
                    Vis_ButtonMultiSave = Visibility.Collapsed;
                    Str_Instruction = "Doppelklick auf einen Prozess, den Sie löschen möchten";
                    Str_Header = "Prozesse löschen";
                    Cmd_RowDoubleClick = Cmd_DeleteProc;

                }
                Set(() => Mode, ref _mode, value);
            }
        }
        /// <summary>
        /// Auswahlmodus für Items des Datagrids je nach Bearbeitungsmodus (Einzeln/Mehrere)
        /// </summary>
        public DataGridSelectionMode SelectionMode { get; set; }
        /// <summary>
        /// Sichtbarkeit des Multi-Save und Export Buttons je nach Bearbeitungsmodus
        /// </summary>
        public Visibility Vis_ButtonMultiSave { get; set; }
        /// <summary>
        /// Anweisung
        /// </summary>
        public string Str_Instruction { get; set; }
        /// <summary>
        /// Überschrift
        /// </summary>
        public string Str_Header { get; set; }
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
        private readonly INavigationService _myNavi;
        private readonly IDialogService _myDia;
        private readonly IExportService _myExport;
        private readonly IDataService_Process _myProc;
        private readonly ISharedResourceService _myShared;
        private readonly ILockService _myLock;
        private readonly IDataService_Setting _mySett;
        #endregion

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="myDia"></param>
        /// <param name="myNavi"></param>
        /// <param name="myExp"></param>
        /// <param name="myProc"></param>
        /// <param name="myShared"></param>
        public ProcessView_ViewModel(IDialogService myDia, INavigationService myNavi,
            IExportService myExp, IDataService_Process myProc, ILockService myLock,
            IDataService_Setting mySett,ISharedResourceService myShared)
        {
            #region Services
            _myDia = myDia;
            _myNavi = myNavi;
            _myExport = myExp;
            _myProc = myProc;
            _myLock = myLock;
            _mySett = mySett;
            _myShared = myShared;
            #endregion

            if (IsInDesignMode)
            {
                List_Processes = _myProc.Get_List_Processes_All();
                Count_Edit = List_Processes.Where(x => x.Datum.Year == DateTime.Now.Year).ToList().Count;
                Count_NonEdit = List_Processes.Where(x => x.Datum.Year != DateTime.Now.Year).ToList().Count;
                
                Str_Header = "TestHeader";
                Str_Instruction = "TestInstruction";
            }
            else
            {
                MessengerInstance.Register<NotificationMessage<ProcAppListMode>>(this, message =>
                {
                    if (!(message.Sender is INavigationService)) return;
                    Mode = message.Content;
                });
                MessengerInstance.Register<NotificationMessage<string>>(this, MessageToken.RefreshData, message =>
                {
                    if (!(message.Sender is INavigationService)) return;
                    Refresh();
                });
                Setting = _mySett.Get_List_Settings();
                //Laden der Daten
                Refresh();
            }
        }

        /// <summary>
        /// Aktualisieren der Daten
        /// </summary>
        public void Refresh()
        {
            ObservableCollection<string> relevantOEsList = _myProc.Get_StringList_OEsForUser(_myShared.User.OE);
            if(_myShared.User.UserGroup == UserGroups.Admin || _myShared.User.UserGroup == UserGroups.CISO)
            {
                List_Processes = _myProc.Get_List_Processes_All();
            }
            else
            {
                List_Processes = _myProc.Get_List_Processes_ByOE(relevantOEsList);
            }
            if (List_Processes == null || relevantOEsList == null)
            {
                Cleanup();
                _myNavi.NavigateBack();
                _myDia.ShowError("Es ist ein Fehler beim Laden der Daten aufgetreten");
            }
            Count_Edit = List_Processes.Where(x => x.Datum.Year == DateTime.Now.Year).ToList().Count;
            Count_NonEdit = List_Processes.Where(x => x.Datum.Year != DateTime.Now.Year).ToList().Count;
            List_SelectedProcesses = new ObservableCollection<ISB_BIA_Prozesse>();
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
