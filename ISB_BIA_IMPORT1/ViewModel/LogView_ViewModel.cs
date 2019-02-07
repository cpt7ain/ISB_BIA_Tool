using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using ISB_BIA_IMPORT1.LinqDataContext;
using ISB_BIA_IMPORT1.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace ISB_BIA_IMPORT1.ViewModel
{
    /// <summary>
    /// VM zur Darstellung des Logs
    /// </summary>
    public class LogView_ViewModel : ViewModelBase
    {
        #region Backing Fields
        private ObservableCollection<ISB_BIA_Log> _logList;
        private CollectionView _view;
        private string _filterText;
        private MyRelayCommand _exportLog;
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
        public CollectionView View
        {
            get => _view;
            set => Set(() => View, ref _view, value);
        }

        /// <summary>
        /// Text, nach dem im <see cref="View"/> gefiltert wird
        /// </summary>
        public string FilterText
        {
            get => _filterText;
            set
            {
                Set(() => FilterText, ref _filterText, value);
                View.Refresh();
            }
        }

        /// <summary>
        /// Command zum Zurückkehren zum vorherigen VM
        /// </summary>
        public MyRelayCommand NavBack
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
        public MyRelayCommand ExportLog
        {
            get => _exportLog
                  ?? (_exportLog = new MyRelayCommand(() =>
                  {
                      _myExport.ExportLog(LogList);
                  }));
        }

        #region Services
        IMyNavigationService _myNavi;
        IMyExportService _myExport;
        IMyDataService _myData;
        #endregion

        /// <summary>
        /// Viewmodel für die Darstellung des Anwendungs-Logs
        /// </summary>
        /// <param name="myNavigationService"> <see cref="IMyNavigationService"/> </param>
        /// <param name="myExportService"><see cref="IMyExportService"/> </param>
        /// <param name="myDataService"><see cref="IMyDataService"/> </param>
        public LogView_ViewModel(IMyNavigationService myNavigationService, IMyExportService myExportService, IMyDataService myDataService)
        {
            #region Services
            _myNavi = myNavigationService;
            _myExport = myExportService;
            _myData = myDataService;
            #endregion
            //Log abrufen
            LogList = _myData.GetLog();
            //Definieren der Quelle für den CollectionView (=> Log Liste)
            View = (CollectionView)CollectionViewSource.GetDefaultView(LogList);
            //Filter der CollectionView festlegen
            View.Filter = item =>
            {
                if (String.IsNullOrEmpty(_filterText))
                    return true;
                else
                {
                    ISB_BIA_Log logItem = (ISB_BIA_Log)item;
                    if (logItem.Action == null) logItem.Action = "";
                    if (logItem.Tabelle == null) logItem.Tabelle = "";
                    if (logItem.Details == null) logItem.Details = "";
                    return (
                        (logItem.Action.IndexOf(_filterText, StringComparison.OrdinalIgnoreCase) >= 0)
                     || (logItem.Tabelle.IndexOf(_filterText, StringComparison.OrdinalIgnoreCase) >= 0)
                     || (logItem.Details.IndexOf(_filterText, StringComparison.OrdinalIgnoreCase) >= 0)
                     || (logItem.Datum.ToString().IndexOf(_filterText, StringComparison.OrdinalIgnoreCase) >= 0)
                     || (logItem.Benutzer.IndexOf(_filterText, StringComparison.OrdinalIgnoreCase) >= 0));
                }
            };
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
