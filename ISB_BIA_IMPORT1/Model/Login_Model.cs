using GalaSoft.MvvmLight;
using System.Collections.Generic;

namespace ISB_BIA_IMPORT1.Model
{
    /// <summary>
    /// Entscheidet, welche Ansicht/Funktionsumfang der User bekommt (gesetzt durch AD abgleich mit Einstellungen)
    /// </summary>
    public enum UserGroups
    {
        /// <summary>
        /// Normaler User, durch OE-Kennung identifiziert, Kann die Prozesse seiner OE('s) ändern, Informationssegment(-e/-Attribute) sehen
        /// </summary>
        Normal_User, //
        /// <summary>
        /// Admin, durch Datenbank eintrag (Settings->Admin's) identifiziert, kann alle Prozesse, alle Anwendungen bearbeiten aber nicht Informationssegment(-e/-Attribute) 
        /// </summary>
        Admin,
        /// <summary>
        /// CISO, durch Datenbankeintrag (CISO's) identifiziert, darf alles
        /// </summary>
        CISO,
        /// <summary>
        /// User für die Durchführung der SBA, durch LF-Nummer(n) identifiziert, Rechte: Normaler User + Alle Anwendungen bearbeiten
        /// </summary>
        SBA_User
    }
    /// <summary>
    /// Login Klasse, zu setzen durch Daten aus der ISB User Datenbank
    /// </summary>
    public class Login_Model : ObservableObject
    {
        private string _username;
        private UserGroups _userGroup;
        private string _givenname;
        private string _surname;
        private string _oE;
        private List<string> _listOE;

        /// <summary>
        /// Nutzername (LF-Nummer des Users)
        /// </summary>
        public string Username
        {
            get => _username;
            set => Set(() => Username, ref _username, value);
        }
        /// <summary>
        /// Gruppenzugehörigkeit des Users, entscheident für Darstellung und Rechte
        /// </summary>
        public UserGroups UserGroup
        {
            get => _userGroup;
            set => Set(() => UserGroup, ref _userGroup, value);
        }
        /// <summary>
        /// Vorname des Users aus AD
        /// </summary>
        public string Givenname
        {
            get => _givenname;
            set => Set(() => Givenname, ref _givenname, value);
        }
        /// <summary>
        /// Nachname des Users aus AD
        /// </summary>
        public string Surname
        {
            get => _surname;
            set => Set(() => Surname, ref _surname, value);
        }
        /// <summary>
        /// OE-Nummer(n) des Users aus AD
        /// </summary>
        public string OE
        {
            get => _oE;
            set => Set(() => OE, ref _oE, value);
        }
        /// <summary>
        /// Liste der OE-Nummer des Users aus AD
        /// </summary>
        public List<string> ListOE
        {
            get => _listOE;
            set => Set(() => ListOE, ref _listOE, value);
        }
        /// <summary>
        /// Kompletter Name des Users aus AD
        /// </summary>
        public string WholeName
        {
            get => _surname + ", "+ _givenname + " ("+_username+")";
        }
    }
}
