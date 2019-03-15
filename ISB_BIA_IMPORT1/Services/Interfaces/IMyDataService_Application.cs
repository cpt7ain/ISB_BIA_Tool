using ISB_BIA_IMPORT1.Model;
using ISB_BIA_IMPORT1.LINQ2SQL;
using System.Collections.ObjectModel;
using ISB_BIA_IMPORT1.ViewModel;
using System;

namespace ISB_BIA_IMPORT1.Services.Interfaces
{
    /// <summary>
    /// Service zum Abrufen der für die Anwendung benötigten Daten aus der Datenbank
    /// </summary>
    public interface IMyDataService_Application
    {
        #region Application
        /// <summary>
        /// Anwendungs Model aus Daten eines Datensatzes der DB erstellen
        /// </summary>
        /// <param name="id"> Anwendungs_Id des Prozesses </param>
        /// <returns> Anwendungs Model Objekt </returns>
        Application_Model Get_ModelFromDB(int id);
        /// <summary>
        /// Mappen der Anwendung vom Model- in das DB-Format
        /// </summary>
        /// <param name="p"> Anwendungs Model Objekt </param>
        /// <returns> Anwendung in Datenbankformat </returns>
        ISB_BIA_Applikationen Map_ModelToDB(Application_Model p);
        /// <summary>
        /// Liste der Rechenzentren für Dropdown1
        /// </summary>
        /// <returns> Liste der Rechenzentren </returns>
        ObservableCollection<string> Get_List_Rechenzentrum();
        /// <summary>
        /// Liste der Server für Dropdown1
        /// </summary>
        /// <returns> Liste der Server </returns>
        ObservableCollection<string> Get_List_Server();
        /// <summary>
        /// Liste der virtuellen Maschinen für Dropdown1
        /// </summary>
        /// <returns> Liste der virtuellen Maschinen </returns>
        ObservableCollection<string> Get_List_Virtuelle_Maschine();
        /// <summary>
        /// Liste der Typen etc für Dropdown1
        /// </summary>
        /// <returns> Liste der Typen </returns>
        ObservableCollection<string> Get_List_Types();
        /// <summary>
        /// Liste der Anwendungskategorien etc für Dropdown1
        /// </summary>
        /// <returns> Liste der Anwendungskategorien </returns>
        ObservableCollection<string> Get_List_Betriebsart();
        /// <summary>
        /// Liste aller Applikationen (für Delta1, Anwendungsübersicht2, SBA Übersicht3)
        /// </summary>
        /// <returns> Liste aller Applikationen </returns>
        ObservableCollection<ISB_BIA_Applikationen> Get_Applications_All(DateTime? date = null);
        /// <summary>
        /// Liste aller aktiven Anwendungen (Aktiv==1) (für Prozess-Anwendungszuordnung1, SBA Übersicht2)
        /// </summary>
        /// <returns> Liste aller aktiven Anwendungen </returns>
        ObservableCollection<ISB_BIA_Applikationen> Get_Applications_Active();
        /// <summary>
        /// Liste aller Versionen einer Anwendung
        /// </summary>
        /// <param name="application_id"> ID der Anwendung </param>
        /// <returns> Liste der Bearbeitungshistorie der Anwendung </returns>
        ObservableCollection<ISB_BIA_Applikationen> Get_History_Application(int application_id);
        /// <summary>
        /// Einfügen eines neuen Eintrags einer Applikation in die Datenbank
        /// </summary>
        /// <param name="a"> Anwendung die Eingefügt werden soll </param>
        /// <param name="mode"> Indikator ob es eine neue Anwendung ist, oder eine vorhandene welche geändert wird </param>
        /// <returns> Wahrheitswert für erfolgreiches Einfügen </returns>
        bool Insert_Application(Application_Model a, ProcAppMode mode);
        /// <summary>
        /// Löschen der ausgewählten Applikation <see cref="SelectedItem"/>
        /// </summary>
        /// <param name="a"> Zu Löschende Anwendung der Liste </param>
        /// <returns> "neue" Gelöschte Anwendung </returns>
        ISB_BIA_Applikationen Delete_Application(ISB_BIA_Applikationen a);
        /// <summary>
        /// Methode zum Speichern/"Aktualisieren" mehrerer Anwendungen ohne Änderungen (Außer Benutzer und Datum)
        /// </summary>
        /// <param name="aList"> Liste der zu speichernden Anwendungen </param>
        bool Insert_AllApplications(ObservableCollection<ISB_BIA_Applikationen> aList);
        /// <summary>
        /// Erstellt eine Prozess-Applikations-Historie (in Form der Delta-Analyse da die Felder passen)
        /// </summary>
        /// <param name="id"> Id der Anwendung </param>
        /// <returns> Prozess-Applikations-Historie </returns>
        ObservableCollection<ISB_BIA_Delta_Analyse> Get_History_ProcAppRelations(int id);
        #endregion
    }
}
