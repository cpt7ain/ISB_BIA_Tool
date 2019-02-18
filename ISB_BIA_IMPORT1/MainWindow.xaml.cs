using GalaSoft.MvvmLight.Messaging;
using ISB_BIA_IMPORT1.ViewModel;
using System.Windows;

namespace ISB_BIA_IMPORT1
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string msg = "Möchten Sie die Anwendung wirklich schließen?\nAlle nicht gespeicherten Änderungen gehen verloren.";
            MessageBoxResult result =
              MessageBox.Show(
                msg,
                "Schließen",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                Messenger.Default.Send("Close", MessageToken.WindowClosingRequest);
            }
            else
            {
                e.Cancel = true;
            }
        }
    }
}
