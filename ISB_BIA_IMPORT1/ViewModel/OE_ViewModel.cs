using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using ISB_BIA_IMPORT1.LINQ2SQL;
using ISB_BIA_IMPORT1.Services;
using System.Collections.ObjectModel;
using System.Windows;
using ISB_BIA_IMPORT1.Helpers;
using ISB_BIA_IMPORT1.Services.Interfaces;

namespace ISB_BIA_IMPORT1.ViewModel
{
    /// <summary>
    /// VM zur Darstellung der OE-Einstellungen
    /// </summary>
    public class OE_ViewModel : ViewModelBase
    {
        #region Backing-Fields
        private ObservableCollection<ISB_BIA_OEs> _list_OENames;
        private ISB_BIA_OEs _oE_SelectedName;
        private string _str_NewName;
        private string _str_EditNewName;
        private ObservableCollection<ISB_BIA_OEs> _list_OENumbers;
        private ISB_BIA_OEs _oE_SelectedNumber;
        private ISB_BIA_OEs _oE_SelectedNewNumberName;
        private string _str_NewNumber;
        private string _str_EditNewNumber;
        private ObservableCollection<ISB_BIA_OEs> _list_OELinks;
        private ISB_BIA_OEs _oE_SelectedLink;
        private ISB_BIA_OEs _oE_SelectedNewLinkName;
        private ISB_BIA_OEs _oE_SelectedNewLinkNumber;
        private MyRelayCommand _cmd_CancelEdit;
        private MyRelayCommand _cmd_ActivateNameNew;
        private MyRelayCommand _cmd_SaveNameNew;
        private MyRelayCommand _cmd_ActivateNameEdit;
        private MyRelayCommand _cmd_SaveNameEdit;
        private MyRelayCommand _cmd_ActivateLinkNew;
        private MyRelayCommand _cmd_SaveLinkNew;
        private MyRelayCommand _cmd_ActivateNumberNew;
        private MyRelayCommand _cmd_SaveNumberNew;
        private MyRelayCommand _cmd_ActivateNumberEdit;
        private MyRelayCommand _cmd_SaveNumberEdit;
        private MyRelayCommand _cmd_DeleteOENameCmd;
        private MyRelayCommand _cmd_DeleteOELinkCmd;
        private MyRelayCommand _cmd_DeleteOENumberCmd;
        private bool _isFree = true;
        private Visibility _vis_NameBorder;
        private Visibility _vis_NameEditBorder;
        private Visibility _vis_NameNewBorder;
        private Visibility _vis_LinkBorder;
        private Visibility _vis_LinkNewBorder;
        private Visibility _vis_NumberBorder;
        private Visibility _vis_NumberEditBorder;
        private Visibility _vis_NumberNewBorder;
        #endregion

        #region Properties zur Bindung an Controls (Listen => Dropdown, String => Textfelder)
        /// <summary>
        /// Liste der angelegten OE-Gruppen
        /// </summary>
        public ObservableCollection<ISB_BIA_OEs> List_OENames
        {
            get => _list_OENames;
            set => Set(() => List_OENames, ref _list_OENames, value);
        }
        /// <summary>
        /// Liste der angelegten OE-Nummern
        /// </summary>
        public ObservableCollection<ISB_BIA_OEs> List_OENumbers
        {
            get => _list_OENumbers;
            set => Set(() => List_OENumbers, ref _list_OENumbers, value);
        }
        /// <summary>
        /// Liste der angelegten OE-Relationen
        /// </summary>
        public ObservableCollection<ISB_BIA_OEs> List_OELinks
        {
            get => _list_OELinks;
            set => Set(() => List_OELinks, ref _list_OELinks, value);
        }
        /// <summary>
        /// Name einer OE-Gruppe bei Neuanlgae
        /// </summary>
        public string Str_NewName
        {
            get => _str_NewName;
            set => Set(() => Str_NewName, ref _str_NewName, value);
        }
        /// <summary>
        /// Name einer OE-Gruppe bei Bearbeitung
        /// </summary>
        public string Str_EditNewName
        {
            get => _str_EditNewName;
            set => Set(() => Str_EditNewName, ref _str_EditNewName, value);
        }
        /// <summary>
        /// Nummer einer OE bei Neuanlage
        /// </summary>
        public string Str_NewNumber
        {
            get => _str_NewNumber;
            set => Set(() => Str_NewNumber, ref _str_NewNumber, value);
        }
        /// <summary>
        /// Nummer einer OE bei Bearbeitung
        /// </summary>
        public string Str_EditNewNumber
        {
            get => _str_EditNewNumber;
            set => Set(() => Str_EditNewNumber, ref _str_EditNewNumber, value);
        }
        #endregion

