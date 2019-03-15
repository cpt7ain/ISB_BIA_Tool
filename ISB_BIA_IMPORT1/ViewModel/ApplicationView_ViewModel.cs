﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using ISB_BIA_IMPORT1.LINQ2SQL;
using ISB_BIA_IMPORT1.Services;
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
        private MyRelayCommand _cmd_ExportApplicationList;
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
        public MyRelayCommand Cmd_ExportApplicationList
        {
            get => _cmd_ExportApplicationList
                    ?? (_cmd_ExportApplicationList = new MyRelayCommand(() =>
                    {
                        _myExport.App_ExportAllApplications();
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
                    Header = "Anwendungen bearbeiten";
                    Instruction = "Doppelklick auf eine Anwendung, die Sie ändern möchten.";
                    Cmd_RowDoubleClick = Cmd_NavToApp;
                }
                else if (value == ProcAppListMode.Delete)
                {
                    Header = "Anwendungen löschen";
                    Instruction = "Doppelklick auf eine Anwendung, die Sie löschen möchten.";
                    Cmd_RowDoubleClick = Cmd_DeleteApp;
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
        private readonly IMyNavigationService _myNavi;
        private readonly IMyDialogService _myDia;
        private readonly IMyExportService _myExport;
        private readonly IMyDataService_Application _myApp;
        private readonly IMyDataService_Lock _myLock;
        #endregion
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="myDia"></param>
        /// <param name="myNavi"></param>
        /// <param name="myExp"></param>
        /// <param name="myApp"></param>
        public ApplicationView_ViewModel(IMyDialogService myDia, IMyNavigationService myNavi, 
            IMyExportService myExp, IMyDataService_Application myApp, IMyDataService_Lock myLock)
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
                List_Application = _myApp.Get_Applications_All();
                Header = "TestHeader";
                Instruction = "TestInstruction";
            }
            else
            {
                //Message Registrierung zur Bestimmung des Ansichtsmodus
                MessengerInstance.Register<NotificationMessage<ProcAppListMode>>(this, a =>
                {
                    if(!(a.Sender is IMyNavigationService)) return;
                    ApplicationViewMode = a.Content;
                });
                //Message Registrierung für Refreshaufforderung nach bearbeiten einer Applikation
                MessengerInstance.Register<NotificationMessage<string>>(this, MessageToken.RefreshData, s =>
                {
                    if (!(s.Sender is IMyNavigationService)) return;
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
            List_Application = _myApp.Get_Applications_All();
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

