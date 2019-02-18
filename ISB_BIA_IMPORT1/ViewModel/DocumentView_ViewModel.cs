using System.Diagnostics;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using ISB_BIA_IMPORT1.Services;
using System.Windows.Documents;

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
                Process.Start(_filename);
                Cleanup();
                _myNavi.NavigateBack();
            });
        }

        IMyNavigationService _myNavi;
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="myNavigationService"></param>
        public DocumentView_ViewModel(IMyNavigationService myNavigationService)
        {
            _myNavi = myNavigationService;
            // Message Registrierungen mit Überlieferung des zu betrachtenden Dokuments
            Messenger.Default.Register<FixedDocumentSequence>(this, a => { DocumentSource = a; });
            Messenger.Default.Register<string>(this, a => { _filename = a; });

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
