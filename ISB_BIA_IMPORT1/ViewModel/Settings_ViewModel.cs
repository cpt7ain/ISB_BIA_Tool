using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using ISB_BIA_IMPORT1.Model;
using ISB_BIA_IMPORT1.Services;
using ISB_BIA_IMPORT1.Helpers;
using ISB_BIA_IMPORT1.Services.Interfaces;

namespace ISB_BIA_IMPORT1.ViewModel
{
    /// <summary>
    /// VM zur Darstellung der Einstellungen
    /// </summary>
    public class Settings_ViewModel : ViewModelBase
    {
        #region Backing-Fields
        private Settings_Model _settingsOld;
        private Settings_Model _settingsNew;

        #endregion
        /// <summary>
        /// Ausgangseinstellungen
        /// </summary>
        public Settings_Model SettingsOld
        {
            get => _settingsOld;
            set => Set(() => SettingsOld, ref _settingsOld, value);
        }

        /// <summary>
        /// Aktuelle Einstellungen zum ändern
        /// </summary>
        public Settings_Model SettingsNew
        {
            get => _settingsNew;
            set => Set(() => SettingsNew, ref _settingsNew, value);
        }

        /// <summary>
        /// Command zum Zurückkehren zum vorherigen VM
        /// </summary>
        public MyRelayCommand Cmd_NavBack
        {
            get => new MyRelayCommand(() =>
            {
                if (_myDia.CancelDecision())
                {
                    Cleanup();
                    _myNavi.NavigateBack();
                    _myLock.Unlock_Object(Table_Lock_Flags.Settings, 0);
                }
            });
        }

        /// <summary>
        /// Command zum Speichern der <see cref="SettingsNew"/>
        /// </summary>
        public MyRelayCommand Cmd_Save
        {
            get => new MyRelayCommand(() =>
            {
                if (_mySett.Insert_Settings(_mySett.Map_ModelToDB(SettingsNew), _mySett.Map_ModelToDB(SettingsOld)))
                {
                    Cleanup();
                    _myNavi.NavigateBack();
                    _myLock.Unlock_Object(Table_Lock_Flags.Settings, 0);
                }
            });
        }

        /// <summary>
        /// Command zum Exportieren der Einstellungshistorie
        /// </summary>
        public MyRelayCommand Cmd_ExportSettingsHistory
        {
            get => new MyRelayCommand(() =>
            {
                _myExport.Set_ExportSettings(_mySett.Get_History_Settings());
            });
        }

        #region Services
        private readonly IMyNavigationService _myNavi;
        private readonly IMyDialogService _myDia;
        private readonly IMyDataService_Setting _mySett;
        private readonly IMyDataService_Lock _myLock;
        private readonly IMyExportService _myExport;
        #endregion

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="myDialogService"></param>
        /// <param name="myNavigationService"></param>
        /// <param name="mySett"></param>
        /// <param name="myExportService"></param>
        public Settings_ViewModel(IMyDialogService myDialogService, IMyNavigationService myNavigationService, 
            IMyDataService_Setting mySett, IMyDataService_Lock myLock, IMyExportService myExportService)
        {
            _myDia = myDialogService;
            _myNavi = myNavigationService;
            _mySett = mySett;
            _myLock = myLock;
            _myExport = myExportService;
            SettingsOld = _mySett.Get_ModelFromDB();
            SettingsNew = _mySett.Get_ModelFromDB();
            EventToCommand a = new EventToCommand();
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
