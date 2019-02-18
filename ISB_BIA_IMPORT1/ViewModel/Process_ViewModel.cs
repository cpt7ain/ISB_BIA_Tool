using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using ISB_BIA_IMPORT1.Model;
using ISB_BIA_IMPORT1.LinqDataContext;
using ISB_BIA_IMPORT1.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;

namespace ISB_BIA_IMPORT1.ViewModel
{
    /// <summary>
    /// VM zur Darstellunf der Prozessansicht
    /// </summary>
    public class Process_ViewModel : ViewModelBase, INotifyDataErrorInfo
    {
        #region Backing-Fields
        private int _sZ_1_Max = 0;
        private int _sZ_2_Max = 0;
        private int _sZ_3_Max = 0;
        private int _sZ_4_Max = 0;
        private int _sZ_5_Max = 0;
        private int _sZ_6_Max = 0;
        private Process_Model _currentProcess;
        private Process_Model _oldCurrentProcess;

        private string _prozessverantwortlicher = "";
        private string _prozessverantwortlicher_Text = "";
        private bool _prozessverantwortlicher_Check;
        private string _vorgelagerte_Prozesse = "";
        private string _vorgelagerte_Prozesse_Text = "";
        private bool _vorgelagerte_Prozesse_Check;
        private string _nachgelagerte_Prozesse = "";
        private string _nachgelagerte_Prozesse_Text = "";
        private bool _nachgelagerte_Prozesse_Check;
        private ObservableCollection<string> _oEList;
        private ObservableCollection<string> _processOwnerList;
        private ObservableCollection<string> _critList;
        private ObservableCollection<string> _maturityList;
        private ObservableCollection<string> _preProcessList;
        private ObservableCollection<string> _postProcessList;
        private ObservableCollection<int> _rTOList;
        private Dictionary<string, string> _iSList;
        private KeyValuePair<string, string> _relevantes_IS_1;
        private KeyValuePair<string, string> _relevantes_IS_2;
        private KeyValuePair<string, string> _relevantes_IS_3;
        private KeyValuePair<string, string> _relevantes_IS_4;
        private KeyValuePair<string, string> _relevantes_IS_5;
        private ObservableCollection<ISB_BIA_Applikationen> _removeApplications;
        private ObservableCollection<ISB_BIA_Applikationen> _newApplications;
        private ISB_BIA_Applikationen _selectedSourceItem;
        private ObservableCollection<ISB_BIA_Applikationen> _allApplications;
        private ISB_BIA_Applikationen _selectedTargetItem;
        private ObservableCollection<string> _applicationCategories;
        private static string _selectedFilterItem;
        private CollectionView _view;
        private int _currentTab;
        private Visibility _buttonSaveVisibility;
        private MyRelayCommand _nextTab;
        private MyRelayCommand _prevTab;
        private MyRelayCommand<object> _addApplication;
        private MyRelayCommand<object> _removeApplication;
        private MyRelayCommand _navToISViewChild;
        private MyRelayCommand<string> _navToIS;
        private MyRelayCommand _exportProcessHistory;
        private MyRelayCommand<string> _info;
        private ProcAppMode _mode;
        private MyRelayCommand<string> _showMsg;
        private MyRelayCommand _resetValues;
        #endregion

        #region Mindesteinstufungswerte der Schutzziele
        /// <summary>
        /// Mindesteinstufung nach Berechnung für 1. Schutzziel
        /// </summary>
        public int SZ_1_Max
        {
            get => _sZ_1_Max;
            set => Set(() => SZ_1_Max, ref _sZ_1_Max, value);
        }
        /// <summary>
        /// Mindesteinstufung nach Berechnung für 2. Schutzziel
        /// </summary>
        public int SZ_2_Max
        {
            get => _sZ_2_Max;
            set => Set(() => SZ_2_Max, ref _sZ_2_Max, value);

        }
        /// <summary>
        /// Mindesteinstufung nach Berechnung für 3. Schutzziel
        /// </summary>
        public int SZ_3_Max
        {
            get => _sZ_3_Max;
            set => Set(() => SZ_3_Max, ref _sZ_3_Max, value);

        }
        /// <summary>
        /// Mindesteinstufung nach Berechnung für 4. Schutzziel
        /// </summary>
        public int SZ_4_Max
        {
            get => _sZ_4_Max;
            set => Set(() => SZ_4_Max, ref _sZ_4_Max, value);

        }
        /// <summary>
        /// Mindesteinstufung nach Berechnung für 5. Schutzziel
        /// </summary>
        public int SZ_5_Max
        {
            get => _sZ_5_Max;
            set => Set(() => SZ_5_Max, ref _sZ_5_Max, value);

        }
        /// <summary>
        /// Mindesteinstufung nach Berechnung für 6. Schutzziel
        /// </summary>
        public int SZ_6_Max
        {
            get => _sZ_6_Max;
            set => Set(() => SZ_6_Max, ref _sZ_6_Max, value);

        }
        #endregion

        /// <summary>
        /// Aktueller Prozess
        /// </summary>
        public Process_Model CurrentProcess
        {
            get => _currentProcess;
            set => Set(() => CurrentProcess, ref _currentProcess, value);
        }

