using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using ISB_BIA_IMPORT1.Model;
using ISB_BIA_IMPORT1.LinqDataContext;
using ISB_BIA_IMPORT1.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace ISB_BIA_IMPORT1.ViewModel
{
    /// <summary>
    /// VM zur Darstellung der Informationssegment-Attribut-Übersicht
    /// </summary>
    public class InformationSegmentsAttributes_ViewModel : ViewModelBase
    {
        #region Backing Fields
        private ObservableCollection<InformationSegmentAttribute_Model> _currentAttList;
        private ISISAttributeMode _iSAttMode;
        private Visibility _newGoalsActivated;
        #endregion

        /// <summary>
        /// Aktuelle Liste der Attribute, welche angezeigt und bearbeitet werden kann
        /// </summary>
        public ObservableCollection<InformationSegmentAttribute_Model> CurrentAttList
        {
            get => _currentAttList;
            set => Set(() => CurrentAttList, ref _currentAttList, value);
        }

        #region Properties für Einstellungen bezüglich Bearbeitung durch Admin/CISO oder betrachten durch Normalen User
        /// <summary>
        /// Property für Einstellungen bezüglich Bearbeitung durch CISO oder betrachten durch andere User
        /// </summary>
        public ISISAttributeMode ISAttMode
        {
            get => _iSAttMode;
            set
            {
                Set(() => ISAttMode, ref _iSAttMode, value);
                if (value == ISISAttributeMode.Edit)
                {
                    EditMode = true;
                }
                else
                {
                    EditMode = false;
                }
            }
        }
        /// <summary>
        /// Property für Einstellungen bezüglich Bearbeitung durch CISO oder betrachten durch andere User
        /// </summary>
        public bool EditMode { get; set; }
        #endregion

        /// <summary>
        /// Command zum Zurückkehren zum vorherigen VM
        /// </summary>
        public MyRelayCommand NavBack
        {
            get => new MyRelayCommand(() =>
                {
            if (ISAttMode == ISISAttributeMode.View)
            {
                Cleanup();
                myNavi.NavigateBack();
            }
            else if (myDia.CancelDecision())
            {
                Cleanup();
                myNavi.NavigateBack();
                myData.UnlockObject(Table_Lock_Flags.Attributes, 0);
            }
        });
        }

        /// <summary>
        /// Sichtbarkeit der neuen Schutzziele
        /// </summary>
        public Visibility NewGoalsActivated
        {
            get => _newGoalsActivated;
            set => Set(()=>NewGoalsActivated, ref _newGoalsActivated, value);
        }

        /// <summary>
        /// Aktuelle Einstellungen
        /// </summary>
        public ISB_BIA_Settings Setting { get; set; }

        /// <summary>
        /// Speichern der Attributliste, entsperren der Liste und zuruückkehren zum vorherigen VM
        /// </summary>
        public MyRelayCommand Save
        {
            get => new MyRelayCommand(() =>
            {
                if (myData.InsertISAtt(CurrentAttList))
                {
                    Cleanup();
                    myNavi.NavigateBack();
                    myData.UnlockObject(Table_Lock_Flags.Attributes, 0);
                }
            });
        }

        #region Services
        IMyNavigationService myNavi;
        IMyDialogService myDia;
        IMyDataService myData;
        IMySharedResourceService myShared;
        #endregion

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="myDialogService"></param>
        /// <param name="myNavigationService"></param>
        /// <param name="myDataService"></param>
        /// <param name="mySharedResourceService"></param>
        public InformationSegmentsAttributes_ViewModel(IMyDialogService myDialogService, IMyNavigationService myNavigationService, IMyDataService myDataService, IMySharedResourceService mySharedResourceService)
        {
            #region Services
            myDia = myDialogService;
            myNavi = myNavigationService;
            myData = myDataService;
            myShared = mySharedResourceService;
            #endregion
            //Messenger Registrierung für Bestimmung des Ansichtsmodus
            Messenger.Default.Register<ISISAttributeMode>(this, a => {
                ISAttMode = a;
            });
            //Messenger Registrierung für Benachrichtigungen bei Fehlerhafter Eingabe
            Messenger.Default.Register<string>(this, MessageToken.ISAttributValidationError, s=> { myDia.ShowWarning("Bitte gültigen Wert eingeben"); });

            #region Aktuelle Einstellungen abrufen
            Setting = myData.GetSettings();
            NewGoalsActivated = (Setting.Neue_Schutzziele_aktiviert == "Ja") ? Visibility.Visible : Visibility.Collapsed;
            #endregion
            //Abrufen der Aktuellen Attributliste
            CurrentAttList = GetAttributeList();
        }

        /// <summary>
        /// Liste der 8-10 Attribute (Abhängig van Einstellungen für Attribut 9/10) abrufen (Mapping von DB zu Model im Model Konstruktor inklusive)
        /// </summary>
        /// <returns></returns>
        private ObservableCollection<InformationSegmentAttribute_Model> GetAttributeList()
        {
            ObservableCollection<InformationSegmentAttribute_Model> result = new ObservableCollection<InformationSegmentAttribute_Model>();
            //Die ersten 8 Attribute abrufen
            for (int i=1; i <= 8; i++)
            {
                InformationSegmentAttribute_Model item = myData.GetAttributeModelFromDB(i);
                if (item == null)
                {
                    myDia.ShowError("Fehler beim Laden der Daten.");
                    Cleanup();
                    myNavi.NavigateBack();
                }
                result.Add(item);
            }
            //Attribut 9
            if (Setting.Attribut9_aktiviert=="Ja")
            {
                InformationSegmentAttribute_Model item = myData.GetAttributeModelFromDB(9);
                if (item == null)
                {
                    myDia.ShowError("Fehler beim Laden der Daten.");
                    Cleanup();
                    myNavi.NavigateBack();
                }
                result.Add(item);
            }
            //Attribut 10
            if (Setting.Attribut10_aktiviert == "Ja")
            {
                InformationSegmentAttribute_Model item = myData.GetAttributeModelFromDB(10);
                if (item == null)
                {
                    myDia.ShowError("Fehler beim Laden der Daten.");
                    Cleanup();
                    myNavi.NavigateBack();
                }
                result.Add(item);
            }
            return result;
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
