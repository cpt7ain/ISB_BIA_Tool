using ISB_BIA_IMPORT1.LINQ2SQL;
using ISB_BIA_IMPORT1.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ISB_BIA_IMPORT1.Services.Interfaces
{
    public interface IDataService_Segment
    {
        /// <summary>
        /// Informationssegment Model aus Daten eines Datensatzes der DB erstellen
        /// </summary>
        /// <param name="id"> Informationssegment_Id des Segments </param>
        /// <returns> Informationssegment Model Objekt </returns>
        Segment_Model Get_Model_FromDB(int id);
        /// <summary>
        /// Mappen des Segments vom Model- in das DB-Format
        /// </summary>
        /// <param name="i"> Informationssegment Model Objekt </param>
        /// <returns> Segment in Datenbankformat </returns>
        ISB_BIA_Informationssegmente Map_Model_ToDB(Segment_Model i);
        /// <summary>
        /// Alle Segmente für CISO/Admin Ansicht/Bearbeitung1
        /// </summary>
        /// <returns> Liste aller Informationssegmente (definierte / undefinierte("Lorem ipsum") </returns>
        ObservableCollection<ISB_BIA_Informationssegmente> Get_List_Segments_All();
        /// <summary>
        /// Aktivierte Segmente für normale User zur Ansicht1 und Berechnung der Prozess Mindesteinstufung2
        /// </summary>
        /// <returns> Liste der aktiven Segmente </returns>
        ObservableCollection<ISB_BIA_Informationssegmente> Get_List_Segments_Enabled();
        /// <summary>
        /// Informationssegment-Attribut-Namen mit Info abrufen (für IS Betrachtung/Bearbeitung)1 => korrekte Benennung der Attribute
        /// </summary>
        /// <returns> Liste von Konkatenierten Strings (IS-Attribut-Name + Info) </returns>
        ObservableCollection<string> Get_StringList_AttributeNamesAndInfo();
        /// <summary>
        /// Informationssegment-Attribut-Namen abrufen für IS Übersicht Header1
        /// </summary>
        /// <returns> Liste der Attrbut-Namen für die Korrekte benennung der Header in der IS-Übersicht </returns>
        ObservableCollection<string> Get_StringList_AttributeNames();
        /// <summary>
        /// Informationssegment Eintrag einfügen (falls Unterschied zwischen Parametern)
        /// </summary>
        /// <param name="newIS"> (eventuell) geändertes Segment </param>
        /// <param name="oldIS"> altes Segment </param>
        /// <returns> Wahrheitswert ob Einfügen erfolgreich </returns>
        bool Insert_Segment(Segment_Model newIS, Segment_Model oldIS);
        /// <summary>
        /// Ruft alle Segmente und Attribute (Inklusive Historischen Daten) ab um sie zu exportieren
        /// </summary>
        /// <returns> Liste aller Segmente & Attribute </returns>
        Tuple<List<ISB_BIA_Informationssegmente>, List<ISB_BIA_Informationssegmente>, List<ISB_BIA_Informationssegmente_Attribute>, List<ISB_BIA_Informationssegmente_Attribute>> Get_History_SegmentsAndAttributes();
    }
}