        // Eigenschaften, welche gebraucht werden um Korrekte Übernahme der vom User gewollten Werte zu sichern,
        // da Werte an jeweils mehrere Controls gebunden sind (TextBox / Dropdown, gesteuert durch CheckBox)
        // Wenn Check-Property sich ändert, wird der jeweilige Wert aus Dropdown oder Textfeld übernommen
        #region Hilfs-Propterties
        /// <summary>
        /// Prozesvernatwortlicher (DropDown)
        /// </summary>
        public string Prozessverantwortlicher
        {
            get => _prozessverantwortlicher;
            set
            {
                Set(() => Prozessverantwortlicher, ref _prozessverantwortlicher, value);

                if (Prozessverantwortlicher_Check)
                    CurrentProcess.Prozessverantwortlicher = Prozessverantwortlicher_Text;
                else
                    CurrentProcess.Prozessverantwortlicher = Prozessverantwortlicher;
                if (!string.IsNullOrWhiteSpace(_prozessverantwortlicher))
                    RemoveError(nameof(Prozessverantwortlicher));
            }
        }
        /// <summary>
        /// Prozessvernatwortlicher (TextBox)
        /// </summary>
        public string Prozessverantwortlicher_Text
        {
            get => _prozessverantwortlicher_Text;
            set
            {
                Set(() => Prozessverantwortlicher_Text, ref _prozessverantwortlicher_Text, value);
                if (Prozessverantwortlicher_Check)
                    CurrentProcess.Prozessverantwortlicher = Prozessverantwortlicher_Text;
                else
                    CurrentProcess.Prozessverantwortlicher = Prozessverantwortlicher;
                if (!string.IsNullOrWhiteSpace(_prozessverantwortlicher_Text))
                    RemoveError(nameof(Prozessverantwortlicher_Text));
            }
        }
        /// <summary>
        /// Prozessverantwortlicher (CheckBox Switch))
        /// </summary>
        public bool Prozessverantwortlicher_Check
        {
            get => _prozessverantwortlicher_Check;
            set
            {
                Set(() => Prozessverantwortlicher_Check, ref _prozessverantwortlicher_Check, value);
                if (value)
                    CurrentProcess.Prozessverantwortlicher = Prozessverantwortlicher_Text;
                else
                    CurrentProcess.Prozessverantwortlicher = Prozessverantwortlicher;
            }
        }
        /// <summary>
        /// Vorgelagerte Prozesse (DropDown)
        /// </summary>
        public string Vorgelagerte_Prozesse
        {
            get => _vorgelagerte_Prozesse;
            set
            {
                Set(() => Vorgelagerte_Prozesse, ref _vorgelagerte_Prozesse, value);
                if (Vorgelagerte_Prozesse_Check)
                    CurrentProcess.Vorgelagerte_Prozesse = Vorgelagerte_Prozesse_Text;
                else
                    CurrentProcess.Vorgelagerte_Prozesse = Vorgelagerte_Prozesse;
            }
        }
        /// <summary>
        /// Vorgelagerte Prozesse (TextBox)
        /// </summary>
        public string Vorgelagerte_Prozesse_Text
        {
            get => _vorgelagerte_Prozesse_Text;
            set
            {
                Set(() => Vorgelagerte_Prozesse_Text, ref _vorgelagerte_Prozesse_Text, value);
                if (Vorgelagerte_Prozesse_Check)
                    CurrentProcess.Vorgelagerte_Prozesse = Vorgelagerte_Prozesse_Text;
                else
                    CurrentProcess.Vorgelagerte_Prozesse = Vorgelagerte_Prozesse;

            }
        }
        /// <summary>
        /// Vorgelagerte Prozesse (CheckBox Switch)
        /// </summary>
        public bool Vorgelagerte_Prozesse_Check
        {
            get => _vorgelagerte_Prozesse_Check;
            set
            {
                Set(() => Vorgelagerte_Prozesse_Check, ref _vorgelagerte_Prozesse_Check, value);
                if (value)
                    CurrentProcess.Vorgelagerte_Prozesse = Vorgelagerte_Prozesse_Text;
                else
                    CurrentProcess.Vorgelagerte_Prozesse = Vorgelagerte_Prozesse;

            }
        }
        /// <summary>
        /// Nachgelagerte Prozesse (DropDown)
        /// </summary>
        public string Nachgelagerte_Prozesse
        {
            get => _nachgelagerte_Prozesse;
            set
            {
                Set(() => Nachgelagerte_Prozesse, ref _nachgelagerte_Prozesse, value);
                if (Nachgelagerte_Prozesse_Check)
                    CurrentProcess.Nachgelagerte_Prozesse = Nachgelagerte_Prozesse_Text;
                else
                    CurrentProcess.Nachgelagerte_Prozesse = Nachgelagerte_Prozesse;

            }
        }
        /// <summary>
        /// Nachgelagerte Prozesse (TextBox)
        /// </summary>
        public string Nachgelagerte_Prozesse_Text
        {
            get => _nachgelagerte_Prozesse_Text;
            set
            {
                Set(() => Nachgelagerte_Prozesse_Text, ref _nachgelagerte_Prozesse_Text, value);
                if (Nachgelagerte_Prozesse_Check)
                    CurrentProcess.Nachgelagerte_Prozesse = Nachgelagerte_Prozesse_Text;
                else
                    CurrentProcess.Nachgelagerte_Prozesse = Nachgelagerte_Prozesse;
            }
        }
        /// <summary>
        /// Nachgelagerte Prozesse (CheckBox Switch)
        /// </summary>
        public bool Nachgelagerte_Prozesse_Check
        {
            get => _nachgelagerte_Prozesse_Check;
            set
            {
                Set(() => Nachgelagerte_Prozesse_Check, ref _nachgelagerte_Prozesse_Check, value);
                if (value && CurrentProcess != null)
                    CurrentProcess.Nachgelagerte_Prozesse = Nachgelagerte_Prozesse_Text;
                else if (!value && CurrentProcess != null)
                    CurrentProcess.Nachgelagerte_Prozesse = Nachgelagerte_Prozesse;
            }
        }
        /// <summary>
        /// Dictionary Eintrag aus der Informationssegemnt-Liste, welcher zur Anzeige und Zurdnung (SegmentName und Beschreibung) des 1. IS benötigt werden
        /// </summary>
        public KeyValuePair<string, string> Relevantes_IS_1
        {
            get
            {
                _relevantes_IS_1 = ISList.Where(x => x.Key == CurrentProcess.Relevantes_IS_1).FirstOrDefault();
                return _relevantes_IS_1;
            } 
            set
            {
                CurrentProcess.Relevantes_IS_1 = ISList.Where(x => x.Key == value.Key).Select(y => y.Key).FirstOrDefault();
                Set(() => Relevantes_IS_1, ref _relevantes_IS_1, value);
                CheckMinValues(CurrentProcess);
                if (Mode == ProcAppMode.New)
                {
                    CurrentProcess.SZ_1 = (SZ_Values)SZ_1_Max;
                    CurrentProcess.SZ_2 = (SZ_Values)SZ_2_Max;
                    CurrentProcess.SZ_3 = (SZ_Values)SZ_3_Max;
                    CurrentProcess.SZ_4 = (SZ_Values)SZ_4_Max;
                    CurrentProcess.SZ_5 = (SZ_Values)SZ_5_Max;
                    CurrentProcess.SZ_6 = (SZ_Values)SZ_6_Max;
                }
            }
        }
        /// <summary>
        /// Dictionary Eintrag aus der Informationssegemnt-Liste, welcher zur Anzeige und Zurdnung (SegmentName und Beschreibung) des 2. IS benötigt werden
        /// </summary>
        public KeyValuePair<string, string> Relevantes_IS_2
        {
            get
            {
                _relevantes_IS_2 = ISList.Where(x => x.Key == _currentProcess.Relevantes_IS_2).FirstOrDefault();
                return _relevantes_IS_2;
            } 
            set
            {
                CurrentProcess.Relevantes_IS_2 = ISList.Where(x => x.Key == value.Key).Select(y => y.Key).FirstOrDefault();
                Set(() => Relevantes_IS_2, ref _relevantes_IS_2, value);
                CheckMinValues(CurrentProcess);
                if (Mode == ProcAppMode.New)
                {
                    CurrentProcess.SZ_1 = (SZ_Values)SZ_1_Max;
                    CurrentProcess.SZ_2 = (SZ_Values)SZ_2_Max;
                    CurrentProcess.SZ_3 = (SZ_Values)SZ_3_Max;
                    CurrentProcess.SZ_4 = (SZ_Values)SZ_4_Max;
                    CurrentProcess.SZ_5 = (SZ_Values)SZ_5_Max;
                    CurrentProcess.SZ_6 = (SZ_Values)SZ_6_Max;
                }
            }
        }
        /// <summary>
        /// Dictionary Eintrag aus der Informationssegemnt-Liste, welcher zur Anzeige und Zurdnung (SegmentName und Beschreibung) des 3. IS benötigt werden
        /// </summary>
        public KeyValuePair<string, string> Relevantes_IS_3
        {
            get
            {
                _relevantes_IS_3 = ISList.Where(x => x.Key == _currentProcess.Relevantes_IS_3).FirstOrDefault();
                return _relevantes_IS_3;
            }
            set
            {
                CurrentProcess.Relevantes_IS_3 = ISList.Where(x => x.Key == value.Key).Select(y => y.Key).FirstOrDefault();
                Set(() => Relevantes_IS_3, ref _relevantes_IS_3, value);
                CheckMinValues(CurrentProcess);
                if (Mode == ProcAppMode.New)
                {
                    CurrentProcess.SZ_1 = (SZ_Values)SZ_1_Max;
                    CurrentProcess.SZ_2 = (SZ_Values)SZ_2_Max;
                    CurrentProcess.SZ_3 = (SZ_Values)SZ_3_Max;
                    CurrentProcess.SZ_4 = (SZ_Values)SZ_4_Max;
                    CurrentProcess.SZ_5 = (SZ_Values)SZ_5_Max;
                    CurrentProcess.SZ_6 = (SZ_Values)SZ_6_Max;
                }
            }
        }
        /// <summary>
        /// Dictionary Eintrag aus der Informationssegemnt-Liste, welcher zur Anzeige und Zurdnung (SegmentName und Beschreibung) des 4. IS benötigt werden
        /// </summary>
        public KeyValuePair<string, string> Relevantes_IS_4
        {
            get
            {
                _relevantes_IS_4 = ISList.Where(x => x.Key == _currentProcess.Relevantes_IS_4).FirstOrDefault();
                return _relevantes_IS_4;
            }
            set
            {
                CurrentProcess.Relevantes_IS_4 = ISList.Where(x => x.Key == value.Key).Select(y => y.Key).FirstOrDefault();
                Set(() => Relevantes_IS_4, ref _relevantes_IS_4, value);
                CheckMinValues(CurrentProcess);
                if (Mode == ProcAppMode.New)
                {
                    CurrentProcess.SZ_1 = (SZ_Values)SZ_1_Max;
                    CurrentProcess.SZ_2 = (SZ_Values)SZ_2_Max;
                    CurrentProcess.SZ_3 = (SZ_Values)SZ_3_Max;
                    CurrentProcess.SZ_4 = (SZ_Values)SZ_4_Max;
                    CurrentProcess.SZ_5 = (SZ_Values)SZ_5_Max;
                    CurrentProcess.SZ_6 = (SZ_Values)SZ_6_Max;
                }
            }
        }
        /// <summary>
        /// Dictionary Eintrag aus der Informationssegemnt-Liste, welcher zur Anzeige und Zurdnung (SegmentName und Beschreibung) des 5. IS benötigt werden
        /// </summary>
        public KeyValuePair<string, string> Relevantes_IS_5
        {
            get
            {
                _relevantes_IS_5 = ISList.Where(x => x.Key == _currentProcess.Relevantes_IS_5).FirstOrDefault();
                return _relevantes_IS_5;
            }
            set
            {
                CurrentProcess.Relevantes_IS_5 = ISList.Where(x => x.Key == value.Key).Select(y => y.Key).FirstOrDefault();
                Set(() => Relevantes_IS_5, ref _relevantes_IS_5, value);
                CheckMinValues(CurrentProcess);
                if (Mode == ProcAppMode.New)
                {
                    CurrentProcess.SZ_1 = (SZ_Values)SZ_1_Max;
                    CurrentProcess.SZ_2 = (SZ_Values)SZ_2_Max;
                    CurrentProcess.SZ_3 = (SZ_Values)SZ_3_Max;
                    CurrentProcess.SZ_4 = (SZ_Values)SZ_4_Max;
                    CurrentProcess.SZ_5 = (SZ_Values)SZ_5_Max;
                    CurrentProcess.SZ_6 = (SZ_Values)SZ_6_Max;
                }
            }
        }
        #endregion

