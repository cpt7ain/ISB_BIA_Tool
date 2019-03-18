using Microsoft.Win32;
using System;

namespace ISB_BIA_IMPORT1.Services.Interfaces
{
    /// <summary>
    /// Service zum anzeigen von Dialogfenstern und Entscheidungsfenstern
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// Zeigt einen Dialog, der auf einen Fehler hinweist
        /// </summary>
        /// <param name="Message"> Fehlernachricht </param>
        /// <param name="Error"> Exception, die gecatcht wurde </param>
        void ShowError(string Message, Exception Error);
        /// <summary>
        /// Zeigt einen Dialog, dass ein Fehler aufgetreten ist
        /// </summary>
        /// <param name="Message"> Fehlernachricht </param>
        void ShowError(string Message);
        /// <summary>
        /// Zeigt eine Infonachricht
        /// </summary>
        /// <param name="Message"> Nachricht </param>
        void ShowInfo(string Message);
        /// <summary>
        /// Zeigt eine Nachrachricht
        /// </summary>
        /// <param name="Message"> Nachricht </param>
        void ShowMessage(string Message);
        /// <summary>
        /// Zeigt eine Nachricht, welche eine Frage enthält die mit einem Wahrheitswert beantortet werden kann
        /// </summary>
        /// <param name="Message"> Frage </param>
        /// <param name="Title"> Titel </param>
        /// <returns> Wahrheitswert als Ergebnis der Frage </returns>
        bool ShowQuestion(string Message, string Title);
        /// <summary>
        /// Zeigt eine Warnungsnachricht
        /// </summary>
        /// <param name="Message"> Warnungsnachricht </param>
        void ShowWarning(string Message);
        /// <summary>
        /// Zeigt eine Nachricht, welche eine Frage ob ein Vorgang abgebrochen werden soll
        /// </summary>
        /// <returns> Wahrheitswert ob Vorgang fortgesetzt oder abgebrochen werden soll </returns>
        bool CancelDecision();
        /// <summary>
        /// Öffnet ein Dialogfeld eines OpenFileDialogs, um eine Datei auszuwählen die geöffnet werden soll
        /// </summary>
        /// <param name="ofd"> OpenFileDialog </param>
        /// <returns> Wahrheitswert ob File ausgewählt wurde oder der Vorgang abgebrochen wurde </returns>
        bool? Open(OpenFileDialog ofd);
        /// <summary>
        /// Öffnet ein Dialogfeld eines SaveFileDialogs, um einen Dateinamen auszuwählen unter dem eine Datei gespeichert werden soll
        /// </summary>
        /// <param name="sfd"></param>
        /// <returns> Wahrheitswert ob File ausgewählt wurde oder der Vorgang abgebrochen wurde </returns>
        bool? Save(SaveFileDialog sfd);
    }
}
