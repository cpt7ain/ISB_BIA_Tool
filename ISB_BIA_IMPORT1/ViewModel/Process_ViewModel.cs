using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using ISB_BIA_IMPORT1.Model;
using ISB_BIA_IMPORT1.LINQ2SQL;
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
using ISB_BIA_IMPORT1.Helpers;
using ISB_BIA_IMPORT1.Services.Interfaces;
using System.Diagnostics;

namespace ISB_BIA_IMPORT1.ViewModel
{
    /// <summary>
    /// VM zur Darstellunf der Prozessansicht
    /// </summary>
    public class Process_ViewModel : ViewModelBase, INotifyDataErrorInfo
    {
        #region Backing-Fields
        private SZ_Values _sZ_1_Max = SZ_Values.None;
        private SZ_Values _sZ_2_Max = SZ_Values.None;
        private SZ_Values _sZ_3_Max = SZ_Values.None;
        private SZ_Values _sZ_4_Max = SZ_Values.None;
        private SZ_Values _sZ_5_Max = SZ_Values.None;
        private SZ_Values _sZ_6_Max = SZ_Values.None;
        private Process_Model _processCurrent;
        private Process_Model _oldCurrentProcess;

        private string _proc_Prozessverantwortlicher = "";
        private string _proc_Prozessverantwortlicher_Text = "";
        private bool _proc_Prozessverantwortlicher_Check;
        private KeyValuePair<int, string> _proc_Schaden;
        private KeyValuePair<int, string> _proc_Frequenz;
        private ObservableCollection<string> _list_OE;
        private ObservableCollection<string> _list_ProcessOwner;
        private ObservableCollection<string> _list_ProcessResponsible;
        private ObservableCollection<string> _list_Crit;
        private Dictionary<int, string> _list_Dmg;
        private Dictionary<int, string> _list_Freq;
        private ObservableCollection<string> _list_Maturity;
        private ObservableCollection<int> _list_RTO;
        private Dictionary<string, string> _list_IS;
        private KeyValuePair<string, string> _proc_Relevantes_IS_1;
        private KeyValuePair<string, string> _proc_Relevantes_IS_2;
        private KeyValuePair<string, string> _proc_Relevantes_IS_3;
        private KeyValuePair<string, string> _proc_Relevantes_IS_4;
        private KeyValuePair<string, string> _proc_Relevantes_IS_5;
        private ObservableCollection<ISB_BIA_Applikationen> _proc_List_RemoveApplications;
        private ObservableCollection<ISB_BIA_Applikationen> _proc_List_NewApplications;
        private ISB_BIA_Applikationen _selectedSourceAppItem;
        private ObservableCollection<ISB_BIA_Applikationen> _list_AllApplications;
        private ISB_BIA_Applikationen _selectedTargetAppItem;
        private ObservableCollection<string> _list_ApplicationCategories;
        private static string _selectedFilterItem;
        private CollectionView _filterView;
        private static string _selectedFilterItemVNp;
        private CollectionView _filterViewVNp;
        private int _currentTab;
        private Visibility _vis_ButtonSave;
        private MyRelayCommand _cmd_NextTab;
        private MyRelayCommand _cmd_PrevTab;
        private ISB_BIA_Prozesse _selectedSourcevnPItem;
        private ISB_BIA_Prozesse _selectedTargetvPItem;
        private ISB_BIA_Prozesse _selectedTargetnPItem;
        private MyRelayCommand<object> _cmd_AddnP;
        private MyRelayCommand<object> _cmd_AddvP;
        private MyRelayCommand<object> _cmd_RemovevP;
        private MyRelayCommand<object> _cmd_RemovenP;
        private MyRelayCommand<object> _cmd_AddApplication;
        private MyRelayCommand<object> _cmd_RemoveApplication;
        private MyRelayCommand _cmd_NavToISViewChild;
        private MyRelayCommand<string> _cmd_NavToIS;
        private MyRelayCommand _cmd_ExportProcessHistory;
        private MyRelayCommand<string> _cmd_Info;
        private MyRelayCommand<string> _cmd_OpenDocExtern;
        private MyRelayCommand<string> _cmd_ShowMsg;
        private MyRelayCommand _cmd_ResetValues;
        private ObservableCollection<ISB_BIA_Prozesse> _list_Processes;
        private ObservableCollection<ISB_BIA_Prozesse> _proc_list_NewPreProcesses;
        private ObservableCollection<ISB_BIA_Prozesse> _proc_list_NewPostProcesses;
        private ObservableCollection<ISB_BIA_Prozesse> _proc_list_RemovePreProcesses;
        private ObservableCollection<ISB_BIA_Prozesse> _proc_list_RemovePostProcesses;
        private ProcAppMode _mode;
        #endregion

        #region Mindesteinstufungswerte der Schutzziele
        /// <summary>
        /// Mindesteinstufung nach Berechnung für 1. Schutzziel
        /// </summary>
        public SZ_Values SZ_1_Max
        {
            get => _sZ_1_Max;
            set => Set(() => SZ_1_Max, ref _sZ_1_Max, value);
        }
        /// <summary>
        /// Mindesteinstufung nach Berechnung für 2. Schutzziel
        /// </summary>
        public SZ_Values SZ_2_Max
        {
            get => _sZ_2_Max;
            set => Set(() => SZ_2_Max, ref _sZ_2_Max, value);

        }
        /// <summary>
        /// Mindesteinstufung nach Berechnung für 3. Schutzziel
        /// </summary>
        public SZ_Values SZ_3_Max
        {
            get => _sZ_3_Max;
            set => Set(() => SZ_3_Max, ref _sZ_3_Max, value);

        }
        /// <summary>
        /// Mindesteinstufung nach Berechnung für 4. Schutzziel
        /// </summary>
        public SZ_Values SZ_4_Max
        {
            get => _sZ_4_Max;
            set => Set(() => SZ_4_Max, ref _sZ_4_Max, value);

        }
        /// <summary>
        /// Mindesteinstufung nach Berechnung für 5. Schutzziel
        /// </summary>
        public SZ_Values SZ_5_Max
        {
            get => _sZ_5_Max;
            set => Set(() => SZ_5_Max, ref _sZ_5_Max, value);

        }
        /// <summary>
        /// Mindesteinstufung nach Berechnung für 6. Schutzziel
        /// </summary>
        public SZ_Values SZ_6_Max
        {
            get => _sZ_6_Max;
            set => Set(() => SZ_6_Max, ref _sZ_6_Max, value);

        }
        #endregion

        /// <summary>
        /// Aktueller Prozess
        /// </summary>
        public Process_Model ProcessCurrent
        {
            get => _processCurrent;
            set => Set(() => ProcessCurrent, ref _processCurrent, value);
        }

