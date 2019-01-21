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
                      myNavi.NavigateBack();
                  });
        }


        IMyNavigationService myNavi;
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="myNavigationService"></param>
        public DocumentView_ViewModel(IMyNavigationService myNavigationService)
        {
            myNavi = myNavigationService;
            // Message Registrierungen mit Überlieferung des zu betrachtenden Dokuments
            Messenger.Default.Register<FixedDocumentSequence>(this, a => { DocumentSource = a; });
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