        #region DropDown Listen
        /// <summary>
        /// Liste aller OE-Gruppen
        /// </summary>
        public ObservableCollection<string> OEList
        {
            get => _oEList;
            set => Set(() => OEList, ref _oEList, value);
        }
        /// <summary>
        /// Liste aller Prozesseigentümer
        /// </summary>
        public ObservableCollection<string> ProcessOwnerList
        {
            get => _processOwnerList;
            set => Set(() => ProcessOwnerList, ref _processOwnerList, value);
        }
        /// <summary>
        /// Liste der Kritikalitätsstufen
        /// </summary>
        public ObservableCollection<string> CritList
        {
            get => _critList;
            set => Set(() => CritList, ref _critList,value);
        }
        /// <summary>
        /// Liste der Prozessreifegrade
        /// </summary>
        public ObservableCollection<string> MaturityList
        {
            get => _maturityList;
            set => Set(() => MaturityList, ref _maturityList, value);
        }
        /// <summary>
        /// Liste der Vorgelagerten Prozesse
        /// </summary>
        public ObservableCollection<string> PreProcessList
        {
            get => _preProcessList;
            set => Set(() => PreProcessList, ref _preProcessList, value);
        }
        /// <summary>
        /// Liste der Nachgelagerten Prozesse
        /// </summary>
        public ObservableCollection<string> PostProcessList
        {
            get => _postProcessList;
            set => Set(() => PostProcessList, ref _postProcessList, value);
        }
        /// <summary>
        /// Liste der Zeiten für das RTO
        /// </summary>
        public ObservableCollection<int> RTOList
        {
            get => _rTOList;
            set => Set(() => RTOList, ref _rTOList, value);
        }
        /// <summary>
        /// Disctionary der Informationssegmente (SegmentName, Beschreibung)
        /// </summary>
        public Dictionary<string, string> ISList
        {
            get => _iSList;
            set => Set(() => ISList, ref _iSList, value);
        }
        #endregion

