using System.Diagnostics;
using System.IO;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Documents;
using ISB_BIA_IMPORT1.Helpers;
using ISB_BIA_IMPORT1.Services.Interfaces;

namespace ISB_BIA_IMPORT1.ViewModel
{
    /// <summary>
    /// VM zur Darstellung des Infofensters
    /// </summary>
    public class DocumentView_ViewModel : ViewModelBase
    {
        private FixedDocumentSequence _documentSource;
        private string _filename;

        /// <summary>
        /// Zu betrachtendes Dokument
        /// </summary>
        public FixedDocumentSequence DocumentSource
        {
            get => _documentSource;
            set => Set(() => DocumentSource, ref _documentSource, value);
        }

        /// <summary>
        /// Command zum Zurückkehren zum vorherigen Viewmodel
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
        /// Command zum Zurückkehren zum vorherigen Viewmodel
        /// </summary>
        public MyRelayCommand OpenExtern
        {
            get => new MyRelayCommand(() =>
            {
                if (File.Exists(_filename))
                {
                    Process.Start(_filename);
                    Cleanup();
                    _myNavi.NavigateBack();
                }
                else
                {
                    _myDia.ShowInfo("Keine Externe Beschreibung verfügbar");
                }
            });
        }

        #region Services
        private readonly IMyNavigationService _myNavi;
        private readonly IMyDialogService _myDia;
        #endregion

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="myNavigationService"></param>
        /// <param name="myDialogService"></param>
        public DocumentView_ViewModel(IMyNavigationService myNavigationService, IMyDialogService myDialogService)
        {
            _myNavi = myNavigationService;
            _myDia = myDialogService;
            // Message Registrierungen mit Überlieferung des zu betrachtenden Dokuments
            MessengerInstance.Register<NotificationMessage<FixedDocumentSequence>>(this, a =>
            {
                DocumentSource = a.Content;
                if (a.Sender is Menu_ViewModel)
                    _filename = a.Notification;
            });

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
