using ISB_BIA_IMPORT1.LINQ2SQL;
using ISB_BIA_IMPORT1.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ISB_BIA_IMPORT1.Services.Interfaces
{
    /// <summary>
    /// Service zum Exportieren von formatierten Objektlisten nach Excel
    /// </summary>
    public interface IExportService
    {
        /// <summary>
        /// Anwendungsliste aller Anwendungen aus DB abrufen
        /// Ruft nach Abfrage <see cref="Export_Applications"/> auf
        /// </summary>
        /// <returns> Erfolgreicher Export oder nicht </returns>
        bool Export_Applications_All();
        /// <summary>
        /// Anwendungsliste aller aktiven Anwendungen aus DB abrufen
        /// Ruft nach Abfrage <see cref="Export_Applications"/> auf
        /// </summary>
        /// <returns> Erfolgreicher Export oder nicht </returns>
        bool Export_Applications_Active();
        /// <summary>
        /// Export der Historie einer einzelnen Applikation(wenn id !=0 angegeben), oder einer angegebenen Applikationsliste
        /// </summary>
        /// <param name="appList"> Zu Exportierende Anwendungsliste </param>
        /// <param name="title"> Indikator am Anfang des Dateinamen ob SBA oder Anwendungsliste </param>
        /// <param name="id"> AnwendungsID der Anwendung ("" wenn Anwendungsliste verschiedener Anwendungen, ID beim Export einer Anwednungs-Historie) </param>
        /// <returns> Erfolgreicher Export oder nicht </returns>
        bool Export_Applications(ObservableCollection<Application_Model> appList, string title, int id=0);
        /// <summary>
        /// Anwendungsliste aller aktiven Prozesse aus DB abrufen
        /// Ruft nach Abfrage <see cref="Export_Processes"/> auf
        /// </summary>
        /// <returns> Erfolgreicher Export oder nicht </returns>
        bool Export_Processes_Active();
        /// <summary>
        /// Export der Historie eines einzelnen Prozesses(wenn id !=0 angegeben), oder einer angegebenen Prozessliste
        /// </summary>
        /// <param name="procList"> Zu Exportierende Prozessliste </param>
        /// <param name="id"> ProzessID des Prozesses ("" wenn Prozessliste verschiedener Prozesse, ID beim Export einer Prozess-Historie </param>
        /// <returns> Erfolgreicher Export oder nicht </returns>
        bool Export_Processes(ObservableCollection<ISB_BIA_Prozesse> procList, int id=0);
        /// <summary>
        /// Export der Segmente und Attribute nach Excel
        /// </summary>
        /// <returns></returns>
        bool Export_IS_Attr_History();
        /// <summary>
        /// Export der Deltaanalyse (nur Einträge mit Delta in mindestens einem Wert)
        /// </summary>
        /// <param name="DeltaList"> Liste der Deltaeinträge </param>
        /// <returns> Erfolgreicher Export oder nicht </returns>
        bool Export_DeltaAnalysis(ObservableCollection<ISB_BIA_Delta_Analyse> DeltaList);
        /// <summary>
        /// Log-Export
        /// </summary>
        /// <param name="Log"> Liste aller Logeinträge </param>
        /// <returns> Erfolgreicher Export oder nicht </returns>
        bool Export_Log(ObservableCollection<ISB_BIA_Log> Log);
        /// <summary>
        /// Einstellungshistory Export
        /// </summary>
        /// <param name="Settings"> Liste aller Einstellungen </param>
        /// <returns> Erfolgreicher Export oder nicht </returns>
        bool Export_Settings(List<ISB_BIA_Settings> Settings);
    }
}
