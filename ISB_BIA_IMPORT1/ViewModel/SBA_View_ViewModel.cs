using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using ISB_BIA_IMPORT1.LINQ2SQL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ISB_BIA_IMPORT1.Helpers;
using ISB_BIA_IMPORT1.Services.Interfaces;

namespace ISB_BIA_IMPORT1.ViewModel
{
    /// <summary>
    /// VM zur Darstellung der Schutzbedarfsanalyse-Anwendungsübersicht
    /// </summary>
    public class SBA_View_ViewModel : ViewModelBase
    {
        #region Backing-Fields
        private MyRelayCommand _cmd_NavToApp;
        private ObservableCollection<ISB_BIA_Applikationen> _list_SelectedApplications;
        private MyRelayCommand<object> _cmd_SaveSelectedApplications;
        private MyRelayCommand _cmd_ExportProcessList;
        private MyRelayCommand _cmd_ExportList;
        private ObservableCollection<ISB_BIA_Applikationen> _list_Applications;
        private object _selectedItem;
        private int _count_NonEdit;
        private int _count_Edit;
        private int _count_AllProcesses;
        private int _count_EditProcesses;
        #endregion

        #region Navigations-Commands die Viewmodel ändern und ggf. Daten daran versenden
        /// <summary>
        /// Command zum Zurückkehren zum vorherigen VM
        /// </summary>
        public MyRelayCommand Cmd_NavBack
        {
            get
            {
                return new MyRelayCommand(() =>
                {
                    Cleanup();
                    _myNavi.NavigateBack();
                });
            }
        }
        /// <summary>
        /// Zu ausgewählter Applikation <see cref="SelectedItem"/> navigieren falls diese nicht gesperrt ist, und für andere User sperren
        /// </summary>
        public MyRelayCommand Cmd_NavToApp
        {
            get
            {
                return _cmd_NavToApp
                    ?? (_cmd_NavToApp = new MyRelayCommand(() =>
                    {
                        if (SelectedItem == null) return;
                        ISB_BIA_Applikationen applicationToChange = (ISB_BIA_Applikationen)SelectedItem;
                        string user = _myLock.Get_ObjectIsLocked(Table_Lock_Flags.Application, applicationToChange.Applikation_Id);
                        if (user == "")
                        {
                            if (_myLock.Lock_Object(Table_Lock_Flags.Application, applicationToChange.Applikation_Id))
                                _myNavi.NavigateTo<Application_ViewModel>(applicationToChange.Applikation_Id, ProcAppMode.Change);
                        }
                        else
                        {
                            _myDia.ShowWarning("Diese Anwendung wird momentan durch einen anderen User bearbeitet und kann daher nicht geöffnet werden.\n\nBelegender Benutzer: " + user + "\n\nSollte der User die Anwendung nicht geöffnet haben, wenden Sie sich bitte an die IT.");
                        }
                    }));
            }
        }
        #endregion

