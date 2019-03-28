using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using ISB_BIA_IMPORT1.ViewModel;
using System;
using System.Collections.Generic;

namespace ISB_BIA_IMPORT1.Model
{
    /// <summary>
    /// Model eines Informationssegmentattributs
    /// </summary>
    public class Attributes_Model : ObservableObject
    {
        #region Backing-Fields
        private int _attribut_Id;
        private string _name="";
        private string _info="";
        private string _sZ_1;
        private string _sZ_2;
        private string _sZ_3;
        private string _sZ_4;
        private string _sZ_5;
        private string _sZ_6;
        private string msg = "Es sind nur Werte zwischen 0 und 4 erlaubt";
        #endregion

        #region Propteries der Attribute (für DataBinding)
        /// <summary>
        /// Attribut Id
        /// </summary>
        public int Attribut_Id
        {
            get => _attribut_Id;
            set => Set(() => Attribut_Id, ref _attribut_Id, value);
        }
        /// <summary>
        /// Name des Attributs
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                    Set(() => Name, ref _name, value);
                else
                    NotifyOnValidationError("Bitte Namen eingeben");
            }
        }
        /// <summary>
        /// Zusätzliche Info
        /// </summary>
        public string Info
        {
            get => _info;
            set => Set(() => Info, ref _info, value);
        }
        /// <summary>
        /// Schutzziel 1 Wert
        /// </summary>
        public string SZ_1
        {
            get => _sZ_1;
            set
            {
                if (numeric.Contains(value))
                    Set(() => SZ_1, ref _sZ_1, value);
                else
                    NotifyOnValidationError(msg);
            }
        }
        /// <summary>
        /// Schutzziel 2 Wert
        /// </summary>
        public string SZ_2
        {
            get => _sZ_2;
            set
            {
                if (numeric.Contains(value))
                    Set(() => SZ_2, ref _sZ_2, value);
                else
                    NotifyOnValidationError(msg);
            }
        }
        /// <summary>
        /// Schutzziel 3 Wert
        /// </summary>
        public string SZ_3
        {
            get => _sZ_3;
            set
            {
                if (numeric.Contains(value))
                    Set(() => SZ_3, ref _sZ_3, value);
                else
                    NotifyOnValidationError(msg);
            }
        }
        /// <summary>
        /// Schutzziel 4 Wert
        /// </summary>
        public string SZ_4
        {
            get => _sZ_4;
            set
            {
                if (numeric.Contains(value))
                    Set(() => SZ_4, ref _sZ_4, value);
                else
                    NotifyOnValidationError(msg);
            }
        }
        /// <summary>
        /// Schutzziel 5 Wert
        /// </summary>
        public string SZ_5
        {
            get => _sZ_5;
            set
            {
                if (numeric.Contains(value))
                    Set(() => SZ_5, ref _sZ_5, value);
                else
                    NotifyOnValidationError(msg);
            }
        }
        /// <summary>
        /// Schutzziel 6 Wert
        /// </summary>
        public string SZ_6
        {
            get => _sZ_6;
            set
            {
                if (numeric.Contains(value))
                    Set(() => SZ_6, ref _sZ_6, value);
                else
                    NotifyOnValidationError(msg);
            }
        }
        #endregion

        /// <summary>
        /// Liste für Wertebeschränkung der Schutzzielwerte (Prüfen im Setter)
        /// </summary>
        private List<string> numeric = new List<string>() { "0", "1", "2", "3", "4" };

        /// <summary>
        /// Sendet Nachricht, dass ein korrekter Wert eingegeben werden muss (Empfangen von <see cref="Attributes_Model"/>)
        /// </summary>
        private void NotifyOnValidationError(string msg)
        {
            Messenger.Default.Send(new NotificationMessage<string>(this, msg, null), MessageToken.ISAttributValidationError);
        }
    }
}