        #region Ausgewähltes Item im jeweiligen Datagrid
        /// <summary>
        /// Ausgewählte OE-Gruppe für Bearbeitung oder Löschung
        /// </summary>
        public ISB_BIA_OEs OE_SelectedName
        {
            get => _oE_SelectedName;
            set => Set(() => OE_SelectedName, ref _oE_SelectedName, value);
        }
        /// <summary>
        /// Ausgewählte OE-Nummer für Bearbeitung oder Löschung
        /// </summary>
        public ISB_BIA_OEs OE_SelectedNumber
        {
            get => _oE_SelectedNumber;
            set => Set(() => OE_SelectedNumber, ref _oE_SelectedNumber, value);
        }
        /// <summary>
        /// Ausgewählte OE-Relation für Löschung
        /// </summary>
        public ISB_BIA_OEs OE_SelectedLink
        {
            get => _oE_SelectedLink;
            set => Set(() => OE_SelectedLink, ref _oE_SelectedLink, value);
        }
        #endregion

        #region Ausgewähltes Item im jeweiligen Dropdownfeld
        /// <summary>
        /// Ausgewählte OE-Gruppe für Erstellung einer neuen OE-Nummer
        /// </summary>
        public ISB_BIA_OEs OE_SelectedNewNumberName
        {
            get => _oE_SelectedNewNumberName;
            set => Set(() => OE_SelectedNewNumberName, ref _oE_SelectedNewNumberName, value);
        }
        /// <summary>
        /// Ausgewählte OE-Gruppe zur Erstellung einer Relation
        /// </summary>
        public ISB_BIA_OEs OE_SelectedNewLinkName
        {
            get => _oE_SelectedNewLinkName;
            set => Set(() => OE_SelectedNewLinkName, ref _oE_SelectedNewLinkName, value);
        }
        /// <summary>
        /// Ausgewählte OE-Nummer zu Erstellung einer Relation
        /// </summary>
        public ISB_BIA_OEs OE_SelectedNewLinkNumber
        {
            get => _oE_SelectedNewLinkNumber;
            set => Set(() => OE_SelectedNewLinkNumber, ref _oE_SelectedNewLinkNumber, value);
        }
        #endregion

