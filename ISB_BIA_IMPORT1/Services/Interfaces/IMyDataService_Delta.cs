using ISB_BIA_IMPORT1.LINQ2SQL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ISB_BIA_IMPORT1.Services.Interfaces
{
    public interface IMyDataService_Delta
    {
        #region Delta

        /// <summary>
        /// Letzte Delta-Analyse aus Datenbank abrufen (für Menü letzte Delta1)
        /// </summary>
        /// <returns> Liste mit allen bei der letzten Analyse erstellten Delta-Einträgen </returns>
        ObservableCollection<ISB_BIA_Delta_Analyse> Get_DeltaAnalysis();

        /// <summary>
        /// Menu_Window Option: Delta Analysis für beliebiges Datum
        /// </summary>
        /// <param name="d"> Datum, für das die Delta-Analyse berechnet werden soll </param>
        /// <returns> Liste der Deltaanalyse </returns>
        ObservableCollection<ISB_BIA_Delta_Analyse> Initiate_DeltaAnalysis(DateTime d);

        /// <summary>
        /// Deltaanalyse für beliebeiges Datum
        /// es existiert hier keine Versionierung da die Daten zum Erzeugen der Analyse selbst versioniert werden        /// </summary>
        /// <param name="date"> Gewähltes Datum für Deltaanalyse </param>
        /// <param name="toDB"> Bestimmt ob Analyse in Datenbank geschrieben wird oder nicht </param>
        /// <param name="proc_App"> Liste der Prozesse-Applikations-Relationen </param>
        /// <returns></returns>
        ObservableCollection<ISB_BIA_Delta_Analyse> ComputeAndGet_DeltaAnalysis_Date(DateTime date, bool toDB, List<ISB_BIA_Prozesse_Applikationen> proc_App);
        #endregion
    }
}
