using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using ISB_BIA_IMPORT1.Model;
using ISB_BIA_IMPORT1.LinqDataContext;
using ISB_BIA_IMPORT1.Services;
using System;

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
                if (myDia.CancelDecision())
                {
                    Cleanup();
                    myNavi.NavigateBack();
                    myData.UnlockObject(Table_Lock_Flags.Settings, 0);
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
                if (myData.InsertSettings(myData.MapSettingsModelToDB(NewSettings), myData.MapSettingsModelToDB(OldSettings)))
                {
                    Cleanup();
                    myNavi.NavigateBack();
                    myData.UnlockObject(Table_Lock_Flags.Settings, 0);
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
                myExport.ExportSettings(myData.GetSettingsHistory());
            });
        }

        #region Services
        IMyNavigationService myNavi;
        IMyDialogService myDia;
        IMyDataService myData;
        IMyExportService myExport;
        #endregion

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="myDialogService"></param>
        /// <param name="myNavigationService"></param>
        /// <param name="myDataService"></param>
        public Settings_ViewModel(IMyDialogService myDialogService, IMyNavigationService myNavigationService, IMyDataService myDataService, IMyExportService myExportService)
        {
            myDia = myDialogService;
            myNavi = myNavigationService;
            myData = myDataService;
            myExport = myExportService;
            OldSettings = myData.GetSettingsModelFromDB();
            NewSettings = myData.GetSettingsModelFromDB();
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
