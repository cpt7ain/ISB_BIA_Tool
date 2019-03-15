using ISB_BIA_IMPORT1.Model;
using ISB_BIA_IMPORT1.LINQ2SQL;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ISB_BIA_IMPORT1.ViewModel;
using System.Data;
using System.Data.SqlClient;
using System;

namespace ISB_BIA_IMPORT1.Services.Interfaces
{
    public enum Table_Lock_Flags
    {
        Process,
        Application,
        Segment,
        Attributes,
        Settings,
        OEs
    }

    /// <summary>
    /// Service zum Abrufen der für die Anwendung benötigten Daten aus der Datenbank
    /// </summary>
    public interface IMyDataService_Lock
    {
        /// <summary>
        /// Prüfen der Datenbankverbindung
        /// </summary>
        bool CheckDBConnection();
        #region Datensatz-Lock Operationen
        /// <summary>
        /// Abfrage, ob ein "Objekt" gesperrt ist
        /// </summary>
        /// <param name="table_Flag"> Indikator für Art des Objektes </param>
        /// <param name="id"> ID des Objektes (Prozess-ID / Applikation-ID) </param>
        /// <returns> null wenn Objekt nicht gesperrt; sonst Username, Name, Vorname des Sperrenden Nutzers </returns>
        string Get_ObjectIsLocked(Table_Lock_Flags table_Flag, int id);
        /// <summary>
        /// Methode zum Sperren eines "Objektes"
        /// </summary>
        /// <param name="table_Flag"> Tabellenindikator </param>
        /// <param name="id"> Id des Objekts in der jeweiligen Tabelle </param>
        /// <returns> true wenn Sperren erfolgreich, false bei Fehler </returns>
        bool Lock_Object(Table_Lock_Flags table_Flag, int id);
        /// <summary>
        /// Methode zum Entsperren eines "Objektes"
        /// </summary>
        /// <param name="table_Flag"> Indikator für die Tabelle des Objektes </param>
        /// <param name="id"> ID des Objektes, das gesperrt werden soll </param>
        /// <returns> true wenn Entsperren erfolgreich, false bei Fehler </returns>
        bool Unlock_Object(Table_Lock_Flags table_Flag, int id);
        /// <summary>
        /// Entsperrt alle Objekte, die durch den User auf derselben Maschine/Computer gesperrt sind
        /// Wird verwendet um bei Applikationsstart und Beendigung die Locks zu entfernen
        /// </summary>
        /// <returns> true wenn Entsperren erfolgreich, false bei Fehler </returns>
        bool Unlock_AllObjectsForUserOnMachine();
        /// <summary>
        /// Entsperrt alle Objekte (Option für Admin)
        /// </summary>
        /// <returns> true wenn Entsperren erfolgreich, false bei Fehler </returns>
        bool Unlock_AllObjects();
        #endregion  
    }
}
