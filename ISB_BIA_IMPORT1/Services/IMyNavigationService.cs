using GalaSoft.MvvmLight;
using ISB_BIA_IMPORT1.ViewModel;
using System.Collections.ObjectModel;

namespace ISB_BIA_IMPORT1.Services
{
    /// <summary>
    /// Service für die Navigation zwischen den Viewmodels. 
    /// Enthält Listeneigenschaft <see cref="VMHistory"/>, in der die Viewmodels gespeichert werden.
    /// Wird zu einem VM navigiert, wird es der Liste hinzugefügt.
    /// Wird zum vorherigen VM zurückgekehrt, wird das letzte VM aus der Liste gelöscht und zum vorletzten navigiert.
    /// </summary>
    public interface IMyNavigationService
    {
        /// <summary>
        /// Historie-Liste der vorerigen Viewmodels
        /// </summary>
        ObservableCollection<ViewModelBase> VMHistory { get; set; }
        /// <summary>
        /// zum vorherigen Viewmodel zurückkehren.
        /// => Löschen des akutell letzten eintrags der <see cref="VMHistory"/> und sendet Nachricht mit dem neuen letzten Eintrag an das <see cref="Main_ViewModel"/>
        /// </summary>
        void NavigateBack();
        /// <summary>
        /// zum vorherigen Viewmodel zurückkehren und Refresh (Von ProzessViewVM etc) initiieren falls parameter true (Listen aktualisieren).
        /// => Löschen des akutell letzten eintrags der <see cref="VMHistory"/> und sendet Nachricht mit dem neuen letzten Eintrag an das <see cref="Main_ViewModel"/>
        /// </summary>
        /// <param name="refresh"> Refresh Ja/Nein </param>
        void NavigateBack(bool refresh);
        /// <summary>
        /// Navgation ohne spezielle Parameter
        /// </summary>
        /// <typeparam name="T"> Typ des Viewmodels, zu dem navigiert werden soll </typeparam>
        void NavigateTo<T>() where T : ViewModelBase;
        /// <summary>
        /// Navigation mit ID und Modus (Bearbeitung/Neuanlage von Prozessen und Anwendungen)
        /// </summary>
        /// <typeparam name="T"> Typ des Viewmodels, zu dem navigiert werden soll </typeparam>
        /// <param name="id"> id des zu bearbeitenden Objektes für die DB Abfrage (bei neuanlage ignoriert) </param>
        /// <param name="mode"> Modus (Edit, New) für Darstellung des neuen Viewmodels </param>
        void NavigateTo<T>(int id, ProcAppMode mode) where T : ViewModelBase;
        /// <summary>
        /// Navigation zu Prozess/Anwendungsliste
        /// </summary>
        /// <typeparam name="T"> Typ des Viewmodels, zu dem navigiert werden soll </typeparam>
        /// <param name="mode"> Modi: Bearbeitung / Löschen der Listenitems </param>
        void NavigateTo<T>(ProcAppListMode mode) where T : ViewModelBase;
        /// <summary>
        /// Navigation mit Modus (zur Informationssegment-Übsersicht)
        /// </summary>
        /// <typeparam name="T"> Typ des Viewmodels, zu dem navigiert werden soll </typeparam>
        /// <param name="mode"> Bearbeitungs / Ansichtsmodus (=>weitergeleitet zum IS VM) </param>
        void NavigateTo<T>(ISISAttributeMode mode) where T : ViewModelBase;
        /// <summary>
        /// Navigation mit ID und Modus (Bearbeitung/Ansicht von Informationssegment(-en / -Attributen))
        /// </summary>
        /// <typeparam name="T"> Typ des Viewmodels, zu dem navigiert werden soll </typeparam>
        /// <param name="id"> id des zu bearbeitenden Objektes für die DB Abfrage </param>
        /// <param name="mode"> Bearbeitungs / Ansichtsmodus </param>
        void NavigateTo<T>(int id, ISISAttributeMode mode) where T : ViewModelBase;
    }
}
