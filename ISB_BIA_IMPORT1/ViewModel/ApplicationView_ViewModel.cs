using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using ISB_BIA_IMPORT1.LINQ2SQL;
using System.Collections.ObjectModel;
using ISB_BIA_IMPORT1.Helpers;
using ISB_BIA_IMPORT1.Services.Interfaces;

namespace ISB_BIA_IMPORT1.ViewModel
{
    /// <summary>
    /// VM zur Darstellung der Applikationsübersicht
    /// </summary>
    public class ApplicationView_ViewModel: ViewModelBase
    {
        #region Backing-Fields
        private MyRelayCommand _cmd_NavToApp;
        private MyRelayCommand _cmd_DeleteApp;
        private MyRelayCommand _cmd_ExportList;
        private ObservableCollection<ISB_BIA_Applikationen> _list_Application;
        private object _selectedItem;
        private ProcAppListMode _applicationViewMode;
        #endregion

        #region Operation/Navigations-Commands die Viewmodel ändern und ggf. Daten daran versenden
        /// <summary>
        /// Command welches je nach Modus <see cref="ApplicationViewMode"/> definiert wird
        /// </summary>
        public MyRelayCommand Cmd_RowDoubleClick { get; set; }
        /// <summary>
        /// Applikation löschen, falls nicht bereits gelöscht (inaktiv)
        /// </summary>
        public MyRelayCommand Cmd_DeleteApp
        {
            get => _cmd_DeleteApp
                    ?? (_cmd_DeleteApp = new MyRelayCommand(() =>
                    {
                        if (SelectedItem == null) return;
                        ISB_BIA_Applikationen applicationToDelete = (ISB_BIA_Applikationen)SelectedItem;
                        string isLockedBy = _myLock.Get_ObjectIsLocked(Table_Lock_Flags.Process, applicationToDelete.Applikation_Id);
                        if (isLockedBy == "")
                        {
                            if (applicationToDelete.Aktiv != 0)
                            {
                                ISB_BIA_Applikationen newApplicationToDelete = _myApp.Delete_Application(applicationToDelete);
                                if (newApplicationToDelete != null)
                                {
                                    Refresh();
                                }
                            }
                            else
                            {
                                _myDia.ShowWarning("Diese Anwendung wurde bereits gelöscht.");
                            }
                        }
                        else
                        {
                            _myDia.ShowWarning("Diese Anwendung wird momentan durch einen anderen User bearbeitet und kann daher nicht gelöscht werden.\n\nBelegender Benutzer: " + isLockedBy + "\n\nSollte der User die Anwendung nicht geöffnet haben, wenden Sie sich bitte an die IT.");
                        }

                    }));
        }
        /// <summary>
        /// Zu ausgewählter Applikation <see cref="SelectedItem"/> navigieren falls diese nicht gesperrt ist, und für andere User sperren
        /// </summary>
        public MyRelayCommand Cmd_NavToApp
        {
            get => _cmd_NavToApp
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
        /// <summary>
        /// Zurück Navigieren (Vorgang abbbrechen)
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
        /// Command zum Exportieren der Applikationsliste nach Excel
        /// </summary>
        public MyRelayCommand Cmd_ExportList
        {
            get => _cmd_ExportList
                    ?? (_cmd_ExportList = new MyRelayCommand(() =>
                    {
                        _myExport.Export_Applications_All();
                    }));
        }
        #endregion

        /// <summary>
        /// Liste aller anzuzeigenden Applikationen
        /// </summary>
        public ObservableCollection<ISB_BIA_Applikationen> List_Application
        {
            get => _list_Application;
            set => Set(() => List_Application, ref _list_Application, value);
        }

        /// <summary>
        /// Die in der Liste ausgewählte Applikation
        /// </summary>
        public object SelectedItem
        {
            get => _selectedItem;
            set => Set(() => SelectedItem, ref _selectedItem, value);
        }

        /// <summary>
        /// Modus, der bestimmt ob die ausgewählte Anwendung bei Doppelklick bearbeitet oder gelöscht werden soll
        /// und Überschrift / Anweisung bestimmt
        /// </summary>
        public ProcAppListMode ApplicationViewMode
        {
            get => _applicationViewMode;
            set
            {
                Set(() => ApplicationViewMode, ref _applicationViewMode, value);
                if (value == ProcAppListMode.Change)
                {
                    Str_Header = "Anwendungen bearbeiten";
                    Str_Instruction = "Doppelklick auf eine Anwendung, die Sie ändern möchten.";
                    Cmd_RowDoubleClick = Cmd_NavToApp;
                }
                else if (value == ProcAppListMode.Delete)
                {
                    Str_Header = "Anwendungen löschen";
                    Str_Instruction = "Doppelklick auf eine Anwendung, die Sie löschen möchten.";
                    Cmd_RowDoubleClick = Cmd_DeleteApp;
                }
            }
        }
        /// <summary>
        /// Überschrift
        /// </summary>
        public string Str_Header { get; set; }
        /// <summary>
        /// Anweisung
        /// </summary>
        public string Str_Instruction { get; set; }

        #region Services
        private readonly INavigationService _myNavi;
        private readonly IDialogService _myDia;
        private readonly IExportService _myExport;
        private readonly IDataService_Application _myApp;
        private readonly ILockService _myLock;
        #endregion
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="myDia"></param>
        /// <param name="myNavi"></param>
        /// <param name="myExp"></param>
        /// <param name="myApp"></param>
        public ApplicationView_ViewModel(IDialogService myDia, INavigationService myNavi, 
            IExportService myExp, IDataService_Application myApp, ILockService myLock)
        {
            #region Services
            _myDia = myDia;
            _myLock = myLock;
            _myNavi = myNavi;
            _myExport = myExp;
            _myApp = myApp;
            #endregion

            if (IsInDesignMode)
            {
                List_Application = _myApp.Get_List_Applications_All();
                Str_Header = "TestHeader";
                Str_Instruction = "TestInstruction";
            }
            else
            {
                //Message Registrierung zur Bestimmung des Ansichtsmodus
                MessengerInstance.Register<NotificationMessage<ProcAppListMode>>(this, a =>
                {
                    if(!(a.Sender is INavigationService)) return;
                    ApplicationViewMode = a.Content;
                });
                //Message Registrierung für Refreshaufforderung nach bearbeiten einer Applikation
                MessengerInstance.Register<NotificationMessage<string>>(this, MessageToken.RefreshData, s =>
                {
                    if (!(s.Sender is INavigationService)) return;
                    Refresh();
                });
                Refresh();
            }

        }

        /// <summary>
        /// Aktualisieren der angezeigten Anwendungen
        /// </summary>
        public void Refresh()
        {
            List_Application = _myApp.Get_List_Applications_All();
            if (List_Application == null)
            {
                Cleanup();
                _myNavi.NavigateBack();
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