        /// <summary>
        /// Liste der ausgewählten Anwendungen für das Gruppenspeichern/>
        /// </summary>
        public ObservableCollection<ISB_BIA_Applikationen> List_SelectedApplications
        {
            get => _list_SelectedApplications;
            set => Set(() => List_SelectedApplications, ref _list_SelectedApplications, value);
        }
        /// <summary>
        /// Command für die Gruppenspeicherung/>
        /// </summary>
        public MyRelayCommand<object> Cmd_SaveSelectedApplications
        {
            get => _cmd_SaveSelectedApplications
                        ?? (_cmd_SaveSelectedApplications = new MyRelayCommand<object>((list) =>
                        {
                            System.Collections.IList items = (System.Collections.IList)list;
                            if(items.Count > 0)
                            {
                                var collection = items.Cast<ISB_BIA_Applikationen>();
                                List_SelectedApplications = new ObservableCollection<ISB_BIA_Applikationen>(collection);
                                List<ISB_BIA_Applikationen> lockedList = new List<ISB_BIA_Applikationen>();
                                string al = "";
                                foreach (ISB_BIA_Applikationen a in List_SelectedApplications)
                                {
                                    string user = _myLock.Get_ObjectIsLocked(Table_Lock_Flags.Application, a.Applikation_Id);
                                    if (user != "")
                                    {
                                        lockedList.Add(a);
                                        al = al + a.IT_Anwendung_System + " (geöffnet von: " + user + ")\n";
                                    }
                                }
                                if (lockedList.Count == 0)
                                {
                                    if (_myApp.Insert_Applications_All(List_SelectedApplications))
                                    {
                                        Refresh();
                                    }
                                }
                                else
                                {
                                    string msg = "In der Auswahl befinden sich Anwendungen, die momentan durch andere User geöffnet sind und deshalb nicht gespeichert werden können.\nBitte warten Sie, bis die Bearbeitung beendet ist oder deselektieren Sie betroffene Prozesse.\n\n";
                                    _myDia.ShowWarning(msg + al);
                                }
                            }
                            else
                            {
                                _myDia.ShowMessage("Bitte wählen Sie Anwendungen aus der Übersicht aus, die Sie ohne Änderungen aktualisieren möchten.");
                            }
                        }, (list) => Setting.Multi_Speichern == "Ja"));
        }
        /// <summary>
        /// Exportieren der Liste der angezeigten Prozesse nach Excel
        /// </summary>
        public MyRelayCommand Cmd_ExportProcessList
        {
            get
            {
                return _cmd_ExportProcessList
                    ?? (_cmd_ExportProcessList = new MyRelayCommand(() =>
                    {
                        _myExport.Export_Processes_Active();
                    }));
            }
        }
        /// <summary>
        /// Exportieren der Liste aller aktiven Applikationen nach Excel
        /// </summary>
        public MyRelayCommand Cmd_ExportList
        {
            get
            {
                return _cmd_ExportList
                    ?? (_cmd_ExportList = new MyRelayCommand(() =>
                    {
                        _myExport.Export_Applications_Active();
                    }));
            }
        }
        /// <summary>
        /// Liste der angezeigten Applikationen
        /// </summary>
        public ObservableCollection<ISB_BIA_Applikationen> List_Applications
        {
            get => _list_Applications;
            set => Set(() => List_Applications, ref _list_Applications, value);

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
        public int Count_NonEdit
        {
            get => _count_NonEdit;
            set => Set(() => Count_NonEdit, ref _count_NonEdit, value);

        }
        /// <summary>
        /// Anzahl bearbeiteter Anwendungen
        /// </summary>
        public int Count_Edit
        {
            get => _count_Edit;
            set => Set(() => Count_Edit, ref _count_Edit, value);

        }
        /// <summary>
        /// Anzahl aller Prozesse der BIA
        /// </summary>
        public int Count_AllProcesses
        {
            get => _count_AllProcesses;
            set => Set(()=>Count_AllProcesses, ref _count_AllProcesses, value);

        }
        /// <summary>
        /// Anzahl bearbeiteter Prozesse der BIA
        /// </summary>
        public int Count_EditProcesses
        {
            get => _count_EditProcesses;
            set => Set(() => Count_EditProcesses, ref _count_EditProcesses, value);
        }
        /// <summary>
        /// Anweisung
        /// </summary>
        public string Str_Instruction
        {
            get => "Doppelklick auf eine Applikation, den Sie bearbeiten möchten.";
        }
        /// <summary>
        /// Überschrift
        /// </summary>
        public string Str_Header
        {
            get => "Anwendungen bearbeiten";
        }
        /// <summary>
        /// Aktuelle Einstellungen
        /// </summary>
        public ISB_BIA_Settings Setting { get; set; }

        #region Services
        private readonly INavigationService _myNavi;
        private readonly IDialogService _myDia;
        private readonly IExportService _myExport;
        private readonly IDataService_Application _myApp;
        private readonly IDataService_Process _myProc;
        private readonly IDataService_Setting _mySett;
        private readonly ILockService _myLock;

        #endregion

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="myDia"></param>
        /// <param name="myNavi"></param>
        /// <param name="myExp"></param>
        /// <param name="myApp"></param>
        public SBA_View_ViewModel(IDialogService myDia, INavigationService myNavi, 
            IExportService myExp, IDataService_Application myApp,ILockService myLock, 
            IDataService_Setting mySett , IDataService_Process myProc)
        {
            #region Services
            _myNavi = myNavi;
            _myDia = myDia;
            _mySett = mySett;
            _myLock = myLock;
            _myProc = myProc;
            _myExport = myExp;
            _myApp = myApp;
            #endregion

            MessengerInstance.Register<NotificationMessage<string>>(this, MessageToken.RefreshData, message =>
            {
                if (!(message.Sender is INavigationService)) return;
                Refresh();
            });
            Setting = _mySett.Get_List_Settings();
            Refresh();
        }

        /// <summary>
        /// Aktualisieren der Daten
        /// </summary>
        public void Refresh()
        {
            List_Applications = _myApp.Get_List_Applications_Active();
            if (List_Applications == null)
            {
                Cleanup();
                _myNavi.NavigateBack();
                _myDia.ShowError("Es konnten keine Applikationen abgerufen werden.");
            }
            Count_Edit = List_Applications.Where(x => x.Datum.Year == DateTime.Now.Year).ToList().Count;
            Count_NonEdit = List_Applications.Where(x => x.Datum.Year != DateTime.Now.Year).ToList().Count;
            ObservableCollection<ISB_BIA_Prozesse> processes = _myProc.Get_List_Processes_Active();
            Count_AllProcesses = processes.Count;
            Count_EditProcesses = processes.Where(x => x.Datum.Year == DateTime.Now.Year).ToList().Count;
            List_SelectedApplications = new ObservableCollection<ISB_BIA_Applikationen>();
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
