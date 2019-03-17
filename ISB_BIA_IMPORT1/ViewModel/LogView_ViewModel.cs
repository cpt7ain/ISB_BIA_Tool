using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using ISB_BIA_IMPORT1.LINQ2SQL;
using ISB_BIA_IMPORT1.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows.Data;
using ISB_BIA_IMPORT1.Helpers;
using ISB_BIA_IMPORT1.Services.Interfaces;

namespace ISB_BIA_IMPORT1.ViewModel
{
    /// <summary>
    /// VM zur Darstellung des Logs
    /// </summary>
    public class LogView_ViewModel : ViewModelBase
    {
        #region Backing Fields
        private ObservableCollection<ISB_BIA_Log> _logList;
        private CollectionView _filterView;
        private string _str_FilterText;
        private MyRelayCommand _cmd_ExportLog;
        #endregion

        /// <summary>
        /// Liste der Logeinträge
        /// </summary>
        public ObservableCollection<ISB_BIA_Log> LogList
        {
            get => _logList;
            set => Set(()=>LogList, ref _logList,value);
        }

        /// <summary>
        /// CollectionView für Suche in der Log-Liste
        /// </summary>
        public CollectionView FilterView
        {
            get => _filterView;
            set => Set(() => FilterView, ref _filterView, value);
        }

        /// <summary>
        /// Text, nach dem im <see cref="FilterView"/> gefiltert wird
        /// </summary>
        public string Str_FilterText
        {
            get => _str_FilterText;
            set
            {
                Set(() => Str_FilterText, ref _str_FilterText, value);
                FilterView.Refresh();
            }
        }

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
        /// Command zum Exportieren der Log-Liste nach Excel
        /// </summary>
        public MyRelayCommand Cmd_ExportLog
        {
            get => _cmd_ExportLog
                  ?? (_cmd_ExportLog = new MyRelayCommand(() =>
                  {
                      _myExport.Export_Log(LogList);
                  }));
        }

        #region Services
        private readonly IMyNavigationService _myNavi;
        private readonly IMyExportService _myExport;
        private readonly IMyDataService_Log _myLog;
        #endregion

        /// <summary>
        /// Viewmodel für die Darstellung des Anwendungs-Logs
        /// </summary>
        /// <param name="myNavigationService"> <see cref="IMyNavigationService"/> </param>
        /// <param name="myExportService"><see cref="IMyExportService"/> </param>
        /// <param name="myLog"><see cref="IMyDataService"/> </param>
        public LogView_ViewModel(IMyNavigationService myNavigationService, IMyExportService myExportService, IMyDataService_Log myLog)
        {
            #region Services
            _myNavi = myNavigationService;
            _myExport = myExportService;
            _myLog = myLog;
            #endregion
            //Log abrufen
            LogList = _myLog.Get_Log();
            //Definieren der Quelle für den CollectionView (=> Log Liste)
            FilterView = (CollectionView)CollectionViewSource.GetDefaultView(LogList);
            //Filter der CollectionView festlegen
            FilterView.Filter = item =>
            {
                if (String.IsNullOrEmpty(_str_FilterText))
                    return true;
                else
                {
                    ISB_BIA_Log logItem = (ISB_BIA_Log)item;
                    if (logItem.Aktion == null) logItem.Aktion = "";
                    if (logItem.Tabelle == null) logItem.Tabelle = "";
                    if (logItem.Details == null) logItem.Details = "";
                    return (
                        (logItem.Aktion.IndexOf(_str_FilterText, StringComparison.OrdinalIgnoreCase) >= 0)
                     || (logItem.Tabelle.IndexOf(_str_FilterText, StringComparison.OrdinalIgnoreCase) >= 0)
                     || (logItem.Details.IndexOf(_str_FilterText, StringComparison.OrdinalIgnoreCase) >= 0)
                     || (logItem.Datum.ToString().IndexOf(_str_FilterText, StringComparison.OrdinalIgnoreCase) >= 0)
                     || (logItem.Benutzer.IndexOf(_str_FilterText, StringComparison.OrdinalIgnoreCase) >= 0));
                }
            };
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
