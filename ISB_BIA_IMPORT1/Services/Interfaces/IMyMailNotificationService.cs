
namespace ISB_BIA_IMPORT1.Services.Interfaces
{
    /// <summary>
    /// Service zum Senden von Email Benachrichtigungen
    /// </summary>
    public interface IMyMailNotificationService
    {

        /// <summary>
        /// Funktion zum Senden von Benachrichtigugs-Emails für bestimmte Anwendungs-Interaktionen (z.B. Prozessbearbeitung)
        /// </summary>
        /// <param name="subject"> Betreff der Email </param>
        /// <param name="body"> Nachricht der Email </param>
        /// <param name="test"> Variable für Testzewcke; Funktion sendet keine Mail wenn true sondern zeigt nur Message </param>
        void Send_NotificationMail(string subject, string body, Current_Environment ce);
    }
}
