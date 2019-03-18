using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using ISB_BIA_IMPORT1.Model;
using ISB_BIA_IMPORT1.LINQ2SQL;
using ISB_BIA_IMPORT1.Services;
using System.Collections.ObjectModel;
using System.Windows;
using ISB_BIA_IMPORT1.Helpers;
using ISB_BIA_IMPORT1.Services.Interfaces;

namespace ISB_BIA_IMPORT1.ViewModel
{
    /// <summary>
    /// VM zur Darstellung der Informationssegment-Ansicht
    /// </summary>
    public class Segment_ViewModel : ViewModelBase
    {
        #region Backing-Fields
        private ObservableCollection<string> _list_AttributeName;
        private Segment_Model _segmentCurrent;
        private Segment_Model _segmentOld;
        private ISISAttributeMode _mode;
        #endregion

        /// <summary>
        ///  Attribut-Namensliste zur Darstellung, welche Attribute auf das aktuelle Segment zutreffen
        /// </summary>
        public ObservableCollection<string> List_AttributeName
        {
            get => _list_AttributeName;
            set => Set(() => List_AttributeName, ref _list_AttributeName, value);
        }

        /// <summary>
        /// Aktuelles Segment an dem Änderungen vorgenommen werden
        /// </summary>
        public Segment_Model SegmentCurrent
        {
            get => _segmentCurrent;
            set => Set(() => SegmentCurrent, ref _segmentCurrent, value);
        }

        /// <summary>
        /// Altes Segment zum aufspüren von Änderungen
        /// </summary>
        public Segment_Model SegmentOld
        {
            get => _segmentOld;
            set => Set(() => SegmentOld, ref _segmentOld, value);
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
        public Visibility Vis_Attribut9
        {
            get => (Settings.Attribut9_aktiviert == "Ja") ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Sichtbarkeit von Attribut 10
        /// </summary>
        public Visibility Vis_Attribut10
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
        public MyRelayCommand Cmd_Save
        {
            get => new MyRelayCommand(() =>
            {
                if (_myIS.Insert_Segment(SegmentCurrent, SegmentOld))
                {
                    Cleanup();
                    _myNavi.NavigateBack(true);
                    _myLock.Unlock_Object(Table_Lock_Flags.Segment, SegmentCurrent.Informationssegment_Id);
                }
            });
        }

        /// <summary>
        /// Command zum Zurückkehren zum vorherigen VM
        /// </summary>
        public MyRelayCommand Cmd_NavBack
        {
            get => new MyRelayCommand(() =>
            {
                if (EditMode == false)
                {
                    Cleanup();
                    _myNavi.NavigateBack();
                }
                else if (_myDia.CancelDecision())
                {
                    Cleanup();
                    _myNavi.NavigateBack();
                    _myLock.Unlock_Object(Table_Lock_Flags.Segment, SegmentCurrent.Informationssegment_Id);
                }
            });
        }

        #region Services
        private readonly INavigationService _myNavi;
        private readonly IDialogService _myDia;
        private readonly IDataService_Segment _myIS;
        private readonly ILockService _myLock;
        private readonly IDataService_Setting _mySett;

        #endregion

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="myDia"></param>
        /// <param name="myNavi"></param>
        /// <param name="myIS"></param>
        public Segment_ViewModel(IDialogService myDia, INavigationService myNavi, 
            IDataService_Setting mySett,IDataService_Segment myIS, ILockService myLock)
        {
            #region Services
            _myDia = myDia;
            _mySett = mySett;
            _myNavi = myNavi;
            _myIS = myIS;
            _myLock = myLock;
            #endregion
            if (IsInDesignMode)
            {
                SegmentCurrent = _myIS.Get_Model_FromDB(1);
                List_AttributeName = _myIS.Get_StringList_AttributeNamesAndInfo();
            }
            else
            {
                //Abrufen der Attributnamen
                List_AttributeName = _myIS.Get_StringList_AttributeNamesAndInfo();
                //Message Registrierung für Bearbeitungsmodus
                MessengerInstance.Register<NotificationMessage<int>>(this, ISISAttributeMode.Edit, idMessage =>
                {
                    if (!(idMessage.Sender is INavigationService)) return;
                    Mode = ISISAttributeMode.Edit;
                    SegmentCurrent = _myIS.Get_Model_FromDB(idMessage.Content);
                    SegmentOld = _myIS.Get_Model_FromDB(idMessage.Content);
                });
                //Message Registrierung für Ansichtssmodus
                MessengerInstance.Register<NotificationMessage<int>>(this, ISISAttributeMode.View, idMessage => 
                {
                    if (!(idMessage.Sender is INavigationService)) return;
                    Mode = ISISAttributeMode.View;
                    SegmentCurrent = _myIS.Get_Model_FromDB(idMessage.Content);
                });
                //Abrfen der Einstellungen
                Settings = _mySett.Get_List_Settings();
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
    }
}
