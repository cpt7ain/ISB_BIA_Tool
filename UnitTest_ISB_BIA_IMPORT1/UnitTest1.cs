using System;
using ISB_BIA_IMPORT1.Services;
using ISB_BIA_IMPORT1.ViewModel;
using ISB_BIA_IMPORT1.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using ISB_BIA_IMPORT1.LinqDataContext;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using Microsoft.Win32;

namespace UnitTest_ISB_BIA_IMPORT1
{
    [TestClass]
    public class UnitTest1
    {

        Mock<IMyDialogService> diaServiceMock = new Mock<IMyDialogService>();
        Mock<IMyDataService> dataServiceMock = new Mock<IMyDataService>();
        ObservableCollection<ViewModelBase> vmHisLoc = new ObservableCollection<ViewModelBase>();
        Mock<IMyNavigationService> naviServiceMock = new Mock<IMyNavigationService>();
        Mock<IMySharedResourceService> sharedServiceMock = new Mock<IMySharedResourceService>();
        Mock<IMyExportService> exportServiceMock = new Mock<IMyExportService>();
        Mock<IMyMailNotificationService> mailServiceMock = new Mock<IMyMailNotificationService>();

        private void DataServiceSetup()
        {
            MyDesignTimeDataService dsDataService = new MyDesignTimeDataService();
            ObservableCollection<ISB_BIA_Prozesse> resultProcesses = dsDataService.GetProcesses();
            ISB_BIA_Prozesse resultProcess = resultProcesses.FirstOrDefault();
            Process_Model resultPM = dsDataService.GetProcessModelFromDB(1);

            ObservableCollection<ISB_BIA_Applikationen> resultApplications = dsDataService.GetApplications();
            ISB_BIA_Applikationen resultApplication = resultApplications.FirstOrDefault();
            Application_Model resultAM = dsDataService.GetApplicationModelFromDB(1);

            ObservableCollection<ISB_BIA_Informationssegmente> resultSegments = dsDataService.GetAllSegments();
            ISB_BIA_Informationssegmente resultSegment = resultSegments.FirstOrDefault();
            InformationSegment_Model resultSM =  dsDataService.GetSegmentModelFromDB(1);

            ObservableCollection<ISB_BIA_Informationssegmente_Attribute> resultAttributes= dsDataService.GetAttributes();
            ISB_BIA_Informationssegmente_Attribute resultAttribute = resultAttributes.FirstOrDefault();
            InformationSegmentAttribute_Model resultAttM = dsDataService.GetAttributeModelFromDB(1);

            ISB_BIA_Settings resultSetting = dsDataService.GetSettings();
            Settings_Model resultSetM = dsDataService.GetSettingsModelFromDB();

            ObservableCollection<ISB_BIA_OEs> resultOEs = dsDataService.GetDummyOEs();
            ISB_BIA_OEs resultOE = resultOEs.FirstOrDefault();


            dataServiceMock.Setup(x => x.GetActiveApplications()).Returns(dsDataService.GetActiveApplications());
            dataServiceMock.Setup(x => x.GetActiveProcesses()).Returns(dsDataService.GetActiveProcesses());
            dataServiceMock.Setup(x => x.GetAllSegments()).Returns(dsDataService.GetAllSegments());
            dataServiceMock.Setup(x => x.GetApplicationCategories()).Returns(dsDataService.GetApplicationCategories());
            dataServiceMock.Setup(x => x.GetApplicationHistory(It.IsAny<int>())).Returns(dsDataService.GetApplicationHistory(1));
            dataServiceMock.Setup(x => x.GetApplicationModelFromDB(It.IsAny<int>())).Returns(dsDataService.GetApplicationModelFromDB(1));
            dataServiceMock.Setup(x => x.GetApplications(It.IsAny<DateTime>())).Returns(dsDataService.GetApplications(DateTime.Now));
            dataServiceMock.Setup(x => x.GetAttributeModelFromDB(It.IsAny<int>())).Returns(dsDataService.GetAttributeModelFromDB(1));
            dataServiceMock.Setup(x => x.GetAttributeNamesAndInfoForIS()).Returns(dsDataService.GetAttributeNamesAndInfoForIS());
            dataServiceMock.Setup(x => x.GetAttributeNamesForHeader()).Returns(dsDataService.GetAttributeNamesForHeader());
            dataServiceMock.Setup(x => x.GetAttributes()).Returns(dsDataService.GetAttributes());
            dataServiceMock.Setup(x => x.GetBetriebsart()).Returns(dsDataService.GetBetriebsart());
            dataServiceMock.Setup(x => x.GetDeltaAnalysis()).Returns(dsDataService.GetDeltaAnalysis());
            dataServiceMock.Setup(x => x.GetEnabledSegments()).Returns(dsDataService.GetEnabledSegments());
            dataServiceMock.Setup(x => x.GetISByISName(It.IsAny<string>())).Returns(dsDataService.GetISByISName("IS1"));
            dataServiceMock.Setup(x => x.GetISList()).Returns(dsDataService.GetISList());
            dataServiceMock.Setup(x => x.GetLog()).Returns(dsDataService.GetLog());
            dataServiceMock.Setup(x => x.GetObjectLocked(It.IsAny<Table_Lock_Flags>(), It.IsAny<int>())).Returns(dsDataService.GetObjectLocked(Table_Lock_Flags.Process, 1));
            dataServiceMock.Setup(x => x.GetOELinks()).Returns(dsDataService.GetOELinks());
            dataServiceMock.Setup(x => x.GetOENames()).Returns(dsDataService.GetOENames());
            dataServiceMock.Setup(x => x.GetOENumbers()).Returns(dsDataService.GetOENumbers());
            dataServiceMock.Setup(x => x.GetOEs()).Returns(dsDataService.GetOEs());
            dataServiceMock.Setup(x => x.GetOEsForUser(It.IsAny<string>())).Returns(dsDataService.GetOEsForUser("IT"));
            dataServiceMock.Setup(x => x.GetPostProcesses()).Returns(dsDataService.GetPostProcesses());
            dataServiceMock.Setup(x => x.GetPreProcesses()).Returns(dsDataService.GetPreProcesses());
            dataServiceMock.Setup(x => x.GetProcessApplicationHistoryForApplication(It.IsAny<int>())).Returns(dsDataService.GetProcessApplicationHistoryForApplication(1));
            dataServiceMock.Setup(x => x.GetProcessApplicationHistoryForProcess(It.IsAny<int>())).Returns(dsDataService.GetProcessApplicationHistoryForProcess(1));
            dataServiceMock.Setup(x => x.GetProcesses(It.IsAny<DateTime>())).Returns(resultProcesses);
            dataServiceMock.Setup(x => x.GetProcesses(null)).Returns(resultProcesses);
            dataServiceMock.Setup(x => x.GetProcessesByOE(It.IsAny<ObservableCollection<string>>())).Returns(resultProcesses);
            dataServiceMock.Setup(x => x.GetProcessHistory(It.IsAny<int>())).Returns(resultProcesses);
            dataServiceMock.Setup(x => x.GetProcessModelFromDB(It.IsAny<int>())).Returns(resultPM);
            dataServiceMock.Setup(x => x.GetProcessOwner()).Returns(dsDataService.GetProcessOwner());
            dataServiceMock.Setup(x => x.GetRechenzentrum()).Returns(dsDataService.GetRechenzentrum());
            dataServiceMock.Setup(x => x.GetSegmentModelFromDB(It.IsAny<int>())).Returns(dsDataService.GetSegmentModelFromDB(1));
            dataServiceMock.Setup(x => x.GetServer()).Returns(dsDataService.GetServer());
            dataServiceMock.Setup(x => x.GetSettings()).Returns(dsDataService.GetSettings());
            dataServiceMock.Setup(x => x.GetSettingsModelFromDB()).Returns(dsDataService.GetSettingsModelFromDB());
            dataServiceMock.Setup(x => x.GetTypes()).Returns(dsDataService.GetTypes());
            dataServiceMock.Setup(x => x.GetVirtuelle_Maschine()).Returns(dsDataService.GetVirtuelle_Maschine());
            dataServiceMock.Setup(x => x.LockObject(It.IsAny<Table_Lock_Flags>(), It.IsAny<int>())).Returns(dsDataService.LockObject(Table_Lock_Flags.Process, 1));
            dataServiceMock.Setup(x => x.MapApplicationModelToDB(It.IsAny<Application_Model>())).Returns(dsDataService.MapApplicationModelToDB(resultAM));
            dataServiceMock.Setup(x => x.MapAttributeModelToDB(It.IsAny<InformationSegmentAttribute_Model>())).Returns(dsDataService.MapAttributeModelToDB(resultAttM));
            dataServiceMock.Setup(x => x.MapProcessModelToDB(It.IsAny<Process_Model>())).Returns(dsDataService.MapProcessModelToDB(resultPM));
            dataServiceMock.Setup(x => x.MapSegmentModelToDB(It.IsAny<InformationSegment_Model>())).Returns(dsDataService.MapSegmentModelToDB(resultSM));
            dataServiceMock.Setup(x => x.MapSettingsModelToDB(It.IsAny<Settings_Model>())).Returns(dsDataService.MapSettingsModelToDB(resultSetM));
            dataServiceMock.Setup(x => x.UnlockAllObjects()).Returns(dsDataService.UnlockAllObjects());
            dataServiceMock.Setup(x => x.UnlockAllObjectsForUser()).Returns(dsDataService.UnlockAllObjectsForUser());
            dataServiceMock.Setup(x => x.UnlockObject(It.IsAny<Table_Lock_Flags>(), It.IsAny<int>())).Returns(dsDataService.UnlockObject(Table_Lock_Flags.Process, 1));

            dataServiceMock.Setup(x => x.DeleteProcess(It.IsAny<ISB_BIA_Prozesse>())).Returns(dsDataService.DeleteProcess(resultProcess));
            dataServiceMock.Setup(x => x.DeleteApplication(It.IsAny<ISB_BIA_Applikationen>())).Returns(dsDataService.DeleteApplication(resultApplication));
            dataServiceMock.Setup(x => x.DeleteOELink(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            dataServiceMock.Setup(x => x.DeleteOEName(It.IsAny<string>())).Returns(true);
            dataServiceMock.Setup(x => x.DeleteOENumber(It.IsAny<string>())).Returns(true);
            dataServiceMock.Setup(x => x.SaveAllApplications(It.IsAny<ObservableCollection<ISB_BIA_Applikationen>>())).Returns(dsDataService.SaveAllApplications(resultApplications));
            dataServiceMock.Setup(x => x.SaveAllProcesses(It.IsAny<ObservableCollection<ISB_BIA_Prozesse>>())).Returns(dsDataService.SaveAllProcesses(resultProcesses));
            dataServiceMock.Setup(x => x.InsertApplication(It.IsAny<Application_Model>(), It.IsAny<ProcAppMode>())).Returns(true);
            dataServiceMock.Setup(x => x.InsertProcessAndRelations(It.IsAny<Process_Model>(), It.IsAny<ProcAppMode>(), It.IsAny<ObservableCollection<ISB_BIA_Applikationen>>(), It.IsAny<ObservableCollection<ISB_BIA_Applikationen>>())).Returns(true);
            dataServiceMock.Setup(x => x.InsertIS(It.IsAny<InformationSegment_Model>(), It.IsAny<InformationSegment_Model>())).Returns(true);
            dataServiceMock.Setup(x => x.InsertISAtt(It.IsAny<ObservableCollection<InformationSegmentAttribute_Model>>())).Returns(true);
            dataServiceMock.Setup(x => x.InsertOELink(It.IsAny<ISB_BIA_OEs>(), It.IsAny<ISB_BIA_OEs>())).Returns(resultOE);
            dataServiceMock.Setup(x => x.InsertOEName(It.IsAny<string>())).Returns(dsDataService.GetDummyOEs().FirstOrDefault());
            dataServiceMock.Setup(x => x.InsertOENumber(It.IsAny<string>(), It.IsAny<ISB_BIA_OEs>())).Returns(resultOE);
            dataServiceMock.Setup(x => x.InsertSettings(It.IsAny<ISB_BIA_Settings>(), It.IsAny<ISB_BIA_Settings>())).Returns(true);
            dataServiceMock.Setup(x => x.CreateDataModel(It.IsAny<List<string>>(), It.IsAny<DataTable>(), It.IsAny<DataTable>(), It.IsAny<DataTable>(), It.IsAny<DataTable>(), It.IsAny<DataTable>())).Returns(true);


        }

        private void SharedServiceSetup()
        {
            MyDesignTimeSharedResourceService dsSharedService = new MyDesignTimeSharedResourceService();
            sharedServiceMock.Setup(x => x.Admin).Returns(dsSharedService.Admin);
            sharedServiceMock.Setup(x => x.ConstructionMode).Returns(dsSharedService.ConstructionMode);
            sharedServiceMock.Setup(x => x.Current_Environment).Returns(dsSharedService.Current_Environment);
            sharedServiceMock.Setup(x => x.User).Returns(dsSharedService.User);
            sharedServiceMock.Setup(x => x.ConnectionString).Returns(dsSharedService.ConnectionString);
            sharedServiceMock.Setup(x => x.InitialDirectory).Returns(dsSharedService.InitialDirectory);
            sharedServiceMock.Setup(x => x.Source).Returns(dsSharedService.Source);

        }

        private void NaviServiceSetup()
        {
            MyDesignTimeNavigationService myNaviService = new MyDesignTimeNavigationService();
            naviServiceMock.Setup(x => x.VMHistory).Returns(myNaviService.VMHistory);
            naviServiceMock.Setup(x => x.NavigateTo<Menu_ViewModel>()).Callback(() => { myNaviService.NavigateTo<Menu_ViewModel>(); });
            naviServiceMock.Setup(x => x.NavigateTo<Process_ViewModel>(It.IsAny<int>(), It.IsAny<ProcAppMode>())).Callback(() => { myNaviService.NavigateTo<Process_ViewModel>(1, ProcAppMode.Change); });
            naviServiceMock.Setup(x => x.NavigateBack(false)).Callback(() => { Console.WriteLine("called"); myNaviService.NavigateBack(); });
            naviServiceMock.Setup(x => x.NavigateBack(true)).Callback(() => { Console.WriteLine("called"); myNaviService.NavigateBack(); });

        }


        private void DiaServiceSetup()
        {
            MyDesignTimeDialogService myNaviService = new MyDesignTimeDialogService();
            diaServiceMock.Setup(x => x.CancelDecision()).Returns(true);
            diaServiceMock.Setup(x => x.Open(It.IsAny<OpenFileDialog>())).Returns(true);
            diaServiceMock.Setup(x => x.Save(It.IsAny<SaveFileDialog>())).Returns(true);
        }

        private void ExpServiceSetup()
        {
            exportServiceMock.Setup(x => x.AllActiveApplicationsExport()).Returns(true);
            exportServiceMock.Setup(x => x.AllActiveProcessesExport()).Returns(true);
            exportServiceMock.Setup(x => x.AllApplicationsExport()).Returns(true);
            exportServiceMock.Setup(x => x.ExportApplications(It.IsAny<ObservableCollection<ISB_BIA_Applikationen>>(), It.IsAny<string>(), It.IsAny<int>())).Returns(true);
            exportServiceMock.Setup(x => x.ExportDeltaAnalysis(It.IsAny<ObservableCollection<ISB_BIA_Delta_Analyse>>())).Returns(true);
            exportServiceMock.Setup(x => x.ExportLog(It.IsAny<ObservableCollection<ISB_BIA_Log>>())).Returns(true);
            exportServiceMock.Setup(x => x.ExportProcesses(It.IsAny<ObservableCollection<ISB_BIA_Prozesse>>(),0)).Returns(true);
        }


        [TestInitialize]
        public void TestInitialize()
        {
            Console.WriteLine("start Testing");
            ViewModelLocator vml = new ViewModelLocator();
            DataServiceSetup();
            SharedServiceSetup();
            NaviServiceSetup();
            DiaServiceSetup();
        }

        [TestMethod]
        public void Test_Main_ViewModel()
        {
            bool received = false;
            Messenger.Default.Register<ViewModelBase>(this, MessageToken.ChangeCurrentVM, s => { received = true; });

            Main_ViewModel mvm = new Main_ViewModel(diaServiceMock.Object, naviServiceMock.Object, sharedServiceMock.Object, dataServiceMock.Object);
            Assert.IsFalse(received);
            Assert.IsTrue(mvm.CurrentViewModel == null);
            Assert.IsTrue(naviServiceMock.Object.VMHistory.Count == 1);
            mvm.AdminSelectGroupCommand.Execute(null);
            //Assert that CurrentViewModel change message is received
            Assert.IsTrue(received);
            //Assert that CurrentViewModel was changed
            Assert.IsTrue(mvm.CurrentViewModel is TargetViewModel);
            //Assert that CurrentViewModel was added to Navigation History
            Assert.IsTrue(naviServiceMock.Object.VMHistory.FirstOrDefault() is TargetViewModel);
        }

        [TestMethod]
        public void Test_Menu_ViewModel()
        {
            Messenger.Default.Register<string>(this, "a", p => { Console.WriteLine("rec"); });

            Menu_ViewModel mvmenu = new Menu_ViewModel(diaServiceMock.Object, naviServiceMock.Object, exportServiceMock.Object, dataServiceMock.Object, sharedServiceMock.Object);
            mvmenu.ChangeTextSize.Execute(null);
            //Console.WriteLine(mvmenu.ProcessCount.ToString());
            mvmenu.NavToLastDelta.Execute(null);
            //Console.WriteLine(mvmenu.EditProcessCount.ToString());
            Messenger.Default.Send("a", "a");
            //mvmenu.e
        }

        [TestMethod]
        public void TestMethod3()
        {
            ProcessView_ViewModel mvmenu = new ProcessView_ViewModel(diaServiceMock.Object, naviServiceMock.Object, exportServiceMock.Object, dataServiceMock.Object, sharedServiceMock.Object, mailServiceMock.Object);
            //mvmenu.NavBack.Execute(null);
            //Console.WriteLine(mvmenu.EditProcessCount.ToString());
            //Messenger.Default.Send("a", "a");
            //mvmenu.e
        }

        [TestMethod]
        public void Test_Process_ViewModel_Change()
        {
            bool received = false;
            Messenger.Default.Register<ViewModelBase>(this, MessageToken.ChangeCurrentVM, s => { received = true; });
            ProcessView_ViewModel mvprocessview = new ProcessView_ViewModel(diaServiceMock.Object, naviServiceMock.Object, exportServiceMock.Object, dataServiceMock.Object, sharedServiceMock.Object, mailServiceMock.Object);
            SourceViewModel mvsource = new SourceViewModel();
            naviServiceMock.Object.VMHistory.Insert(0, mvprocessview);
            Assert.IsTrue(naviServiceMock.Object.VMHistory.Count == 2);

            //Process Count of Dummy Processes
            Assert.IsTrue(mvprocessview.ProcessList.Count == 20);
            //Set Change Mode
            mvprocessview.ProcessViewMode = ProcAppListMode.Change;
            Assert.IsTrue(mvprocessview.DeleteProc.CanExecute(null) == false);
            Assert.IsTrue(mvprocessview.NavToProcess.CanExecute(null) == true);
            Assert.AreEqual(mvprocessview.RowDoubleClick.GetHashCode(), mvprocessview.NavToProcess.GetHashCode());
            Assert.AreNotEqual(mvprocessview.RowDoubleClick.GetHashCode(), mvprocessview.DeleteProc.GetHashCode());

            Assert.IsTrue(mvprocessview.SelectedItem == null);
            mvprocessview.NavToProcess.Execute(null);
            Assert.IsFalse(received);
            Assert.IsTrue(naviServiceMock.Object.VMHistory.Count == 2);

            mvprocessview.SelectedItem = mvprocessview.ProcessList.FirstOrDefault();
            mvprocessview.NavToProcess.Execute(null);
            Assert.IsTrue(received);
            Assert.IsTrue(naviServiceMock.Object.VMHistory.Count == 3);
            Assert.IsTrue(naviServiceMock.Object.VMHistory.FirstOrDefault() is TargetViewModel);

            received = false;
            mvprocessview.NavBack.Execute(null);
            Assert.IsTrue(received);
            Assert.IsTrue(naviServiceMock.Object.VMHistory.Count == 2);
            Assert.IsTrue(naviServiceMock.Object.VMHistory.FirstOrDefault() is ProcessView_ViewModel);

            received = false;
            mvprocessview.NavBack.Execute(null);
            Assert.IsTrue(received);
            Assert.IsTrue(naviServiceMock.Object.VMHistory.Count == 1);
            Assert.IsTrue(naviServiceMock.Object.VMHistory.FirstOrDefault() is SourceViewModel);

            Assert.IsTrue(mvprocessview.ButtonVis == Visibility.Visible);
            Assert.AreEqual(mvprocessview.Instruction, "Doppelklick auf einen Prozess, den Sie ändern möchten, oder Prozesse anhaken, welche ohne Änderungen gespeichert werden sollen.");
            Assert.AreEqual(mvprocessview.Header, "Prozesse bearbeiten");
        }

        [TestMethod]
        public void Test_Process_ViewModel_Delete()
        {
            ProcessView_ViewModel mvprocessview = new ProcessView_ViewModel(diaServiceMock.Object, naviServiceMock.Object, exportServiceMock.Object, dataServiceMock.Object, sharedServiceMock.Object, mailServiceMock.Object);
            //Process Count of Dummy Processes
            Assert.IsTrue(mvprocessview.ProcessList.Count == 20);

            //Set Delete Mode
            mvprocessview.ProcessViewMode = ProcAppListMode.Delete;
            Assert.IsTrue(mvprocessview.SaveSelectedProcesses.CanExecute(null) == false);
            Assert.IsTrue(mvprocessview.DeleteProc.CanExecute(null) == true);
            Assert.IsTrue(mvprocessview.NavToProcess.CanExecute(null) == false);
            Assert.AreNotEqual(mvprocessview.RowDoubleClick.GetHashCode(), mvprocessview.NavToProcess.GetHashCode());
            Assert.AreEqual(mvprocessview.RowDoubleClick.GetHashCode(), mvprocessview.DeleteProc.GetHashCode());

            //Delete Process with SelectedItem = 0
            Assert.IsTrue(mvprocessview.SelectedItem == null);
            mvprocessview.DeleteProc.Execute(null);
            //Set SelectedItem for Deletion
            mvprocessview.SelectedItem = mvprocessview.ProcessList.FirstOrDefault();
            //Delete Process
            mvprocessview.DeleteProc.Execute(null);
            //Assert that following Methods were called once
            dataServiceMock.Verify(mock => mock.GetObjectLocked(It.IsAny<Table_Lock_Flags>(), It.IsAny<int>()), Times.Once);
            dataServiceMock.Verify(mock => mock.DeleteProcess(It.IsAny<ISB_BIA_Prozesse>()), Times.Once);

            mvprocessview.SelectedItem = new ISB_BIA_Prozesse() { Id = 1, Aktiv = 0 };
            mvprocessview.DeleteProc.Execute(null);
            dataServiceMock.Verify(mock => mock.GetObjectLocked(It.IsAny<Table_Lock_Flags>(), It.IsAny<int>()), Times.Exactly(2));
            dataServiceMock.Verify(mock => mock.DeleteProcess(It.IsAny<ISB_BIA_Prozesse>()), Times.Exactly(1));

            Assert.IsTrue(mvprocessview.ButtonVis == Visibility.Collapsed);
            Assert.AreEqual(mvprocessview.Instruction, "Doppelklick auf einen Prozess, den Sie löschen möchten");
            Assert.AreEqual(mvprocessview.Header, "Prozesse löschen");
        }

    }
}
