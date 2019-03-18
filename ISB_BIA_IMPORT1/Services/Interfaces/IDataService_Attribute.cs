using ISB_BIA_IMPORT1.Model;
using ISB_BIA_IMPORT1.LINQ2SQL;
using System.Collections.ObjectModel;

namespace ISB_BIA_IMPORT1.Services.Interfaces
{
    /// <summary>
    /// Service zum Abrufen der für die Anwendung benötigten Daten aus der Datenbank
    /// </summary>
    public interface IDataService_Attribute
    {
        /// <summary>
        /// Informationssegmentattribut Model aus Daten eines Datensatzes der DB erstellen
        /// </summary>
        /// <param name="id"> Attribut_Id des Attributs </param>
        /// <returns> Attribut Model Objekt </returns>
        Attributes_Model Get_Model_FromDB(int id);
        /// <summary>
        /// Mappen des Attributs vom Model- in das DB-Format
        /// </summary>
        /// <param name="ia"> Prozess Model Objekt </param>
        /// <returns> Attribut in Datenbankformat </returns>
        ISB_BIA_Informationssegmente_Attribute Map_Model_ToDB(Attributes_Model ia);
        /// <summary>
        /// Informationssegment-Attribute abrufen
        /// </summary>
        /// <returns> Liste der 10 IS-Attribute </returns>
        ObservableCollection<ISB_BIA_Informationssegmente_Attribute> Get_List_Attributes();
        /// <summary>
        /// Informationssegment-Attribut einfügen
        /// </summary>
        /// <param name="newAttributeList"> Liste aller Attribute, deren Elemente einzeln auf Änderungen geprüft und entsprechend in die Datenbank geschrieben werden </param>
        /// <returns> Wahrheitswert ob einfügen erfolgreich () </returns>
        bool Insert_Attribute(ObservableCollection<Attributes_Model> newAttributeList);
    }
}
