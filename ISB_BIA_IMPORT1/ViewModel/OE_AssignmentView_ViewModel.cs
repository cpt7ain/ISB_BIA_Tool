using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using ISB_BIA_IMPORT1.LinqDataContext;
using ISB_BIA_IMPORT1.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace ISB_BIA_IMPORT1.ViewModel
{
    /// <summary>
    /// VM zur Darstellung der OE-Einstellungen
    /// </summary>
    public class OE_AssignmentView_ViewModel : ViewModelBase
    {
        #region Backing-Fields
        private ObservableCollection<ISB_BIA_OEs> _oENames;
        private ISB_BIA_OEs _selectedName;
        private string _newName;
        private string _editNewName;
        private ObservableCollection<ISB_BIA_OEs> _oENumbers;
        private ISB_BIA_OEs _selectedNumber;
        private ISB_BIA_OEs _selectedNewNumberName;
        private string _newNumber;
        private string _editNewNumber;
        private ObservableCollection<ISB_BIA_OEs> _oELinks;
        private ISB_BIA_OEs _selectedLink;
        private ISB_BIA_OEs _selectedNewLinkName;
        private ISB_BIA_OEs _selectedNewLinkNumber;
        private MyRelayCommand _cancelEdit;
        private MyRelayCommand _activateNameNew;
        private MyRelayCommand _saveNameNew;
        private MyRelayCommand _activateNameEdit;
        private MyRelayCommand _saveNameEdit;
        private MyRelayCommand _activateLinkNew;
        private MyRelayCommand _saveLinkNew;
        private MyRelayCommand _activateNumberNew;
        private MyRelayCommand _saveNumberNew;
        private MyRelayCommand _activateNumberEdit;
        private MyRelayCommand _saveNumberEdit;
        private MyRelayCommand _deleteOENameCmd;
        private MyRelayCommand _deleteOELinkCmd;
        private MyRelayCommand _deleteOENumberCmd;
        private bool _isFree = true;
        private Visibility _nameVis;
        private Visibility _nameEditBorderVis;
        private Visibility _nameNewBorderVis;
        private Visibility _linkVis;
        private Visibility _linkNewBorderVis;
        private Visibility _numberVis;
        private Visibility _numberEditBorderVis;
        private Visibility _numberNewBorderVis;
        #endregion

        #region Properties zur Bindung an Controls (Listen => Dropdown, String => Textfelder)
        /// <summary>
        /// Liste der angelegten OE-Gruppen
        /// </summary>
        public ObservableCollection<ISB_BIA_OEs> OENames
        {
            get => _oENames;
            set => Set(() => OENames, ref _oENames, value);
        }
        /// <summary>
        /// Liste der angelegten OE-Nummern
        /// </summary>
        public ObservableCollection<ISB_BIA_OEs> OENumbers
        {
            get => _oENumbers;
            set => Set(() => OENumbers, ref _oENumbers, value);
        }
        /// <summary>
        /// Liste der angelegten OE-Relationen
        /// </summary>
        public ObservableCollection<ISB_BIA_OEs> OELinks
        {
            get => _oELinks;
            set => Set(() => OELinks, ref _oELinks, value);
        }
        /// <summary>
        /// Name einer OE-Gruppe bei Neuanlgae
        /// </summary>
        public string NewName
        {
            get => _newName;
            set => Set(() => NewName, ref _newName, value);
        }
        /// <summary>
        /// Name einer OE-Gruppe bei Bearbeitung
        /// </summary>
        public string EditNewName
        {
            get => _editNewName;
            set => Set(() => EditNewName, ref _editNewName, value);
        }
        /// <summary>
        /// Nummer einer OE bei Neuanlage
        /// </summary>
        public string NewNumber
        {
            get => _newNumber;
            set => Set(() => NewNumber, ref _newNumber, value);
        }
        /// <summary>
        /// Nummer einer OE bei Bearbeitung
        /// </summary>
        public string EditNewNumber
        {
            get => _editNewNumber;
            set => Set(() => EditNewNumber, ref _editNewNumber, value);
        }
        #endregion

        #region Ausgewähltes Item im jeweiligen Datagrid
        /// <summary>
        /// Ausgewählte OE-Gruppe für Bearbeitung oder Löschung
        /// </summary>
        public ISB_BIA_OEs SelectedName
        {
            get => _selectedName;
            set => Set(() => SelectedName, ref _selectedName, value);
        }
        /// <summary>
        /// Ausgewählte OE-Nummer für Bearbeitung oder Löschung
        /// </summary>
        public ISB_BIA_OEs SelectedNumber
        {
            get => _selectedNumber;
            set => Set(() => SelectedNumber, ref _selectedNumber, value);
        }
        /// <summary>
        /// Ausgewählte OE-Relation für Löschung
        /// </summary>
        public ISB_BIA_OEs SelectedLink
        {
            get => _selectedLink;
            set => Set(() => SelectedLink, ref _selectedLink, value);
        }
        #endregion

        #region Ausgewähltes Item im jeweiligen Dropdownfeld
        /// <summary>
        /// Ausgewählte OE-Gruppe für Erstellung einer neuen OE-Nummer
        /// </summary>
        public ISB_BIA_OEs SelectedNewNumberName
        {
            get => _selectedNewNumberName;
            set => Set(() => SelectedNewNumberName, ref _selectedNewNumberName, value);
        }
        /// <summary>
        /// Ausgewählte OE-Gruppe zur Erstellung einer Relation
        /// </summary>
        public ISB_BIA_OEs SelectedNewLinkName
        {
            get => _selectedNewLinkName;
            set => Set(() => SelectedNewLinkName, ref _selectedNewLinkName, value);
        }
        /// <summary>
        /// Ausgewählte OE-Nummer zu Erstellung einer Relation
        /// </summary>
        public ISB_BIA_OEs SelectedNewLinkNumber
        {
            get => _selectedNewLinkNumber;
            set => Set(() => SelectedNewLinkNumber, ref _selectedNewLinkNumber, value);
        }
        #endregion

        #region Commands zur Regelung der Sichtbarkeit und Aktivierung einzelner Controls in den verschiedenen Bearbeitungsmodi
        /// <summary>
        /// Zum Ausgangszustnd der Ansicht zurückkehren
        /// </summary>
        public MyRelayCommand CancelEdit
        {
            get => _cancelEdit
                  ?? (_cancelEdit = new MyRelayCommand(() =>
                  {
                      CancelEditFunc();
                  }));
        }
        /// <summary>
        /// Command zum Aktivieren der OE-Gruppen Erstellung
        /// </summary>
        public MyRelayCommand ActivateNameNew
        {
            get => _activateNameNew
                  ?? (_activateNameNew = new MyRelayCommand(() =>
                  {
                      IsFree = false;
                      NameNewBorderVis = Visibility.Visible;
                      NameVis = Visibility.Collapsed;
                      NameEditBorderVis = Visibility.Collapsed;
                  }, () => IsFree));
        }
        /// <summary>
        /// Command zum Aktivieren der OE-Gruppen BEarbeitung
        /// </summary>
        public MyRelayCommand ActivateNameEdit
        {
            get => _activateNameEdit
                  ?? (_activateNameEdit = new MyRelayCommand(() =>
                  {
                      if (SelectedName != null)
                      {
                          IsFree = false;
                          NameEditBorderVis = Visibility.Visible;
                          NameVis = Visibility.Collapsed;
                          NameNewBorderVis = Visibility.Collapsed;
                      }
                      else
                      {
                          _myDia.ShowWarning("Bitte zu ändernden Eintrag auswählen");
                      }
                  }, () => IsFree));
        }
        /// <summary>
        /// Command zum Aktivieren der OE-Relation Erstellung
        /// </summary>
        public MyRelayCommand ActivateLinkNew
        {
            get => _activateLinkNew
                  ?? (_activateLinkNew = new MyRelayCommand(() =>
                  {
                      if (OENumbers.Count > 0)
                      {
                          IsFree = false;
                          LinkNewBorderVis = Visibility.Visible;
                          LinkVis = Visibility.Collapsed;
                      }
                      else
                      {
                          _myDia.ShowWarning("Es stehen keine OE-Kennungen ür die Zuweisung zur Verfügung.");
                      }
                  }, () => IsFree));
        }
        /// <summary>
        /// Command zum Aktivieren der OE-Nummern Erstellung
        /// </summary>
        public MyRelayCommand ActivateNumberNew
        {
            get => _activateNumberNew
                  ?? (_activateNumberNew = new MyRelayCommand(() =>
                  {
                      IsFree = false;
                      NumberNewBorderVis = Visibility.Visible;
                      NumberVis = Visibility.Collapsed;
                      NumberEditBorderVis = Visibility.Collapsed;
                  }, () => IsFree));
        }
        /// <summary>
        /// Command zum Aktivieren der OE-Nummern Bearbeitung
        /// </summary>
        public MyRelayCommand ActivateNumberEdit
        {
            get => _activateNumberEdit
                  ?? (_activateNumberEdit = new MyRelayCommand(() =>
                  {
                      if (SelectedNumber != null)
                      {
                          IsFree = false;
                          NumberEditBorderVis = Visibility.Visible;
                          NumberVis = Visibility.Collapsed;
                          NumberNewBorderVis = Visibility.Collapsed;
                      }
                      else
                      {
                          _myDia.ShowInfo("Bitte zu ändernden Eintrag auswählen");
                      }
                  }, () => IsFree));
        }
        #endregion

        #region Commands zur Neuanlage, Bearbeitung und Löschung von OE-Gruppen, Relationen und OE's + Refresh der Listen
        /// <summary>
        /// Command zum Speichern einer neuen OE-Gruppe
        /// </summary>
        public MyRelayCommand SaveNameNew
        {
            get => _saveNameNew
                  ?? (_saveNameNew = new MyRelayCommand(() =>
                  {
                      ISB_BIA_OEs result = _myData.InsertOEName(NewName);
                      if (result != null)
                      {
                          CancelEditFunc();
                          OENames = _myData.GetOENames();
                          OENumbers = _myData.GetOENumbers();
                          OELinks = _myData.GetOELinks();
                      }
                  }));
        }
        /// <summary>
        /// Command zum Speichern einer geänderten OE-Gruppe
        /// </summary>
        public MyRelayCommand SaveNameEdit
        {
            get => _saveNameEdit
                  ?? (_saveNameEdit = new MyRelayCommand(() =>
                  {
                      bool result = _myData.EditOEName(EditNewName, SelectedName.OE_Name);
                      if (result)
                      {
                          CancelEditFunc();
                          OENames = _myData.GetOENames();
                          OENumbers = _myData.GetOENumbers();
                          OELinks = _myData.GetOELinks();
                      }
                  }));
        }
        /// <summary>
        /// Command zum Speichern einer OE-Relation
        /// </summary>
        public MyRelayCommand SaveLinkNew
        {
            get => _saveLinkNew
                  ?? (_saveLinkNew = new MyRelayCommand(() =>
                  {
                      ISB_BIA_OEs result = _myData.InsertOELink(SelectedNewLinkName, SelectedNewLinkNumber);
                      if (result != null)
                      {
                          CancelEditFunc();
                          OENames = _myData.GetOENames();
                          OENumbers = _myData.GetOENumbers();
                          OELinks = _myData.GetOELinks();
                      }
                  }));
        }
        /// <summary>
        /// Command zum Speichern einer neuen OE-Nummer
        /// </summary>
        public MyRelayCommand SaveNumberNew
        {
            get => _saveNumberNew
                  ?? (_saveNumberNew = new MyRelayCommand(() =>
                  {
                      ISB_BIA_OEs result = _myData.InsertOENumber(NewNumber, SelectedNewNumberName);
                      if (result != null)
                      {
                          CancelEditFunc();
                          OENames = _myData.GetOENames();
                          OENumbers = _myData.GetOENumbers();
                          OELinks = _myData.GetOELinks();
                      }
                  }));
        }
        /// <summary>
        /// Command zum Speichern einer geänderten OE-Nummer
        /// </summary>
        public MyRelayCommand SaveNumberEdit
        {
            get => _saveNumberEdit
                  ?? (_saveNumberEdit = new MyRelayCommand(() =>
                  {
                      bool result = _myData.EditOENumber(EditNewNumber, SelectedNumber.OE_Nummer);
                      if (result)
                      {
                          CancelEditFunc();
                          OENames = _myData.GetOENames();
                          OENumbers = _myData.GetOENumbers();
                          OELinks = _myData.GetOELinks();
                      }
                  }));
        }
        /// <summary>
        /// Command zum Löschen einer OE-Gruppe
        /// </summary>
        public MyRelayCommand DeleteOENameCmd
        {
            get => _deleteOENameCmd
                  ?? (_deleteOENameCmd = new MyRelayCommand(() =>
                  {
                      if (SelectedName != null)
                      {
                          if (_myData.DeleteOEName(SelectedName.OE_Name))
                          {
                              OENames = _myData.GetOENames();
                              OENumbers = _myData.GetOENumbers();
                              OELinks = _myData.GetOELinks();
                          }
                      }
                      else
                      {
                          _myDia.ShowInfo("Bitte zu löschenden Eintrag auswählen.");
                      }
                  }, () => IsFree ));
        }
        /// <summary>
        /// Command zum Speichern einer OE-Relation
        /// </summary>
        public MyRelayCommand DeleteOELinkCmd
        {
            get => _deleteOELinkCmd
                  ?? (_deleteOELinkCmd = new MyRelayCommand(() =>
                  {
                      if (SelectedLink != null)
                      {
                          if (_myData.DeleteOELink(SelectedLink.OE_Name, SelectedLink.OE_Nummer))
                          {
                              OENames = _myData.GetOENames();
                              OENumbers = _myData.GetOENumbers();
                              OELinks = _myData.GetOELinks();
                          }
                      }
                      else
                      {
                          _myDia.ShowInfo("Bitte zu löschenden Eintrag auswählen.");
                      }
                  }, () => IsFree));
        }
        /// <summary>
        /// Command zum Löschen einer OE-Nummer
        /// </summary>
        public MyRelayCommand DeleteOENumberCmd
        {
            get => _deleteOENumberCmd
                  ?? (_deleteOENumberCmd = new MyRelayCommand(() =>
                  {
                      if (SelectedNumber != null)
                      {
                          if (_myData.DeleteOENumber(SelectedNumber.OE_Nummer))
                          {
                              OENames = _myData.GetOENames();
                              OENumbers = _myData.GetOENumbers();
                              OELinks = _myData.GetOELinks();
                          }
                      }
                      else
                      {
                          _myDia.ShowInfo("Bitte zu löschenden Eintrag auswählen.");
                      }
                  }, () => IsFree));
        }
        #endregion

        #region Properties für Sichtbarkeiten und Steuerung der Aktivierung von Controls
        /// <summary>
        /// Bestimmt, ob Buttons aktiviert oder deaktiviert sind
        /// </summary>
        public bool IsFree
        {
            get => _isFree;
            set => Set(() => IsFree, ref _isFree, value);
        }
        /// <summary>
        /// Sichtbarkeit des OE-Gruppen Startfensters
        /// </summary>
        public Visibility NameVis
        {
            get => _nameVis;
            set => Set(() => NameVis, ref _nameVis, value);
        }
        /// <summary>
        /// Sichtbarkeit des OE-Gruppen Bearbeitungsfensters
        /// </summary>
        public Visibility NameEditBorderVis
        {
            get => _nameEditBorderVis;
            set => Set(() => NameEditBorderVis, ref _nameEditBorderVis, value);
        }
        /// <summary>
        /// Sichtbarkeit des OE-Gruppen Erstellungsfensters
        /// </summary>
        public Visibility NameNewBorderVis
        {
            get => _nameNewBorderVis;
            set => Set(() => NameNewBorderVis, ref _nameNewBorderVis, value);
        }
        /// <summary>
        /// Sichtbarkeit des OE-Relation Startfensters
        /// </summary>
        public Visibility LinkVis
        {
            get => _linkVis;
            set => Set(() => LinkVis, ref _linkVis, value);
        }
        /// <summary>
        /// Sichtbarkeit des OE Relation Erstellungsfensters
        /// </summary>
        public Visibility LinkNewBorderVis
        {
            get => _linkNewBorderVis;
            set => Set(() => LinkNewBorderVis, ref _linkNewBorderVis, value);
        }
        /// <summary>
        /// Sichtbarkeit des OE-Nummern Startfensters
        /// </summary>
        public Visibility NumberVis
        {
            get => _numberVis;
            set => Set(() => NumberVis, ref _numberVis, value);
        }
        /// <summary>
        /// Sichtbarkeit des OE-Nummern Bearbeitungsfensters
        /// </summary>
        public Visibility NumberEditBorderVis
        {
            get => _numberEditBorderVis;
            set => Set(() => NumberEditBorderVis, ref _numberEditBorderVis, value);
        }
        /// <summary>
        /// Sichtbarkeit des OE-Nummern Erstellungsfensters
        /// </summary>
        public Visibility NumberNewBorderVis
        {
            get => _numberNewBorderVis;
            set => Set(() => NumberNewBorderVis, ref _numberNewBorderVis, value);
        }
        #endregion

        /// <summary>
        /// Command zum Zurückkehren zum vorherigen VM
        /// </summary>
        public MyRelayCommand NavBack
        {
            get => new MyRelayCommand(() =>
                {
                    Cleanup();
                    _myNavi.NavigateBack();
                    _myData.UnlockObject(Table_Lock_Flags.OEs, 0);
                });
        }

        #region Services
        IMyNavigationService _myNavi;
        IMyDialogService _myDia;
        IMyDataService _myData;
        #endregion

        /// <summary>
        /// Viewmodel für die OE-Verwaltung. Hier werden OE-Gruppen angelegt, denen beliebig viele OE-Nummer zugeordnet werden können
        /// OE-Gruppen sind jene, die einem Prozess als "Verantwortliche OE" zugewiesen werden.
        /// Zur Steuerung der angezeigten Prozesse für einen user müssen somit den Gruppen die korrekten Nummern zugeordnet werden.
        /// </summary>
        /// <param name="myDialogService"></param>
        /// <param name="myNavigationService"></param>
        /// <param name="myDataService"></param>
        public OE_AssignmentView_ViewModel(IMyDialogService myDialogService, IMyNavigationService myNavigationService, IMyDataService myDataService)
        {
            _myDia = myDialogService;
            _myNavi = myNavigationService;
            _myData = myDataService;

            OENames = _myData.GetOENames();
            OENumbers = _myData.GetOENumbers();
            OELinks = _myData.GetOELinks();
            NameVis = Visibility.Visible;
            NameNewBorderVis = Visibility.Collapsed;
            NameEditBorderVis = Visibility.Collapsed;

            LinkVis = Visibility.Visible;
            LinkNewBorderVis = Visibility.Collapsed;

            NumberVis = Visibility.Visible;
            NumberNewBorderVis = Visibility.Collapsed;
            NumberEditBorderVis = Visibility.Collapsed;
        }

        /// <summary>
        /// Bricht die Bearbeitung ab
        /// </summary>
        public void CancelEditFunc()
        {
            IsFree = true;
            NameVis = Visibility.Visible;
            NameNewBorderVis = Visibility.Collapsed;
            NameEditBorderVis = Visibility.Collapsed;

            LinkVis = Visibility.Visible;
            LinkNewBorderVis = Visibility.Collapsed;

            NumberVis = Visibility.Visible;
            NumberNewBorderVis = Visibility.Collapsed;
            NumberEditBorderVis = Visibility.Collapsed;
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
    }
}
