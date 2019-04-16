/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:ISB_BIA_IMPORT1"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using ISB_BIA_IMPORT1.Services;
using ISB_BIA_IMPORT1.Services.Interfaces;
using ISB_BIA_IMPORT1.ViewModel;

namespace ISB_BIA_IMPORT1.Helpers
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            // Setzen des verwendeten IoC Conatiners (auf SimpleIoC) um Referenzen auf den verwendeten IoC Container zu vermeiden
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
          
            if (ViewModelBase.IsInDesignModeStatic)
            {
                // Create design time view services and models
                #region Services, per Dependency Injection injiziert
                SimpleIoc.Default.Register<IDialogService, DesignTimeDialogService>();
                SimpleIoc.Default.Register<ISharedResourceService, DesignTimeSharedResourceService>();
                SimpleIoc.Default.Register<INavigationService, DesignTimeNavigationService>();

                SimpleIoc.Default.Register<ILockService, DesignTimeDataService_Lock>();
                SimpleIoc.Default.Register<IDataService_Setting, DesignTimeDataService_Setting>();
                SimpleIoc.Default.Register<IDataModelService, DesignTimeDataService_DataModel>();
                SimpleIoc.Default.Register<IDataService_Log, DesignTimeDataService_Log>();
                SimpleIoc.Default.Register<IDataService_Segment, DesignTimeDataService_Segment>();
                SimpleIoc.Default.Register<IDataService_Attribute, DesignTimeDataService_Attribute>();
                SimpleIoc.Default.Register<IDataService_Process, DesignTimeDataService_Process>();
                SimpleIoc.Default.Register<IDataService_Application, DesignTimeDataService_Application>();
                SimpleIoc.Default.Register<IDataService_Delta, DesignTimeDataService_Delta>();
                SimpleIoc.Default.Register<IDataService_OE, DataService_OE>();

                SimpleIoc.Default.Register<IDataService_OE, DataService_OE>(); SimpleIoc.Default.Register<IExportService, DesignTimeExportService>();
                SimpleIoc.Default.Register<IMailNotificationService, DesignTimeMailNotificationService>();
                #endregion
            }
            else
            {
                // Create run time view services and models
                #region Services, per Dependency Injection injiziert
                SimpleIoc.Default.Register<IDialogService, DialogService>();
                SimpleIoc.Default.Register<ISharedResourceService, SharedResourceService>();
                SimpleIoc.Default.Register<INavigationService, NavigationService>();
              
                SimpleIoc.Default.Register<ILockService, LockService>();
                SimpleIoc.Default.Register<IDataService_Setting, DataService_Setting>();
                SimpleIoc.Default.Register<IDataModelService, DataModelService>();
                SimpleIoc.Default.Register<IDataService_Log, DataService_Log>();
                SimpleIoc.Default.Register<IDataService_Segment, DataService_Segment>();
                SimpleIoc.Default.Register<IDataService_Attribute, DataService_Attribute>();
                SimpleIoc.Default.Register<IDataService_Process, DataService_Process>();
                SimpleIoc.Default.Register<IDataService_Application, DataService_Application>();
                SimpleIoc.Default.Register<IDataService_Delta, DataService_Delta>();
                SimpleIoc.Default.Register<IDataService_OE, DataService_OE>();
                
                SimpleIoc.Default.Register<IExportService, ExportService>();
                SimpleIoc.Default.Register<IMailNotificationService, MailNotificationService>();
                #endregion
            }

            //Registrieren aller der Viewmodels
            SimpleIoc.Default.Register<Main_ViewModel>();
            SimpleIoc.Default.Register<Menu_ViewModel>();
            SimpleIoc.Default.Register<ProcessView_ViewModel>();
            SimpleIoc.Default.Register<Process_ViewModel>();
            SimpleIoc.Default.Register<ApplicationView_ViewModel>();
            SimpleIoc.Default.Register<Application_ViewModel>();
            SimpleIoc.Default.Register<SBA_View_ViewModel>();
            SimpleIoc.Default.Register<SegmentsView_ViewModel>();
            SimpleIoc.Default.Register<Segment_ViewModel>();
            SimpleIoc.Default.Register<Attributes_ViewModel>();
            SimpleIoc.Default.Register<OE_ViewModel>();
            SimpleIoc.Default.Register<Settings_ViewModel>();
            SimpleIoc.Default.Register<DataModel_ViewModel>();
            SimpleIoc.Default.Register<LogView_ViewModel>();
            SimpleIoc.Default.Register<DeltaAnalysis_ViewModel>();
            SimpleIoc.Default.Register<DocumentView_ViewModel>();
        }

        /// <summary>
        /// Auflösung und Rückgabe der Instanz des MainViewModels durch den definierten IoC Container (hier SimpleIoc)
        /// </summary>
        public Main_ViewModel Main
        {
            get => ServiceLocator.Current.GetInstance<Main_ViewModel>();
        }
        /// <summary>
        /// Auflösung und Rückgabe der Instanz des MenuViewModels durch den definierten IoC Container (hier SimpleIoc)
        /// </summary>
        public Menu_ViewModel Menu
        {
            get => ServiceLocator.Current.GetInstance<Menu_ViewModel>();
        }
        /// <summary>
        /// Auflösung und Rückgabe der Instanz des MainViewModels durch den definierten IoC Container (hier SimpleIoc)
        /// </summary>
        public ProcessView_ViewModel ProcessView
        {
            get => ServiceLocator.Current.GetInstance<ProcessView_ViewModel>();
        }
        /// <summary>
        /// Auflösung und Rückgabe der Instanz des ProcessViewVM durch den definierten IoC Container (hier SimpleIoc)
        /// </summary>
        public Process_ViewModel Process
        {
            get => ServiceLocator.Current.GetInstance<Process_ViewModel>();
        }
        /// <summary>
        /// Auflösung und Rückgabe der Instanz des MainViewModels durch den definierten IoC Container (hier SimpleIoc)
        /// </summary>
        public ApplicationView_ViewModel ApplicationView
        {
            get => ServiceLocator.Current.GetInstance<ApplicationView_ViewModel>();
        }
        /// <summary>
        /// Auflösung und Rückgabe der Instanz des MainViewModels durch den definierten IoC Container (hier SimpleIoc)
        /// </summary>
        public Application_ViewModel Application
        {
            get => ServiceLocator.Current.GetInstance<Application_ViewModel>();
        }
        /// <summary>
        /// Auflösung und Rückgabe der Instanz des MainViewModels durch den definierten IoC Container (hier SimpleIoc)
        /// </summary>
        public SBA_View_ViewModel SBA_View
        {
            get => ServiceLocator.Current.GetInstance<SBA_View_ViewModel>();
        }
        /// <summary>
        /// Auflösung und Rückgabe der Instanz des MainViewModels durch den definierten IoC Container (hier SimpleIoc)
        /// </summary>
        public SegmentsView_ViewModel SegmentsView
        {
            get => ServiceLocator.Current.GetInstance<SegmentsView_ViewModel>();
        }
        /// <summary>
        /// Auflösung und Rückgabe der Instanz des MainViewModels durch den definierten IoC Container (hier SimpleIoc)
        /// </summary>
        public Segment_ViewModel Segment
        {
            get => ServiceLocator.Current.GetInstance<Segment_ViewModel>();
        }
        /// <summary>
        /// Auflösung und Rückgabe der Instanz des MainViewModels durch den definierten IoC Container (hier SimpleIoc)
        /// </summary>
        public Attributes_ViewModel Attributes
        {
            get => ServiceLocator.Current.GetInstance<Attributes_ViewModel>();
        }
        /// <summary>
        /// Auflösung und Rückgabe der Instanz des MainViewModels durch den definierten IoC Container (hier SimpleIoc)
        /// </summary>
        public OE_ViewModel OE
        {
            get => ServiceLocator.Current.GetInstance<OE_ViewModel>();
        }
        /// <summary>
        /// Auflösung und Rückgabe der Instanz des MainViewModels durch den definierten IoC Container (hier SimpleIoc)
        /// </summary>
        public Settings_ViewModel Settings
        {
            get => ServiceLocator.Current.GetInstance<Settings_ViewModel>();
        }
        /// <summary>
        /// Auflösung und Rückgabe der Instanz des MainViewModels durch den definierten IoC Container (hier SimpleIoc)
        /// </summary>
        public DataModel_ViewModel DataModel
        {
            get => ServiceLocator.Current.GetInstance<DataModel_ViewModel>();
        }
        /// <summary>
        /// Auflösung und Rückgabe der Instanz des MainViewModels durch den definierten IoC Container (hier SimpleIoc)
        /// </summary>
        public LogView_ViewModel LogView
        {
            get => ServiceLocator.Current.GetInstance<LogView_ViewModel>();
        }
        /// <summary>
        /// Auflösung und Rückgabe der Instanz des MainViewModels durch den definierten IoC Container (hier SimpleIoc)
        /// </summary>
        public DeltaAnalysis_ViewModel Delta
        {
            get => ServiceLocator.Current.GetInstance<DeltaAnalysis_ViewModel>();
        }
        /// <summary>
        /// Auflösung und Rückgabe der Instanz des MainViewModels durch den definierten IoC Container (hier SimpleIoc)
        /// </summary>
        public DocumentView_ViewModel DocumentView
        {
            get => ServiceLocator.Current.GetInstance<DocumentView_ViewModel>();
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}