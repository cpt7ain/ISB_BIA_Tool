using ISB_BIA_IMPORT1.LINQ2SQL;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ISB_BIA_IMPORT1.Services.Interfaces
{
    /// <summary>
    /// Service zum Exportieren von formatierten Objektlisten nach Excel
    /// </summary>
    public interface IMyExportService
    {
        /// <summary>
        /// Anwendungsliste aller Anwendungen aus DB abrufen
        /// Ruft nach Abfrage <see cref="App_ExportApplications"/> auf
        /// </summary>
        /// <returns> Erfolgreicher Export oder nicht </returns>
        bool App_ExportAllApplications();
        /// <summary>
        /// Anwendungsliste aller aktiven Anwendungen aus DB abrufen
        /// Ruft nach Abfrage <see cref="App_ExportApplications"/> auf
        /// </summary>
        /// <returns> Erfolgreicher Export oder nicht </returns>
        bool App_ExportActiveApplications();
        /// <summary>
        /// Export der Historie einer einzelnen Applikation(wenn id !=0 angegeben), oder einer angegebenen Applikationsliste
        /// </summary>
        /// <param name="appList"> Zu Exportierende Anwendungsliste </param>
        /// <param name="title"> Indikator am Anfang des Dateinamen ob SBA oder Anwendungsliste </param>
        /// <param name="id"> AnwendungsID der Anwendung ("" wenn Anwendungsliste verschiedener Anwendungen, ID beim Export einer Anwednungs-Historie) </param>
        /// <returns> Erfolgreicher Export oder nicht </returns>
        bool App_ExportApplications(ObservableCollection<ISB_BIA_Applikationen> appList, string title, int id=0);
        /// <summary>
        /// Anwendungsliste aller aktiven Prozesse aus DB abrufen
        /// Ruft nach Abfrage <see cref="Proc_ExportProcesses"/> auf
        /// </summary>
        /// <returns> Erfolgreicher Export oder nicht </returns>
        bool Proc_ExportActiveProcesses();
        /// <summary>
        /// Export der Historie eines einzelnen Prozesses(wenn id !=0 angegeben), oder einer angegebenen Prozessliste
        /// </summary>
        /// <param name="procList"> Zu Exportierende Prozessliste </param>
        /// <param name="id"> ProzessID des Prozesses ("" wenn Prozessliste verschiedener Prozesse, ID beim Export einer Prozess-Historie </param>
        /// <returns> Erfolgreicher Export oder nicht </returns>
        bool Proc_ExportProcesses(ObservableCollection<ISB_BIA_Prozesse> procList, int id=0);
        /// <summary>
        /// Export der Segmente und Attribute nach Excel
        /// </summary>
        /// <returns></returns>
        bool IS_Attr_ExportSegmentAndAttributeHistory();
        /// <summary>
        /// Export der Deltaanalyse (nur Einträge mit Delta in mindestens einem Wert)
        /// </summary>
        /// <param name="DeltaList"> Liste der Deltaeinträge </param>
        /// <returns> Erfolgreicher Export oder nicht </returns>
        bool Delta_ExportDeltaAnalysis(ObservableCollection<ISB_BIA_Delta_Analyse> DeltaList);
        /// <summary>
        /// Log-Export
        /// </summary>
        /// <param name="Log"> Liste aller Logeinträge </param>
        /// <returns> Erfolgreicher Export oder nicht </returns>
        bool Log_ExportLog(ObservableCollection<ISB_BIA_Log> Log);
        /// <summary>
        /// Einstellungshistory Export
        /// </summary>
        /// <param name="Settings"> Liste aller Einstellungen </param>
        /// <returns> Erfolgreicher Export oder nicht </returns>
        bool Set_ExportSettings(List<ISB_BIA_Settings> Settings);
    }
}
