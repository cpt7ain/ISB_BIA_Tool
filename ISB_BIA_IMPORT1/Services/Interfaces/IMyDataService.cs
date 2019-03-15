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
    public enum Table_Lock_Flags1
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
    public interface IMyDataService
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
        bool DataModel_Create(DataTable dt_Processes, DataTable dt_Applications, DataTable dt_Relation, DataTable dt_InformationSegments, DataTable dt_InformationSegmentAttributes);
        /// <summary>
        /// Funktion in der die SQL Bulk Copy Konfiguration bestimmt wird
        /// Wird benutzt um Prozess/Anwendungs/IS(-Attribut) Daten aus den Datatables in die Datenbank zu schreiben 
        /// </summary>
        /// <param name="tableName"> Name der Zieltabelle </param>
        /// <param name="con"> SQL Connection </param>
        /// <param name="dt"> Datatable, der die zu schreibenden Daten enthält </param>
        void DataModel_SQLBulkCopy(string tableName, SqlConnection con, DataTable dt);
        #endregion
        #region Datensatz-Lock Operationen & co
        /// <summary>
        /// Prüfen der Datenbankverbindung
        /// </summary>
        bool Con_CheckDBConnection();
        /// <summary>
        /// Abfrage, ob ein "Objekt" gesperrt ist
        /// </summary>
        /// <param name="table_Flag"> Indikator für Art des Objektes zB "A"=Anwendung "P"=Prozess </param>
        /// <param name="id"> ID des Objektes (Prozess-ID / Applikation-ID) </param>
        /// <returns> null wenn Objekt nicht gesperrt; sonst Username, Name, Vorname des Sperrenden Nutzers </returns>
        string Lock_Get_ObjectIsLocked(Table_Lock_Flags table_Flag, int id);
        /// <summary>
        /// Methode zum Sperren eines "Objektes"
        /// </summary>
        /// <param name="table_Flag"> Tabellenindikator </param>
        /// <param name="id"> Id des Objekts in der jeweiligen Tabelle </param>
        /// <returns> true wenn Sperren erfolgreich, false bei Fehler </returns>
        bool Lock_Lock_Object(Table_Lock_Flags table_Flag, int id);
        /// <summary>
        /// Methode zum Entsperren eines "Objektes"
        /// </summary>
        /// <param name="table_Flag"> Indikator für die Tabelle des Objektes </param>
        /// <param name="id"> ID des Objektes, das gesperrt werden soll </param>
        /// <returns> true wenn Entsperren erfolgreich, false bei Fehler </returns>
        bool Lock_Unlock_Object(Table_Lock_Flags table_Flag, int id);
        /// <summary>
        /// Entsperrt alle Objekte, die durch den User auf derselben Maschine/Computer gesperrt sind
        /// Wird verwendet um bei Applikationsstart und Beendigung die Locks zu entfernen
        /// </summary>
        /// <returns> true wenn Entsperren erfolgreich, false bei Fehler </returns>
        bool Lock_Unlock_AllObjectsForUserOnMachine();
        /// <summary>
        /// Entsperrt alle Objekte (Option für Admin)
        /// </summary>
        /// <returns> true wenn Entsperren erfolgreich, false bei Fehler </returns>
        bool Lock_Unlock_AllObjects();
        #endregion
        #region Process
        /// <summary>
        /// Prozess Model aus Daten eines Datensatzes der DB erstellen
        /// </summary>
        /// <param name="id"> Prozess_Id des Prozesses </param>
        /// <returns> Prozess Model Objekt </returns>
        Process_Model Proc_Get_ProcessModelFromDB(int id);
        /// <summary>
        /// Mappen des Prozesses vom Model- in das DB-Format
        /// </summary>
        /// <param name="p"> Prozess Model Objekt </param>
        /// <returns> Prozess in Datenbankformat </returns>
        ISB_BIA_Prozesse Proc_Map_ProcessModelToDB(Process_Model p);
        /// <summary>
        /// Liste aller (aktiven) Prozesse, welche anhand der OE Zuordnungen für eine Liste von OE's relevant sind für Prozessübersicht1
        /// </summary>
        /// <param name="listOE"> Liste von OE's welche für einen User "zutreffen" </param>
        /// <returns> Liste aller (aktiven) Prozesse, welchee je nach Parameter relevant sind </returns>
        ObservableCollection<ISB_BIA_Prozesse> Proc_Get_ProcessesByOE(ObservableCollection<string> listOE);
        /// <summary>
        /// Liste aller Prozesse (Für Delta-Analyse, inaktive werden noch herausgefilter1; und für CISO/ADMIN ansicht2)
        /// </summary>
        /// <returns> Liste aller Prozesse (aktiv / inaktiv) </returns>
        ObservableCollection<ISB_BIA_Prozesse> Proc_Get_AllProcesses(DateTime? d = null);
        /// <summary>
        /// Liste aller (aktiven) Prozesse (Benutzt für ProzessCount in Menü1 und SBA Ansicht2)
        /// </summary>
        /// <returns> Liste aller (aktiven) Prozesse </returns>
        ObservableCollection<ISB_BIA_Prozesse> Proc_Get_ActiveProcesses();
        /// <summary>
        /// Liste aller momentanen Prozess Owner (für dropdown in Prozessbearbeitung1)
        /// </summary>
        /// <returns> String Liste der existierenden Prozesseigentümer </returns>
        ObservableCollection<string> Proc_Get_ListProcessOwner();
        /// <summary>
        /// Liste aller OE's welche für eine bestimmte OE(/User) relevant sind (für dropdown in Prozessbearbeitung1 und als Grundlage für bestimmung der relevanen Prozesse (=>OE anhängig) für Prozessübersicht2 <see cref="Proc_Get_ProcessesByOE"/>)
        /// </summary>
        /// <param name="userOE"> OE (e.g. 4.4) </param>
        /// <returns> Liste aller relevanten (=>untergeordneten) OE's (e.g. {4.4,4.401,4,402,4.403} ) </returns>
        ObservableCollection<string> Proc_Get_ListOEsForUser(string userOE);
        /// <summary>
        /// Liste aller OE's (für Admin/CISO dropdown1)
        /// </summary>
        /// <returns> String Liste aller OE's </returns>
        ObservableCollection<string> Proc_Get_ListOEs();
        /// <summary>
        /// Liste aller vorgelagerten Prozesse für Dropdown1
        /// </summary>
        /// <returns> String Liste aller vorgelagerten Prozesse</returns>
        ObservableCollection<string> Proc_Get_ListPreProcesses();
        /// <summary>
        /// Liste aller nachgelagerten Prozesse für Dropdown1
        /// </summary>
        /// <returns> String Liste aller nachgelagerten Prozesse </returns>
        ObservableCollection<string> Proc_Get_ListPostProcesses();
        /// <summary>
        /// Liste der Anwendungskategorien (für Dropdown-Filter in Prozess-Anwendungszuordnung1)
        /// </summary>
        /// <returns></returns>
        ObservableCollection<string> Proc_Get_ListApplicationCategories();
        /// <summary>
        /// Liste aller Versionen eines Prozesses
        /// </summary>
        /// <param name="process_id"> ID des Prozesses </param>
        /// <returns> Liste der Bearbeitungshistorie des Prozesses </returns>
        ObservableCollection<ISB_BIA_Prozesse> Proc_Get_ProcessHistory(int process_id);
        /// <summary>
        /// Einfügen einer neuen Version eines Prozesses und entsprechende Prozess-Applikation Relationen in die Datenbank
        /// </summary>
        /// <param name="p"> Einzufügender Prozess </param>
        /// <param name="mode"> Neuanlage oder Bearbeitung </param>
        /// <param name="add"> Liste der Relationen welche auf 1 gesetzt eingefügt werden sollen </param>
        /// <param name="remove"> Liste der Relationen welche auf 0 gesetzt eingefügt werden sollen </param>
        /// <returns></returns>
        bool Proc_Insert_ProcessAndRelations(Process_Model p, ProcAppMode mode, ObservableCollection<ISB_BIA_Applikationen> add, ObservableCollection<ISB_BIA_Applikationen> remove);
        /// <summary>
        /// Erstellen des Prozesseintrags und Weiterleitung an <see cref="TryDeleteProcess"/>
        /// </summary>
        /// <param name="p"> zu Löschender Prozess </param>
        /// <returns> geschriebener Prozesseintrag </returns>
        ISB_BIA_Prozesse Proc_Delete_Process(ISB_BIA_Prozesse p);
        /// <summary>
        /// Methode zum Speichern/"Aktualisieren" mehrerer Prozesse ohne Änderungen (Außer Benutzer und Datum)
        /// </summary>
        /// <param name="pList"> Liste der zu speichernden Prozesse </param>
        /// <returns> Erfolg </returns>
        bool Proc_Insert_AllProcesses(ObservableCollection<ISB_BIA_Prozesse> pList);
        /// <summary>
        /// Erstellt eine Prozess-Applikations-Historie (in Form der Delta-Analyse da die Felder passen)
        /// </summary>
        /// <param name="id"> Id des Prozesses </param>
        /// <returns> Prozess-Applikations-Historie </returns>
        ObservableCollection<ISB_BIA_Delta_Analyse> Proc_Get_HistoryProcessApplicationForProcess(int id);
        #endregion
        #region IS
        /// <summary>
        /// Informationssegment Model aus Daten eines Datensatzes der DB erstellen
        /// </summary>
        /// <param name="id"> Informationssegment_Id des Segments </param>
        /// <returns> Informationssegment Model Objekt </returns>
        InformationSegment_Model IS_Get_SegmentModelFromDB(int id);
        /// <summary>
        /// Mappen des Segments vom Model- in das DB-Format
        /// </summary>
        /// <param name="i"> Informationssegment Model Objekt </param>
        /// <returns> Segment in Datenbankformat </returns>
        ISB_BIA_Informationssegmente IS_Map_SegmentModelToDB(InformationSegment_Model i);
        /// <summary>
        /// Informationssegmentattribut Model aus Daten eines Datensatzes der DB erstellen
        /// </summary>
        /// <param name="id"> Attribut_Id des Attributs </param>
        /// <returns> Attribut Model Objekt </returns>
        InformationSegmentAttribute_Model Attr_Get_AttributeModelFromDB(int id);
        /// <summary>
        /// Mappen des Attributs vom Model- in das DB-Format
        /// </summary>
        /// <param name="ia"> Prozess Model Objekt </param>
        /// <returns> Attribut in Datenbankformat </returns>
        ISB_BIA_Informationssegmente_Attribute Attr_Map_AttributeModelToDB(InformationSegmentAttribute_Model ia);
        /// <summary>
        /// Alle Segmente für CISO/Admin Ansicht/Bearbeitung1
        /// </summary>
        /// <returns> Liste aller Informationssegmente (definierte / undefinierte("Lorem ipsum") </returns>
        ObservableCollection<ISB_BIA_Informationssegmente> IS_Get_AllSegments();
        /// <summary>
        /// Aktivierte Segmente für normale User zur Ansicht1 und Berechnung der Prozess Mindesteinstufung2
        /// </summary>
        /// <returns> Liste der aktiven Segmente </returns>
        ObservableCollection<ISB_BIA_Informationssegmente> IS_Get_EnabledSegments();
        /// <summary>
        /// Im Prozess ausgewählte Segmente abrufen
        /// </summary>
        /// <param name="process"> Aktueller Prozess </param>
        /// <returns> Liste der ausgewählten Segmente zur Berechnung der Mindesteinstufung des Prozesses </returns>
        List<ISB_BIA_Informationssegmente> IS_Get_5SegmentsForCalculation(Process_Model process);
        /// <summary>
        /// IS auswählen anhand von Dictionary Key aus Dropdown auswahl in Prozessansicht1
        /// </summary>
        /// <param name="ISName"> Name des Informationssegments (zB "IS1") </param>
        /// <returns> Informationssegment, dessen Name dem Parameter entspricht </returns>
        ISB_BIA_Informationssegmente IS_Get_ISByISName(string ISName);
        /// <summary>
        /// Informationssegment-Attribut-Namen mit Info abrufen (für IS Betrachtung/Bearbeitung)1 => korrekte Benennung der Attribute
        /// </summary>
        /// <returns> Liste von Konkatenierten Strings (IS-Attribut-Name + Info) </returns>
        ObservableCollection<string> Attr_Get_AttributeNamesAndInfoForIS();
        /// <summary>
        /// Dictionary als Quelle für die IS Dropdown aswahl in Prozessbearbeitung1
        /// </summary>
        /// <returns> Dictionary (Name, Segment) (der Informationssegmente) </returns>
        Dictionary<string, string> IS_Get_ISDropDownList();
        /// <summary>
        /// Informationssegment-Attribute abrufen
        /// </summary>
        /// <returns> Liste der 10 IS-Attribute </returns>
        ObservableCollection<ISB_BIA_Informationssegmente_Attribute> Attr_Get_Attributes();
        /// <summary>
        /// Informationssegment-Attribut-Namen abrufen für IS Übersicht Header1
        /// </summary>
        /// <returns> Liste der Attrbut-Namen für die Korrekte benennung der Header in der IS-Übersicht </returns>
        ObservableCollection<string> Attr_Get_AttributeNamesForHeader();
        /// <summary>
        /// Informationssegment Eintrag einfügen (falls Unterschied zwischen Parametern)
        /// </summary>
        /// <param name="newIS"> (eventuell) geändertes Segment </param>
        /// <param name="oldIS"> altes Segment </param>
        /// <returns> Wahrheitswert ob Einfügen erfolgreich </returns>
        bool IS_Insert_Segment(InformationSegment_Model newIS, InformationSegment_Model oldIS);
        /// <summary>
        /// Informationssegment-Attribut einfügen
        /// </summary>
        /// <param name="newAttributeList"> Liste aller Attribute, deren Elemente einzeln auf Änderungen geprüft und entsprechend in die Datenbank geschrieben werden </param>
        /// <returns> Wahrheitswert ob einfügen erfolgreich () </returns>
        bool Attr_Insert_Attribute(ObservableCollection<InformationSegmentAttribute_Model> newAttributeList);
        /// <summary>
        /// Ruft alle Segmente und Attribute (Inklusive Historischen Daten) ab um sie zu exportieren
        /// </summary>
        /// <returns> Liste aller Segmente & Attribute </returns>
        Tuple<List<ISB_BIA_Informationssegmente>, List<ISB_BIA_Informationssegmente>, List<ISB_BIA_Informationssegmente_Attribute>, List<ISB_BIA_Informationssegmente_Attribute>> IS_Attr_Get_ISAndISAttForExport();
        #endregion
        #region Application
        /// <summary>
        /// Anwendungs Model aus Daten eines Datensatzes der DB erstellen
        /// </summary>
        /// <param name="id"> Anwendungs_Id des Prozesses </param>
        /// <returns> Anwendungs Model Objekt </returns>
        Application_Model App_Get_ApplicationModelFromDB(int id);
        /// <summary>
        /// Mappen der Anwendung vom Model- in das DB-Format
        /// </summary>
        /// <param name="p"> Anwendungs Model Objekt </param>
        /// <returns> Anwendung in Datenbankformat </returns>
        ISB_BIA_Applikationen App_Map_ApplicationModelToDB(Application_Model p);
        /// <summary>
        /// Liste der Rechenzentren für Dropdown1
        /// </summary>
        /// <returns> Liste der Rechenzentren </returns>
        ObservableCollection<string> App_Get_ListRechenzentrum();
        /// <summary>
        /// Liste der Server für Dropdown1
        /// </summary>
        /// <returns> Liste der Server </returns>
        ObservableCollection<string> App_Get_ListServer();
        /// <summary>
        /// Liste der virtuellen Maschinen für Dropdown1
        /// </summary>
        /// <returns> Liste der virtuellen Maschinen </returns>
        ObservableCollection<string> App_Get_ListVirtuelle_Maschine();
        /// <summary>
        /// Liste der Typen etc für Dropdown1
        /// </summary>
        /// <returns> Liste der Typen </returns>
        ObservableCollection<string> App_Get_ListTypes();
        /// <summary>
        /// Liste der Anwendungskategorien etc für Dropdown1
        /// </summary>
        /// <returns> Liste der Anwendungskategorien </returns>
        ObservableCollection<string> App_Get_ListBetriebsart();
        /// <summary>
        /// Liste aller Applikationen (für Delta1, Anwendungsübersicht2, SBA Übersicht3)
        /// </summary>
        /// <returns> Liste aller Applikationen </returns>
        ObservableCollection<ISB_BIA_Applikationen> App_Get_AllApplications(DateTime? date = null);
        /// <summary>
        /// Liste aller aktiven Anwendungen (Aktiv==1) (für Prozess-Anwendungszuordnung1, SBA Übersicht2)
        /// </summary>
        /// <returns> Liste aller aktiven Anwendungen </returns>
        ObservableCollection<ISB_BIA_Applikationen> App_Get_ActiveApplications();
        /// <summary>
        /// Liste aller Versionen einer Anwendung
        /// </summary>
        /// <param name="application_id"> ID der Anwendung </param>
        /// <returns> Liste der Bearbeitungshistorie der Anwendung </returns>
        ObservableCollection<ISB_BIA_Applikationen> App_Get_ApplicationHistory(int application_id);
        /// <summary>
        /// Einfügen eines neuen Eintrags einer Applikation in die Datenbank
        /// </summary>
        /// <param name="a"> Anwendung die Eingefügt werden soll </param>
        /// <param name="mode"> Indikator ob es eine neue Anwendung ist, oder eine vorhandene welche geändert wird </param>
        /// <returns> Wahrheitswert für erfolgreiches Einfügen </returns>
        bool App_Insert_Application(Application_Model a, ProcAppMode mode);
        /// <summary>
        /// Löschen der ausgewählten Applikation <see cref="SelectedItem"/>
        /// </summary>
        /// <param name="a"> Zu Löschende Anwendung der Liste </param>
        /// <returns> "neue" Gelöschte Anwendung </returns>
        ISB_BIA_Applikationen App_Delete_Application(ISB_BIA_Applikationen a);
        /// <summary>
        /// Methode zum Speichern/"Aktualisieren" mehrerer Anwendungen ohne Änderungen (Außer Benutzer und Datum)
        /// </summary>
        /// <param name="aList"> Liste der zu speichernden Anwendungen </param>
        bool App_Insert_AllApplications(ObservableCollection<ISB_BIA_Applikationen> aList);
        /// <summary>
        /// Erstellt eine Prozess-Applikations-Historie (in Form der Delta-Analyse da die Felder passen)
        /// </summary>
        /// <param name="id"> Id der Anwendung </param>
        /// <returns> Prozess-Applikations-Historie </returns>
        ObservableCollection<ISB_BIA_Delta_Analyse> App_Get_HistoryProcessApplicationForApplication(int id);
        #endregion
        #region Settings
        /// <summary>
        /// Settings Model aus Daten eines Datensatzes der DB erstellen
        /// </summary>
        /// <returns> Settings Model Objekt </returns>
        Settings_Model Set_Get_SettingsModelFromDB();
        /// <summary>
        /// Mappen des Settings vom Model- in das DB-Format
        /// </summary>
        /// <param name="s"> Settings Model Objekt </param>
        /// <returns> Settings in Datenbankformat </returns>
        ISB_BIA_Settings Set_Map_SettingsModelToDB(Settings_Model s);
        /// <summary>
        /// aktuelle Einstellungen
        /// </summary>
        /// <returns> Datensatz der aktuellen Einstellungen </returns>
        ISB_BIA_Settings Set_Get_Settings();
        /// <summary>
        /// Einstellungshistorie
        /// </summary>
        /// <returns> Liste aller Einstellungen </returns>
        List<ISB_BIA_Settings> Set_Get_SettingsHistory();
        /// <summary>
        /// Speichern der Einstellungen falls keine Änderungen vorgenommen
        /// </summary>
        /// <param name="newSettings"> zu speichernde Einstellungen </param>
        /// <param name="oldSettings"> Alte Einstellungen zum Prüfen auf Änderungen </param>
        /// <returns></returns>
        bool Set_Insert_Settings(ISB_BIA_Settings newSettings, ISB_BIA_Settings oldSettings);
        #endregion
        #region Delta
        /// <summary>
        /// Letzte Delta-Analyse aus Datenbank abrufen (für Menü letzte Delta1)
        /// </summary>
        /// <returns> Liste mit allen bei der letzten Analyse erstellten Delta-Einträgen </returns>
        ObservableCollection<ISB_BIA_Delta_Analyse> Delta_Get_DeltaAnalysis();
        /// <summary>
        /// Menu_Window Option: Delta Analysis für beliebiges Datum
        /// </summary>
        /// <param name="d"> Datum, für das die Delta-Analyse berechnet werden soll </param>
        /// <returns> Liste der Deltaanalyse </returns>
        ObservableCollection<ISB_BIA_Delta_Analyse> Delta_InitiateDeltaAnalysis(DateTime d);
        /// <summary>
        /// Deltaanalyse für beliebeiges Datum
        /// es existiert hier keine Versionierung da die Daten zum Erzeugen der Analyse selbst versioniert werden        /// </summary>
        /// <param name="date"> Gewähltes Datum für Deltaanalyse </param>
        /// <param name="toDB"> Bestimmt ob Analyse in Datenbank geschrieben wird oder nicht </param>
        /// <param name="proc_App"> Liste der Prozesse-Applikations-Relationen </param>
        /// <returns></returns>
        ObservableCollection<ISB_BIA_Delta_Analyse> Delta_DateDeltaAnalysis(DateTime date, bool toDB, List<ISB_BIA_Prozesse_Applikationen> proc_App);
        #endregion
        #region OE
        /// <summary>
        /// Liste der OE-Gruppierungen (Für OE-Einstellungen Namen Dropdowns und Übersicht1)
        /// </summary>
        /// <returns> Liste der OE-Gruppierungen </returns>
        ObservableCollection<ISB_BIA_OEs> OE_Get_ListOENames();
        /// <summary>
        /// Liste der OE-Nummern (Für OE-Einstellungen Nummern Dropdowns und Übersicht1)
        /// </summary>
        /// <returns> Liste der OE-Nummern </returns>
        ObservableCollection<ISB_BIA_OEs> OE_Get_ListOENumbers();
        /// <summary>
        /// Liste der OE-Gruppierung-Nummern Relationen (Für OE-Einstellungen Relationen Dropdowns und Übersicht1)
        /// </summary>
        /// <returns> Liste der OE-Gruppierung-Nummern Relationen </returns>
        ObservableCollection<ISB_BIA_OEs> OE_Get_ListOELinks();
        /// <summary>
        /// OE Gruppierung einfügen
        /// </summary>
        /// <param name="name"> Name der OE-Gruppe </param>
        /// <returns> Eingefügte OE-Gruppe </returns>
        ISB_BIA_OEs OE_Insert_NewOEName(string name);
        /// <summary>
        /// OE-Gruppen bearbeiten
        /// </summary>
        /// <param name="name"> neuer Name </param>
        /// <param name="oldName"> alter Name </param>
        /// <returns></returns>
        bool OE_Insert_EditOEName(string name, string oldName);
        /// <summary>
        /// Löscht eine OE-Gruppe
        /// </summary>
        /// <param name="oeName"> Name der OE Gruppe </param>
        /// <returns> Erfolgreiche Löschung </returns>
        bool OE_Delete_OEName(string oeName);
        /// <summary>
        /// Löscht eine OE-Gruppen Relation
        /// </summary>
        /// <param name="oeName"> Name der Gruppe </param>
        /// <param name="oeNumber"> Nummer der OE </param>
        /// <returns> Erfolgreiche Löschung </returns>
        bool OE_Delete_OELink(string oeName, string oeNumber);
        /// <summary>
        /// Löscht eine OE (Nummer)
        /// </summary>
        /// <param name="oeNumber"> Nummer der OE </param>
        /// <returns> Erfolgreiche Löschung </returns>
        bool OE_Delete_OENumber(string oeNumber);
        /// <summary>
        /// OE-Gruppen-Zuordnung erstellen
        /// </summary>
        /// <param name="name"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        ISB_BIA_OEs OE_Insert_OELink(ISB_BIA_OEs name, ISB_BIA_OEs number);
        /// <summary>
        /// OE-Nummer erstellen (zugehörige Gruppe muss mit angegeben werden)
        /// </summary>
        /// <param name="number"> Anzulegende Nummer </param>
        /// <param name="name"> Zugehörige Gruppe </param>
        /// <returns></returns>
        ISB_BIA_OEs OE_Insert_NewOENumber(string number, ISB_BIA_OEs name);
        /// <summary>
        /// OE-Nummer ändern
        /// </summary>
        /// <param name="number"> neue Nummer </param>
        /// <param name="oldNumber"> alte Nummer </param>
        /// <returns></returns>
        bool OE_Insert_EditOENumber(string number, string oldNumber);
        #endregion
        #region Log
        /// <summary>
        /// Log abrufen (für Log Ansicht / Export1)
        /// </summary>
        /// <returns> Log-Liste </returns>
        ObservableCollection<ISB_BIA_Log> Log_Get_Log();
        #endregion    
    }
}
