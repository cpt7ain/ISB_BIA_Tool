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
        private Settings_Model _oldSettings;
        private Settings_Model _newSettings;

        #endregion
        /// <summary>
        /// Ausgangseinstellungen
        /// </summary>
        public Settings_Model OldSettings
        {
            get => _oldSettings;
            set => Set(() => OldSettings, ref _oldSettings, value);
        }

        /// <summary>
        /// Aktuelle Einstellungen zum ändern
        /// </summary>
        public Settings_Model NewSettings
        {
            get => _newSettings;
            set => Set(() => NewSettings, ref _newSettings, value);
        }

        /// <summary>
        /// Command zum Zurückkehren zum vorherigen VM
        /// </summary>
        public MyRelayCommand NavBack
        {
            get => new MyRelayCommand(() =>
            {
                if (_myDia.CancelDecision())
                {
                    Cleanup();
                    _myNavi.NavigateBack();
                    _myData.UnlockObject(Table_Lock_Flags.Settings, 0);
                }
            });
        }

        /// <summary>
        /// Command zum Speichern der <see cref="NewSettings"/>
        /// </summary>
        public MyRelayCommand Save
        {
            get => new MyRelayCommand(() =>
            {
                if (_myData.InsertSettings(_myData.MapSettingsModelToDB(NewSettings), _myData.MapSettingsModelToDB(OldSettings)))
                {
                    Cleanup();
                    _myNavi.NavigateBack();
                    _myData.UnlockObject(Table_Lock_Flags.Settings, 0);
                }
            });
        }

        /// <summary>
        /// Command zum Exportieren der Einstellungshistorie
        /// </summary>
        public MyRelayCommand Export
        {
            get => new MyRelayCommand(() =>
            {
                _myExport.ExportSettings(_myData.GetSettingsHistory());
            });
        }

        #region Services
        private readonly IMyNavigationService _myNavi;
        private readonly IMyDialogService _myDia;
        private readonly IMyDataService _myData;
        private readonly IMyExportService _myExport;
        #endregion

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="myDialogService"></param>
        /// <param name="myNavigationService"></param>
        /// <param name="myDataService"></param>
        /// <param name="myExportService"></param>
        public Settings_ViewModel(IMyDialogService myDialogService, IMyNavigationService myNavigationService, IMyDataService myDataService, IMyExportService myExportService)
        {
            _myDia = myDialogService;
            _myNavi = myNavigationService;
            _myData = myDataService;
            _myExport = myExportService;
            OldSettings = _myData.GetSettingsModelFromDB();
            NewSettings = _myData.GetSettingsModelFromDB();
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
