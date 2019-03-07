using Microsoft.Win32;
using System;
using System.Windows;
using ISB_BIA_IMPORT1.Services.Interfaces;

namespace ISB_BIA_IMPORT1.Services
{
    public class MyDialogService : IMyDialogService
    {
        public void ShowError(string Message, Exception Error)
        {
            MessageBox.Show(Message +"\n"+ Error.Message.ToString(), "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ShowError(string Message)
        {
            MessageBox.Show(Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ShowInfo(string Message)
        {
            MessageBox.Show(Message, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ShowMessage(string Message)
        {
            MessageBox.Show(Message, "Hinweis", MessageBoxButton.OK);
        }

        public bool ShowQuestion(string Message, string Title)
        {
            return MessageBox.Show(Message, Title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

        public void ShowWarning(string Message)
        {
            MessageBox.Show(Message, "Warnung", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public bool CancelDecision()
        {
            MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
            MessageBoxResult rsltMessageBox = MessageBox.Show("Möchten Sie den Vorgang wirklich abbrechen?", "Bestätigen", btnMessageBox);
            switch (rsltMessageBox)
            {
                case MessageBoxResult.Yes:
                    return true;
                default:
                    return false;
            }
        }

        public bool? Open(OpenFileDialog ofd)
        {
            return ofd.ShowDialog();
        }

        public bool? Save(SaveFileDialog sfd)
        {
            return sfd.ShowDialog();
        }
    }
}
