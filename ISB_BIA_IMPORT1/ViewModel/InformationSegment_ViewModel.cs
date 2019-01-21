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
    /// VM zur Darstellung der Informationssegment-Ansicht
    /// </summary>
    public class InformationSegment_ViewModel : ViewModelBase
    {
        #region Backing-Fields
        private ObservableCollection<string> _attributeNameList;
        private InformationSegment_Model _currentSegment;
        private InformationSegment_Model _oldSegment;
        private ISISAttributeMode _mode;
        #endregion

        /// <summary>
        ///  Attribut-Namensliste zur Darstellung, welche Attribute auf das aktuelle Segment zutreffen
        /// </summary>
        public ObservableCollection<string> AttributeNameList
        {
            get => _attributeNameList;
            set => Set(() => AttributeNameList, ref _attributeNameList, value);
        }

        /// <summary>
        /// Aktuelles Segment an dem Änderungen vorgenommen werden
        /// </summary>
        public InformationSegment_Model CurrentSegment
        {
            get => _currentSegment;
            set => Set(() => CurrentSegment, ref _currentSegment, value);
        }

        /// <summary>
        /// Altes Segment zum aufspüren von Änderungen
        /// </summary>
        public InformationSegment_Model OldSegment
        {
            get => _oldSegment;
            set => Set(() => OldSegment, ref _oldSegment, value);
        }

        #region Properties für Einstellungen bezüglich Bearbeitung durch Admin/CISO oder betrachten durch Normalen User
        /// <summary>
        /// Property für Einstellungen bezüglich Bearbeitung durch CISO oder betrachten durch andere User
        /// </summary>
        public ISISAttributeMode Mode
        {
            get => _mode;
            set
            {
                Set(()=>Mode, ref _mode, value);
                if(value == ISISAttributeMode.Edit)
                    EditMode = true;
                else
                    EditMode = false;
            }
        }
        /// <summary>
        /// Property für Einstellungen bezüglich Bearbeitung durch CISO oder betrachten durch andere User
        /// </summary>
        public bool EditMode { get; set; }
        #endregion

        /// <summary>
        /// Sichtbarkeit von Attribut 9
        /// </summary>
        public Visibility EnableAttribut9
        {
            get => (Settings.Attribut9_aktiviert == "Ja") ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Sichtbarkeit von Attribut 10
        /// </summary>
        public Visibility EnableAttribut10
        {
            get => (Settings.Attribut10_aktiviert == "Ja") ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Aktuelle Einstellungen
        /// </summary>
        public ISB_BIA_Settings Settings { get; set; }

        /// <summary>
        /// Command zum Speichern des Segments und zurückkehren zu Prozess oder Übersicht
        /// </summary>
        public MyRelayCommand Save
        {
            get => new MyRelayCommand(() =>
            {
                if (myData.InsertIS(CurrentSegment, OldSegment))
                {
                    Cleanup();
                    myNavi.NavigateBack(true);
                    myData.UnlockObject(Table_Lock_Flags.Segment, CurrentSegment.Informationssegment_Id);
                }
            });
        }

        /// <summary>
        /// Command zum Zurückkehren zum vorherigen VM
        /// </summary>
        public MyRelayCommand NavBack
        {
            get => new MyRelayCommand(() =>
            {
                if (EditMode == false)
                {
                    Cleanup();
                    myNavi.NavigateBack();
                }
                else if (myDia.CancelDecision())
                {
                    Cleanup();
                    myNavi.NavigateBack();
                    myData.UnlockObject(Table_Lock_Flags.Segment, CurrentSegment.Informationssegment_Id);
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
        public InformationSegment_ViewModel(IMyDialogService myDialogService, IMyNavigationService myNavigationService, IMyDataService myDataService, IMySharedResourceService mySharedResourceService)
        {
            #region Services
            myDia = myDialogService;
            myNavi = myNavigationService;
            myData = myDataService;
            myShared = mySharedResourceService;
            #endregion
            if (IsInDesignMode)
            {
                CurrentSegment = myData.GetSegmentModelFromDB(1);
                AttributeNameList = myData.GetAttributeNamesAndInfoForIS();
            }
            else
            {
                //Abrufen der Attributnamen
                AttributeNameList = myData.GetAttributeNamesAndInfoForIS();
                //Message Registrierung für Bearbeitungsmodus
                Messenger.Default.Register<int>(this, ISISAttributeMode.Edit, id => {
                    Mode = ISISAttributeMode.Edit;
                    CurrentSegment = myData.GetSegmentModelFromDB(id);
                    OldSegment = myData.GetSegmentModelFromDB(id);
                });
                //Message Registrierung für Ansichtssmodus
                Messenger.Default.Register<int>(this, ISISAttributeMode.View, id => {
                    Mode = ISISAttributeMode.View;
                    CurrentSegment = myData.GetSegmentModelFromDB(id);
                });
                //Abrfen der Einstellungen
                Settings = myData.GetSettings();
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
    }
}
