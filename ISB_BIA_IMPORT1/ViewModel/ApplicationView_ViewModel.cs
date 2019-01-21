using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using ISB_BIA_IMPORT1.LinqDataContext;
using ISB_BIA_IMPORT1.Services;
using System;
using System.Collections.ObjectModel;

namespace ISB_BIA_IMPORT1.ViewModel
{
    /// <summary>
    /// VM zur Darstellung der Applikationsübersicht
    /// </summary>
    public class ApplicationView_ViewModel: ViewModelBase
    {
        #region Backing-Fields
        private MyRelayCommand _navToApp;
        private MyRelayCommand _deleteApp;
        private MyRelayCommand _exportApplicationList;
        private ObservableCollection<ISB_BIA_Applikationen> _applicationList;
        private object _selectedItem;
        private ProcAppListMode _applicationViewMode;
        #endregion

        #region Operation/Navigations-Commands die Viewmodel ändern und ggf. Daten daran versenden
        /// <summary>
        /// Command welches je nach Modus <see cref="ApplicationViewMode"/> definiert wird
        /// </summary>
        public MyRelayCommand RowDoubleClick { get; set; }
        /// <summary>
        /// Applikation löschen, falls nicht bereits gelöscht (inaktiv)
        /// </summary>
        public MyRelayCommand DeleteApp
        {
            get => _deleteApp
                    ?? (_deleteApp = new MyRelayCommand(() =>
                    {
                        if (SelectedItem == null) return;
                        ISB_BIA_Applikationen ApplicationToDelete = (ISB_BIA_Applikationen)SelectedItem;
                        string isLockedBy = myData.GetObjectLocked(Table_Lock_Flags.Process, ApplicationToDelete.Applikation_Id);
                        if (isLockedBy == "")
                        {
                            if (ApplicationToDelete.Aktiv != 0)
                            {
                                ISB_BIA_Applikationen NewApplicationToDelete = myData.DeleteApplication(ApplicationToDelete);
                                if (NewApplicationToDelete != null)
                                {
                                    Refresh();
                                }
                            }
                            else
                            {
                                myDia.ShowWarning("Diese Anwendung wurde bereits gelöscht.");
                            }
                        }
                        else
                        {
                            myDia.ShowWarning("Diese Anwendung wird momentan durch einen anderen User bearbeitet und kann daher nicht gelöscht werden.\n\nBelegender Benutzer: " + isLockedBy + "\n\nSollte der User die Anwendung nicht geöffnet haben, wenden Sie sich bitte an die IT.");
                        }

                    }));
        }
        /// <summary>
        /// Zu ausgewählter Applikation <see cref="SelectedItem"/> navigieren falls diese nicht gesperrt ist, und für andere User sperren
        /// </summary>
        public MyRelayCommand NavToApp
        {
            get => _navToApp
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
        /// <summary>
        /// Zurück Navigieren (Vorgang abbbrechen)
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
        /// Command zum Exportieren der Applikationsliste nach Excel
        /// </summary>
        public MyRelayCommand ExportApplicationList
        {
            get => _exportApplicationList
                    ?? (_exportApplicationList = new MyRelayCommand(() =>
                    {
                        if (myExport.AllApplicationsExport())
                        {
                            myDia.ShowMessage("Export erfolgreich");
                        }
                    }));
        }
        #endregion

        /// <summary>
        /// Liste aller anzuzeigenden Applikationen
        /// </summary>
        public ObservableCollection<ISB_BIA_Applikationen> ApplicationList
        {
            get => _applicationList;
            set => Set(() => ApplicationList, ref _applicationList, value);
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
                    Header = "Anwendungen bearbeiten";
                    Instruction = "Doppelklick auf eine Anwendung, die Sie ändern möchten.";
                    RowDoubleClick = NavToApp;
                }
                else if (value == ProcAppListMode.Delete)
                {
                    Header = "Anwendungen löschen";
                    Instruction = "Doppelklick auf eine Anwendung, die Sie löschen möchten.";
                    RowDoubleClick = DeleteApp;
                }
            }
        }
        /// <summary>
        /// Überschrift
        /// </summary>
        public string Header { get; set; }
        /// <summary>
        /// Anweisung
        /// </summary>
        public string Instruction { get; set; }

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
        public ApplicationView_ViewModel(IMyDialogService myDialogService, IMyNavigationService myNavigationService, IMyExportService myExportService, IMyDataService myDataService, IMySharedResourceService mySharedResourceService)
        {
            #region Services
            myDia = myDialogService;
            myNavi = myNavigationService;
            myExport = myExportService;
            myData = myDataService;
            myShared = mySharedResourceService;
            #endregion

            if (IsInDesignMode)
            {
                ApplicationList = myData.GetApplications();
                Header = "TestHeader";
                Instruction = "TestInstruction";
            }
            else
            {
                //Message Registrierung zur Bestimmung des Ansichtsmodus
                Messenger.Default.Register<ProcAppListMode>(this, a => { ApplicationViewMode = a; });
                //Message Registrierung für Refreshaufforderung nach bearbeiten einer Applikation
                Messenger.Default.Register<string>(this, MessageToken.RefreshData, a => { Refresh(); });
                Refresh();
            }

        }

        /// <summary>
        /// Aktualisieren der angezeigten Anwendungen
        /// </summary>
        public void Refresh()
        {
            ApplicationList = myData.GetApplications();
            if (ApplicationList == null)
            {
                Cleanup();
                myNavi.NavigateBack();
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

