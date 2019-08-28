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
    /// <summary>
    /// Service zum Abrufen der für die Anwendung benötigten Daten aus der Datenbank
    /// </summary>
    public interface IDataService_Setting
    {
        #region Settings
        /// <summary>
        /// Settings Model aus Daten eines Datensatzes der DB erstellen
        /// </summary>
        /// <returns> Settings Model Objekt </returns>
        Settings_Model Get_Model_FromDB();
        /// <summary>
        /// Mappen des Settings vom Model- in das DB-Format
        /// </summary>
        /// <param name="s"> Settings Model Objekt </param>
        /// <returns> Settings in Datenbankformat </returns>
        ISB_BIA_Settings Map_Model_ToDB(Settings_Model s);
        /// <summary>
        /// aktuelle Einstellungen
        /// </summary>
        /// <returns> Datensatz der aktuellen Einstellungen </returns>
        ISB_BIA_Settings Get_List_Settings();
        /// <summary>
        /// Einstellungshistorie
        /// </summary>
        /// <returns> Liste aller Einstellungen </returns>
        List<ISB_BIA_Settings> Get_History_Settings();
        /// <summary>
        /// Speichern der Einstellungen falls keine Änderungen vorgenommen
        /// </summary>
        /// <param name="newSettings"> zu speichernde Einstellungen </param>
        /// <param name="oldSettings"> Alte Einstellungen zum Prüfen auf Änderungen </param>
        /// <returns></returns>
        bool Insert_Settings(Settings_Model newSettings, Settings_Model oldSettings);
        #endregion
   
    }
}
