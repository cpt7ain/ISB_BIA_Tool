using ISB_BIA_IMPORT1.LINQ2SQL;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ISB_BIA_IMPORT1.Services
{
    /// <summary>
    /// Service zum Exportieren von formatierten Objektlisten nach Excel
    /// </summary>
    public interface IMyExportService
    {
        /// <summary>
        /// Anwendungsliste aller Anwendungen aus DB abrufen
        /// Ruft nach Abfrage <see cref="ExportApplications"/> auf
        /// </summary>
        /// <returns> Erfolgreicher Export oder nicht </returns>
        bool AllApplicationsExport();
        /// <summary>
        /// Anwendungsliste aller aktiven Anwendungen aus DB abrufen
        /// Ruft nach Abfrage <see cref="ExportApplications"/> auf
        /// </summary>
        /// <returns> Erfolgreicher Export oder nicht </returns>
        bool AllActiveApplicationsExport();
        /// <summary>
        /// Anwendungsliste nach Excel exportieren
        /// </summary>
        /// <param name="appList"> Zu Exportierende Anwendungsliste </param>
        /// <param name="title"> Indikator am Anfang des Dateinamen ob SBA oder Anwendungsliste </param>
        /// <param name="id"> AnwendungsID der Anwendung ("" wenn Anwendungsliste verschiedener Anwendungen, ID beim Export einer Anwednungs-Historie) </param>
        /// <returns> Erfolgreicher Export oder nicht </returns>
        bool ExportApplications(ObservableCollection<ISB_BIA_Applikationen> appList, string title, int id=0);
        /// <summary>
        /// Anwendungsliste aller aktiven Prozesse aus DB abrufen
        /// Ruft nach Abfrage <see cref="ExportProcesses"/> auf
        /// </summary>
        /// <returns> Erfolgreicher Export oder nicht </returns>
        bool AllActiveProcessesExport();
        /// <summary>
        /// Export einer angegebenen Prozessliste
        /// </summary>
        /// <param name="procList"> Zu Exportierende Prozessliste </param>
        /// <param name="id"> ProzessID des Prozesses ("" wenn Prozessliste verschiedener Prozesse, ID beim Export einer Prozess-Historie </param>
        /// <returns> Erfolgreicher Export oder nicht </returns>
        bool ExportProcesses(ObservableCollection<ISB_BIA_Prozesse> procList, int id=0);
        /// <summary>
        /// Export der Segmente und Attribute nach Excel
        /// </summary>
        /// <returns></returns>
        bool ExportSegmentAndAttributeHistory();
        /// <summary>
        /// Export der Deltaanalyse (nur Einträge mit Delta in mindestens einem Wert)
        /// </summary>
        /// <param name="DeltaList"> Liste der Deltaeinträge </param>
        /// <returns> Erfolgreicher Export oder nicht </returns>
        bool ExportDeltaAnalysis(ObservableCollection<ISB_BIA_Delta_Analyse> DeltaList);
        /// <summary>
        /// Log-Export
        /// </summary>
        /// <param name="Log"> Liste aller Logeinträge </param>
        /// <returns> Erfolgreicher Export oder nicht </returns>
        bool ExportLog(ObservableCollection<ISB_BIA_Log> Log);
        /// <summary>
        /// Einstellungshistory Export
        /// </summary>
        /// <param name="Settings"> Liste aller Einstellungen </param>
        /// <returns> Erfolgreicher Export oder nicht </returns>
        bool ExportSettings(List<ISB_BIA_Settings> Settings);
    }
}
