using ISB_BIA_IMPORT1.LINQ2SQL;
using System.Collections.ObjectModel;

namespace ISB_BIA_IMPORT1.Services.Interfaces
{
    public interface IMyDataService_OE
    {
        #region OE
        /// <summary>
        /// Liste der OE-Gruppierungen (Für OE-Einstellungen Namen Dropdowns und Übersicht1)
        /// </summary>
        /// <returns> Liste der OE-Gruppierungen </returns>
        ObservableCollection<ISB_BIA_OEs> Get_OENames();
        /// <summary>
        /// Liste der OE-Nummern (Für OE-Einstellungen Nummern Dropdowns und Übersicht1)
        /// </summary>
        /// <returns> Liste der OE-Nummern </returns>
        ObservableCollection<ISB_BIA_OEs> Get_OENumbers();
        /// <summary>
        /// Liste der OE-Gruppierung-Nummern Relationen (Für OE-Einstellungen Relationen Dropdowns und Übersicht1)
        /// </summary>
        /// <returns> Liste der OE-Gruppierung-Nummern Relationen </returns>
        ObservableCollection<ISB_BIA_OEs> Get_OELinks();
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
        bool Delete_OELink(string oeName, string oeNumber);
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
        ISB_BIA_OEs Insert_OELink(ISB_BIA_OEs name, ISB_BIA_OEs number);
        /// <summary>
        /// OE-Nummer erstellen (zugehörige Gruppe muss mit angegeben werden)
        /// </summary>
        /// <param name="number"> Anzulegende Nummer </param>
        /// <param name="name"> Zugehörige Gruppe </param>
        /// <returns></returns>
        ISB_BIA_OEs Insert_OENumber_New(string number, ISB_BIA_OEs name);
        /// <summary>
        /// OE-Nummer ändern
        /// </summary>
        /// <param name="number"> neue Nummer </param>
        /// <param name="oldNumber"> alte Nummer </param>
        /// <returns></returns>
        bool Insert_OENumber_Edit(string number, string oldNumber);
        #endregion
    }
}