        #region Commands zur Regelung der Sichtbarkeit und Aktivierung einzelner Controls in den verschiedenen Bearbeitungsmodi
        /// <summary>
        /// Zum Ausgangszustnd der Ansicht zurückkehren
        /// </summary>
        public MyRelayCommand Cmd_CancelEdit
        {
            get => _cmd_CancelEdit
                  ?? (_cmd_CancelEdit = new MyRelayCommand(() =>
                  {
                      CancelEdit();
                  }));
        }
        /// <summary>
        /// Command zum Aktivieren der OE-Gruppen Erstellung
        /// </summary>
        public MyRelayCommand Cmd_ActivateNameNew
        {
            get => _cmd_ActivateNameNew
                  ?? (_cmd_ActivateNameNew = new MyRelayCommand(() =>
                  {
                      IsFree = false;
                      Vis_NameNewBorder = Visibility.Visible;
                      Vis_NameBorder = Visibility.Collapsed;
                      Vis_NameEditBorder = Visibility.Collapsed;
                  }, () => IsFree));
        }
        /// <summary>
        /// Command zum Aktivieren der OE-Gruppen BEarbeitung
        /// </summary>
        public MyRelayCommand Cmd_ActivateNameEdit
        {
            get => _cmd_ActivateNameEdit
                  ?? (_cmd_ActivateNameEdit = new MyRelayCommand(() =>
                  {
                      if (OE_SelectedName != null)
                      {
                          IsFree = false;
                          Vis_NameEditBorder = Visibility.Visible;
                          Vis_NameBorder = Visibility.Collapsed;
                          Vis_NameNewBorder = Visibility.Collapsed;
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
        public MyRelayCommand Cmd_ActivateLinkNew
        {
            get => _cmd_ActivateLinkNew
                  ?? (_cmd_ActivateLinkNew = new MyRelayCommand(() =>
                  {
                      if (List_OENumbers.Count > 0)
                      {
                          IsFree = false;
                          Vis_LinkNewBorder = Visibility.Visible;
                          Vis_LinkBorder = Visibility.Collapsed;
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
        public MyRelayCommand Cmd_ActivateNumberNew
        {
            get => _cmd_ActivateNumberNew
                  ?? (_cmd_ActivateNumberNew = new MyRelayCommand(() =>
                  {
                      IsFree = false;
                      Vis_NumberNewBorder = Visibility.Visible;
                      Vis_NumberBorder = Visibility.Collapsed;
                      Vis_NumberEditBorder = Visibility.Collapsed;
                  }, () => IsFree));
        }
        /// <summary>
        /// Command zum Aktivieren der OE-Nummern Bearbeitung
        /// </summary>
        public MyRelayCommand Cmd_ActivateNumberEdit
        {
            get => _cmd_ActivateNumberEdit
                  ?? (_cmd_ActivateNumberEdit = new MyRelayCommand(() =>
                  {
                      if (OE_SelectedNumber != null)
                      {
                          IsFree = false;
                          Vis_NumberEditBorder = Visibility.Visible;
                          Vis_NumberBorder = Visibility.Collapsed;
                          Vis_NumberNewBorder = Visibility.Collapsed;
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
        public MyRelayCommand Cmd_SaveNameNew
        {
            get => _cmd_SaveNameNew
                  ?? (_cmd_SaveNameNew = new MyRelayCommand(() =>
                  {
                      ISB_BIA_OEs result = _myOE.Insert_OEName_New(Str_NewName);
                      if (result != null)
                      {
                          CancelEdit();
                          List_OENames = _myOE.Get_List_OENames();
                          List_OENumbers = _myOE.Get_List_OENumbers();
                          List_OELinks = _myOE.Get_List_OERelation();
                      }
                  }));
        }
        /// <summary>
        /// Command zum Speichern einer geänderten OE-Gruppe
        /// </summary>
        public MyRelayCommand Cmd_SaveNameEdit
        {
            get => _cmd_SaveNameEdit
                  ?? (_cmd_SaveNameEdit = new MyRelayCommand(() =>
                  {
                      bool result = _myOE.Insert_OEName_Edit(Str_EditNewName, OE_SelectedName.OE_Name);
                      if (result)
                      {
                          CancelEdit();
                          List_OENames = _myOE.Get_List_OENames();
                          List_OENumbers = _myOE.Get_List_OENumbers();
                          List_OELinks = _myOE.Get_List_OERelation();
                      }
                  }));
        }
        /// <summary>
        /// Command zum Speichern einer OE-Relation
        /// </summary>
        public MyRelayCommand Cmd_SaveLinkNew
        {
            get => _cmd_SaveLinkNew
                  ?? (_cmd_SaveLinkNew = new MyRelayCommand(() =>
                  {
                      ISB_BIA_OEs result = _myOE.Insert_OERelation(OE_SelectedNewLinkName, OE_SelectedNewLinkNumber);
                      if (result != null)
                      {
                          CancelEdit();
                          List_OENames = _myOE.Get_List_OENames();
                          List_OENumbers = _myOE.Get_List_OENumbers();
                          List_OELinks = _myOE.Get_List_OERelation();
                      }
                  }));
        }
        /// <summary>
        /// Command zum Speichern einer neuen OE-Nummer
        /// </summary>
        public MyRelayCommand Cmd_SaveNumberNew
        {
            get => _cmd_SaveNumberNew
                  ?? (_cmd_SaveNumberNew = new MyRelayCommand(() =>
                  {
                      ISB_BIA_OEs result = _myOE.Insert_OENumber_New(Str_NewNumber, OE_SelectedNewNumberName);
                      if (result != null)
                      {
                          CancelEdit();
                          List_OENames = _myOE.Get_List_OENames();
                          List_OENumbers = _myOE.Get_List_OENumbers();
                          List_OELinks = _myOE.Get_List_OERelation();
                      }
                  }));
        }
        /// <summary>
        /// Command zum Speichern einer geänderten OE-Nummer
        /// </summary>
        public MyRelayCommand Cmd_SaveNumberEdit
        {
            get => _cmd_SaveNumberEdit
                  ?? (_cmd_SaveNumberEdit = new MyRelayCommand(() =>
                  {
                      bool result = _myOE.Insert_OENumber_Edit(Str_EditNewNumber, OE_SelectedNumber.OE_Nummer);
                      if (result)
                      {
                          CancelEdit();
                          List_OENames = _myOE.Get_List_OENames();
                          List_OENumbers = _myOE.Get_List_OENumbers();
                          List_OELinks = _myOE.Get_List_OERelation();
                      }
                  }));
        }
        /// <summary>
        /// Command zum Löschen einer OE-Gruppe
        /// </summary>
        public MyRelayCommand Cmd_DeleteOENameCmd
        {
            get => _cmd_DeleteOENameCmd
                  ?? (_cmd_DeleteOENameCmd = new MyRelayCommand(() =>
                  {
                      if (OE_SelectedName != null)
                      {
                          if (_myOE.Delete_OEName(OE_SelectedName.OE_Name))
                          {
                              List_OENames = _myOE.Get_List_OENames();
                              List_OENumbers = _myOE.Get_List_OENumbers();
                              List_OELinks = _myOE.Get_List_OERelation();
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
        public MyRelayCommand Cmd_DeleteOELinkCmd
        {
            get => _cmd_DeleteOELinkCmd
                  ?? (_cmd_DeleteOELinkCmd = new MyRelayCommand(() =>
                  {
                      if (OE_SelectedLink != null)
                      {
                          if (_myOE.Delete_OERelation(OE_SelectedLink.OE_Name, OE_SelectedLink.OE_Nummer))
                          {
                              List_OENames = _myOE.Get_List_OENames();
                              List_OENumbers = _myOE.Get_List_OENumbers();
                              List_OELinks = _myOE.Get_List_OERelation();
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
        public MyRelayCommand Cmd_DeleteOENumberCmd
        {
            get => _cmd_DeleteOENumberCmd
                  ?? (_cmd_DeleteOENumberCmd = new MyRelayCommand(() =>
                  {
                      if (OE_SelectedNumber != null)
                      {
                          if (_myOE.Delete_OENumber(OE_SelectedNumber.OE_Nummer))
                          {
                              List_OENames = _myOE.Get_List_OENames();
                              List_OENumbers = _myOE.Get_List_OENumbers();
                              List_OELinks = _myOE.Get_List_OERelation();
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
        public Visibility Vis_NameBorder
        {
            get => _vis_NameBorder;
            set => Set(() => Vis_NameBorder, ref _vis_NameBorder, value);
        }
        /// <summary>
        /// Sichtbarkeit des OE-Gruppen Bearbeitungsfensters
        /// </summary>
        public Visibility Vis_NameEditBorder
        {
            get => _vis_NameEditBorder;
            set => Set(() => Vis_NameEditBorder, ref _vis_NameEditBorder, value);
        }
        /// <summary>
        /// Sichtbarkeit des OE-Gruppen Erstellungsfensters
        /// </summary>
        public Visibility Vis_NameNewBorder
        {
            get => _vis_NameNewBorder;
            set => Set(() => Vis_NameNewBorder, ref _vis_NameNewBorder, value);
        }
        /// <summary>
        /// Sichtbarkeit des OE-Relation Startfensters
        /// </summary>
        public Visibility Vis_LinkBorder
        {
            get => _vis_LinkBorder;
            set => Set(() => Vis_LinkBorder, ref _vis_LinkBorder, value);
        }
        /// <summary>
        /// Sichtbarkeit des OE Relation Erstellungsfensters
        /// </summary>
        public Visibility Vis_LinkNewBorder
        {
            get => _vis_LinkNewBorder;
            set => Set(() => Vis_LinkNewBorder, ref _vis_LinkNewBorder, value);
        }
        /// <summary>
        /// Sichtbarkeit des OE-Nummern Startfensters
        /// </summary>
        public Visibility Vis_NumberBorder
        {
            get => _vis_NumberBorder;
            set => Set(() => Vis_NumberBorder, ref _vis_NumberBorder, value);
        }
        /// <summary>
        /// Sichtbarkeit des OE-Nummern Bearbeitungsfensters
        /// </summary>
        public Visibility Vis_NumberEditBorder
        {
            get => _vis_NumberEditBorder;
            set => Set(() => Vis_NumberEditBorder, ref _vis_NumberEditBorder, value);
        }
        /// <summary>
        /// Sichtbarkeit des OE-Nummern Erstellungsfensters
        /// </summary>
        public Visibility Vis_NumberNewBorder
        {
            get => _vis_NumberNewBorder;
            set => Set(() => Vis_NumberNewBorder, ref _vis_NumberNewBorder, value);
        }
        #endregion

        /// <summary>
        /// Command zum Zurückkehren zum vorherigen VM
        /// </summary>
        public MyRelayCommand Cmd_NavBack
        {
            get => new MyRelayCommand(() =>
                {
                    Cleanup();
                    _myNavi.NavigateBack();
                    _myLock.Unlock_Object(Table_Lock_Flags.OEs, 0);
                });
        }

        #region Services
        private readonly INavigationService _myNavi;
        private readonly IDialogService _myDia;
        private readonly IDataService_OE _myOE;
        private readonly ILockService _myLock;
        #endregion

        /// <summary>
        /// Viewmodel für die OE-Verwaltung. Hier werden OE-Gruppen angelegt, denen beliebig viele OE-Nummer zugeordnet werden können
        /// OE-Gruppen sind jene, die einem Prozess als "Verantwortliche OE" zugewiesen werden.
        /// Zur Steuerung der angezeigten Prozesse für einen user müssen somit den Gruppen die korrekten Nummern zugeordnet werden.
        /// </summary>
        /// <param name="myDia"></param>
        /// <param name="myNavi"></param>
        /// <param name="myOE"></param>
        public OE_ViewModel(IDialogService myDia, INavigationService myNavi, 
            IDataService_OE myOE, ILockService myLock)
        {
            _myDia = myDia;
            _myLock = myLock;
            _myNavi = myNavi;
            _myOE = myOE;

            List_OENames = _myOE.Get_List_OENames();
            List_OENumbers = _myOE.Get_List_OENumbers();
            List_OELinks = _myOE.Get_List_OERelation();
            Vis_NameBorder = Visibility.Visible;
            Vis_NameNewBorder = Visibility.Collapsed;
            Vis_NameEditBorder = Visibility.Collapsed;

            Vis_LinkBorder = Visibility.Visible;
            Vis_LinkNewBorder = Visibility.Collapsed;

            Vis_NumberBorder = Visibility.Visible;
            Vis_NumberNewBorder = Visibility.Collapsed;
            Vis_NumberEditBorder = Visibility.Collapsed;
        }

        /// <summary>
        /// Bricht die Bearbeitung ab
        /// </summary>
        public void CancelEdit()
        {
            IsFree = true;
            Vis_NameBorder = Visibility.Visible;
            Vis_NameNewBorder = Visibility.Collapsed;
            Vis_NameEditBorder = Visibility.Collapsed;

            Vis_LinkBorder = Visibility.Visible;
            Vis_LinkNewBorder = Visibility.Collapsed;

            Vis_NumberBorder = Visibility.Visible;
            Vis_NumberNewBorder = Visibility.Collapsed;
            Vis_NumberEditBorder = Visibility.Collapsed;
        }

        /// <summary>
        /// Bereinigt das Viewmodel
        /// </summary>
        override public void Cleanup()
        {
            SimpleIoc.Default.Unregister(this);
            base.Cleanup();
        }
    }
}
