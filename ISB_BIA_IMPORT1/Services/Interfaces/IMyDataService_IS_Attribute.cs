using ISB_BIA_IMPORT1.Model;
using ISB_BIA_IMPORT1.LINQ2SQL;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;

namespace ISB_BIA_IMPORT1.Services.Interfaces
{
    /// <summary>
    /// Service zum Abrufen der für die Anwendung benötigten Daten aus der Datenbank
    /// </summary>
    public interface IMyDataService_IS_Attribute
    {
        #region IS
        /// <summary>
        /// Informationssegment Model aus Daten eines Datensatzes der DB erstellen
        /// </summary>
        /// <param name="id"> Informationssegment_Id des Segments </param>
        /// <returns> Informationssegment Model Objekt </returns>
        InformationSegment_Model Get_SegmentModelFromDB(int id);
        /// <summary>
        /// Mappen des Segments vom Model- in das DB-Format
        /// </summary>
        /// <param name="i"> Informationssegment Model Objekt </param>
        /// <returns> Segment in Datenbankformat </returns>
        ISB_BIA_Informationssegmente Map_SegmentModelToDB(InformationSegment_Model i);
        /// <summary>
        /// Informationssegmentattribut Model aus Daten eines Datensatzes der DB erstellen
        /// </summary>
        /// <param name="id"> Attribut_Id des Attributs </param>
        /// <returns> Attribut Model Objekt </returns>
        InformationSegmentAttribute_Model Get_AttributeModelFromDB(int id);
        /// <summary>
        /// Mappen des Attributs vom Model- in das DB-Format
        /// </summary>
        /// <param name="ia"> Prozess Model Objekt </param>
        /// <returns> Attribut in Datenbankformat </returns>
        ISB_BIA_Informationssegmente_Attribute Map_AttributeModelToDB(InformationSegmentAttribute_Model ia);
        /// <summary>
        /// Alle Segmente für CISO/Admin Ansicht/Bearbeitung1
        /// </summary>
        /// <returns> Liste aller Informationssegmente (definierte / undefinierte("Lorem ipsum") </returns>
        ObservableCollection<ISB_BIA_Informationssegmente> Get_Segments_All();
        /// <summary>
        /// Aktivierte Segmente für normale User zur Ansicht1 und Berechnung der Prozess Mindesteinstufung2
        /// </summary>
        /// <returns> Liste der aktiven Segmente </returns>
        ObservableCollection<ISB_BIA_Informationssegmente> Get_Segments_Enabled();
        /// <summary>
        /// Informationssegment-Attribut-Namen mit Info abrufen (für IS Betrachtung/Bearbeitung)1 => korrekte Benennung der Attribute
        /// </summary>
        /// <returns> Liste von Konkatenierten Strings (IS-Attribut-Name + Info) </returns>
        ObservableCollection<string> Get_List_AttributeNamesAndInfoForIS();
        /// <summary>
        /// Informationssegment-Attribute abrufen
        /// </summary>
        /// <returns> Liste der 10 IS-Attribute </returns>
        ObservableCollection<ISB_BIA_Informationssegmente_Attribute> Get_Attributes();
        /// <summary>
        /// Informationssegment-Attribut-Namen abrufen für IS Übersicht Header1
        /// </summary>
        /// <returns> Liste der Attrbut-Namen für die Korrekte benennung der Header in der IS-Übersicht </returns>
        ObservableCollection<string> Get_List_AttributeNamesForHeader();
        /// <summary>
        /// Informationssegment Eintrag einfügen (falls Unterschied zwischen Parametern)
        /// </summary>
        /// <param name="newIS"> (eventuell) geändertes Segment </param>
        /// <param name="oldIS"> altes Segment </param>
        /// <returns> Wahrheitswert ob Einfügen erfolgreich </returns>
        bool Insert_Segment(InformationSegment_Model newIS, InformationSegment_Model oldIS);
        /// <summary>
        /// Informationssegment-Attribut einfügen
        /// </summary>
        /// <param name="newAttributeList"> Liste aller Attribute, deren Elemente einzeln auf Änderungen geprüft und entsprechend in die Datenbank geschrieben werden </param>
        /// <returns> Wahrheitswert ob einfügen erfolgreich () </returns>
        bool Insert_Attribute(ObservableCollection<InformationSegmentAttribute_Model> newAttributeList);
        /// <summary>
        /// Ruft alle Segmente und Attribute (Inklusive Historischen Daten) ab um sie zu exportieren
        /// </summary>
        /// <returns> Liste aller Segmente & Attribute </returns>
        Tuple<List<ISB_BIA_Informationssegmente>, List<ISB_BIA_Informationssegmente>, List<ISB_BIA_Informationssegmente_Attribute>, List<ISB_BIA_Informationssegmente_Attribute>> Get_ISAndISAttForExport();
        #endregion
    }
}
