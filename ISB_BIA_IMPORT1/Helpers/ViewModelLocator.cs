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
                SimpleIoc.Default.Register<IMyDialogService, MyDesignTimeDialogService>();
                SimpleIoc.Default.Register<IMySharedResourceService, MyDesignTimeSharedResourceService>();
                SimpleIoc.Default.Register<IMyNavigationService, MyDesignTimeNavigationService>();

                SimpleIoc.Default.Register<IMyDataService_Lock, MyDesignTimeDataService_Lock>();
                SimpleIoc.Default.Register<IMyDataService_Setting, MyDesignTimeDataService_Setting>();
                SimpleIoc.Default.Register<IMyDataService_DataModel, MyDesignTimeDataService_DataModel>();
                SimpleIoc.Default.Register<IMyDataService_Log, MyDesignTimeDataService_Log>();
                SimpleIoc.Default.Register<IMyDataService_IS_Attribute, MyDesignTimeDataService_IS_Attribute>();
                SimpleIoc.Default.Register<IMyDataService_Process, MyDesignTimeDataService_Process>();
                SimpleIoc.Default.Register<IMyDataService_Application, MyDesignTimeDataService_Application>();
                SimpleIoc.Default.Register<IMyDataService_Delta, MyDesignTimeDataService_Delta>();
                SimpleIoc.Default.Register<IMyDataService_OE, MyDataService_OE>();

                SimpleIoc.Default.Register<IMyDataService_OE, MyDataService_OE>(); SimpleIoc.Default.Register<IMyExportService, MyDesignTimeExportService>();
                SimpleIoc.Default.Register<IMyMailNotificationService, MyDesignTimeMailNotificationService>();
                #endregion
            }
            else
            {
                // Create run time view services and models
                #region Services, per Dependency Injection injiziert
                SimpleIoc.Default.Register<IMyDialogService, MyDialogService>();
                SimpleIoc.Default.Register<IMySharedResourceService, MySharedResourceService>();
                SimpleIoc.Default.Register<IMyNavigationService, MyNavigationService>();
              
                SimpleIoc.Default.Register<IMyDataService_Lock, MyDataService_Lock>();
                SimpleIoc.Default.Register<IMyDataService_Setting, MyDataService_Setting>();
                SimpleIoc.Default.Register<IMyDataService_DataModel, MyDataService_DataModel>();
                SimpleIoc.Default.Register<IMyDataService_Log, MyDataService_Log>();
                SimpleIoc.Default.Register<IMyDataService_IS_Attribute, MyDataService_IS_Attribute>();
                SimpleIoc.Default.Register<IMyDataService_Process, MyDataService_Process>();
                SimpleIoc.Default.Register<IMyDataService_Application, MyDataService_Application>();
                SimpleIoc.Default.Register<IMyDataService_Delta, MyDataService_Delta>();
                SimpleIoc.Default.Register<IMyDataService_OE, MyDataService_OE>();
                
                SimpleIoc.Default.Register<IMyExportService, MyExportService>();
                SimpleIoc.Default.Register<IMyMailNotificationService, MyMailNotificationService>();
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
            SimpleIoc.Default.Register<InformationSegmentsView_ViewModel>();
            SimpleIoc.Default.Register<InformationSegment_ViewModel>();
            SimpleIoc.Default.Register<InformationSegmentsAttributes_ViewModel>();
            SimpleIoc.Default.Register<OE_AssignmentView_ViewModel>();
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
        public InformationSegmentsView_ViewModel InformationSegmentsView
        {
            get => ServiceLocator.Current.GetInstance<InformationSegmentsView_ViewModel>();
        }
        /// <summary>
        /// Auflösung und Rückgabe der Instanz des MainViewModels durch den definierten IoC Container (hier SimpleIoc)
        /// </summary>
        public InformationSegment_ViewModel InformationSegment
        {
            get => ServiceLocator.Current.GetInstance<InformationSegment_ViewModel>();
        }
        /// <summary>
        /// Auflösung und Rückgabe der Instanz des MainViewModels durch den definierten IoC Container (hier SimpleIoc)
        /// </summary>
        public InformationSegmentsAttributes_ViewModel InformationSegmentsAttributes
        {
            get => ServiceLocator.Current.GetInstance<InformationSegmentsAttributes_ViewModel>();
        }
        /// <summary>
        /// Auflösung und Rückgabe der Instanz des MainViewModels durch den definierten IoC Container (hier SimpleIoc)
        /// </summary>
        public OE_AssignmentView_ViewModel OE_AssignmentView
        {
            get => ServiceLocator.Current.GetInstance<OE_AssignmentView_ViewModel>();
        }
        /// <summary>
        /// Auflösung und Rückgabe der Instanz des MainViewModels durch den definierten IoC Container (hier SimpleIoc)
        /// </summary>
        public Settings_ViewModel SettingsView
        {
            get => ServiceLocator.Current.GetInstance<Settings_ViewModel>();
        }
        /// <summary>
        /// Auflösung und Rückgabe der Instanz des MainViewModels durch den definierten IoC Container (hier SimpleIoc)
        /// </summary>
        public DataModel_ViewModel DataModelView
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
        public DeltaAnalysis_ViewModel DeltaView
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