using System.Windows;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Interop;

namespace ISB_BIA_IMPORT1.Helpers
{
    public enum MessageType
    {
        Ok,
        OkCancel,
        YesNo,
    }
    /// <summary>
    /// Interaktionslogik für MsgWindow.xaml
    /// </summary>
    public partial class MsgWindow : Window
    {
        public MsgWindow(string caption, string msg, Icon img, MessageType t)
        {
            Bitmap b = img.ToBitmap();
            this.Title = caption;
            BitmapSource bs = Imaging.CreateBitmapSourceFromHIcon(img.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            MsgImg.Source = bs;
            MsgBlock.Text = msg;
            Window mainWindow = Application.Current.MainWindow;
            this.Left = mainWindow.Left + (mainWindow.Width - this.ActualWidth) / 2;
            this.Top = mainWindow.Top + (mainWindow.Height - this.ActualHeight) / 2;
            InitializeComponent();
        }
    }
}
