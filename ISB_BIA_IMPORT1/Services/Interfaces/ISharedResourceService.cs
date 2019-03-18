using ISB_BIA_IMPORT1.Model;

namespace ISB_BIA_IMPORT1.Services.Interfaces
{
    public enum Current_Environment
    {
        Local_Test,
        Test,
        Prod,
    }

    /// <summary>
    /// Service zur Bereitstellung von Ressourcen, welche von mehreren Viewmodels benötigt werden
    /// </summary>
    public interface ISharedResourceService
    {
        /// <summary>
        /// Zum Deaktivieren aller Startbedingungen (DB Check etc.)
        /// </summary>
        bool Conf_ConstructionMode { get; set; }
        /// <summary>
        /// Variable zum Testen der Anwendung außerhalb der Produktivumgebung (Deaktiviert ActiveDirectory-Check etc.)
        /// </summary>
        Current_Environment Conf_CurrentEnvironment { get; set; }
        /// <summary>
        /// Variable zum Testen der Anwendung als Admin in der Produktivumgebung 
        /// </summary>
        bool Conf_Admin { get; set; }
        /// <summary>
        /// User welcher aus Active Directory und PC-Usernamen sowie aktuellen Einstellungen für diese Anwendung erstellt wird
        /// </summary>
        Login_Model User { get; set; }
        /// <summary>
        /// Zum Release definierter initialer Ordner mit den Daten (Excel-Datei) zur Erstellung des Datenmodells
        /// </summary>
        string Dir_InitialDirectory { get; set; }
        /// <summary>
        /// string welcher die Datenbankverbindung definiert
        /// </summary>
        string Conf_ConnectionString { get; set; }
        /// <summary>
        /// Dateipfad, welcher Standardmäßig beim Öffnen des OpenFileDialogs in den Einstellungen für das Datenmodell angezeigt wird
        /// </summary>
        string Dir_Source { get; set; }
        /// <summary>
        /// Mail-Adresse, an die Benachrichtigungen gesendet werden sollen
        /// </summary>
        string Conf_TargetMail { get; set; }
        /// <summary>
        /// String der CISO AD-Gruppe der jeweiligen Test/Prod-Umgebung
        /// </summary>
        string Conf_AD_Group_CISO { get; set; }
        /// <summary>
        /// String der Admin AD-Gruppe der jeweiligen Test/Prod-Umgebung
        /// </summary>
        string Conf_AD_Group_Admin { get; set; }
        /// <summary>
        /// String der SBA-User AD-Gruppe der jeweiligen Test/Prod-Umgebung
        /// </summary>
        string Conf_AD_Group_SBA { get; set; }
        /// <summary>
        /// String der Normal-User AD-Gruppe der jeweiligen Test/Prod-Umgebung
        /// </summary>
        string Conf_AD_Group_Normal { get; set; }


        #region Tabellennamen in Datenbank
        /// <summary>
        /// Name der ProzessTabelle bei Erstellung des Datenmodells
        /// </summary>
        string Tbl_Prozesse { get; set; }
        /// <summary>
        /// Name der Prozess-Applikation-Relationstabelle bei Erstellung des Datenmodells
        /// </summary>
        string Tbl_Proz_App { get; set; }
        /// <summary>
        /// Name der Delta-Analyse-Tabelle bei Erstellung des Datenmodells
        /// </summary>
        string Tbl_Delta { get; set; }
        /// <summary>
        /// Name der Informationssegment-Tabelle bei Erstellung des Datenmodells
        /// </summary>
        string Tbl_IS { get; set; }
        /// <summary>
        /// Name der Informationssegment-Attribut-Tabelle bei Erstellung des Datenmodells
        /// </summary>
        string Tbl_IS_Attribute { get; set; }
        /// <summary>
        /// Name der Applikationstabelle bei Erstellung des Datenmodells
        /// </summary>
        string Tbl_Applikationen { get; set; }
        /// <summary>
        /// Name der Logtabelle bei Erstellung des Datenmodells
        /// </summary>
        string Tbl_Log { get; set; }
        /// <summary>
        /// Name der OE-Tabelle bei Erstellung des Datenmodells
        /// </summary>
        string Tbl_OEs { get; set; }
        /// <summary>
        /// Name der Einstellungstabelle bei Erstellung des Datenmodells
        /// </summary>
        string Tbl_Settings { get; set; }
        /// <summary>
        /// Name der Lock/Sperrtabelle bei Erstellung des Datenmodells
        /// </summary>
        string Tbl_Lock { get; set; }

        #endregion


    }
}
