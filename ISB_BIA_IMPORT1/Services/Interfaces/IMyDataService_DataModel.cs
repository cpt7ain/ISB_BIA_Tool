using System.Data;
using System.Data.SqlClient;

namespace ISB_BIA_IMPORT1.Services.Interfaces
{
    public interface IMyDataService_DataModel
    {
        #region Datenmodell erstellen
        /// <summary>
        /// Datenbankoperationen bei Erstellung des Datenmodells (Tabellen, Daten)
        /// </summary>
        /// <param name="dt_Processes"> Datatable der die Prozessdaten hält </param>
        /// <param name="dt_Applications"> Datatable der die Anwendungsdaten hält</param>
        /// <param name="dt_Relation"> Datatable der die Relationsdaten hält </param>
        /// <param name="dt_InformationSegments"> Datatable der die Segmentdaten hält </param>
        /// <param name="dt_InformationSegmentAttributes"> Datatable der die Attributdaten hält </param>
        /// <returns></returns>
        bool Create(DataTable dt_Processes, DataTable dt_Applications, DataTable dt_Relation, DataTable dt_InformationSegments, DataTable dt_InformationSegmentAttributes);
        #endregion
    }
}
