//------------------------------------------------------------------------------
// <auto-generated>
//     Der Code wurde von einer Vorlage generiert.
//
//     Manuelle Änderungen an dieser Datei führen möglicherweise zu unerwartetem Verhalten der Anwendung.
//     Manuelle Änderungen an dieser Datei werden überschrieben, wenn der Code neu generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ISB_BIA_IMPORT1.LinqEntityContext
{
    using System;
    using System.Collections.Generic;
    
    public partial class ISB_BIA_Delta_Analyse
    {
        public int Prozess_Id { get; set; }
        public string Prozess { get; set; }
        public string Sub_Prozess { get; set; }
        public System.DateTime Datum_Prozess { get; set; }
        public int Applikation_Id { get; set; }
        public string Applikation { get; set; }
        public System.DateTime Datum_Applikation { get; set; }
        public int SZ_1 { get; set; }
        public int SZ_2 { get; set; }
        public int SZ_3 { get; set; }
        public int SZ_4 { get; set; }
        public int SZ_5 { get; set; }
        public int SZ_6 { get; set; }
        public System.DateTime Datum { get; set; }
    
        public virtual ISB_BIA_Applikationen ISB_BIA_Applikationen { get; set; }
        public virtual ISB_BIA_Prozesse ISB_BIA_Prozesse { get; set; }
    }
}