        // Eigenschaften, welche gebraucht werden um Korrekte Übernahme der vom User gewollten Werte zu sichern,
        // da Werte an jeweils mehrere Controls gebunden sind (TextBox / Dropdown, gesteuert durch CheckBox)
        // Wenn Check-Property sich ändert, wird der jeweilige Wert aus Dropdown oder Textfeld übernommen
        #region Hilfs-Propterties
        private string _proc_OE;
        /// <summary>
        /// Prozesseigentümer (DropDown)
        /// </summary>
        public string Proc_OE
        {
            get => _proc_OE;
            set
            {
                Set(() => Proc_OE, ref _proc_OE, value);
                ProcessCurrent.OE_Filter = _proc_OE;
                ProcessCurrent.Prozesseigentümer = _myOE.Get_OwnerOfOEName(_proc_OE, ProcessCurrent);
                if (!String.IsNullOrWhiteSpace(_proc_OE))
                    RemoveError(nameof(Proc_OE));
            }
        }
        /// <summary>
        /// Prozesvernatwortlicher (DropDown)
        /// </summary>
        public string Proc_Prozessverantwortlicher
        {
            get => _proc_Prozessverantwortlicher;
            set
            {
                Set(() => Proc_Prozessverantwortlicher, ref _proc_Prozessverantwortlicher, value);
                if (Proc_Prozessverantwortlicher_Check)
                    ProcessCurrent.Prozessverantwortlicher = Proc_Prozessverantwortlicher_Text;
                else
                    ProcessCurrent.Prozessverantwortlicher = Proc_Prozessverantwortlicher;
                if (!String.IsNullOrWhiteSpace(_proc_Prozessverantwortlicher))
                    RemoveError(nameof(Proc_Prozessverantwortlicher));
            }
        }
        /// <summary>
        /// Prozessvernatwortlicher (TextBox)
        /// </summary>
        public string Proc_Prozessverantwortlicher_Text
        {
            get => _proc_Prozessverantwortlicher_Text;
            set
            {
                Set(() => Proc_Prozessverantwortlicher_Text, ref _proc_Prozessverantwortlicher_Text, value);
                if (Proc_Prozessverantwortlicher_Check)
                    ProcessCurrent.Prozessverantwortlicher = Proc_Prozessverantwortlicher_Text;
                else
                    ProcessCurrent.Prozessverantwortlicher = Proc_Prozessverantwortlicher;
                if (!String.IsNullOrWhiteSpace(_proc_Prozessverantwortlicher_Text))
                    RemoveError(nameof(Proc_Prozessverantwortlicher_Text));
            }
        }
        /// <summary>
        /// Prozessverantwortlicher (CheckBox Switch))
        /// </summary>
        public bool Proc_Prozessverantwortlicher_Check
        {
            get => _proc_Prozessverantwortlicher_Check;
            set
            {
                Set(() => Proc_Prozessverantwortlicher_Check, ref _proc_Prozessverantwortlicher_Check, value);
                if (value)
                    ProcessCurrent.Prozessverantwortlicher = Proc_Prozessverantwortlicher_Text;
                else
                    ProcessCurrent.Prozessverantwortlicher = Proc_Prozessverantwortlicher;
            }
        }
        /// <summary>
        /// Dictionary Eintrag aus der Informationssegemnt-Liste, welcher zur Anzeige und Zurdnung (SegmentName und Beschreibung) des 1. IS benötigt werden
        /// </summary>
        public KeyValuePair<string, string> Proc_Relevantes_IS_1
        {
            get
            {
                _proc_Relevantes_IS_1 = List_IS.Where(x => x.Key == ProcessCurrent.Relevantes_IS_1).FirstOrDefault();
                return _proc_Relevantes_IS_1;
            }
            set
            {
                ProcessCurrent.Relevantes_IS_1 = List_IS.Where(x => x.Key == value.Key).Select(y => y.Key).FirstOrDefault();
                Set(() => Proc_Relevantes_IS_1, ref _proc_Relevantes_IS_1, value);
                CheckMinValues(ProcessCurrent);
                if (Mode == ProcAppMode.New)
                {
                    ProcessCurrent.SZ_1 = (SZ_Values)SZ_1_Max;
                    ProcessCurrent.SZ_2 = (SZ_Values)SZ_2_Max;
                    ProcessCurrent.SZ_3 = (SZ_Values)SZ_3_Max;
                    ProcessCurrent.SZ_4 = (SZ_Values)SZ_4_Max;
                    ProcessCurrent.SZ_5 = (SZ_Values)SZ_5_Max;
                    ProcessCurrent.SZ_6 = (SZ_Values)SZ_6_Max;
                }
            }
        }
        /// <summary>
        /// Dictionary Eintrag aus der Informationssegemnt-Liste, welcher zur Anzeige und Zurdnung (SegmentName und Beschreibung) des 2. IS benötigt werden
        /// </summary>
        public KeyValuePair<string, string> Proc_Relevantes_IS_2
        {
            get
            {
                _proc_Relevantes_IS_2 = List_IS.Where(x => x.Key == _processCurrent.Relevantes_IS_2).FirstOrDefault();
                return _proc_Relevantes_IS_2;
            }
            set
            {
                ProcessCurrent.Relevantes_IS_2 = List_IS.Where(x => x.Key == value.Key).Select(y => y.Key).FirstOrDefault();
                Set(() => Proc_Relevantes_IS_2, ref _proc_Relevantes_IS_2, value);
                CheckMinValues(ProcessCurrent);
                if (Mode == ProcAppMode.New)
                {
                    ProcessCurrent.SZ_1 = (SZ_Values)SZ_1_Max;
                    ProcessCurrent.SZ_2 = (SZ_Values)SZ_2_Max;
                    ProcessCurrent.SZ_3 = (SZ_Values)SZ_3_Max;
                    ProcessCurrent.SZ_4 = (SZ_Values)SZ_4_Max;
                    ProcessCurrent.SZ_5 = (SZ_Values)SZ_5_Max;
                    ProcessCurrent.SZ_6 = (SZ_Values)SZ_6_Max;
                }
            }
        }
        /// <summary>
        /// Dictionary Eintrag aus der Informationssegemnt-Liste, welcher zur Anzeige und Zurdnung (SegmentName und Beschreibung) des 3. IS benötigt werden
        /// </summary>
        public KeyValuePair<string, string> Proc_Relevantes_IS_3
        {
            get
            {
                _proc_Relevantes_IS_3 = List_IS.Where(x => x.Key == _processCurrent.Relevantes_IS_3).FirstOrDefault();
                return _proc_Relevantes_IS_3;
            }
            set
            {
                ProcessCurrent.Relevantes_IS_3 = List_IS.Where(x => x.Key == value.Key).Select(y => y.Key).FirstOrDefault();
                Set(() => Proc_Relevantes_IS_3, ref _proc_Relevantes_IS_3, value);
                CheckMinValues(ProcessCurrent);
                if (Mode == ProcAppMode.New)
                {
                    ProcessCurrent.SZ_1 = (SZ_Values)SZ_1_Max;
                    ProcessCurrent.SZ_2 = (SZ_Values)SZ_2_Max;
                    ProcessCurrent.SZ_3 = (SZ_Values)SZ_3_Max;
                    ProcessCurrent.SZ_4 = (SZ_Values)SZ_4_Max;
                    ProcessCurrent.SZ_5 = (SZ_Values)SZ_5_Max;
                    ProcessCurrent.SZ_6 = (SZ_Values)SZ_6_Max;
                }
            }
        }
        /// <summary>
        /// Dictionary Eintrag aus der Informationssegemnt-Liste, welcher zur Anzeige und Zurdnung (SegmentName und Beschreibung) des 4. IS benötigt werden
        /// </summary>
        public KeyValuePair<string, string> Proc_Relevantes_IS_4
        {
            get
            {
                _proc_Relevantes_IS_4 = List_IS.Where(x => x.Key == _processCurrent.Relevantes_IS_4).FirstOrDefault();
                return _proc_Relevantes_IS_4;
            }
            set
            {
                ProcessCurrent.Relevantes_IS_4 = List_IS.Where(x => x.Key == value.Key).Select(y => y.Key).FirstOrDefault();
                Set(() => Proc_Relevantes_IS_4, ref _proc_Relevantes_IS_4, value);
                CheckMinValues(ProcessCurrent);
                if (Mode == ProcAppMode.New)
                {
                    ProcessCurrent.SZ_1 = (SZ_Values)SZ_1_Max;
                    ProcessCurrent.SZ_2 = (SZ_Values)SZ_2_Max;
                    ProcessCurrent.SZ_3 = (SZ_Values)SZ_3_Max;
                    ProcessCurrent.SZ_4 = (SZ_Values)SZ_4_Max;
                    ProcessCurrent.SZ_5 = (SZ_Values)SZ_5_Max;
                    ProcessCurrent.SZ_6 = (SZ_Values)SZ_6_Max;
                }
            }
        }
        /// <summary>
        /// Dictionary Eintrag aus der Informationssegemnt-Liste, welcher zur Anzeige und Zurdnung (SegmentName und Beschreibung) des 5. IS benötigt werden
        /// </summary>
        public KeyValuePair<string, string> Proc_Relevantes_IS_5
        {
            get
            {
                _proc_Relevantes_IS_5 = List_IS.Where(x => x.Key == _processCurrent.Relevantes_IS_5).FirstOrDefault();
                return _proc_Relevantes_IS_5;
            }
            set
            {
                ProcessCurrent.Relevantes_IS_5 = List_IS.Where(x => x.Key == value.Key).Select(y => y.Key).FirstOrDefault();
                Set(() => Proc_Relevantes_IS_5, ref _proc_Relevantes_IS_5, value);
                CheckMinValues(ProcessCurrent);
                if (Mode == ProcAppMode.New)
                {
                    ProcessCurrent.SZ_1 = (SZ_Values)SZ_1_Max;
                    ProcessCurrent.SZ_2 = (SZ_Values)SZ_2_Max;
                    ProcessCurrent.SZ_3 = (SZ_Values)SZ_3_Max;
                    ProcessCurrent.SZ_4 = (SZ_Values)SZ_4_Max;
                    ProcessCurrent.SZ_5 = (SZ_Values)SZ_5_Max;
                    ProcessCurrent.SZ_6 = (SZ_Values)SZ_6_Max;
                }
            }
        }
        /// <summary>
        /// Nachgelagerte Prozesse (DropDown)
        /// </summary>
        public KeyValuePair<int, string> Proc_Schaden
        {
            get => _proc_Schaden;
            set
            {
                Set(() => Proc_Schaden, ref _proc_Schaden, value);
                int i = 0;
                if (value.Key == 6 && Proc_Frequenz.Key == 6) i = 4;
                else if ((value.Key > 4 && Proc_Frequenz.Key <= 6 && Proc_Frequenz.Key >= 1) || (value.Key == 4 && Proc_Frequenz.Key > 4)) i = 3;
                else if (value.Key > 2 && value.Key < 5 && Proc_Frequenz.Key <= 6 && Proc_Frequenz.Key >= 1) i = 2;
                else if (value.Key < 3 && value.Key >= 1 && Proc_Frequenz.Key <= 6 && Proc_Frequenz.Key >= 1) i = 1;
                switch (i)
                {
                    case 0:
                        ProcessCurrent.Kritikalität_des_Prozesses = null;
                        break;
                    case 1:
                        ProcessCurrent.Kritikalität_des_Prozesses = List_Crit[0];
                        break;
                    case 2:
                        ProcessCurrent.Kritikalität_des_Prozesses = List_Crit[1];
                        break;
                    case 3:
                        ProcessCurrent.Kritikalität_des_Prozesses = List_Crit[2];
                        break;
                    case 4:
                        ProcessCurrent.Kritikalität_des_Prozesses = List_Crit[3];
                        break;
                }
                if (_proc_Schaden.Value != null)
                    RemoveError(nameof(Proc_Schaden));
            }
        }
        /// <summary>
        /// Nachgelagerte Prozesse (TextBox)
        /// </summary>
        public KeyValuePair<int, string> Proc_Frequenz
        {
            get => _proc_Frequenz;
            set
            {
                Set(() => Proc_Frequenz, ref _proc_Frequenz, value);
                int i = 0;
                if (Proc_Schaden.Key == 6 && value.Key == 6) i = 4;
                else if ((Proc_Schaden.Key > 4 && value.Key <= 6 && value.Key >= 1) || (Proc_Schaden.Key == 4 && value.Key > 4)) i = 3;
                else if (Proc_Schaden.Key > 2 && Proc_Schaden.Key < 5 && value.Key <= 6 && value.Key >= 1) i = 2;
                else if (Proc_Schaden.Key < 3 && Proc_Schaden.Key >= 1 && value.Key <= 6 && value.Key >= 1) i = 1;
                switch (i)
                {
                    case 0:
                        ProcessCurrent.Kritikalität_des_Prozesses = null;
                        break;
                    case 1:
                        ProcessCurrent.Kritikalität_des_Prozesses = List_Crit[0];
                        break;
                    case 2:
                        ProcessCurrent.Kritikalität_des_Prozesses = List_Crit[1];
                        break;
                    case 3:
                        ProcessCurrent.Kritikalität_des_Prozesses = List_Crit[2];
                        break;
                    case 4:
                        ProcessCurrent.Kritikalität_des_Prozesses = List_Crit[3];
                        break;
                }
                if (_proc_Frequenz.Value != null)
                    RemoveError(nameof(Proc_Frequenz));
            }
        }
        #endregion

