using ISB_BIA_IMPORT1.LINQ2SQL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ISB_BIA_IMPORT1.Services.Interfaces
{
    public interface IDataService_Delta
    {
        #region Delta

        /// <summary>
        /// Letzte Delta-Analyse aus Datenbank abrufen (für Menü letzte Delta1)
        /// </summary>
        /// <returns> Liste mit allen bei der letzten Analyse erstellten Delta-Einträgen </returns>
        ObservableCollection<ISB_BIA_Delta_Analyse> Get_List_Delta();

        /// <summary>
        /// Menu_Window Option: Delta Analysis für beliebiges Datum
        /// </summary>
        /// <param name="d"> Datum, für das die Delta-Analyse berechnet werden soll </param>
        /// <returns> Liste der Deltaanalyse </returns>
        ObservableCollection<ISB_BIA_Delta_Analyse> Create_DeltaAnalysis(DateTime d);
        #endregion
    }
}