        #region ApplicationToProcess
        /// <summary>
        /// Liste der Applikation, für welche mit diesem Prozess ein Eintrag mit Relation=0 erzeugt werden soll
        /// </summary>
        public ObservableCollection<ISB_BIA_Applikationen> RemoveApplications
        {
            get => _removeApplications;
            set => Set(() => RemoveApplications, ref _removeApplications, value);
        }
        /// <summary>
        /// Liste der Applikation, für welche mit diesem Prozess ein Eintrag mit Relation=1 erzeugt werden soll
        /// </summary>
        public ObservableCollection<ISB_BIA_Applikationen> NewApplications
        {
            get => _newApplications;
            set => Set(() => NewApplications, ref _newApplications, value);
        }    
        /// <summary>
        /// Ausgewähltes Item der Quell-Applikationsliste
        /// </summary>
        public ISB_BIA_Applikationen SelectedSourceItem
        {
            get => _selectedSourceItem;
            set => Set(() => SelectedSourceItem, ref _selectedSourceItem, value);
        }
        /// <summary>
        /// Liste aller Applikationen
        /// </summary>
        public ObservableCollection<ISB_BIA_Applikationen> AllApplications
        {
            get => _allApplications;
            set => Set(() => AllApplications, ref _allApplications, value);

        }
        /// <summary>
        /// Ausgewähltes Item der Ziel-Applikationsliste
        /// </summary>
        public ISB_BIA_Applikationen SelectedTargetItem
        {
            get => _selectedTargetItem;
            set => Set(() => SelectedTargetItem, ref _selectedTargetItem, value);
        }
        /// <summary>
        /// Liste der Applikationskategorien für die Filterung
        /// </summary>
        public ObservableCollection<string> ApplicationCategories
        {
            get => _applicationCategories;
            set => Set(() => ApplicationCategories, ref _applicationCategories, value);
        }
        /// <summary>
        /// Ausgewähltes Filter Item aus der Liste der Applikationskategorien
        /// </summary>
        public string SelectedFilterItem
        {
            get => _selectedFilterItem;
            set
            {
                Set(() => SelectedFilterItem, ref _selectedFilterItem, value);
                View.Refresh();
            }
        }
        /// <summary>
        /// CollectionView für die Filterung
        /// </summary>
        public CollectionView View
        {
            get => _view;
            set => Set(() => View, ref _view, value);
        }
        /// <summary>
        /// Definition des Filters für die Applikationsliste (Filtern auf Kategorien der Anwendungenen)
        /// </summary>
        /// <param name="item"> zu Filterndes Item </param>
        /// <returns></returns>
        public static bool ApplicationFilter(object item)
        {
            ISB_BIA_Applikationen logItem = (ISB_BIA_Applikationen)item;
            if (String.IsNullOrEmpty(_selectedFilterItem))
                return true;
            else if (logItem.IT_Betriebsart == null)
            {
                return true;
            }
            else
            {
                if (_selectedFilterItem == "<Alle>") return true;
                return (logItem.IT_Betriebsart.IndexOf(_selectedFilterItem, StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }
        /// <summary>
        /// Sichtbarkeit der Notifikation, ob einem Prozess momentan inaktive Anwendungen zugeordnet sind (Wenn eine Anwendung auf inaktiv gesetzt wurd, ohne sie vorher von allen Prozessen zu entfernen)
        /// </summary>
        public Visibility DeletedApplicationsNotification
        {
            get
            {
                //Prüfen ob dem Prozess momentan Anwendungen zugeordnet sind, welche durch den IT-Betrieb deaktiviert/entfernt wurden
                int inactive = 0;
                foreach (ISB_BIA_Applikationen pa in CurrentProcess.ApplicationList)
                {
                    if (pa.Aktiv == 0) inactive = 1;
                }
                if (inactive == 1)
                    return Visibility.Visible;
                else
                    return Visibility.Hidden;
            }
        }
        #endregion

        #region Commands
        /// <summary>
        /// Aktueller Tab
        /// </summary>
        public int CurrentTab
        {
            get => _currentTab;
            set
            {
                Set(() => CurrentTab, ref _currentTab, value);
                ButtonSaveVisibility = (_currentTab == 6) ? Visibility.Visible : Visibility.Hidden;
            }
        }
        /// <summary>
        /// Sichtbarkeit des Speicherbuttons (nur wenn letzter Tab aktiv)
        /// </summary>
        public Visibility ButtonSaveVisibility
        {
            get => _buttonSaveVisibility;
            set => Set(() => ButtonSaveVisibility, ref _buttonSaveVisibility, value);

        }
        /// <summary>
        /// Zum nächsten Tab navigieren
        /// </summary>
        public MyRelayCommand NextTab
        {
            get => _nextTab
                    ?? (_nextTab = new MyRelayCommand(() =>
                    {
                        CurrentTab += 1;
                    }, () => { return CurrentTab != 6; }));
        }
        /// <summary>
        /// Zum vorherigen Tab navigieren
        /// </summary>
        public MyRelayCommand PrevTab
        {
            get => _prevTab
                    ?? (_prevTab = new MyRelayCommand(() =>
                    {
                        CurrentTab -= 1;
                    }, () => { return CurrentTab != 0; }));
        }
        /// <summary>
        /// Anwendung zur Liste verknüpfter Anwendungen hinzufügen
        /// </summary>
        public MyRelayCommand<object> AddApplication
        {
            get => _addApplication
                  ?? (_addApplication = new MyRelayCommand<object>((list) =>
                  {
                      IList items = (IList)list;
                      if (items.Count > 0)
                      {
                          var collection = items.Cast<ISB_BIA_Applikationen>();
                          //Wenn einzelne Applikation hinzugefügt wird
                          if (collection.Count() == 1)
                          {
                              //Prüfen ob Anwendung bereits in Zielliste
                              bool contained = CurrentProcess.ApplicationList.Count(x => x.Applikation_Id == SelectedSourceItem.Applikation_Id)==1;
                              if (contained)
                              {
                                  _myDia.ShowInfo("Anwendung bereits zugewiesen");
                                  SelectedTargetItem = SelectedSourceItem;
                              }
                              //Wenn aktuell nicht zugeordnet
                              else
                              {
                                  //Einfügen
                                  CurrentProcess.ApplicationList.Insert(0, SelectedSourceItem);                                 
                                  _myDia.ShowMessage(SelectedSourceItem.IT_Anwendung_System + " wurde hinzugefügt.");
                                  SelectedTargetItem = SelectedSourceItem;
                              }
                          }
                          //Wenn mehr als eine Applikation hinzugefügt wird
                          else
                          {
                              //Liste ausgewählter Anwendungen in Quellliste
                              List<ISB_BIA_Applikationen> selectedList = new List<ISB_BIA_Applikationen>(collection);
                              //Liste der Anwendungen die bereits zugewiesen
                              List<ISB_BIA_Applikationen> listRedundant = new List<ISB_BIA_Applikationen>(selectedList.Where(x =>
                                  CurrentProcess.ApplicationList.Select(v => v.Applikation_Id).ToList().Contains(x.Applikation_Id)).ToList());
                              //Liste der Anwendung die noch nicht zugewiesen
                              List<ISB_BIA_Applikationen> listAdd = new List<ISB_BIA_Applikationen>(selectedList.Where(x=>
                                  !CurrentProcess.ApplicationList.Select(v => v.Applikation_Id).ToList().Contains(x.Applikation_Id)).ToList());
                              //Alle gültigen Anwendungen in Zielliste einfügen
                              if (listAdd.Count > 0)
                              {
                                  foreach (var item in listAdd)
                                  {
                                      //Einfügen
                                      CurrentProcess.ApplicationList.Insert(0, item);
                                      SelectedTargetItem = listAdd.FirstOrDefault();
                                  }
                                  #region Benachrichtigung über hinzugefügte Anwendungen
                                  string a = "Folgende Anwendungen wurden hinzugefügt:";
                                  string b = "\nFolgende Anwendungen waren bereits in der Liste:";
                                  int i = 0;
                                  foreach (var f in listAdd)
                                  {                          
                                      i++;
                                      if (i < 11) a = a + "\n-" + f.IT_Anwendung_System; 
                                  }

                                  if (i > 10) a = a + "\nund " + (i - 10) + " weitere Anwendungen";

                                  if (listRedundant.Count > 0)
                                  {
                                      i = 0;
                                      foreach (var f in listRedundant)
                                      {                                          
                                          i++;
                                          if (i < 11) b = b + "\n" + f.IT_Anwendung_System;
                                      }
                                      if (i > 10) b = b + "\nund " + (i - 10) + " weitere Anwendungen";
                                      _myDia.ShowMessage(a+"\n"+b);
                                  }
                                  else
                                  {
                                      _myDia.ShowMessage(a);
                                  }
                                  #endregion
                              }
                              else
                              {
                                  _myDia.ShowInfo("Alle ausgewählten Applikationen sind dem Prozess bereits zugeordnet");
                              }
                          }
                      }
                      else
                          _myDia.ShowInfo("Bitte hinzuzufügende Anwendung auswählen");
                  }));
        }
        /// <summary>
        /// Anwendung von Liste verknüpfter Anwendungen entfernen
        /// </summary>
        public MyRelayCommand<object> RemoveApplication
        {
            get => _removeApplication
                  ?? (_removeApplication = new MyRelayCommand<object>((list) =>
                  {
                      IList items = (IList)list;
                      if (items.Count > 0)
                      {
                          var collection = items.Cast<ISB_BIA_Applikationen>();
                          //Wenn einzelne Applikation entfernt wird
                          if (collection.Count() == 1)
                          {
                              //Entfernen
                              ISB_BIA_Applikationen tmp = SelectedTargetItem;
                              CurrentProcess.ApplicationList.Remove(SelectedTargetItem);
                              _myDia.ShowMessage(tmp.IT_Anwendung_System + " wurde entfernt.");
                          }
                          //Wenn mehrere Applikationen entfernt werden
                          else
                          {
                              List<ISB_BIA_Applikationen> selectedList = new List<ISB_BIA_Applikationen>(collection);
                              string a = "Folgende Anwendungen wurden entfernt:\n";

                              foreach (ISB_BIA_Applikationen item in selectedList)
                              {
                                  //Entfernen
                                  CurrentProcess.ApplicationList.Remove(item);
                                  a = a + "\n" + item.IT_Anwendung_System;
                              }
                              _myDia.ShowMessage(a);
                          }
                          RaisePropertyChanged(() => DeletedApplicationsNotification);
                      }
                      else
                          _myDia.ShowInfo("Bitte zu löschende Anwendung auswählen");
                  }));
        }
        /// <summary>
        /// Command zum Zurückkehren zum vorherigen VM
        /// </summary>
        public MyRelayCommand NavBack
        {
            get => new MyRelayCommand(() =>
                {
                    if (_myDia.CancelDecision())
                    {
                        Cleanup();
                        _myNavi.NavigateBack(true);
                        _myData.UnlockObject(Table_Lock_Flags.Process, CurrentProcess.Prozess_Id);
                    }
                });
        }
        /// <summary>
        /// Zum <see cref="InformationSegmentsView_ViewModel"/> navigieren
        /// </summary>
        public MyRelayCommand NavToISViewChild
        {
            get => _navToISViewChild
                  ?? (_navToISViewChild = new MyRelayCommand(() =>
                  {
                      _myNavi.NavigateTo<InformationSegmentsView_ViewModel>(ISISAttributeMode.View);
                  }));
        }
        /// <summary>
        /// Zum <see cref="InformationSegment_ViewModel"/> des ausgewählten Segments navigieren
        /// </summary>
        public MyRelayCommand<string> NavToIS
        {
            get => _navToIS
                  ?? (_navToIS = new MyRelayCommand<string>((msg) =>
                  {
                      ISB_BIA_Informationssegmente seg = _myData.GetISByISName(msg);
                      if (seg != null)
                      {
                          _myNavi.NavigateTo<InformationSegment_ViewModel>(seg.Informationssegment_Id, ISISAttributeMode.View);
                      }
                      else
                      {
                          _myDia.ShowInfo("Bitte gültiges Segment auswählen.");
                      }
                  }));
        }
        /// <summary>
        /// Speichern der Prozess- und Relationsdaten und zum vorherigen VM navigieren
        /// </summary>
        public MyRelayCommand SaveAndContinue
        {
            get => new MyRelayCommand(() =>
            {
                //Liste der aktuellen Zuordnung durchgehen und zur Addliste hinzufügen, welche in alter Liste noch nicht vorhanden waren
                NewApplications = new ObservableCollection<ISB_BIA_Applikationen>(
                    CurrentProcess.ApplicationList.Where(x => 
                        !_oldCurrentProcess.ApplicationList.Select(c => c.Applikation_Id).ToList().Contains(x.Applikation_Id)).ToList());
                //Liste der Ausgangszuordnung durchgehen und zur Removeliste hinzufügen, welche in aktueller Liste nicht mehr vorhanden
                RemoveApplications = new ObservableCollection<ISB_BIA_Applikationen>(
                    _oldCurrentProcess.ApplicationList.Where(x =>
                        !CurrentProcess.ApplicationList.Select(c => c.Applikation_Id).ToList().Contains(x.Applikation_Id)).ToList());
                //Prozessdaten auf Fehler prüfen
                CurrentProcess.EvaluateErrors();
                EvaluateErrors();
                //Prozess + Relationen einfügen
                if (_myData.InsertProcessAndRelations(CurrentProcess, Mode, NewApplications, RemoveApplications))
                {
                    bool refreshMsg = (Mode == ProcAppMode.Change) ? true : false;
                    Cleanup();
                    _myNavi.NavigateBack(refreshMsg);
                    _myData.UnlockObject(Table_Lock_Flags.Process, CurrentProcess.Prozess_Id);
                }
            });
        }
        /// <summary>
        /// Prozesshistorie nach Excel exportieren
        /// </summary>
        public MyRelayCommand ExportProcessHistory
        {
            get => _exportProcessHistory
                    ?? (_exportProcessHistory = new MyRelayCommand(() =>
                    {
                        _myExport.ExportProcesses(_myData.GetProcessHistory(CurrentProcess.Prozess_Id), CurrentProcess.Prozess_Id);
                    },()=>Mode == ProcAppMode.Change));
        }
        /// <summary>
        /// Zum Infofenster navigieren und jeweilige Info anzeigen (Parameter in XAML angegeben)
        /// </summary>
        public MyRelayCommand<string> Info
        {
            get => _info
                    ?? (_info = new MyRelayCommand<string>((name) =>
                    {
                        try
                        {
                            string file = _myShared.InitialDirectory + @"\" + name + "_Info.xps";
                            if (File.Exists(file))
                            {
                                XpsDocument xpsDocument = new XpsDocument(file, FileAccess.Read);
                                FixedDocumentSequence fds = xpsDocument.GetFixedDocumentSequence();
                                _myNavi.NavigateTo<DocumentView_ViewModel>();
                                Messenger.Default.Send(fds);
                            }
                            else
                            {
                                _myDia.ShowInfo("Keine Beschreibung verfügbar.");
                            }
                        }
                        catch (Exception ex)
                        {
                            _myDia.ShowError("Keine Beschreibung verfügbar.", ex);
                        }
                    }));
        }
        /// <summary>
        /// Anzeigen defnierter Nachrichten (Parameter in XAML definiert)
        /// </summary>
        public MyRelayCommand<string> ShowMsg
        {
            get => _showMsg
                  ?? (_showMsg = new MyRelayCommand<string>((msg) =>
                  {
                      _myDia.ShowMessage(msg);
                  }));
        }
        /// <summary>
        /// Zurücksetzen der Daten der aktuellen Maske (Nur bei Prozessbearbeitung, nicht Neuanlage)
        /// </summary>
        public MyRelayCommand ResetValues
        {
            get => _resetValues
                   ?? (_resetValues = new MyRelayCommand(() =>
                   {
                       switch (CurrentTab)
                       {
                           case 0:
                               CurrentProcess.Prozess = _oldCurrentProcess.Prozess;
                               CurrentProcess.Sub_Prozess = _oldCurrentProcess.Sub_Prozess;
                               CurrentProcess.OE_Filter = _oldCurrentProcess.OE_Filter;
                               Prozessverantwortlicher = _oldCurrentProcess.Prozessverantwortlicher;
                               Prozessverantwortlicher_Text = "";
                               CurrentProcess.Reifegrad_des_Prozesses = _oldCurrentProcess.Reifegrad_des_Prozesses;
                               break;
                           case 1:
                               _myDia.ShowMessage((_oldCurrentProcess.Vorgelagerte_Prozesse == string.Empty)?"ist ''":"passt nicht, ist'"+_oldCurrentProcess.Vorgelagerte_Prozesse+"'" );
                               Vorgelagerte_Prozesse = _oldCurrentProcess.Vorgelagerte_Prozesse;
                               Vorgelagerte_Prozesse_Text = "";
                               Nachgelagerte_Prozesse = _oldCurrentProcess.Nachgelagerte_Prozesse;
                               Nachgelagerte_Prozesse_Text = "";
                               break;
                           case 2:
                               Relevantes_IS_1 = ISList.Where(x => x.Key == _oldCurrentProcess.Relevantes_IS_1).FirstOrDefault();
                               Relevantes_IS_2 = ISList.Where(x => x.Key == _oldCurrentProcess.Relevantes_IS_2).FirstOrDefault();
                               Relevantes_IS_3 = ISList.Where(x => x.Key == _oldCurrentProcess.Relevantes_IS_3).FirstOrDefault();
                               Relevantes_IS_4 = ISList.Where(x => x.Key == _oldCurrentProcess.Relevantes_IS_4).FirstOrDefault();
                               Relevantes_IS_5 = ISList.Where(x => x.Key == _oldCurrentProcess.Relevantes_IS_5).FirstOrDefault();
                               break;
                           case 3:
                               CurrentProcess.SZ_1 = _oldCurrentProcess.SZ_1;
                               CurrentProcess.SZ_2 = _oldCurrentProcess.SZ_2;
                               CurrentProcess.SZ_3 = _oldCurrentProcess.SZ_3;
                               CurrentProcess.SZ_4 = _oldCurrentProcess.SZ_4;
                               CurrentProcess.SZ_5 = _oldCurrentProcess.SZ_5;
                               CurrentProcess.SZ_6 = _oldCurrentProcess.SZ_6;
                               break;
                           case 4:
                               CurrentProcess.Servicezeit_Helpdesk = _oldCurrentProcess.Servicezeit_Helpdesk;
                               CurrentProcess.RPO_Datenverlustzeit_Recovery_Point_Objective = _oldCurrentProcess.RPO_Datenverlustzeit_Recovery_Point_Objective;
                               CurrentProcess.RTO_Wiederanlaufzeit_Recovery_Time_Objective = _oldCurrentProcess.RTO_Wiederanlaufzeit_Recovery_Time_Objective;
                               CurrentProcess.RTO_Wiederanlaufzeit_Recovery_Time_Objective_Notfall = _oldCurrentProcess.RTO_Wiederanlaufzeit_Recovery_Time_Objective_Notfall;
                               break;
                           case 5:
                               CurrentProcess.Kritikalität_des_Prozesses = _oldCurrentProcess.Kritikalität_des_Prozesses;
                               CurrentProcess.Regulatorisch = _oldCurrentProcess.Regulatorisch;
                               CurrentProcess.Reputatorisch = _oldCurrentProcess.Reputatorisch;
                               CurrentProcess.Finanziell = _oldCurrentProcess.Finanziell;
                               break;
                           case 6:
                               CurrentProcess.ApplicationList = new ObservableCollection<ISB_BIA_Applikationen>(_oldCurrentProcess.ApplicationList);
                               NewApplications = new ObservableCollection<ISB_BIA_Applikationen>();
                               RemoveApplications = new ObservableCollection<ISB_BIA_Applikationen>();
                               RaisePropertyChanged(()=>DeletedApplicationsNotification);
                               break;
                       }
                   }));
        }
        #endregion

        #region Platzhalter und Nachrichtenstringfelder
        /// <summary>
        /// Platzhalter Prozessverantwortlicher
        /// </summary>
        public string Ph_Responsible
        {
            get => "Bsp.: Fr. Vorname Nachname / Hr. Vorname Nachname";
        }
        /// <summary>
        /// Platzhalter Servicezeiten
        /// </summary>
        public string Ph_Times
        {
            get => "Bsp.: Mo-Do: 08-12 / 14-16 Uhr, Fr: 08-14 Uhr";
        }

        /// <summary>
        /// Nachricht bei Kritischer Einstufung des Prozesses
        /// </summary>
        public string Krit_Msg
        {
            get => "Ein Prozess wird als 'Kritischer Prozess' eingestuft wenn mindestens eine der folgenden Bedingungen zutrifft:"
                 + "\n- mindestens 3 der Schutzziele werden als mindestens 'Hoch' eingestuft oder mindestens 2 als 'Sehr Hoch'"
                 + "\n- Das Recovery Time Objective (RTO) wurde auf 1 Tag festgelegt"
                 + "\n- Die Kritikalität des Prozesses wurde auf 'Sehr Hoch' festgelegt\n"
                 + "\nKritische Prozesse müssen identifiziert und dokumentiert werden!"
                 + "\nDie Dokumentation muss folgende Anforderungen erfüllen:"
                 + "\n- kurze Beschreibung des Prozesses (Prozessbezeichnung, was bewirkt der Prozess)"
                 + "\n- Begründung, warum der Prozess ein zentraler/kritischer Prozess "
                 + "\nbzw. ein Prozess mit hohem Schadenspotential ist"
                 + "\n- Benennung des Prozessverantwortlichen"
                 + "\n- Maximal tolerierbare Ausfallzeit des Prozesses"
                 + "\n- Auswirkung auf Folgeprozesse"
                 + "\n- ggf. Workaround bei Prozessstörung";
        }
        /// <summary>
        /// Regulatorisch Info
        /// </summary>
        public string Reg_Msg
        {
            get => "Rechtliche und aufsichtsrechtliche Vorgaben werden nicht eingehalten.\nAus einer Nichteinhaltung können aufsichtsrechtliche Konsequenzen für die Bank und somit ein Schaden entstehen.";
        }
        /// <summary>
        /// Reputatorisch info
        /// </summary>
        public string Rep_Msg
        {
            get => "Durch den Verlust einer der Grundwerte Vertraulichkeit, Verfügbarkeit, Integrität oder Authentizität der verarbeiteten Informationen in einer IT-Anwendung können verschiedenartige negative Innen- oder Außenwirkungen entstehen:" +
                        "\n- Ansehensverlust des Unternehmens" +
                        "\n- Vertrauensverlust ggü. dem Unternehmen" +
                        "\n- Demoralisierung der Mitarbeiter" +
                        "\n- Beeinträchtigung der wirtschaftlichen Beziehungen zusammenarbeitender Institutionen" +
                        "\n- verlorenes Vertrauen in die Arbeitsqualität des Unternehmens und" +
                        "\n- Einbuße der Wettbewerbsfähigkeit";
        }
        /// <summary>
        /// Finanziell Info
        /// </summary>
        public string Fin_Msg
        {
            get => "Unmittelbare oder mittelbare Schäden können durch den Verlust der Vertraulichkeit schutzbedürftiger Daten, die Veränderung von Daten, mangelnde Verbindlichkeit einer Transaktion oder den Ausfall einer IT-Anwendung entstehen."+
                        "\nBeispiele dafür sind:"+
                        "\n -unerlaubte Weitergabe von strategischen und taktischen Geschäftsinformationen"+
                        "\n -Manipulation von finanzwirksamen Daten in einem Abrechnungssystem" +
                        "\n -Ausfall eines IT - gesteuerten Produktionssystems und dadurch bedingte Umsatzverluste" +
                        "\n -Einsichtnahme in Marketingstrategiepapiere oder Umsatzzahlen" +
                        "\n -Diebstahl oder Zerstörung von Hardware";
        }
        /// <summary>
        /// Informationssegment Info
        /// </summary>
        public string IS_Msg
        {
            get => "Bitte wählen Sie bis zu fünf Informationssegmente aus, die Ihren Prozess betreffen.\nEin Informationssegment beschreibt dabei eine Kategorie von Informationen die für unterschiedliche Geschäftsprozesse Relevanz haben, wie z.B. Kundenstammdaten oder Rechtsdaten.\nWeitere Informationen zu dem ausgewählten Informationssegment erhalten Sie über den jeweiligen Infobutton rechts daneben.";
        }
        /// <summary>
        /// Recovery Point Objective Info
        /// </summary>
        public string RPO_Msg
        {
            get => "Datenverlustzeit – Recovery Point Objective (RPO)\nDefinierter Zeitraum der zwischen zwei Backups liegen darf, um den Normalbetrieb im Notfall oder bei einer Störung sicherzustellen.\nDer RPO berechnet sich rückwärts vom Zeitpunkt des Absturzes (aufgrund der Backup-Zyklen max. 24 Stunden).";
        }
        /// <summary>
        /// Recovery Time Objective Info
        /// </summary>
        public string RTO_Msg
        {
            get => "Wiederanlaufzeit Regelbetrieb – Recovery Time Objective (RTO)\nDauer für die Wiederaufnahme eines Bankprozesses nach einer Störung oder eines Notfalls im Regelbetrieb (in 1, 5, 10, 20 Tagen).";
        }
        /// <summary>
        /// Recovery Time Objective Notfall Info
        /// </summary>
        public string RTON_Msg
        {
            get => "Wiederanlaufzeit (Notfall, Störung) – Recovery Time Objective (RTO)\nDauer für die Wiederaufnahme eines Bankprozesses nach einer Störung oder eines Notfalls im Notfall/Störfall (in 1, 5, 10, 20 Tagen).";
        }
        /// <summary>
        /// Prozessverantwortlicher Info
        /// </summary>
        public string Responsible_Msg
        {
            get => "Die Neuerhebung oder Neuweinwertung erfolgt in der Regel durch den Prozesseigentümer oder einen Vertreter. Der Prozesseigentümer hat entsprechende Verpflichtungen wahrzunehmen. Diese sind u.a.:"+
                        "\n- Aufnahme des Bankprozesses in die Business Impact Analyse"+
                        "\n- Bewertung der Kritikalität der Bankprozesse innerhalb der Business Impact Analyse(BIA) in Bezug auf die vier Schutzziele(Verfügbarkeit, Integrität, Vertraulichkeit, und Authentizität) u.a.als Basis für das zentrale Auslagerungsmanagement, das Informationsrisikomanagement / Business Continuity Management und dem Informationssicherheitsmanagement"+
                        "\n- Kurzbeschreibung des Prozesses" +
                        "\n- Begründung, warum der Prozess ein zentraler/kritischer Prozess bzw. ein Prozess mit hohem Schadenspotential ist" +
                        "\n- Maximal tolerierbare Ausfallzeit des Prozesses" +
                        "\n- Auswirkung auf Folgeprozesse.";
        }
        /// <summary>
        /// Vorgelagerte Prozesse Info
        /// </summary>
        public string PreProc_Msg
        {
            get => "Die Prozesse, die Ihrem Prozess Startdaten liefern oder ersatzweise die OE's / externe Stellen, die die Vorarbeit zu diesem Prozess leisten.";
        }
        /// <summary>
        /// Nachgelagerte Prozesse Info
        /// </summary>
        public string PostProc_Msg
        {
            get => "Die Prozesse, die aus Prozess Ergebnisse erhalten oder ersatzweise die OE's / externe Stellen, die die Ergebnisse Ihres Prozesses weiterverarbeiten.";
        }
        /// <summary>
        /// Prozessname Info
        /// </summary>
        public string PName_Msg
        {
            get => "Der Name des Prozesses";
        }
        /// <summary>
        /// Sub-Prozessname Info
        /// </summary>
        public string SName_Msg
        {
            get => "Der Name des Sub-Prozesses (optional falls Sub-Prozess vorhanden)";
        }
        /// <summary>
        /// OE-Info
        /// </summary>
        public string OE_Msg
        {
            get => "Die für den Prozess verantwortliche OE";
        }
        /// <summary>
        /// Servicezeiten Info
        /// </summary>
        public string ServiceTimes_Msg
        {
            get => "Die zur Sicherstellung des Prozessablaufs optimale Servicezeit";
        }
        /// <summary>
        /// Applikations-Prozess Zuordnung Info
        /// </summary>
        public string AppToProc_Msg
        {
            get => "Wählen Sie ein oder mehrere Anwendungen aus der Übersicht aller Anwendungen (links) aus und klicken Sie den Button \"hinzufügen\", um die jeweilige Anwendung mit dem Prozess zu verknüpfen.\nAnalog wählen Sie eine Anwendung aus der Liste der bereits verknüpften Anwendungen (rechts) und klicken Sie \"entfernen\", um die jeweilige Verknüpfung zu entfernen.";
        }
        /// <summary>
        /// Zuordnung inativer Applikationenen Benachrichtigung
        /// </summary>
        public string InactiveApp_Msg
        {
            get => "Diesem Prozess sind inaktive Anwendungen zugeordnet. " +
                "\nDas bedeutet, dass die in der Liste rot markierten Anwendungen vom IT-Betrieb als inaktiv markiert wurden, da sie nicht mehr genutzt werden oder nicht mehr zur Verfügung stehen. " +
                "\nPrüfen Sie daher bitte die gekennzeichneten Anwendungen und entfernen Sie diese aus der Liste, falls Sie die Anwendung tatsächlich nicht mehr nutzen. " +
                "\nBei Fragen wenden Sie sich ggf. an den IT-Betrieb.";
        }
        #endregion

        #region sonstige Eigenschaften
        /// <summary>
        /// Überschrift
        /// </summary>
        public string Header { get; set; }
        /// <summary>
        /// Bestimmt, ob neue Schutzziele angezeigt werden oder nicht (abhängig von Einstellungen)
        /// </summary>
        public bool SchutzzielVisible { get; set; }
        /// <summary>
        /// Schriftgröße für Kritischer Prozess Label
        /// </summary>
        public int FontSize { get; set; }
        /// <summary>
        /// Modus (Neuanlage, Bearbeitung) bestimmt Überschrift etc. 
        /// </summary>
        public ProcAppMode Mode
        {
            get => _mode;
            set
            {
                _mode = value;
                CurrentTab = 0;
                if (value == ProcAppMode.Change)
                {
                    Header = "Prozess bearbeiten";
                }
                else if (value == ProcAppMode.New)
                {
                    Header = "Neuen Prozess anlegen";
                }
            }
        }
        #endregion

        /// <summary>
        /// Liste aller aktiven Informationssegmente
        /// </summary>
        ObservableCollection<ISB_BIA_Informationssegmente> EnabledISSegments;
        /// <summary>
        /// Liste aller Attribute
        /// </summary>
        ObservableCollection<ISB_BIA_Informationssegmente_Attribute> AllAttributesList;
        /// <summary>
        /// Aktuelle Anwendungseinstellungen
        /// </summary>
        public ISB_BIA_Settings Setting { get; set; }

        #region Services
        IMyNavigationService _myNavi;
        IMyDialogService _myDia;
        IMyDataService _myData;
        IMyExportService _myExport;
        IMySharedResourceService _myShared;
        #endregion

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="myDialogService"></param>
        /// <param name="myNavigationService"></param>
        /// <param name="myDataService"></param>
        /// <param name="myExportService"></param>
        /// <param name="mySharedResourceService"></param>
        public Process_ViewModel(IMyDialogService myDialogService, IMyNavigationService myNavigationService, IMyDataService myDataService, IMyExportService myExportService, IMySharedResourceService mySharedResourceService)
        {
            #region Services
            _myDia = myDialogService;
            _myNavi = myNavigationService;
            _myData = myDataService;
            _myExport = myExportService;
            _myShared = mySharedResourceService;
            #endregion

            if (IsInDesignMode)
            {
                CurrentProcess = _myData.GetProcessModelFromDB(1);
                Prozessverantwortlicher = CurrentProcess.Prozessverantwortlicher;
                Vorgelagerte_Prozesse = CurrentProcess.Vorgelagerte_Prozesse;
                Nachgelagerte_Prozesse = CurrentProcess.Nachgelagerte_Prozesse;
            }

            //Messenger Registrierung für Prozessbearbeitungsmodus
            MessengerInstance.Register<int>(this, ProcAppMode.Change, p => {
                Mode = ProcAppMode.Change;
                CurrentProcess = _myData.GetProcessModelFromDB(p);
                _oldCurrentProcess = _myData.GetProcessModelFromDB(p);
                //Wenn Daten Fehlerhaft dann zurückkehren
                if (CurrentProcess == null)
                {
                    _myDia.ShowError("Fehler beim Laden der Daten.");
                    Cleanup();
                    _myNavi.NavigateBack();
                }
                else
                {
                    //Setzen der Properties der Textfelder der Control-abhängigen Infos 
                    Prozessverantwortlicher = CurrentProcess.Prozessverantwortlicher;
                    Vorgelagerte_Prozesse = CurrentProcess.Vorgelagerte_Prozesse;
                    Nachgelagerte_Prozesse = CurrentProcess.Nachgelagerte_Prozesse;
                    //Berechnen der Mindesteinstufung
                    CheckMinValues(CurrentProcess);
                }
            });
            //Messenger Registrierung für Prozesserstellungsmodus
            MessengerInstance.Register<int>(this, ProcAppMode.New, p => {
                Mode = ProcAppMode.New;
                CurrentProcess = new Process_Model();
                _oldCurrentProcess = new Process_Model();
            });
            //Messenger Registrierung für Benachrichtigung eines Kritischen Prozesses (kommt von Process_Model)
            //Messenger.Default.Register<string>(this, MessageToken.ChangedToCriticalNotification,p=> { _myDia.ShowInfo(Krit_Ntf); });
            
            #region Listen und Daten für Prozess-Anwendungszuordnung
            AllApplications = new ObservableCollection<ISB_BIA_Applikationen>(_myData.GetActiveApplications().OrderBy(c=>c.IT_Anwendung_System));
            ApplicationCategories = _myData.GetApplicationCategories();
            #endregion

            #region Filter für Anwendungsliste definieren
            View = (CollectionView)CollectionViewSource.GetDefaultView(AllApplications);
            View.Filter = ApplicationFilter;
            SelectedFilterItem = ApplicationCategories.FirstOrDefault();
            #endregion

            #region Füllen der Dropdownlisten
            ProcessOwnerList = _myData.GetProcessOwner();
            OEList = (_myShared.User.UserGroup == UserGroups.Admin || _myShared.User.UserGroup == UserGroups.CISO)?_myData.GetOEs():_myData.GetOEsForUser(_myShared.User.OE);
            MaturityList = new ObservableCollection<string>(new List<string>() { "1 - Initial", "2 - Wiederholbar", "3 - Definiert", "4 - Gemanagt", "5 - Optimiert" });
            CritList = new ObservableCollection<string>(new List<string>() { "Normal", "Mittel", "Hoch", "Sehr hoch" });
            RTOList = new ObservableCollection<int>(new List<int>() { 1, 5, 10, 20 });
            PreProcessList = _myData.GetPreProcesses();
            PostProcessList = _myData.GetPostProcesses();
            ISList = _myData.GetISList();
            #endregion

            #region Einstellungen abrufen
            Setting = _myData.GetSettings();
            SchutzzielVisible = (Setting.Neue_Schutzziele_aktiviert == "Ja") ? true : false;
            #endregion

            #region Informationssegmente und Attribute abrufen
            EnabledISSegments = _myData.GetEnabledSegments();
            AllAttributesList = _myData.GetAttributes();
            #endregion
        }

        /// <summary>
        /// Methode zur berechnung der Mindesteinstufung der Schutzziele eines Prozesses abhängig von den gewählten Informationssegmenten
        /// </summary>
        /// <param name="process"> Aktueller Prozess </param>
        public void CheckMinValues(Process_Model process)
        {
            try
            {

                List<ISB_BIA_Informationssegmente> queryIS = _myData.Get5SegmentsForCalculation(process);
                //Liste zur speicherung der zutreffenden Attribute
                List<int> list = new List<int>();

                //Der Liste die zu den Informationssegmenten zugehörige Attribut-ID's hinzufügen
                foreach (ISB_BIA_Informationssegmente a in queryIS)
                {
                    if (a.Attribut_1 == "P") list.Add(1);
                    if (a.Attribut_2 == "P") list.Add(2);
                    if (a.Attribut_3 == "P") list.Add(3);
                    if (a.Attribut_4 == "P") list.Add(4);
                    if (a.Attribut_5 == "P") list.Add(5);
                    if (a.Attribut_6 == "P") list.Add(6);
                    if (a.Attribut_7 == "P") list.Add(7);
                    if (a.Attribut_8 == "P") list.Add(8);
                    if (Setting != null && Setting.Attribut9_aktiviert == "Ja")
                    {
                        if (a.Attribut_9 == "P") list.Add(9);
                    }
                    if (Setting != null && Setting.Attribut10_aktiviert == "Ja")
                    {
                        if (a.Attribut_10 == "P") list.Add(10);
                    }
                }
                //Wenn die Liste der Attribut-ID's nicht leer ist, anhand der ID's die Mindesteinstufung der Schutzziele errechnen
                //=> Maximalwert der Schutzziele über alle Attribute berechnen
                if (list.Count != 0)
                {
                    SZ_1_Max = AllAttributesList.Where(n => list.Contains(n.Attribut_Id)).GroupBy(x => x.Attribut_Id).Select(z => z.OrderByDescending(q => q.Datum).FirstOrDefault()).OrderBy(k => k.Attribut_Id).Max(x => x.SZ_1);
                    SZ_2_Max = AllAttributesList.Where(n => list.Contains(n.Attribut_Id)).GroupBy(x => x.Attribut_Id).Select(z => z.OrderByDescending(q => q.Datum).FirstOrDefault()).OrderBy(k => k.Attribut_Id).Max(x => x.SZ_2);
                    SZ_3_Max = AllAttributesList.Where(n => list.Contains(n.Attribut_Id)).GroupBy(x => x.Attribut_Id).Select(z => z.OrderByDescending(q => q.Datum).FirstOrDefault()).OrderBy(k => k.Attribut_Id).Max(x => x.SZ_3);
                    SZ_4_Max = AllAttributesList.Where(n => list.Contains(n.Attribut_Id)).GroupBy(x => x.Attribut_Id).Select(z => z.OrderByDescending(q => q.Datum).FirstOrDefault()).OrderBy(k => k.Attribut_Id).Max(x => x.SZ_4);
                    SZ_5_Max = AllAttributesList.Where(n => list.Contains(n.Attribut_Id)).GroupBy(x => x.Attribut_Id).Select(z => z.OrderByDescending(q => q.Datum).FirstOrDefault()).OrderBy(k => k.Attribut_Id).Max(x => x.SZ_5);
                    SZ_6_Max = AllAttributesList.Where(n => list.Contains(n.Attribut_Id)).GroupBy(x => x.Attribut_Id).Select(z => z.OrderByDescending(q => q.Datum).FirstOrDefault()).OrderBy(k => k.Attribut_Id).Max(x => x.SZ_6);
                }
                else
                {
                    //wenn keine Attribute in der Liste stehen (kein Informationssegment ausgewählt) alles auf 0 setzen 
                    SZ_1_Max = 0;
                    SZ_2_Max = 0;
                    SZ_3_Max = 0;
                    SZ_4_Max = 0;
                    SZ_5_Max = 0;
                    SZ_6_Max = 0;
                }
            }
            catch(Exception ex)
            {
                _myDia.ShowError("Fehler: Konnte Daten über Informationssegmente nicht abrufen!",ex);
            }
        }

        /// <summary>
        /// Bereinigt das Viewmodel
        /// </summary>
        override public void Cleanup()
        {
            Messenger.Default.Unregister(this);
            SimpleIoc.Default.Unregister(this);
            base.Cleanup();
        }

        #region INotifyDataerrorInfo für Wrapping Properties (Prozessverantwortlicher)
        /// <summary>
        /// Dictionary mit Liste der Fehler pro Eigenschaft
        /// </summary>
        private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        /// <summary>
        /// Auf Fehler prüfen
        /// </summary>
        private void EvaluateErrors()
        {
            if (string.IsNullOrEmpty(_prozessverantwortlicher))
                AddError(nameof(Prozessverantwortlicher), "Pflichtfeld");
            if (string.IsNullOrEmpty(_prozessverantwortlicher_Text))
                AddError(nameof(Prozessverantwortlicher_Text), "Pflichtfeld");
        }

        /// <summary>
        /// Methode um Fehler hinzuzufügen
        /// </summary>
        /// <param name="propertyName"> Name der Fehlerhaften Eigenschaft </param>
        /// <param name="error"> Fehlerbeschreibung </param>
        public void AddError(string propertyName, string error)
        {
            // Add error to list
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
        public bool HasErrors => CurrentProcess._errors.Count > 0;

        /// <summary>
        /// Event für Änderungsbenachrichtigung bzgl. Fehler
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        #endregion
    }
}
