using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using ISB_BIA_IMPORT1.Model;
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
                if (_mySett.Insert_Settings(SettingsNew, SettingsOld))
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
                _myExport.Export_Settings(_mySett.Get_History_Settings());
            });
        }

        #region Services
        private readonly INavigationService _myNavi;
        private readonly IDialogService _myDia;
        private readonly IDataService_Setting _mySett;
        private readonly ILockService _myLock;
        private readonly IExportService _myExport;
        #endregion

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="myDia"></param>
        /// <param name="myNavi"></param>
        /// <param name="mySett"></param>
        /// <param name="myExp"></param>
        public Settings_ViewModel(IDialogService myDia, INavigationService myNavi, 
            IDataService_Setting mySett, ILockService myLock, IExportService myExp)
        {
            #region Services
            _myDia = myDia;
            _myNavi = myNavi;
            _mySett = mySett;
            _myLock = myLock;
            _myExport = myExp;
            #endregion

            SettingsOld = _mySett.Get_Model_FromDB();
            SettingsNew = _mySett.Get_Model_FromDB();
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
