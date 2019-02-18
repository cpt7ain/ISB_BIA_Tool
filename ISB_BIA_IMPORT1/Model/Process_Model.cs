using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using ISB_BIA_IMPORT1.LinqDataContext;
using ISB_BIA_IMPORT1.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ISB_BIA_IMPORT1.Model
{
    /// <summary>
    /// Model eines Prozesses
    /// </summary>
    public class Process_Model : ObservableObject, INotifyDataErrorInfo
    {
        #region Backing-Fields

        private ObservableCollection<ISB_BIA_Applikationen> _applicationList = new ObservableCollection<ISB_BIA_Applikationen>();
        private int _prozess_Id;
        private string _oE_Filter = "";
        private string _prozess = "";
        private string _sub_Prozess = "";
        private string _kritischer_Prozess = "Nein";
        private string _kritikalität_des_Prozesses = "";
        private string _reifegrad_des_Prozesses = "";
        private bool _regulatorisch = false;
        private bool _reputatorisch = false;
        private bool _finanziell = false;
        private SZ_Values _sZ_1 = 0;
        private SZ_Values _sZ_2 = 0;
        private SZ_Values _sZ_3 = 0;
        private SZ_Values _sZ_4 = 0;
        private SZ_Values _sZ_5 = 0;
        private SZ_Values _sZ_6 = 0;
        private string _servicezeit_Helpdesk = "";
        private int _rPO_Datenverlustzeit_Recovery_Point_Objective = 24;
        private int _rTO_Wiederanlaufzeit_Recovery_Time_Objective = 0;
        private int _rTO_Wiederanlaufzeit_Recovery_Time_Objective_Notfall = 0;
        private string _prozessverantwortlicher = "";
        private string _vorgelagerte_Prozesse = "";
        private string _nachgelagerte_Prozesse = "";
        private string _relevantes_IS_1 = "";
        private string _relevantes_IS_2 = "";
        private string _relevantes_IS_3 = "";
        private string _relevantes_IS_4 = "";
        private string _relevantes_IS_5 = "";
        private string _benutzer;
        private DateTime _datum;
        private int _aktiv = 1;

        #endregion

        #region Prozess Eigenschaften

        /// <summary>
        /// Liste der Anwendungen, welche dem Prozess momentan zugeordnet sind
        /// </summary>
        public ObservableCollection<ISB_BIA_Applikationen> ApplicationList
        {
            get => _applicationList;
            set => Set(() => ApplicationList, ref _applicationList, value);

        }

        /// <summary>
        /// Id des Prozesses
        /// </summary>
        public int Prozess_Id
        {
            get => _prozess_Id;
            set => Set(() => Prozess_Id, ref _prozess_Id, value);

        }

        /// <summary>
        /// OE Zugehörigkeit des Prozesses
        /// </summary>
        public string OE_Filter
        {
            get => _oE_Filter;
            set
            {
                Set(() => OE_Filter, ref _oE_Filter, value);
                if (!string.IsNullOrWhiteSpace(_oE_Filter))
                    RemoveError(nameof(OE_Filter));
            }
        }

        /// <summary>
        /// Prozessname
        /// </summary>
        public string Prozess
        {
            get => _prozess;
            set
            {
                Set(() => Prozess, ref _prozess, value);
                if (!string.IsNullOrWhiteSpace(_prozess))
                    RemoveError(nameof(Prozess));
            }
        }

        /// <summary>
        /// Sub-Prozessname
        /// </summary>
        public string Sub_Prozess
        {
            get => _sub_Prozess;
            set => Set(() => Sub_Prozess, ref _sub_Prozess, value);
        }

        /// <summary>
        /// Gibt an, ob Prozess kritisch oder nicht
        /// Wert wird wie folgt berechnet: <see cref="RTO_Wiederanlaufzeit_Recovery_Time_Objective"/> 1 oder <see cref="Kritikalität_des_Prozesses"/> == "Sehr Hoch oder (min 2 Schutzziele "Sehr Hoch" oder mindestens 3 mindestens "Hoch" <see cref="CritCalculation(int, int, int, int)"/>, <see cref="UnCritCalculation(int, int, int, int)"/>)
        /// </summary>
        public string Kritischer_Prozess
        {
            get => _kritischer_Prozess;
            set
            {
                //Meldung wird im Code-Behind des Views aufgerufen
                /*
                if (value == "Ja" && _kritischer_Prozess == "Nein")
                {
                    Messenger.Default.Send("Changed", MessageToken.ChangedToCriticalNotification);
                }
                */
                Set(() => Kritischer_Prozess, ref _kritischer_Prozess, value);
            }
        }

        /// <summary>
        /// Kritikalität des Prozesses
        /// </summary>
        public string Kritikalität_des_Prozesses
        {
            get => _kritikalität_des_Prozesses;
            set
            {
                Set(() => Kritikalität_des_Prozesses, ref _kritikalität_des_Prozesses, value);
                if (Kritischer_Prozess != "Ja" && value == "Sehr hoch")
                {
                    Kritischer_Prozess = "Ja";
                }
                else if (Kritischer_Prozess == "Ja" && value != "Sehr hoch" &&
                         RTO_Wiederanlaufzeit_Recovery_Time_Objective != 1 &&
                         !CritCalculation((int) SZ_4, (int) SZ_1, (int) SZ_3, (int) SZ_2))
                {
                    Kritischer_Prozess = "Nein";
                }
                if (!string.IsNullOrWhiteSpace(_kritikalität_des_Prozesses))
                    RemoveError(nameof(Kritikalität_des_Prozesses));
            }
        }

        /// <summary>
        /// Reifegrad des Prozesses
        /// </summary>
        public string Reifegrad_des_Prozesses
        {
            get => _reifegrad_des_Prozesses;
            set
            {
                Set(() => Reifegrad_des_Prozesses, ref _reifegrad_des_Prozesses, value);
                if (!string.IsNullOrWhiteSpace(_reifegrad_des_Prozesses))
                    RemoveError(nameof(Reifegrad_des_Prozesses));
            }
        }

        /// <summary>
        /// Indikator, ob Prozess Auswirkunegn auf Regularien haben kann
        /// </summary>
        public bool Regulatorisch
        {
            get => _regulatorisch;
            set => Set(() => Regulatorisch, ref _regulatorisch, value);
        }

        /// <summary>
        /// Indikator, ob Prozess Auwirkungen auf Reputation haben kann
        /// </summary>
        public bool Reputatorisch
        {
            get => _reputatorisch;
            set => Set(() => Reputatorisch, ref _reputatorisch, value);

        }

        /// <summary>
        ///  Indikator, ob Prozess Auwirkungen auf Finanzen haben kann
        /// </summary>
        public bool Finanziell
        {
            get => _finanziell;
            set => Set(() => Finanziell, ref _finanziell, value);

        }

        /// <summary>
        /// Wert von Schutzziel 1
        /// </summary>
        public SZ_Values SZ_1
        {
            get => _sZ_1;
            set
            {
                Set(() => SZ_1, ref _sZ_1, value);
                if (Kritischer_Prozess != "Ja" && CritCalculation((int) value, (int) SZ_2, (int) SZ_3, (int) SZ_4))
                {
                    Kritischer_Prozess = "Ja";
                }
                else if (Kritischer_Prozess == "Ja" &&
                         UnCritCalculation((int) value, (int) SZ_2, (int) SZ_3, (int) SZ_4) &&
                         Kritikalität_des_Prozesses != "Sehr hoch" && RTO_Wiederanlaufzeit_Recovery_Time_Objective != 1)
                {
                    Kritischer_Prozess = "Nein";
                }
            }
        }

        /// <summary>
        /// Wert von Schutzziel 2
        /// </summary>
        public SZ_Values SZ_2
        {
            get => _sZ_2;
            set
            {
                Set(() => SZ_2, ref _sZ_2, value);
                if (Kritischer_Prozess != "Ja" && CritCalculation((int) value, (int) SZ_1, (int) SZ_3, (int) SZ_4))
                {
                    Kritischer_Prozess = "Ja";
                }
                else if (Kritischer_Prozess == "Ja" &&
                         UnCritCalculation((int) value, (int) SZ_1, (int) SZ_3, (int) SZ_4) &&
                         Kritikalität_des_Prozesses != "Sehr hoch" && RTO_Wiederanlaufzeit_Recovery_Time_Objective != 1)
                {
                    Kritischer_Prozess = "Nein";
                }
            }
        }

        /// <summary>
        /// Wert von Schutzziel 3
        /// </summary>
        public SZ_Values SZ_3
        {
            get => _sZ_3;
            set
            {
                Set(() => SZ_3, ref _sZ_3, value);

                if (Kritischer_Prozess != "Ja" && CritCalculation((int) value, (int) SZ_1, (int) SZ_2, (int) SZ_4))
                {
                    Kritischer_Prozess = "Ja";
                }
                else if (Kritischer_Prozess == "Ja" &&
                         UnCritCalculation((int) value, (int) SZ_1, (int) SZ_2, (int) SZ_4) &&
                         Kritikalität_des_Prozesses != "Sehr hoch" && RTO_Wiederanlaufzeit_Recovery_Time_Objective != 1)
                {
                    Kritischer_Prozess = "Nein";
                }
            }
        }

        /// <summary>
        /// Wert von Schutzziel 4
        /// </summary>
        public SZ_Values SZ_4
        {
            get => _sZ_4;
            set
            {
                Set(() => SZ_4, ref _sZ_4, value);
                if (Kritischer_Prozess != "Ja" && CritCalculation((int) value, (int) SZ_1, (int) SZ_3, (int) SZ_2))
                {
                    Kritischer_Prozess = "Ja";
                }
                else if (Kritischer_Prozess == "Ja" &&
                         UnCritCalculation((int) value, (int) SZ_1, (int) SZ_3, (int) SZ_2) &&
                         Kritikalität_des_Prozesses != "Sehr hoch" && RTO_Wiederanlaufzeit_Recovery_Time_Objective != 1)
                {
                    Kritischer_Prozess = "Nein";
                }
            }
        }

        /// <summary>
        /// Wert von Schutzziel 5
        /// </summary>
        public SZ_Values SZ_5
        {
            get => _sZ_5;
            set => Set(() => SZ_5, ref _sZ_5, value);

        }

        /// <summary>
        /// Wert von Schutzziel 6
        /// </summary>
        public SZ_Values SZ_6
        {
            get => _sZ_6;
            set => Set(() => SZ_6, ref _sZ_6, value);

        }

        /// <summary>
        /// Servicezeiten für diesen Prozess
        /// </summary>
        public string Servicezeit_Helpdesk
        {
            get => _servicezeit_Helpdesk;
            set
            {
                Set(() => Servicezeit_Helpdesk, ref _servicezeit_Helpdesk, value);
                if (!string.IsNullOrWhiteSpace(_servicezeit_Helpdesk))
                    RemoveError(nameof(Servicezeit_Helpdesk));
            } 
        }

        /// <summary>
        /// Zeitangabe des RPO in Stunden
        /// </summary>
        public int RPO_Datenverlustzeit_Recovery_Point_Objective
        {
            get => _rPO_Datenverlustzeit_Recovery_Point_Objective;
            set => Set(() => RPO_Datenverlustzeit_Recovery_Point_Objective,
                ref _rPO_Datenverlustzeit_Recovery_Point_Objective, value);
        }

        /// <summary>
        /// Zeitangabe des RTO in Tagen
        /// </summary>
        public int RTO_Wiederanlaufzeit_Recovery_Time_Objective
        {
            get => _rTO_Wiederanlaufzeit_Recovery_Time_Objective;
            set
            {
                Set(() => RTO_Wiederanlaufzeit_Recovery_Time_Objective,
                    ref _rTO_Wiederanlaufzeit_Recovery_Time_Objective, value);
                if (Kritischer_Prozess != "Ja" && value == 1)
                {
                    Kritischer_Prozess = "Ja";
                }
                else if (Kritischer_Prozess == "Ja" && value != 1 && Kritikalität_des_Prozesses != "Sehr hoch" &&
                         !CritCalculation((int) SZ_4, (int) SZ_1, (int) SZ_3, (int) SZ_2))
                {
                    Kritischer_Prozess = "Nein";
                }
                if (_rTO_Wiederanlaufzeit_Recovery_Time_Objective != 0)
                    RemoveError(nameof(RTO_Wiederanlaufzeit_Recovery_Time_Objective));
            }
        }

        /// <summary>
        /// Zeitangabe des RTO_Notfall in Tagen
        /// </summary>
        public int RTO_Wiederanlaufzeit_Recovery_Time_Objective_Notfall
        {
            get => _rTO_Wiederanlaufzeit_Recovery_Time_Objective_Notfall;
            set
            {
                Set(() => RTO_Wiederanlaufzeit_Recovery_Time_Objective_Notfall,
                    ref _rTO_Wiederanlaufzeit_Recovery_Time_Objective_Notfall, value);
                if (_rTO_Wiederanlaufzeit_Recovery_Time_Objective_Notfall != 0)
                    RemoveError(nameof(RTO_Wiederanlaufzeit_Recovery_Time_Objective_Notfall));
            }

        }

        /// <summary>
        /// Prozessverantwortlicher/Eigentümer des Prozesses
        /// </summary>
        public string Prozessverantwortlicher
        {
            get => _prozessverantwortlicher;
            set
            {
                Set(() => Prozessverantwortlicher, ref _prozessverantwortlicher, value);
                if (!string.IsNullOrWhiteSpace(_prozessverantwortlicher))
                    RemoveError(nameof(Prozessverantwortlicher));
            }
        }

        /// <summary>
        /// Vorgelagerter Prozess/OE
        /// </summary>
        public string Vorgelagerte_Prozesse
        {
            get => _vorgelagerte_Prozesse;
            set => Set(() => Vorgelagerte_Prozesse, ref _vorgelagerte_Prozesse, value);

        }

        /// <summary>
        /// Nachgelagerter Prozess/OE
        /// </summary>
        public string Nachgelagerte_Prozesse
        {
            get => _nachgelagerte_Prozesse;
            set => Set(() => Nachgelagerte_Prozesse, ref _nachgelagerte_Prozesse, value);

        }

        /// <summary>
        /// Das erste für diesen Prozess relevante Segment
        /// </summary>
        public string Relevantes_IS_1
        {
            get => _relevantes_IS_1;
            set => Set(() => Relevantes_IS_1, ref _relevantes_IS_1, value);

        }

        /// <summary>
        /// Das zweite für diesen Prozess relevante Segment
        /// </summary>
        public string Relevantes_IS_2
        {
            get => _relevantes_IS_2;
            set => Set(() => Relevantes_IS_2, ref _relevantes_IS_2, value);

        }

        /// <summary>
        /// Das dritte für diesen Prozess relevante Segment
        /// </summary>
        public string Relevantes_IS_3
        {
            get => _relevantes_IS_3;
            set => Set(() => Relevantes_IS_3, ref _relevantes_IS_3, value);

        }

        /// <summary>
        /// Das vierte für diesen Prozess relevante Segment
        /// </summary>
        public string Relevantes_IS_4
        {
            get => _relevantes_IS_4;
            set => Set(() => Relevantes_IS_4, ref _relevantes_IS_4, value);

        }

        /// <summary>
        /// Das fünfte für diesen Prozess relevante Segment
        /// </summary>
        public string Relevantes_IS_5
        {
            get => _relevantes_IS_5;
            set => Set(() => Relevantes_IS_5, ref _relevantes_IS_5, value);

        }

        /// <summary>
        /// Bearbeitender Nutzer des Datensatzes
        /// </summary>
        public string Benutzer
        {
            get => _benutzer;
            set => Set(() => Benutzer, ref _benutzer, value);

        }

        /// <summary>
        /// Bearbeitungsdatum des Datensatzes
        /// </summary>
        public DateTime Datum
        {
            get => _datum;
            set => Set(() => Datum, ref _datum, value);

        }

        /// <summary>
        /// Indikator, ob Prozess momentan aktiv oder nicht (1=ja, 0=nein)
        /// </summary>
        public int Aktiv
        {
            get => _aktiv;
            set => Set(() => Aktiv, ref _aktiv, value);

        }

        #endregion

        /// <summary>
        /// Methode, um zu berechnen ob Prozess aufgrund der Einstufung (der ersten 4 Schutzziele) Kritisch wird
        /// </summary>
        /// <param name="value"> Schutzzielwert </param>
        /// <param name="s1"> Schutzzielwert </param>
        /// <param name="s2"> Schutzzielwert </param>
        /// <param name="s3"> Schutzzielwert </param>
        /// <returns> Wahrheitswert ob kritisch oder nicht </returns>
        public bool CritCalculation(int value, int s1, int s2, int s3)
        {
            List<int> list = new List<int>() {value, s1, s2, s3};
            if (list.Where(x => x >= 3).Count() >= 3 || list.Where(x => x == 4).Count() >= 2)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Methode, um zu berechnen ob Prozess aufgrund der Einstufung (der ersten 4 Schutzziele) nicht Kritisch wird
        /// </summary>
        /// <param name="value"> Schutzzielwert </param>
        /// <param name="s1"> Schutzzielwert </param>
        /// <param name="s2"> Schutzzielwert </param>
        /// <param name="s3"> Schutzzielwert </param>
        /// <returns> Wahrheitswert ob kritisch oder nicht </returns>
        public bool UnCritCalculation(int value, int s1, int s2, int s3)
        {
            List<int> list = new List<int>() {value, s1, s2, s3};
            if (list.Where(x => x >= 3).Count() <= 2 && list.Where(x => x == 4).Count() <= 1)
            {
                return true;
            }

            return false;
        }


        #region Notify data error
        /// <summary>
        /// Auf Fehler prüfen
        /// </summary>
        public void EvaluateErrors()
        {
            if(string.IsNullOrWhiteSpace(_prozess))
                AddError(nameof(Prozess), "Pflichtfeld");
            if (string.IsNullOrWhiteSpace(_oE_Filter))
                AddError(nameof(OE_Filter), "Pflichtfeld");
            if (string.IsNullOrWhiteSpace(_prozessverantwortlicher))
                AddError(nameof(Prozessverantwortlicher), "Pflichtfeld");
            if (string.IsNullOrWhiteSpace(_kritikalität_des_Prozesses))
                AddError(nameof(Kritikalität_des_Prozesses), "Pflichtfeld");
            if (string.IsNullOrWhiteSpace(_reifegrad_des_Prozesses))
                AddError(nameof(Reifegrad_des_Prozesses), "Pflichtfeld");
            if (string.IsNullOrWhiteSpace(_servicezeit_Helpdesk) || _servicezeit_Helpdesk.StartsWith("Bsp"))
                AddError(nameof(Servicezeit_Helpdesk), "Pflichtfeld");
            if (_rTO_Wiederanlaufzeit_Recovery_Time_Objective.ToString() =="0")
                AddError(nameof(RTO_Wiederanlaufzeit_Recovery_Time_Objective), "Pflichtfeld");
            if (_rTO_Wiederanlaufzeit_Recovery_Time_Objective_Notfall.ToString() == "0")
                AddError(nameof(RTO_Wiederanlaufzeit_Recovery_Time_Objective_Notfall), "Pflichtfeld");
        }

        /// <summary>
        /// Dictionary mit Liste der Fehler pro Eigenschaft
        /// </summary>
        public Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        /// <summary>
        /// Event für Änderungsbenachrichtigung bzgl. Fehler
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Methode um Fehler für Eigenschaft abzufragen
        /// </summary>
        /// <param name="propertyName"> Name der zu prüfenden Eigenschaft </param>
        /// <returns> Liste der Fehler für eine Eigenschaft </returns>
        public IEnumerable GetErrors(string propertyName)
        {
            if (_errors.ContainsKey(propertyName))
                return _errors[propertyName];
            return null;
        }

        /// <summary>
        /// Indikator ob Fehler vorhanden
        /// </summary>
        public bool HasErrors => _errors.Count > 0;

        /// <summary>
        /// Indikator ob Objekt gültig (keine Fehler)
        /// </summary>
        public bool IsValid => !HasErrors;

        /// <summary>
        /// Methode um Fehler hinzuzufügen
        /// </summary>
        /// <param name="propertyName"> Name der Fehlerhaften Eigenschaft </param>
        /// <param name="error"> Fehlerbeschreibung </param>
        public void AddError(string propertyName, string error)
        {
            _errors[propertyName] = new List<string>() { error };
            NotifyErrorsChanged(propertyName);
        }

        /// <summary>
        /// Methode um Fehler zu entfernen
        /// </summary>
        /// <param name="propertyName"> Name der ehem. Fehlerhaften Eigenschaft </param>
        public void RemoveError(string propertyName)
        {
            if (_errors.ContainsKey(propertyName))
                _errors.Remove(propertyName);
            NotifyErrorsChanged(propertyName);
        }

        /// <summary>
        /// Event auslösen
        /// </summary>
        /// <param name="propertyName"></param>
        public void NotifyErrorsChanged(string propertyName)
        {
            if (ErrorsChanged != null)
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }
        #endregion
    }
}
