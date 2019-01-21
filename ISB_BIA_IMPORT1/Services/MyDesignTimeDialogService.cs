using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace ISB_BIA_IMPORT1.Services
{
    public class MyDesignTimeDialogService : IMyDialogService
    {
        public bool CancelDecision()
        {
            return true;
        }

        public bool? Open(OpenFileDialog ofd)
        {
            return true;
        }

        public bool? Save(SaveFileDialog svd)
        {
            return true;
        }

        public void ShowError(string Message, Exception Error)
        {
        }

        public void ShowError(string Message)
        {
        }

        public void ShowInfo(string Message)
        {
        }

        public void ShowMessage(string Message)
        {
        }

        public bool ShowQuestion(string Message, string Title)
        {
            return true;
        }

        public void ShowWarning(string Message)
        {
        }
    }
}
