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
    public interface IDataService_Process
    {
        #region Process
        /// <summary>
        /// Prozess Model aus Daten eines Datensatzes der DB erstellen
        /// </summary>
        /// <param name="id"> Prozess_Id des Prozesses </param>
        /// <returns> Prozess Model Objekt </returns>
        Process_Model Get_Model_FromDB(int id);
        /// <summary>
        /// Mappen des Prozesses vom Model- in das DB-Format
        /// </summary>
        /// <param name="p"> Prozess Model Objekt </param>
        /// <returns> Prozess in Datenbankformat </returns>
        ISB_BIA_Prozesse Map_Model_ToDB(Process_Model p);
        /// <summary>
        /// Liste aller (aktiven) Prozesse, welche anhand der OE Zuordnungen für eine Liste von OE's relevant sind für Prozessübersicht1
        /// </summary>
        /// <param name="listOE"> Liste von OE's welche für einen User "zutreffen" </param>
        /// <returns> Liste aller (aktiven) Prozesse, welchee je nach Parameter relevant sind </returns>
        ObservableCollection<ISB_BIA_Prozesse> Get_List_Processes_ByOE_All(ObservableCollection<string> listOE);
        /// <summary>
        /// Liste aller Prozesse (Für Delta-Analyse, inaktive werden noch herausgefilter1; und für CISO/ADMIN ansicht2)
        /// </summary>
        /// <returns> Liste aller Prozesse (aktiv / inaktiv) </returns>
        ObservableCollection<ISB_BIA_Prozesse> Get_List_Processes_All(DateTime? d = null);
        /// <summary>
        /// Liste aller (aktiven) Prozesse (Benutzt für ProzessCount in Menü1 und SBA Ansicht2)
        /// </summary>
        /// <returns> Liste aller (aktiven) Prozesse </returns>
        ObservableCollection<ISB_BIA_Prozesse> Get_List_Processes_Active();
        /// <summary>
        /// Dictionary als Quelle für die IS Dropdown aswahl in Prozessbearbeitung1
        /// </summary>
        /// <returns> Dictionary (Name, Segment) (der Informationssegmente) </returns>
        Dictionary<string, string> Get_StringList_ISDictionary();
        /// <summary>
        /// IS auswählen anhand von Dictionary Key aus Dropdown auswahl in Prozessansicht1
        /// </summary>
        /// <param name="ISName"> Name des Informationssegments (zB "IS1") </param>
        /// <returns> Informationssegment, dessen Name dem Parameter entspricht </returns>
        ISB_BIA_Informationssegmente Get_IS_ByName(string ISName);
        /// <summary>
        /// Im Prozess ausgewählte Segmente abrufen
        /// </summary>
        /// <param name="process"> Aktueller Prozess </param>
        /// <returns> Liste der ausgewählten Segmente zur Berechnung der Mindesteinstufung des Prozesses </returns>
        List<ISB_BIA_Informationssegmente> Get_List_Segments_5ForCalculation(Process_Model process);
        /// <summary>
        /// Liste aller momentanen Prozess Owner (für dropdown in Prozessbearbeitung1)
        /// </summary>
        /// <returns> String Liste der existierenden Prozesseigentümer </returns>
        ObservableCollection<string> Get_StringList_ProcessResponsible();
        /// <summary>
        /// Liste aller OE's welche für eine bestimmte OE(/User) relevant sind (für dropdown in Prozessbearbeitung1 und als Grundlage für bestimmung der relevanen Prozesse (=>OE anhängig) für Prozessübersicht2 <see cref="Get_List_Processes_ByOE"/>)
        /// </summary>
        /// <param name="userOE"> OE (e.g. 4.4) </param>
        /// <returns> Liste aller relevanten (=>untergeordneten) OE's (e.g. {4.4,4.401,4,402,4.403} ) </returns>
        ObservableCollection<string> Get_StringList_OEsForUser(List<string> userOE);
        /// <summary>
        /// Liste aller OE's (für Admin/CISO dropdown1)
        /// </summary>
        /// <returns> String Liste aller OE's </returns>
        ObservableCollection<string> Get_StringList_OEs_All();
        /// <summary>
        /// Liste aller Versionen eines Prozesses
        /// </summary>
        /// <param name="process_id"> ID des Prozesses </param>
        /// <returns> Liste der Bearbeitungshistorie des Prozesses </returns>
        ObservableCollection<ISB_BIA_Prozesse> Get_History_Process(int process_id);
        /// <summary>
        /// Einfügen einer neuen Version eines Prozesses und entsprechende Prozess-Applikation Relationen in die Datenbank
        /// </summary>
        /// <param name="p"> Einzufügender Prozess </param>
        /// <param name="mode"> Neuanlage oder Bearbeitung </param>
        /// <param name="add_Applications"> Liste der Relationen welche auf 1 gesetzt eingefügt werden sollen </param>
        /// <param name="remove_Applications"> Liste der Relationen welche auf 0 gesetzt eingefügt werden sollen </param>
        /// <returns></returns>
        bool Insert_ProcessAndRelations(Process_Model p, ProcAppMode mode, ObservableCollection<ISB_BIA_Applikationen> add_Applications, ObservableCollection<ISB_BIA_Applikationen> remove_Applications, ObservableCollection<ISB_BIA_Prozesse> add_vP, ObservableCollection<ISB_BIA_Prozesse> remove_vP, ObservableCollection<ISB_BIA_Prozesse> add_nP, ObservableCollection<ISB_BIA_Prozesse> remove_nP);
        /// <summary>
        /// Erstellen des Prozesseintrags
        /// </summary>
        /// <param name="p"> zu Löschender Prozess </param>
        /// <returns> geschriebener Prozesseintrag </returns>
        ISB_BIA_Prozesse Delete_Process(ISB_BIA_Prozesse p);
        /// <summary>
        /// Erstellen des Prozesseintrags
        /// </summary>
        /// <param name="p"> zu Reaktivierender Prozess </param>
        /// <returns> geschriebener Prozesseintrag </returns>
        ISB_BIA_Prozesse Reactivate_Process(ISB_BIA_Prozesse p);
        /// <summary>
        /// Methode zum Speichern/"Aktualisieren" mehrerer Prozesse ohne Änderungen (Außer Benutzer und Datum)
        /// </summary>
        /// <param name="pList"> Liste der zu speichernden Prozesse </param>
        /// <returns> Erfolg </returns>
        bool Insert_Processes_All(ObservableCollection<ISB_BIA_Prozesse> pList);
        #endregion
        #region Process<-->Application
        /// <summary>
        /// Liste aller aktiven Anwendungen (Aktiv==1) (für Prozess-Anwendungszuordnung1, SBA Übersicht2)
        /// </summary>
        /// <returns> Liste aller aktiven Anwendungen </returns>
        ObservableCollection<ISB_BIA_Applikationen> Get_List_Applications_Active();
        /// <summary>
        /// Liste der Anwendungskategorien (für Dropdown-Filter in Prozess-Anwendungszuordnung1)
        /// </summary>
        /// <returns></returns>
        ObservableCollection<string> Get_StringList_AppCategories();
        /// <summary>
        /// Erstellt eine Prozess-Applikations-Historie (in Form der Delta-Analyse da die Felder passen)
        /// </summary>
        /// <param name="id"> Id des Prozesses </param>
        /// <returns> Prozess-Applikations-Historie </returns>
        ObservableCollection<ISB_BIA_Delta_Analyse> Get_History_ProcAppRelations(int id);
        /// <summary>
        /// Erstellt eine Prozess-Applikations-Historie (in Form der Delta-Analyse da die Felder passen)
        /// </summary>
        /// <param name="id"> Id des Prozesses </param>
        /// <returns> Prozess-Applikations-Historie </returns>
        ObservableCollection<ISB_BIA_Delta_Analyse> Get_History_PreProcRelations(int id);
        /// <summary>
        /// Erstellt eine Prozess-Applikations-Historie (in Form der Delta-Analyse da die Felder passen)
        /// </summary>
        /// <param name="id"> Id des Prozesses </param>
        /// <returns> Prozess-Applikations-Historie </returns>
        ObservableCollection<ISB_BIA_Delta_Analyse> Get_History_PostProcRelations(int id);
        #endregion   
    }
}