        #region DropDown Listen
        /// <summary>
        /// Liste aller OE-Gruppen
        /// </summary>
        public ObservableCollection<string> List_OE
        {
            get => _list_OE;
            set => Set(() => List_OE, ref _list_OE, value);
        }
        /// <summary>
        /// Liste aller Prozesseigentümer
        /// </summary>
        public ObservableCollection<string> List_ProcessOwner
        {
            get => _list_ProcessOwner;
            set => Set(() => List_ProcessOwner, ref _list_ProcessOwner, value);
        }
        /// <summary>
        /// Liste aller Prozessverantwortlicher
        /// </summary>
        public ObservableCollection<string> List_ProcessResponsible
        {
            get => _list_ProcessResponsible;
            set => Set(() => List_ProcessResponsible, ref _list_ProcessResponsible, value);
        }
        /// <summary>
        /// Liste der Kritikalitätsstufen
        /// </summary>
        public ObservableCollection<string> List_Crit
        {
            get => _list_Crit;
            set => Set(() => List_Crit, ref _list_Crit, value);
        }
        /// <summary>
        /// Liste der Schadensstufen
        /// </summary>
        public Dictionary<int, string> List_Dmg
        {
            get => _list_Dmg;
            set => Set(() => List_Dmg, ref _list_Dmg, value);
        }
        /// <summary>
        /// Liste der Häufigkeiten
        /// </summary>
        public Dictionary<int, string> List_Freq
        {
            get => _list_Freq;
            set => Set(() => List_Freq, ref _list_Freq, value);
        }
        /// <summary>
        /// Liste der Prozessreifegrade
        /// </summary>
        public ObservableCollection<string> List_Maturity
        {
            get => _list_Maturity;
            set => Set(() => List_Maturity, ref _list_Maturity, value);
        }

        /// <summary>
        /// Liste der Zeiten für das RTO
        /// </summary>
        public ObservableCollection<int> List_RTO
        {
            get => _list_RTO;
            set => Set(() => List_RTO, ref _list_RTO, value);
        }
        /// <summary>
        /// Disctionary der Informationssegmente (SegmentName, Beschreibung)
        /// </summary>
        public Dictionary<string, string> List_IS
        {
            get => _list_IS;
            set => Set(() => List_IS, ref _list_IS, value);
        }
        #endregion

