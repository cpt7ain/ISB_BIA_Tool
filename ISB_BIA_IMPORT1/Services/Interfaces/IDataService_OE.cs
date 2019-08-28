using ISB_BIA_IMPORT1.LINQ2SQL;
using ISB_BIA_IMPORT1.Model;
using System.Collections.ObjectModel;

namespace ISB_BIA_IMPORT1.Services.Interfaces
{
    public interface IDataService_OE
    {
        #region OE
        /// <summary>
        /// Ermittelt den aktuellen Stabsabteilungsleiter anhand der OE-Nummer (eines Users) (für die automatische Eigentümer-Zuweisung)
        /// </summary>
        /// <returns> Prozesseigentümer (Stabsabteilungs/Bereichsleiter) </returns>
        string Get_OwnerOfOENumber(string stab);
        /// <summary>
        /// Ermittelt den aktuellen Stabsabteilungsleiter anhand der OE des OE-Namen (eines Prozesses)
        /// </summary>
        /// <param name="stab"> OE-Name </param>
        /// <param name="p"> Prozess (wird übergeben für unterschiedloches verhalten bei Reaktivierung eines Prozesses) </param>
        /// <returns></returns>
        string Get_OwnerOfOEName(string stab, Process_Model p);
        /// <summary>
        /// Liste der OE-Gruppierungen (Für OE-Einstellungen Namen Dropdowns und Übersicht1)
        /// </summary>
        /// <returns> Liste der OE-Gruppierungen </returns>
        ObservableCollection<ISB_BIA_OEs> Get_List_OENames();
        /// <summary>
        /// Liste der OE-Nummern (Für OE-Einstellungen Nummern Dropdowns und Übersicht1)
        /// </summary>
        /// <returns> Liste der OE-Nummern </returns>
        ObservableCollection<ISB_BIA_OEs> Get_List_OENumbers();
        /// <summary>
        /// Liste der OE-Gruppierung-Nummern Relationen (Für OE-Einstellungen Relationen Dropdowns und Übersicht1)
        /// </summary>
        /// <returns> Liste der OE-Gruppierung-Nummern Relationen </returns>
        ObservableCollection<ISB_BIA_OEs> Get_List_OERelation();
        /// <summary>
        /// OE Gruppierung einfügen
        /// </summary>
        /// <param name="name"> Name der OE-Gruppe </param>
        /// <returns> Eingefügte OE-Gruppe </returns>
        ISB_BIA_OEs Insert_OEName_New(string name);
        /// <summary>
        /// OE-Gruppen bearbeiten
        /// </summary>
        /// <param name="name"> neuer Name </param>
        /// <param name="oldName"> alter Name </param>
        /// <returns></returns>
        bool Insert_OEName_Edit(string name, string oldName);
        /// <summary>
        /// Löscht eine OE-Gruppe
        /// </summary>
        /// <param name="oeName"> Name der OE Gruppe </param>
        /// <returns> Erfolgreiche Löschung </returns>
        bool Delete_OEName(string oeName);
        /// <summary>
        /// Löscht eine OE-Gruppen Relation
        /// </summary>
        /// <param name="oeName"> Name der Gruppe </param>
        /// <param name="oeNumber"> Nummer der OE </param>
        /// <returns> Erfolgreiche Löschung </returns>
        bool Delete_OERelation(string oeName, string oeNumber);
        /// <summary>
        /// Löscht eine OE (Nummer)
        /// </summary>
        /// <param name="oeNumber"> Nummer der OE </param>
        /// <returns> Erfolgreiche Löschung </returns>
        bool Delete_OENumber(string oeNumber);
        /// <summary>
        /// OE-Gruppen-Zuordnung erstellen
        /// </summary>
        /// <param name="name"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        ISB_BIA_OEs Insert_OERelation(ISB_BIA_OEs name, ISB_BIA_OEs number);
        /// <summary>
        /// OE-Nummer erstellen (zugehörige Gruppe muss mit angegeben werden)
        /// </summary>
        /// <param name="number"> Anzulegende Nummer </param>
        /// <param name="owner"> Anzulegender Prozesseigentümer </param>
        /// <param name="name"> Zugehörige Gruppe </param>
        /// <returns></returns>
        ISB_BIA_OEs Insert_OENumber_New(string number, string owner, ISB_BIA_OEs name);
        /// <summary>
        /// OE-Nummer ändern
        /// </summary>
        /// <param name="number"> neue Nummer </param>
        /// <param name="owner"> neuer Prozesseigentümer </param>
        /// <param name="oldNumber"> alte Nummer </param>
        /// <param name="oldOwner"> alter Prozesseigentümer </param>
        /// <returns></returns>
        bool Insert_OENumber_Edit(string number, string owner, string oldNumber, string oldOwner);
        #endregion
    }
}