        #region Eigenschaften für Prozess-Prozesszuordnung 
        /// <summary>
        /// Prozessliste für vorgelagerte Prozesse
        /// </summary>
        public ObservableCollection<ISB_BIA_Prozesse> List_Processes
        {
            get => _list_Processes;
            set => Set(() => List_Processes, ref _list_Processes, value);
        }
        /// <summary>
        /// neue vorgelagerte Prozesse
        /// </summary>
        public ObservableCollection<ISB_BIA_Prozesse> Proc_List_NewPreProcesses
        {
            get => _proc_list_NewPreProcesses;
            set => Set(() => Proc_List_NewPreProcesses, ref _proc_list_NewPreProcesses, value);
        }
        /// <summary>
        /// neue nachgelagerte Prozesse
        /// </summary>
        public ObservableCollection<ISB_BIA_Prozesse> Proc_List_NewPostProcesses
        {
            get => _proc_list_NewPostProcesses;
            set => Set(() => Proc_List_NewPostProcesses, ref _proc_list_NewPostProcesses, value);
        }
        /// <summary>
        /// alte vorgelagerte Prozesse
        /// </summary>
        public ObservableCollection<ISB_BIA_Prozesse> Proc_List_RemovePreProcesses
        {
            get => _proc_list_RemovePreProcesses;
            set => Set(() => Proc_List_RemovePreProcesses, ref _proc_list_RemovePreProcesses, value);
        }
        /// <summary>
        /// alte nachgelagerte Prozesse
        /// </summary>
        public ObservableCollection<ISB_BIA_Prozesse> Proc_List_RemovePostProcesses
        {
            get => _proc_list_RemovePostProcesses;
            set => Set(() => Proc_List_RemovePostProcesses, ref _proc_list_RemovePostProcesses, value);
        }
        /// <summary>
        /// Ausgewähltes Item der Quell-vnPliste
        /// </summary>
        public ISB_BIA_Prozesse SelectedSourcevnPItem
        {
            get => _selectedSourcevnPItem;
            set => Set(() => SelectedSourcevnPItem, ref _selectedSourcevnPItem, value);
        }
        /// <summary>
        /// Ausgewähltes Item der Ziel-vPliste
        /// </summary>
        public ISB_BIA_Prozesse SelectedTargetvPItem
        {
            get => _selectedTargetvPItem;
            set => Set(() => SelectedTargetvPItem, ref _selectedTargetvPItem, value);
        }
        /// <summary>
        /// Ausgewähltes Item der Ziel-nPliste
        /// </summary>
        public ISB_BIA_Prozesse SelectedTargetnPItem
        {
            get => _selectedTargetnPItem;
            set => Set(() => SelectedTargetnPItem, ref _selectedTargetnPItem, value);
        }
        public ObservableCollection<string> List_OEVNp { get; private set; }
        public CollectionView FilterViewVNp
        {
            get => _filterViewVNp;
            set => Set(() => FilterViewVNp, ref _filterViewVNp, value);
        }
        /// <summary>
        /// Definition des Filters für die vorgelagerten Prozesse (Filtern auf OE's der Prozesse)
        /// </summary>
        /// <param name="item"> zu Filterndes Item </param>
        /// <returns></returns>
        public static bool VNpFilter(object item)
        {
            ISB_BIA_Prozesse logItem = (ISB_BIA_Prozesse)item;
            if (String.IsNullOrEmpty(_selectedFilterItemVNp))
                return true;
            else if (logItem.OE_Filter == null)
            {
                return true;
            }
            else
            {
                if (_selectedFilterItemVNp == "<Alle>") return true;
                return (logItem.OE_Filter.Equals(_selectedFilterItemVNp));
            }
        }
        /// <summary>
        /// Ausgewähltes Filter Item aus der Liste der vorgelagerten Prozesse
        /// </summary>
        public string SelectedFilterItemVNp
        {
            get => _selectedFilterItemVNp;
            set
            {
                Set(() => SelectedFilterItemVNp, ref _selectedFilterItemVNp, value);
                FilterViewVNp.Refresh();
            }
        }
        /// <summary>
        /// Sichtbarkeit der Notifikation, ob einem Prozess momentan inaktive Anwendungen zugeordnet sind (Wenn eine Anwendung auf inaktiv gesetzt wurd, ohne sie vorher von allen Prozessen zu entfernen)
        /// </summary>
        public Visibility Vis_DeletedProcessVpNotification
        {
            get
            {
                //Prüfen ob dem Prozess momentan Anwendungen zugeordnet sind, welche durch den IT-Betrieb deaktiviert/entfernt wurden
                int inactive = 0;
                foreach (ISB_BIA_Prozesse pa in ProcessCurrent.VPList)
                {
                    if (pa.Aktiv == 0) inactive = 1;
                }
                if (inactive == 1)
                    return Visibility.Visible;
                else
                    return Visibility.Hidden;
            }
        }
        /// <summary>
        /// Sichtbarkeit der Notifikation, ob einem Prozess momentan inaktive Anwendungen zugeordnet sind (Wenn eine Anwendung auf inaktiv gesetzt wurd, ohne sie vorher von allen Prozessen zu entfernen)
        /// </summary>
        public Visibility Vis_DeletedProcessNpNotification
        {
            get
            {
                //Prüfen ob dem Prozess momentan Anwendungen zugeordnet sind, welche durch den IT-Betrieb deaktiviert/entfernt wurden
                int inactive = 0;
                foreach (ISB_BIA_Prozesse pa in ProcessCurrent.NPList)
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

        #region Eigenschaften für Prozess-Anwendungszuordnung
        /// <summary>
        /// Liste der Applikation, für welche mit diesem Prozess ein Eintrag mit Relation=0 erzeugt werden soll
        /// </summary>
        public ObservableCollection<ISB_BIA_Applikationen> Proc_List_RemoveApplications
        {
            get => _proc_List_RemoveApplications;
            set => Set(() => Proc_List_RemoveApplications, ref _proc_List_RemoveApplications, value);
        }
        /// <summary>
        /// Liste der Applikation, für welche mit diesem Prozess ein Eintrag mit Relation=1 erzeugt werden soll
        /// </summary>
        public ObservableCollection<ISB_BIA_Applikationen> Proc_List_NewApplications
        {
            get => _proc_List_NewApplications;
            set => Set(() => Proc_List_NewApplications, ref _proc_List_NewApplications, value);
        }
        /// <summary>
        /// Ausgewähltes Item der Quell-Applikationsliste
        /// </summary>
        public ISB_BIA_Applikationen SelectedSourceAppItem
        {
            get => _selectedSourceAppItem;
            set => Set(() => SelectedSourceAppItem, ref _selectedSourceAppItem, value);
        }
        /// <summary>
        /// Ausgewähltes Item der Ziel-Applikationsliste
        /// </summary>
        public ISB_BIA_Applikationen SelectedTargetAppItem
        {
            get => _selectedTargetAppItem;
            set => Set(() => SelectedTargetAppItem, ref _selectedTargetAppItem, value);
        }
        /// <summary>
        /// Liste aller Applikationen
        /// </summary>
        public ObservableCollection<ISB_BIA_Applikationen> List_AllApplications
        {
            get => _list_AllApplications;
            set => Set(() => List_AllApplications, ref _list_AllApplications, value);

        }
        /// <summary>
        /// Liste der Applikationskategorien für die Filterung
        /// </summary>
        public ObservableCollection<string> List_ApplicationCategories
        {
            get => _list_ApplicationCategories;
            set => Set(() => List_ApplicationCategories, ref _list_ApplicationCategories, value);
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
                FilterView.Refresh();
            }
        }
        /// <summary>
        /// CollectionView für die Filterung
        /// </summary>
        public CollectionView FilterView
        {
            get => _filterView;
            set => Set(() => FilterView, ref _filterView, value);
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
                return (logItem.IT_Betriebsart.Equals(_selectedFilterItem));
            }
        }
        /// <summary>
        /// Sichtbarkeit der Notifikation, ob einem Prozess momentan inaktive Anwendungen zugeordnet sind (Wenn eine Anwendung auf inaktiv gesetzt wurd, ohne sie vorher von allen Prozessen zu entfernen)
        /// </summary>
        public Visibility Vis_DeletedApplicationsNotification
        {
            get
            {
                //Prüfen ob dem Prozess momentan Anwendungen zugeordnet sind, welche durch den IT-Betrieb deaktiviert/entfernt wurden
                int inactive = 0;
                foreach (ISB_BIA_Applikationen pa in ProcessCurrent.ApplicationList)
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
                Vis_ButtonSave = (_currentTab == 6) ? Visibility.Visible : Visibility.Hidden;
            }
        }
        /// <summary>
        /// Sichtbarkeit des Speicherbuttons (nur wenn letzter Tab aktiv)
        /// </summary>
        public Visibility Vis_ButtonSave
        {
            get => _vis_ButtonSave;
            set => Set(() => Vis_ButtonSave, ref _vis_ButtonSave, value);
        }
        /// <summary>
        /// Zum nächsten Tab navigieren
        /// </summary>
        public MyRelayCommand Cmd_NextTab
        {
            get => _cmd_NextTab
                    ?? (_cmd_NextTab = new MyRelayCommand(() =>
                    {
                        CurrentTab += 1;
                    }, () => { return CurrentTab != 6; }));
        }
        /// <summary>
        /// Zum vorherigen Tab navigieren
        /// </summary>
        public MyRelayCommand Cmd_PrevTab
        {
            get => _cmd_PrevTab
                    ?? (_cmd_PrevTab = new MyRelayCommand(() =>
                    {
                        CurrentTab -= 1;
                    }, () => { return CurrentTab != 0; }));
        }
        /// <summary>
        /// Prozesse zur Liste vorgelagerter Prozesse hinzufügen
        /// </summary>
        public MyRelayCommand<object> Cmd_AddvP
        {
            get => _cmd_AddvP
                  ?? (_cmd_AddvP = new MyRelayCommand<object>((list) =>
                  {
                      IList items = (IList)list;
                      if (items.Count > 0)
                      {
                          var collection = items.Cast<ISB_BIA_Prozesse>().ToList();
                          //Wenn einzelne Applikation hinzugefügt wird
                          if (collection.Count() == 1)
                          {
                              //Prüfen ob Anwendung bereits in Zielliste
                              bool contained = ProcessCurrent.VPList.Count(x => x.Prozess_Id == SelectedSourcevnPItem.Prozess_Id) == 1;
                              if (contained)
                              {
                                  _myDia.ShowInfo("Prozess bereits als vorgelagerter Prozess definiert");
                                  SelectedTargetvPItem = SelectedSourcevnPItem;
                              }
                              //Wenn aktuell nicht zugeordnet
                              else
                              {
                                  //Einfügen
                                  ProcessCurrent.VPList.Insert(0, SelectedSourcevnPItem);
                                  _myDia.ShowMessage(SelectedSourcevnPItem.Prozess + " ~ " + SelectedSourcevnPItem.Sub_Prozess + " wurde hinzugefügt.");
                                  SelectedTargetvPItem = SelectedSourcevnPItem;
                              }
                          }
                          //Wenn mehr als ein Prozess hinzugefügt wird
                          else
                          {
                              //Liste ausgewählter Prozesse in Quellliste
                              List<ISB_BIA_Prozesse> selectedList = new List<ISB_BIA_Prozesse>(collection);
                              //Liste der Prozesse die bereits zugewiesen
                              List<ISB_BIA_Prozesse> listRedundant = new List<ISB_BIA_Prozesse>(selectedList.Where(x =>
                                  ProcessCurrent.VPList.Select(v => v.Prozess_Id).ToList().Contains(x.Prozess_Id)).ToList());
                              //Liste der Prozesse die noch nicht zugewiesen
                              List<ISB_BIA_Prozesse> listAdd = new List<ISB_BIA_Prozesse>(selectedList.Where(x =>
                                  !ProcessCurrent.VPList.Select(v => v.Prozess_Id).ToList().Contains(x.Prozess_Id)).ToList());
                              //Alle gültigen Prozesse in Zielliste einfügen
                              if (listAdd.Count > 0)
                              {
                                  foreach (var item in listAdd)
                                  {
                                      //Einfügen
                                      ProcessCurrent.VPList.Insert(0, item);
                                      SelectedTargetvPItem = listAdd.FirstOrDefault();
                                  }
                                  #region Benachrichtigung über hinzugefügte Prozesse
                                  string a = "Folgende Prozesse wurden hinzugefügt:";
                                  string b = "\nFolgende Prozesse waren bereits in der Liste:";
                                  int i = 0;
                                  foreach (var f in listAdd)
                                  {
                                      i++;
                                      if (i < 11) a = a + "\n-" + f.Prozess + " ~ " + f.Sub_Prozess;
                                  }

                                  if (i > 10) a = a + "\nund " + (i - 10) + " weitere Prozesse";

                                  if (listRedundant.Count > 0)
                                  {
                                      i = 0;
                                      foreach (var f in listRedundant)
                                      {
                                          i++;
                                          if (i < 11) b = b + "\n" + f.Prozess + " ~ " + f.Sub_Prozess;
                                      }
                                      if (i > 10) b = b + "\nund " + (i - 10) + " weitere Prozesse";
                                      _myDia.ShowMessage(a + "\n" + b);
                                  }
                                  else
                                  {
                                      _myDia.ShowMessage(a);
                                  }
                                  #endregion
                              }
                              else
                              {
                                  _myDia.ShowInfo("Alle ausgewählten Prozesse sind dem Prozess bereits zugeordnet");
                              }
                          }
                      }
                      else
                          _myDia.ShowInfo("Bitte hinzuzufügende Prozesse auswählen");
                  }));
        }
        /// <summary>
        /// Anwendung von Liste vorgelagerter Prozesse entfernen
        /// </summary>
        public MyRelayCommand<object> Cmd_RemovevP
        {
            get => _cmd_RemovevP
                  ?? (_cmd_RemovevP = new MyRelayCommand<object>((list) =>
                  {
                      IList items = (IList)list;
                      if (items.Count > 0)
                      {
                          var collection = items.Cast<ISB_BIA_Prozesse>().ToList();
                          //Wenn einzelner Applikation entfernt wird
                          if (collection.Count() == 1)
                          {
                              //Entfernen
                              ISB_BIA_Prozesse tmp = SelectedTargetvPItem;
                              ProcessCurrent.VPList.Remove(SelectedTargetvPItem);
                              _myDia.ShowMessage(tmp.Prozess + " ~ " + tmp.Sub_Prozess + " wurde entfernt.");
                          }
                          //Wenn mehrere Applikationen entfernt werden
                          else
                          {
                              List<ISB_BIA_Prozesse> selectedList = new List<ISB_BIA_Prozesse>(collection);
                              string a = "Folgende Prozesse wurden entfernt:";
                              int i = 0;
                              foreach (ISB_BIA_Prozesse item in selectedList)
                              {
                                  i++;
                                  //Entfernen
                                  ProcessCurrent.VPList.Remove(item);
                                  if (i < 11) a = a + "\n" + item.Prozess + " ~ " + item.Sub_Prozess;
                              }
                              if (i > 10) a = a + "\nund " + (i - 10) + " weitere Prozesse";
                              _myDia.ShowMessage(a);
                          }
                          RaisePropertyChanged(() => Vis_DeletedProcessVpNotification);
                      }
                      else
                          _myDia.ShowInfo("Bitte zu entfernenden Prozess auswählen");
                  }));
        }
        /// <summary>
        /// Prozesse zur Liste vorgelagerter Prozesse hinzufügen
        /// </summary>
        public MyRelayCommand<object> Cmd_AddnP
        {
            get => _cmd_AddnP
                  ?? (_cmd_AddnP = new MyRelayCommand<object>((list) =>
                  {
                      IList items = (IList)list;
                      if (items.Count > 0)
                      {
                          var collection = items.Cast<ISB_BIA_Prozesse>().ToList();
                          //Wenn einzelne Applikation hinzugefügt wird
                          if (collection.Count() == 1)
                          {
                              //Prüfen ob Anwendung bereits in Zielliste
                              bool contained = ProcessCurrent.NPList.Count(x => x.Prozess_Id == SelectedSourcevnPItem.Prozess_Id) == 1;
                              if (contained)
                              {
                                  _myDia.ShowInfo("Prozess bereits als nachgelagerter Prozess definiert");
                                  SelectedTargetnPItem = SelectedSourcevnPItem;
                              }
                              //Wenn aktuell nicht zugeordnet
                              else
                              {
                                  //Einfügen
                                  ProcessCurrent.NPList.Insert(0, SelectedSourcevnPItem);
                                  _myDia.ShowMessage(SelectedSourcevnPItem.Prozess + " ~ " + SelectedSourcevnPItem.Sub_Prozess + " wurde hinzugefügt.");
                                  SelectedTargetnPItem = SelectedSourcevnPItem;
                              }
                          }
                          //Wenn mehr als ein Prozess hinzugefügt wird
                          else
                          {
                              //Liste ausgewählter Prozesse in Quellliste
                              List<ISB_BIA_Prozesse> selectedList = new List<ISB_BIA_Prozesse>(collection);
                              //Liste der Prozesse die bereits zugewiesen
                              List<ISB_BIA_Prozesse> listRedundant = new List<ISB_BIA_Prozesse>(selectedList.Where(x =>
                                  ProcessCurrent.NPList.Select(v => v.Prozess_Id).ToList().Contains(x.Prozess_Id)).ToList());
                              //Liste der Prozesse die noch nicht zugewiesen
                              List<ISB_BIA_Prozesse> listAdd = new List<ISB_BIA_Prozesse>(selectedList.Where(x =>
                                  !ProcessCurrent.NPList.Select(v => v.Prozess_Id).ToList().Contains(x.Prozess_Id)).ToList());
                              //Alle gültigen Prozesse in Zielliste einfügen
                              if (listAdd.Count > 0)
                              {
                                  foreach (var item in listAdd)
                                  {
                                      //Einfügen
                                      ProcessCurrent.NPList.Insert(0, item);
                                      SelectedTargetnPItem = listAdd.FirstOrDefault();
                                  }
                                  #region Benachrichtigung über hinzugefügte Prozesse
                                  string a = "Folgende Prozesse wurden hinzugefügt:";
                                  string b = "\nFolgende Prozesse waren bereits in der Liste:";
                                  int i = 0;
                                  foreach (var f in listAdd)
                                  {
                                      i++;
                                      if (i < 11) a = a + "\n-" + f.Prozess + " ~ " + f.Sub_Prozess;
                                  }

                                  if (i > 10) a = a + "\nund " + (i - 10) + " weitere Prozesse";

                                  if (listRedundant.Count > 0)
                                  {
                                      i = 0;
                                      foreach (var f in listRedundant)
                                      {
                                          i++;
                                          if (i < 11) b = b + "\n" + f.Prozess + " ~ " + f.Sub_Prozess;
                                      }
                                      if (i > 10) b = b + "\nund " + (i - 10) + " weitere Prozesse";
                                      _myDia.ShowMessage(a + "\n" + b);
                                  }
                                  else
                                  {
                                      _myDia.ShowMessage(a);
                                  }
                                  #endregion
                              }
                              else
                              {
                                  _myDia.ShowInfo("Alle ausgewählten Prozesse sind dem Prozess bereits zugeordnet");
                              }
                          }
                      }
                      else
                          _myDia.ShowInfo("Bitte hinzuzufügende Prozesse auswählen");
                  }));
        }
        /// <summary>
        /// Anwendung von Liste vorgelagerter Prozesse entfernen
        /// </summary>
        public MyRelayCommand<object> Cmd_RemovenP
        {
            get => _cmd_RemovenP
                  ?? (_cmd_RemovenP = new MyRelayCommand<object>((list) =>
                  {
                      IList items = (IList)list;
                      if (items.Count > 0)
                      {
                          var collection = items.Cast<ISB_BIA_Prozesse>().ToList();
                          //Wenn einzelner Applikation entfernt wird
                          if (collection.Count() == 1)
                          {
                              //Entfernen
                              ISB_BIA_Prozesse tmp = SelectedTargetnPItem;
                              ProcessCurrent.NPList.Remove(SelectedTargetnPItem);
                              _myDia.ShowMessage(tmp.Prozess + " ~ " + tmp.Sub_Prozess + " wurde entfernt.");
                          }
                          //Wenn mehrere Applikationen entfernt werden
                          else
                          {
                              List<ISB_BIA_Prozesse> selectedList = new List<ISB_BIA_Prozesse>(collection);
                              string a = "Folgende Prozesse wurden entfernt:";
                              int i = 0;
                              foreach (ISB_BIA_Prozesse item in selectedList)
                              {
                                  i++;
                                  //Entfernen
                                  ProcessCurrent.NPList.Remove(item);
                                  if (i < 11) a = a + "\n" + item.Prozess + " ~ " + item.Sub_Prozess;
                              }
                              if (i > 10) a = a + "\nund " + (i - 10) + " weitere Prozesse";

                              _myDia.ShowMessage(a);
                          }
                          RaisePropertyChanged(() => Vis_DeletedProcessNpNotification);
                      }
                      else
                          _myDia.ShowInfo("Bitte zu entfernenden Prozess auswählen");
                  }));
        }
        /// <summary>
        /// Anwendung zur Liste verknüpfter Anwendungen hinzufügen
        /// </summary>
        public MyRelayCommand<object> Cmd_AddApplication
        {
            get => _cmd_AddApplication
                  ?? (_cmd_AddApplication = new MyRelayCommand<object>((list) =>
                  {
                      IList items = (IList)list;
                      if (items.Count > 0)
                      {
                          var collection = items.Cast<ISB_BIA_Applikationen>().ToList();
                          //Wenn einzelne Applikation hinzugefügt wird
                          if (collection.Count() == 1)
                          {
                              //Prüfen ob Anwendung bereits in Zielliste
                              bool contained = ProcessCurrent.ApplicationList.Count(x => x.Applikation_Id == SelectedSourceAppItem.Applikation_Id) == 1;
                              if (contained)
                              {
                                  _myDia.ShowInfo("Anwendung bereits zugewiesen");
                                  SelectedTargetAppItem = SelectedSourceAppItem;
                              }
                              //Wenn aktuell nicht zugeordnet
                              else
                              {
                                  //Einfügen
                                  ProcessCurrent.ApplicationList.Insert(0, SelectedSourceAppItem);
                                  _myDia.ShowMessage(SelectedSourceAppItem.IT_Anwendung_System + " wurde hinzugefügt.");
                                  SelectedTargetAppItem = SelectedSourceAppItem;
                              }
                          }
                          //Wenn mehr als eine Applikation hinzugefügt wird
                          else
                          {
                              //Liste ausgewählter Anwendungen in Quellliste
                              List<ISB_BIA_Applikationen> selectedList = new List<ISB_BIA_Applikationen>(collection);
                              //Liste der Anwendungen die bereits zugewiesen
                              List<ISB_BIA_Applikationen> listRedundant = new List<ISB_BIA_Applikationen>(selectedList.Where(x =>
                                  ProcessCurrent.ApplicationList.Select(v => v.Applikation_Id).ToList().Contains(x.Applikation_Id)).ToList());
                              //Liste der Anwendung die noch nicht zugewiesen
                              List<ISB_BIA_Applikationen> listAdd = new List<ISB_BIA_Applikationen>(selectedList.Where(x =>
                                  !ProcessCurrent.ApplicationList.Select(v => v.Applikation_Id).ToList().Contains(x.Applikation_Id)).ToList());
                              //Alle gültigen Anwendungen in Zielliste einfügen
                              if (listAdd.Count > 0)
                              {
                                  foreach (var item in listAdd)
                                  {
                                      //Einfügen
                                      ProcessCurrent.ApplicationList.Insert(0, item);
                                      SelectedTargetAppItem = listAdd.FirstOrDefault();
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
                                      _myDia.ShowMessage(a + "\n" + b);
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
        public MyRelayCommand<object> Cmd_RemoveApplication
        {
            get => _cmd_RemoveApplication
                  ?? (_cmd_RemoveApplication = new MyRelayCommand<object>((list) =>
                  {
                      IList items = (IList)list;
                      if (items.Count > 0)
                      {
                          var collection = items.Cast<ISB_BIA_Applikationen>().ToList();
                          //Wenn einzelne Applikation entfernt wird
                          if (collection.Count() == 1)
                          {
                              //Entfernen
                              ISB_BIA_Applikationen tmp = SelectedTargetAppItem;
                              ProcessCurrent.ApplicationList.Remove(SelectedTargetAppItem);
                              _myDia.ShowMessage(tmp.IT_Anwendung_System + " wurde entfernt.");
                          }
                          //Wenn mehrere Applikationen entfernt werden
                          else
                          {
                              List<ISB_BIA_Applikationen> selectedList = new List<ISB_BIA_Applikationen>(collection);
                              string a = "Folgende Anwendungen wurden entfernt:\n";
                              int i = 0;
                              foreach (ISB_BIA_Applikationen item in selectedList)
                              {
                                  //Entfernen
                                  i++;
                                  ProcessCurrent.ApplicationList.Remove(item);
                                  if (i < 11) a = a + "\n" + item.IT_Anwendung_System;
                              }
                              if (i > 10) a = a + "\nund " + (i - 10) + " weitere Prozesse";
                              _myDia.ShowMessage(a);
                          }
                          RaisePropertyChanged(() => Vis_DeletedApplicationsNotification);
                      }
                      else
                          _myDia.ShowInfo("Bitte zu löschende Anwendung auswählen");
                  }));
        }
        /// <summary>
        /// Command zum Zurückkehren zum vorherigen VM
        /// </summary>
        public MyRelayCommand Cmd_NavBack
        {
            get => new MyRelayCommand(() =>
                {
                    if (_myDia.CancelDecision())
                    {
                        Cleanup();
                        _myNavi.NavigateBack(true);
                        _myLock.Unlock_Object(Table_Lock_Flags.Process, ProcessCurrent.Prozess_Id);
                    }
                });
        }
        /// <summary>
        /// Zum <see cref="SegmentsView_ViewModel"/> navigieren
        /// </summary>
        public MyRelayCommand Cmd_NavToISViewChild
        {
            get => _cmd_NavToISViewChild
                  ?? (_cmd_NavToISViewChild = new MyRelayCommand(() =>
                  {
                      _myNavi.NavigateTo<SegmentsView_ViewModel>(ISISAttributeMode.View);
                  }));
        }
        /// <summary>
        /// Zum <see cref="Segment_ViewModel"/> des ausgewählten Segments navigieren
        /// </summary>
        public MyRelayCommand<string> Cmd_NavToIS
        {
            get => _cmd_NavToIS
                  ?? (_cmd_NavToIS = new MyRelayCommand<string>((msg) =>
                  {
                      ISB_BIA_Informationssegmente seg = _myProc.Get_IS_ByName(msg);
                      if (seg != null)
                      {
                          _myNavi.NavigateTo<Segment_ViewModel>(seg.Informationssegment_Id, ISISAttributeMode.View);
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
        public MyRelayCommand Cmd_SaveAndContinue
        {
            get => new MyRelayCommand(() =>
            {
                //Liste der aktuellen Zuordnung von Prozessressourcen durchgehen und zur Addliste hinzufügen, welche in alter Liste noch nicht vorhanden waren
                Proc_List_NewApplications = new ObservableCollection<ISB_BIA_Applikationen>(
                    ProcessCurrent.ApplicationList.Where(x =>
                        !_oldCurrentProcess.ApplicationList.Select(c => c.Applikation_Id).ToList().Contains(x.Applikation_Id)).ToList());
                //Liste der Ausgangszuordnung von Prozessressourcen durchgehen und zur Removeliste hinzufügen, welche in aktueller Liste nicht mehr vorhanden
                Proc_List_RemoveApplications = new ObservableCollection<ISB_BIA_Applikationen>(
                    _oldCurrentProcess.ApplicationList.Where(x =>
                        !ProcessCurrent.ApplicationList.Select(c => c.Applikation_Id).ToList().Contains(x.Applikation_Id)).ToList());

                //Liste der aktuellen Zuordnung vorgelagerter Prozesse durchgehen und zur Addliste hinzufügen, welche in alter Liste noch nicht vorhanden waren
                Proc_List_NewPreProcesses = new ObservableCollection<ISB_BIA_Prozesse>(
                    ProcessCurrent.VPList.Where(x =>
                        !_oldCurrentProcess.VPList.Select(c => c.Prozess_Id).ToList().Contains(x.Prozess_Id)).ToList());
                //Liste der Ausgangszuordnung vorgelagerter Prozesse durchgehen und zur Removeliste hinzufügen, welche in aktueller Liste nicht mehr vorhanden
                Proc_List_RemovePreProcesses = new ObservableCollection<ISB_BIA_Prozesse>(
                    _oldCurrentProcess.VPList.Where(x =>
                        !ProcessCurrent.VPList.Select(c => c.Prozess_Id).ToList().Contains(x.Prozess_Id)).ToList());

                //Liste der aktuellen Zuordnung nachgelagerter Prozesse durchgehen und zur Addliste hinzufügen, welche in alter Liste noch nicht vorhanden waren
                Proc_List_NewPostProcesses = new ObservableCollection<ISB_BIA_Prozesse>(
                    ProcessCurrent.NPList.Where(x =>
                        !_oldCurrentProcess.NPList.Select(c => c.Prozess_Id).ToList().Contains(x.Prozess_Id)).ToList());
                //Liste der Ausgangszuordnung nachgelagerter Prozesse durchgehen und zur Removeliste hinzufügen, welche in aktueller Liste nicht mehr vorhanden
                Proc_List_RemovePostProcesses = new ObservableCollection<ISB_BIA_Prozesse>(
                    _oldCurrentProcess.NPList.Where(x =>
                        !ProcessCurrent.NPList.Select(c => c.Prozess_Id).ToList().Contains(x.Prozess_Id)).ToList());

                //Prozessdaten auf Fehler prüfen
                ProcessCurrent.EvaluateErrors();
                EvaluateErrors();
                //Prozess & Relationen einfügen
                if (_myProc.Insert_ProcessAndRelations(ProcessCurrent, Mode, Proc_List_NewApplications, Proc_List_RemoveApplications, Proc_List_NewPreProcesses, Proc_List_RemovePreProcesses, Proc_List_NewPostProcesses, Proc_List_RemovePostProcesses))
                {
                    bool refreshMsg = (Mode == ProcAppMode.Change);
                    Cleanup();
                    _myNavi.NavigateBack(refreshMsg);
                    _myLock.Unlock_Object(Table_Lock_Flags.Process, ProcessCurrent.Prozess_Id);
                }
            });
        }
        /// <summary>
        /// Prozesshistorie nach Excel exportieren
        /// </summary>
        public MyRelayCommand Cmd_ExportProcessHistory
        {
            get => _cmd_ExportProcessHistory
                    ?? (_cmd_ExportProcessHistory = new MyRelayCommand(() =>
                    {
                        _myExp.Export_Processes(_myProc.Get_History_Process(ProcessCurrent.Prozess_Id), ProcessCurrent.Prozess_Id);
                    }, () => Mode == ProcAppMode.Change));
        }
        /// <summary>
        /// Command zum Zurückkehren zum vorherigen Viewmodel
        /// </summary>
        public MyRelayCommand<string> Cmd_OpenDocExtern
        {
            get => _cmd_OpenDocExtern
                ?? (_cmd_OpenDocExtern = new MyRelayCommand<string>((name) =>
                {
                    try
                    {
                        string _str_Filename = _myShared.Dir_InitialDirectory + @"\" + name + "_Info.pdf";
                        if (File.Exists(_str_Filename))
                        {
                            Process.Start(_str_Filename);
                        }
                        else
                        {
                            _myDia.ShowInfo("Keine Beschreibung verfügbar");
                        }
                    }
                    catch (Exception ex)
                    {
                        _myDia.ShowError("Keine Beschreibung verfügbar.", ex);
                    }
                }));

        }
        /// <summary>
        /// Zum Infofenster navigieren und jeweilige Info anzeigen (Parameter in XAML angegeben)
        /// </summary>
        public MyRelayCommand<string> Cmd_Info
        {
            get => _cmd_Info
                    ?? (_cmd_Info = new MyRelayCommand<string>((name) =>
                    {
                        try
                        {
                            string file = _myShared.Dir_InitialDirectory + @"\" + name + "_Info.xps";
                            if (File.Exists(file))
                            {
                                XpsDocument xpsDocument = new XpsDocument(file, FileAccess.Read);
                                FixedDocumentSequence fds = xpsDocument.GetFixedDocumentSequence();
                                _myNavi.NavigateTo<DocumentView_ViewModel>();
                                MessengerInstance.Send(new NotificationMessage<FixedDocumentSequence>(this, fds, file));
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
        public MyRelayCommand<string> Cmd_ShowMsg
        {
            get => _cmd_ShowMsg
                  ?? (_cmd_ShowMsg = new MyRelayCommand<string>((msg) =>
                  {
                      _myDia.ShowMessage(msg);
                  }));
        }
        /// <summary>
        /// Zurücksetzen der Daten der aktuellen Maske (Nur bei Prozessbearbeitung, nicht Neuanlage)
        /// </summary>
        public MyRelayCommand Cmd_ResetValues
        {
            get => _cmd_ResetValues
                   ?? (_cmd_ResetValues = new MyRelayCommand(() =>
                   {
                       switch (CurrentTab)
                       {
                           case 0:
                               ProcessCurrent.Prozess = _oldCurrentProcess.Prozess;
                               ProcessCurrent.Sub_Prozess = _oldCurrentProcess.Sub_Prozess;
                               ProcessCurrent.OE_Filter = _oldCurrentProcess.OE_Filter;
                               Proc_Prozessverantwortlicher = _oldCurrentProcess.Prozessverantwortlicher;
                               Proc_Prozessverantwortlicher_Text = "";
                               ProcessCurrent.Reifegrad_des_Prozesses = _oldCurrentProcess.Reifegrad_des_Prozesses;
                               break;
                           case 1:
                               ProcessCurrent.VPList = new ObservableCollection<ISB_BIA_Prozesse>(_oldCurrentProcess.VPList);
                               Proc_List_NewPreProcesses = new ObservableCollection<ISB_BIA_Prozesse>();
                               Proc_List_RemovePreProcesses = new ObservableCollection<ISB_BIA_Prozesse>();
                               ProcessCurrent.NPList = new ObservableCollection<ISB_BIA_Prozesse>(_oldCurrentProcess.NPList);
                               Proc_List_NewPostProcesses = new ObservableCollection<ISB_BIA_Prozesse>();
                               Proc_List_RemovePostProcesses = new ObservableCollection<ISB_BIA_Prozesse>();
                               break;
                           case 2:
                               Proc_Relevantes_IS_1 = List_IS.Where(x => x.Key == _oldCurrentProcess.Relevantes_IS_1).FirstOrDefault();
                               Proc_Relevantes_IS_2 = List_IS.Where(x => x.Key == _oldCurrentProcess.Relevantes_IS_2).FirstOrDefault();
                               Proc_Relevantes_IS_3 = List_IS.Where(x => x.Key == _oldCurrentProcess.Relevantes_IS_3).FirstOrDefault();
                               Proc_Relevantes_IS_4 = List_IS.Where(x => x.Key == _oldCurrentProcess.Relevantes_IS_4).FirstOrDefault();
                               Proc_Relevantes_IS_5 = List_IS.Where(x => x.Key == _oldCurrentProcess.Relevantes_IS_5).FirstOrDefault();
                               break;
                           case 3:
                               ProcessCurrent.SZ_1 = _oldCurrentProcess.SZ_1;
                               ProcessCurrent.SZ_2 = _oldCurrentProcess.SZ_2;
                               ProcessCurrent.SZ_3 = _oldCurrentProcess.SZ_3;
                               ProcessCurrent.SZ_4 = _oldCurrentProcess.SZ_4;
                               ProcessCurrent.SZ_5 = _oldCurrentProcess.SZ_5;
                               ProcessCurrent.SZ_6 = _oldCurrentProcess.SZ_6;
                               break;
                           case 4:
                               ProcessCurrent.Servicezeit_Helpdesk = _oldCurrentProcess.Servicezeit_Helpdesk;
                               ProcessCurrent.RPO_Datenverlustzeit_Recovery_Point_Objective = _oldCurrentProcess.RPO_Datenverlustzeit_Recovery_Point_Objective;
                               ProcessCurrent.RTO_Wiederanlaufzeit_Recovery_Time_Objective = _oldCurrentProcess.RTO_Wiederanlaufzeit_Recovery_Time_Objective;
                               ProcessCurrent.RTO_Wiederanlaufzeit_Recovery_Time_Objective_Notfall = _oldCurrentProcess.RTO_Wiederanlaufzeit_Recovery_Time_Objective_Notfall;
                               break;
                           case 5:
                               Proc_Schaden = default(KeyValuePair<int, string>);
                               Proc_Frequenz = default(KeyValuePair<int, string>);
                               ProcessCurrent.Kritikalität_des_Prozesses = _oldCurrentProcess.Kritikalität_des_Prozesses;
                               ProcessCurrent.Regulatorisch = _oldCurrentProcess.Regulatorisch;
                               ProcessCurrent.Reputatorisch = _oldCurrentProcess.Reputatorisch;
                               ProcessCurrent.Finanziell = _oldCurrentProcess.Finanziell;
                               break;
                           case 6:
                               ProcessCurrent.ApplicationList = new ObservableCollection<ISB_BIA_Applikationen>(_oldCurrentProcess.ApplicationList);
                               Proc_List_NewApplications = new ObservableCollection<ISB_BIA_Applikationen>();
                               Proc_List_RemoveApplications = new ObservableCollection<ISB_BIA_Applikationen>();
                               RaisePropertyChanged(() => Vis_DeletedApplicationsNotification);
                               break;
                       }
                   }));
        }
        #endregion

        #region Platzhalter und Nachrichtenstringfelder
        /// <summary>
        /// Platzhalter Prozessverantwortlicher
        /// </summary>
        public string Str_Ph_Responsible
        {
            get => "Bsp.: Vorname Nachname";
        }
        /// <summary>
        /// Platzhalter Servicezeiten
        /// </summary>
        public string Str_Ph_Times
        {
            get => "Bsp.: Mo-Do: 08-12 / 14-16 Uhr, Fr: 08-14 Uhr";
        }
        /// <summary>
        /// Nachricht bei Kritischer Einstufung des Prozesses
        /// </summary>
        public string Str_Msg_Krit
        {
            get => "Ein Prozess wird als 'Kritischer Prozess' eingestuft wenn mindestens eine der folgenden Bedingungen zutrifft:"
                 + "\n- mindestens 3 der Schutzziele werden als mindestens 'Hoch' eingestuft oder mindestens 2 als 'Sehr Hoch'"
                 + "\n- Max. tolerierbare Ausfallzeit (MTA Normalbetrieb) wurde auf 1 Tag festgelegt"
                 + "\n- Das Ergebnis der Berechnung für die Kritikalität des Prozesses ist 'Sehr hoch'\n"
                 + "\nKritische Prozesse müssen identifiziert und dokumentiert werden!"
                 + "\nDie Dokumentation muss folgende Anforderungen erfüllen:"
                 + "\n- kurze Beschreibung des Prozesses (Prozessbezeichnung, was bewirkt der Prozess)"
                 + "\n- Begründung, warum der Prozess ein zentraler/kritischer Prozess "
                 + "\nbzw. ein Prozess mit hohem Schadenspotential ist"
                 + "\n- Benennung des Prozessverantwortlichen"
                 + "\n- Maximal tolerierbare Ausfallzeit des Prozesses"
                 + "\n- Auswirkung auf Folgeprozesse"
                 + "\n- ggf. Workaround bei Prozessstörung"
                 + "\n- eine Risikoanalyse muss durchgeführt werden";
        }
        /// <summary>
        /// Nachricht bei Kritischer Einstufung des Prozesses
        /// </summary>
        public string Str_Msg_Krit1
        {
            get => "Um die Kritikalität des Prozesses zu berechnen, wählen Sie die zutreffende 'Potenzielle Schadenshöhe' und die 'Eintrittswahrscheinlichkeit' aus den Auswahlfeldern aus.";
        }
        /// <summary>
        /// Regulatorisch Info
        /// </summary>
        public string Str_Msg_Reg
        {
            get => "Rechtliche und aufsichtsrechtliche Vorgaben werden nicht eingehalten.\nAus einer Nichteinhaltung können aufsichtsrechtliche Konsequenzen für die Bank und somit ein Schaden entstehen.";
        }
        /// <summary>
        /// Reputatorisch info
        /// </summary>
        public string Str_Msg_Rep
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
        public string Str_Msg_Fin
        {
            get => "Unmittelbare oder mittelbare Schäden können durch den Verlust der Vertraulichkeit schutzbedürftiger Daten, die Veränderung von Daten, mangelnde Verbindlichkeit einer Transaktion oder den Ausfall einer IT-Anwendung entstehen." +
                        "\nBeispiele dafür sind:" +
                        "\n -unerlaubte Weitergabe von strategischen und taktischen Geschäftsinformationen" +
                        "\n -Manipulation von finanzwirksamen Daten in einem Abrechnungssystem" +
                        "\n -Ausfall eines IT - gesteuerten Produktionssystems und dadurch bedingte Umsatzverluste" +
                        "\n -Einsichtnahme in Marketingstrategiepapiere oder Umsatzzahlen" +
                        "\n -Diebstahl oder Zerstörung von Hardware";
        }
        /// <summary>
        /// Informationssegment Info
        /// </summary>
        public string Str_Msg_IS
        {
            get => "Bitte wählen Sie bis zu fünf Informationssegmente aus, die Ihren Prozess betreffen.\nEin Informationssegment beschreibt dabei eine Kategorie von Informationen die für unterschiedliche Geschäftsprozesse Relevanz haben, wie z.B. Kundenstammdaten oder Rechtsdaten.\nWeitere Informationen zu dem ausgewählten Informationssegment erhalten Sie über den jeweiligen Infobutton rechts daneben.";
        }
        /// <summary>
        /// Recovery Point Objective Info
        /// </summary>
        public string Str_Msg_RPO
        {
            //get => "Datenverlustzeit – Recovery Point Objective (RPO)\nDefinierter Zeitraum der zwischen zwei Backups liegen darf, um den Normalbetrieb im Notfall oder bei einer Störung sicherzustellen.\nDer RPO berechnet sich rückwärts vom Zeitpunkt des Absturzes (aufgrund der Backup-Zyklen max. 24 Stunden).";
            get => "Maximal tolerierbarer Datenverlust (MTD) (auch Recovery Point Objective, RPO)\n" +
                "Der MTD legt fest, wie groß der maximale zeitliche Abstand zu jenem System- und Datenzustand sein darf, " +
                "der z.B. nach einer Rückspeicherung der Daten wiederherstellbar ist. " +
                "Werden Daten beispielsweise täglich einmal gesichert, so beträgt der maximale Datenverlust einen Tag, " +
                "d. h. der Wiederherstellungszeitpunkt liegt maximal einen Tag zurück. (aufgrund der Backup-Zyklen max. 24 Stunden).";
        }
        /// <summary>
        /// Recovery Time Objective Info
        /// </summary>
        public string Str_Msg_RTO
        {
            //get => "Wiederanlaufzeit Regelbetrieb – Recovery Time Objective (RTO)\nDauer für die Wiederaufnahme eines Bankprozesses nach einer Störung oder einem Notfall im Regelbetrieb (in 1, 5, 10, 20 Tagen).";
            get => "Maximal tolerierbare Ausfalldauer (MTA) (auch Recovery Time Objective, RPO)\n" +
                "Die MTA legt fest, wie groß der Zeitraum sein darf zwischen Ausfall einer Ressource " +
                "und dem Übergang in den regulären Betrieb oder Notbetrieb (ggf. inklusive " +
                "dem Anlauf benötigter technischer Ressourcen). Im Notfallmanagement ist es der Wert, der angibt, " +
                "wann ein Prozess spätestens wieder anlaufen muss, damit die Überlebensfähigkeit einer Institution kurz- " +
                "oder langfristig nicht gefährdet ist.";
        }
        /// <summary>
        /// Recovery Time Objective Notfall Info
        /// </summary>
        public string Str_Msg_RTON
        {
            get => "Wiederanlaufzeit (Notfall, Störung) – Recovery Time Objective (RTO)\nDauer für die Wiederaufnahme eines Bankprozesses nach einer Störung oder einem Notfall im Notfall/Störfall (in 1, 5, 10, 20 Tagen).";
        }
        /// <summary>
        /// Prozesseigentümer Info
        /// </summary>
        public string Str_Msg_Owner
        {
            get => "Die Neuerhebung oder Neuweinwertung erfolgt in der Regel durch den Prozesseigentümer oder einen Vertreter. Der Prozesseigentümer hat entsprechende Verpflichtungen wahrzunehmen. Diese sind u.a.:" +
                        "\n- Aufnahme des Bankprozesses in die Business Impact Analyse" +
                        "\n- Bewertung der Kritikalität der Bankprozesse innerhalb der Business Impact Analyse(BIA) in Bezug auf die vier Schutzziele(Verfügbarkeit, Integrität, Vertraulichkeit, und Authentizität) u.a.als Basis für das zentrale Auslagerungsmanagement, das Informationsrisikomanagement / Business Continuity Management und dem Informationssicherheitsmanagement" +
                        "\n- Kurzbeschreibung des Prozesses" +
                        "\n- Begründung, warum der Prozess ein zentraler/kritischer Prozess bzw. ein Prozess mit hohem Schadenspotential ist" +
                        "\n- Maximal tolerierbare Ausfallzeit des Prozesses" +
                        "\n- Auswirkung auf Folgeprozesse.";
        }
        /// <summary>
        /// Prozesseigentümer Info
        /// </summary>
        public string Str_Msg_Responsible
        {
            get => "Die für den Prozess verantwortliche Person.\n-Wird durch den Prozesseigentümer benannt.";
        }
        /// <summary>
        /// Vorgelagerte Prozesse Info
        /// </summary>
        public string Str_Msg_PreProc
        {
            get => "Die Prozesse, die Ihrem Prozess Startdaten liefern oder ersatzweise die OE's / externe Stellen, die die Vorarbeit zu diesem Prozess leisten.";
        }
        /// <summary>
        /// Nachgelagerte Prozesse Info
        /// </summary>
        public string Str_Msg_PostProc
        {
            get => "Die Prozesse, die aus Prozess Ergebnisse erhalten oder ersatzweise die OE's / externe Stellen, die die Ergebnisse Ihres Prozesses weiterverarbeiten.";
        }
        /// <summary>
        /// Prozessname Info
        /// </summary>
        public string Str_Msg_ProcessName
        {
            get => "Der Name des Prozesses";
        }
        /// <summary>
        /// Sub-Prozessname Info
        /// </summary>
        public string Str_Msg_SubProcessName
        {
            get => "Der Name des Sub-Prozesses (optional falls Sub-Prozess vorhanden)";
        }
        /// <summary>
        /// OE-Info
        /// </summary>
        public string Str_Msg_OE
        {
            get => "Die für den Prozess verantwortliche OE";
        }
        /// <summary>
        /// Servicezeiten Info
        /// </summary>
        public string Str_Msg_ServiceTimes
        {
            get => "Die zur Sicherstellung des Prozessablaufs optimale Servicezeit";
        }
        /// <summary>
        /// Applikations-Prozess Zuordnung Info
        /// </summary>
        public string Str_Msg_AppToProc
        {
            get => "Wählen Sie ein oder mehrere Anwendungen aus der Übersicht aller Anwendungen (links) aus und klicken Sie den Button \"hinzufügen\", um die jeweilige Anwendung mit dem Prozess zu verknüpfen.\nAnalog wählen Sie eine Anwendung aus der Liste der bereits verknüpften Anwendungen (rechts) und klicken Sie \"entfernen\", um die jeweilige Verknüpfung zu entfernen.";
        }
        /// <summary>
        /// Applikations-Prozess Zuordnung Info (vorgelagerte/nachgelagerte Prozesse)
        /// </summary>
        public string Str_Msg_ProcToProc
        {
            get => "Wählen Sie ein oder mehrere Prozesse aus der jeweiligen Übersicht aller Prozesse (links) aus und klicken Sie den Button \"hinzufügen\", um den jeweiligen Prozess als vor- oder nachgelagerten Prozess zu definieren.\nAnalog wählen Sie eine Anwendung aus der Liste der bereits zugeordneten Prozesse (rechts) und klicken Sie \"entfernen\", um die jeweilige Zuordnung zu aufzuheben.";
        }
        /// <summary>
        /// Zuordnung inativer Applikationenen Benachrichtigung
        /// </summary>
        public string Str_Msg_InactiveApps
        {
            get => "Diesem Prozess sind inaktive Anwendungen zugeordnet. " +
                "\nDas bedeutet, dass die in der Liste rot markierten Anwendungen vom IT-Betrieb als inaktiv markiert wurden, da sie nicht mehr genutzt werden oder nicht mehr zur Verfügung stehen. " +
                "\nPrüfen Sie daher bitte die gekennzeichneten Anwendungen und entfernen Sie diese aus der Liste, falls Sie die Anwendung tatsächlich nicht mehr nutzen. " +
                "\nBei Fragen wenden Sie sich ggf. an den IT-Betrieb.";
        }
        /// <summary>
        /// Zuordnung inativer Applikationenen Benachrichtigung
        /// </summary>
        public string Str_Msg_InactiveProcs
        {
            get => "Diesem Prozess sind inaktive Prozesse zugeordnet. " +
                "\nDas bedeutet, dass die in der Liste rot markierten Prozesse von der verantwortlichen OE gelöscht wurden. " +
                "\nBringen Sie ggf. in Erfahrung, ob die zuständige OE eine Änderung an dem Prozess vorgenommen hat.";
        }
        #endregion

        #region sonstige Eigenschaften
        /// <summary>
        /// Überschrift
        /// </summary>
        public string Str_Header { get; set; }
        /// <summary>
        /// Bestimmt, ob neue Schutzziele angezeigt werden oder nicht (abhängig von Einstellungen)
        /// </summary>
        public bool Vis_NewSecurityGoals { get; set; }
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
                    Str_Header = "Prozess bearbeiten";
                }
                else if (value == ProcAppMode.New)
                {
                    Str_Header = "Neuen Prozess anlegen";
                }
            }
        }
        /// <summary>
        /// Sichtbarkeit für Prozesseigentümer-Definition (Nur von Admin und CISO definierbar, sonst abhängig vom Bereichs-/Stabsabteilungsleiter)
        /// </summary>
        public Visibility Vis_Mode { get; set; }
        /// <summary>
        /// Aktiviert Prozesseigentümer-Freitext (Nur von Admin und CISO)
        /// </summary>
        public bool Owner_Check { get; set; } = false;
        /// <summary>
        /// Liste aller Attribute
        /// </summary>
        readonly ObservableCollection<ISB_BIA_Informationssegmente_Attribute> _list_AllAttributes;
        /// <summary>
        /// Aktuelle Anwendungseinstellungen
        /// </summary>
        public ISB_BIA_Settings Setting { get; set; }
        #endregion

        #region Services
        private readonly INavigationService _myNavi;
        private readonly IDialogService _myDia;
        private readonly IDataService_Process _myProc;
        private readonly ILockService _myLock;
        private readonly IDataService_Setting _mySett;
        private readonly IDataService_Application _myApp;
        private readonly IDataService_Attribute _myIS;
        private readonly IExportService _myExp;
        private readonly ISharedResourceService _myShared;
        private readonly IDataService_OE _myOE;
        #endregion

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="myDia"></param>
        /// <param name="myNavi"></param>
        /// <param name="myProc"></param>
        /// <param name="myLock"></param>
        /// <param name="myExp"></param>
        /// <param name="myShared"></param>
        public Process_ViewModel(IDialogService myDia, INavigationService myNavi,
            IDataService_Process myProc, IDataService_Setting mySett, IDataService_Attribute myIS,
            IDataService_Application myApp, ILockService myLock,
            IExportService myExp, ISharedResourceService myShared, IDataService_OE myOE)
        {
            #region Services
            _myDia = myDia;
            _myNavi = myNavi;
            _myProc = myProc;
            _myLock = myLock;
            _myIS = myIS;
            _myApp = myApp;
            _myExp = myExp;
            _mySett = mySett;
            _myShared = myShared;
            _myOE = myOE;
            #endregion

            if (IsInDesignMode)
            {
                ProcessCurrent = _myProc.Get_Model_FromDB(1);
                _oldCurrentProcess = ProcessCurrent.Copy();
                Proc_Prozessverantwortlicher = ProcessCurrent.Prozessverantwortlicher;
            }

            //Messenger Registrierung für Prozessbearbeitungsmodus
            MessengerInstance.Register<NotificationMessage<int>>(this, ProcAppMode.Change, message =>
            {
                if (!(message.Sender is INavigationService)) return;
                Mode = ProcAppMode.Change;
                ProcessCurrent = _myProc.Get_Model_FromDB(message.Content);
                //Wenn Daten Fehlerhaft dann zurückkehren
                if (ProcessCurrent == null)
                {
                    _myDia.ShowError("Fehler beim Laden der Daten.");
                    Cleanup();
                    _myNavi.NavigateBack();
                }
                else
                {
                    _oldCurrentProcess = ProcessCurrent.Copy();
                    //Setzen der Properties der Hilfs-Eigenschaften
                    Proc_OE = ProcessCurrent.OE_Filter;
                    Proc_Prozessverantwortlicher = ProcessCurrent.Prozessverantwortlicher;
                    //Berechnen der Mindesteinstufung
                    CheckMinValues(ProcessCurrent);
                }
            });

            //Messenger Registrierung für Prozesserstellungsmodus
            MessengerInstance.Register<NotificationMessage<int>>(this, ProcAppMode.New, message =>
            {
                if (!(message.Sender is INavigationService)) return;
                Mode = ProcAppMode.New;
                ProcessCurrent = new Process_Model();
                _oldCurrentProcess = ProcessCurrent.Copy();
            });

            #region Listen und Daten für Prozess-Anwendungszuordnung
            List_AllApplications = new ObservableCollection<ISB_BIA_Applikationen>(_myApp.Get_List_Applications_Active().OrderBy(c => c.IT_Anwendung_System));
            List_ApplicationCategories = _myProc.Get_StringList_AppCategories();
            #endregion

            #region Filter für Anwendungsliste definieren
            FilterView = (CollectionView)CollectionViewSource.GetDefaultView(List_AllApplications);
            FilterView.Filter = ApplicationFilter;
            SelectedFilterItem = List_ApplicationCategories.FirstOrDefault();
            #endregion

            #region Listen und Daten für Prozess-vPnP-Zuordnung
            List_Processes = new ObservableCollection<ISB_BIA_Prozesse>(_myProc.Get_List_Processes_Active().OrderBy(c => c.Prozess));
            List_OEVNp = _myProc.Get_StringList_OEs_All();
            List_OEVNp.Insert(0, "<Alle>");
            #endregion

            #region Filter für Prozessliste (Schnittstellen) definieren
            FilterViewVNp = (CollectionView)CollectionViewSource.GetDefaultView(List_Processes);
            FilterViewVNp.Filter = VNpFilter;
            SelectedFilterItemVNp = List_OEVNp.FirstOrDefault();
            #endregion

            #region Füllen der Dropdownlisten
            List_ProcessResponsible = _myProc.Get_StringList_ProcessResponsible();
            List_OE = (_myShared.User.UserGroup == UserGroups.Admin || _myShared.User.UserGroup == UserGroups.CISO) ? _myProc.Get_StringList_OEs_All() : _myProc.Get_StringList_OEsForUser(_myShared.User.ListOE);
            List_Maturity = new ObservableCollection<string>(new List<string>() { "1 - Initial", "2 - Wiederholbar", "3 - Definiert", "4 - Gemanagt", "5 - Optimiert" });
            List_Crit = new ObservableCollection<string>(new List<string>() { "Normal", "Mittel", "Hoch", "Sehr hoch" });
            List_Dmg = new Dictionary<int, string>(){ { 1, "A - Marginal (0 - 20.000)" }, { 2, "B - Spürbar (20.000 - 50.000)" }, { 3, "C - Bedeutend (50.000 - 500.000)"}, { 4, "D - Kritisch (500.000 - 2 Mio.)"}, { 5, "E - Katastrophal (2 Mio. - 5 Mio.)"}, { 6, "F - Existenzgefährdend (mehr als 5 Mio.)"} };
            List_Freq = new Dictionary<int, string>() { { 1, "1 - Unwahrscheinlich (Alle 30+ Jahre)" }, { 2, "2 - Sehr niedrig (Alle 10 bis 30 Jahre)" }, { 3, "3 - Niedrig (Alle 3 bis 10 Jahre)" }, { 4, "4 - Gelegentlich (Alle 2 bis 3 Jahre)" }, { 5, "5 - Hoch (Max. alle 2 Jahre)" }, { 6, "6 - Sehr hoch (Max. 1 Mal im Jahr)" } };
            List_RTO = new ObservableCollection<int>(new List<int>() { 1, 5, 10, 20 });
            List_IS = _myProc.Get_StringList_ISDictionary();
            #endregion

            #region Einstellungen abrufen
            Setting = _mySett.Get_List_Settings();
            Vis_NewSecurityGoals = (Setting.Neue_Schutzziele_aktiviert == "Ja");
            #endregion

            #region Informationssegmente und Attribute abrufen
            _list_AllAttributes = _myIS.Get_List_Attributes();
            #endregion
        }

        #region Hilfsfunktionen
        /// <summary>
        /// Methode zur Berechnung der Mindesteinstufung der Schutzziele eines Prozesses abhängig von den gewählten Informationssegmenten
        /// </summary>
        /// <param name="process"> Aktueller Prozess </param>
        public void CheckMinValues(Process_Model process)
        {
            try
            {
                List<ISB_BIA_Informationssegmente> queryIS = _myProc.Get_List_Segments_5ForCalculation(process);
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
                    var maxList = _list_AllAttributes.Where(n => list.Contains(n.Attribut_Id)).GroupBy(x => x.Attribut_Id).Select(z => z.OrderByDescending(q => q.Datum).FirstOrDefault());
                    SZ_1_Max = (SZ_Values) maxList.Max(x => x.SZ_1);
                    SZ_2_Max = (SZ_Values) maxList.Max(x => x.SZ_2);
                    SZ_3_Max = (SZ_Values) maxList.Max(x => x.SZ_3);
                    SZ_4_Max = (SZ_Values) maxList.Max(x => x.SZ_4);
                    SZ_5_Max = (SZ_Values) maxList.Max(x => x.SZ_5);
                    SZ_6_Max = (SZ_Values) maxList.Max(x => x.SZ_6);
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
            SimpleIoc.Default.Unregister(this);
            base.Cleanup();
        }
        #endregion

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
            if (string.IsNullOrEmpty(_proc_OE))
                AddError(nameof(Proc_OE), "Pflichtfeld");
            if (string.IsNullOrEmpty(_proc_Prozessverantwortlicher))
                AddError(nameof(Proc_Prozessverantwortlicher), "Pflichtfeld");
            if (string.IsNullOrEmpty(_proc_Prozessverantwortlicher_Text))
                AddError(nameof(Proc_Prozessverantwortlicher_Text), "Pflichtfeld");
            if (String.IsNullOrEmpty(ProcessCurrent.Kritikalität_des_Prozesses))
            {
                if (_proc_Frequenz.Value == null)
                    AddError(nameof(Proc_Frequenz), "Pflichtfeld");
                if (_proc_Schaden.Value == null)
                    AddError(nameof(Proc_Schaden), "Pflichtfeld");
            }
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
        public bool HasErrors => ProcessCurrent._errors.Count > 0;
        /// <summary>
        /// Event für Änderungsbenachrichtigung bzgl. Fehler
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        #endregion
    }
}
