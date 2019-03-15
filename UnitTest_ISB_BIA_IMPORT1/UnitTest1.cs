using System;
using ISB_BIA_IMPORT1.Services;
using ISB_BIA_IMPORT1.ViewModel;
using ISB_BIA_IMPORT1.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using ISB_BIA_IMPORT1.LINQ2SQL;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using ISB_BIA_IMPORT1.Helpers;
using ISB_BIA_IMPORT1.Services.Interfaces;
using Microsoft.Win32;
using GalaSoft.MvvmLight.Ioc;

namespace UnitTest_ISB_BIA_IMPORT1
{
    /*
    [TestClass]
    public class UnitTest1
    {
        SimpleIoc a = new SimpleIoc();

        Mock<IMyDialogService> diaServiceMock = new Mock<IMyDialogService>();
        Mock<IMyDataService_Lock> myLockMock = new Mock<IMyDataService_Lock>();
        Mock<IMyDataService_Application> myAppMock = new Mock<IMyDataService_Application>();
        Mock<IMyDataService_DataModel> myDMMock = new Mock<IMyDataService_DataModel>();
        Mock<IMyDataService_Delta> myDeltaMock = new Mock<IMyDataService_Delta>();
        Mock<IMyDataService_IS_Attribute> myISMock = new Mock<IMyDataService_IS_Attribute>();
        Mock<IMyDataService_Log> myLogMock = new Mock<IMyDataService_Log>();
        Mock<IMyDataService_OE> myOEMock = new Mock<IMyDataService_OE>();
        Mock<IMyDataService_Process> myProcMock = new Mock<IMyDataService_Process>();
        Mock<IMyDataService_Setting> mySettMock = new Mock<IMyDataService_Setting>();

        ObservableCollection<ViewModelBase> vmHisLoc = new ObservableCollection<ViewModelBase>();
        Mock<IMyNavigationService> naviServiceMock = new Mock<IMyNavigationService>();
        Mock<IMySharedResourceService> sharedServiceMock = new Mock<IMySharedResourceService>();
        Mock<IMyExportService> exportServiceMock = new Mock<IMyExportService>();
        Mock<IMyMailNotificationService> mailServiceMock = new Mock<IMyMailNotificationService>();

        private void DataServiceSetup()
        {
            a.Register<MyDesignTimeDataService_IS_Attribute>();
            a.Register<MyDesignTimeDataService_Lock>();
            a.Register<MyDesignTimeDataService_Log>();
            a.Register<MyDesignTimeDataService_OE>();
            a.Register<MyDesignTimeDataService_Application>();
            a.Register<MyDesignTimeDataService_Setting>();
            a.Register<MyDesignTimeDataService_Delta>();
            a.Register<MyDesignTimeDataService_Process>();
            a.Register<MyDesignTimeDataService_DataModel>();


            myAppMock.Setup(x => x.Get_ActiveApplications()).Returns(a.GetInstance<MyDesignTimeDataService_Application>().Get_ActiveApplications());
            myProcMock.Setup(x => x.Get_ActiveProcesses()).Returns(a.GetInstance<MyDesignTimeDataService_Process>().Get_ActiveProcesses());
            myISMock.Setup(x => x.Get_AllSegments()).Returns(a.GetInstance<MyDesignTimeDataService_IS_Attribute>().Get_AllSegments());
            myProcMock.Setup(x => x.Get_ListApplicationCategories()).Returns(a.GetInstance<MyDesignTimeDataService_Process>().Get_ListApplicationCategories());
            myAppMock.Setup(x => x.Get_ApplicationHistory(It.IsAny<int>())).Returns(a.GetInstance<MyDesignTimeDataService_Application>().Get_ApplicationHistory(1));
            myAppMock.Setup(x => x.Get_ApplicationModelFromDB(It.IsAny<int>())).Returns(a.GetInstance<MyDesignTimeDataService_Application>().Get_ApplicationModelFromDB(1));
            myAppMock.Setup(x => x.Get_AllApplications(It.IsAny<DateTime>())).Returns(a.GetInstance<MyDesignTimeDataService_Application>().Get_AllApplications(DateTime.Now));
            myISMock.Setup(x => x.Get_AttributeModelFromDB(It.IsAny<int>())).Returns(a.GetInstance<MyDesignTimeDataService_IS_Attribute>().Get_AttributeModelFromDB(1));
            myISMock.Setup(x => x.Get_AttributeNamesAndInfoForIS()).Returns(a.GetInstance<MyDesignTimeDataService_IS_Attribute>().Get_AttributeNamesAndInfoForIS());
            myISMock.Setup(x => x.Get_AttributeNamesForHeader()).Returns(a.GetInstance<MyDesignTimeDataService_IS_Attribute>().Get_AttributeNamesForHeader());
            myISMock.Setup(x => x.Get_Attributes()).Returns(a.GetInstance<MyDesignTimeDataService_IS_Attribute>().Get_Attributes());
            myAppMock.Setup(x => x.Get_ListBetriebsart()).Returns(a.GetInstance<MyDesignTimeDataService_Application>().Get_ListBetriebsart());
            myDeltaMock.Setup(x => x.Get_DeltaAnalysis()).Returns(a.GetInstance<MyDesignTimeDataService_Delta>().Get_DeltaAnalysis());
            myISMock.Setup(x => x.Get_EnabledSegments()).Returns(a.GetInstance<MyDesignTimeDataService_IS_Attribute>().Get_EnabledSegments());
            myProcMock.Setup(x => x.Get_ISByISName(It.IsAny<string>())).Returns(a.GetInstance<MyDesignTimeDataService_Process>().Get_ISByISName("IS1"));
            myProcMock.Setup(x => x.Get_ISDropDownList()).Returns(a.GetInstance<MyDesignTimeDataService_Process>().Get_ISDropDownList());
            myLogMock.Setup(x => x.Get_Log()).Returns(a.GetInstance<MyDesignTimeDataService_Log>().Get_Log);
            myLockMock.Setup(x => x.Get_ObjectIsLocked(It.IsAny<Table_Lock_Flags>(), It.IsAny<int>())).Returns(a.GetInstance<MyDesignTimeDataService_Lock>().Get_ObjectIsLocked(Table_Lock_Flags.Process, 1));
            myOEMock.Setup(x => x.Get_ListOELinks()).Returns(a.GetInstance<MyDesignTimeDataService_OE>().Get_ListOELinks());
            myOEMock.Setup(x => x.Get_ListOENames()).Returns(a.GetInstance<MyDesignTimeDataService_OE>().Get_ListOENames());
            myOEMock.Setup(x => x.Get_ListOENumbers()).Returns(a.GetInstance<MyDesignTimeDataService_OE>().Get_ListOENumbers());
            myProcMock.Setup(x => x.Get_ListOEs()).Returns(a.GetInstance<MyDesignTimeDataService_Process>().Get_ListOEs());
            myProcMock.Setup(x => x.Get_ListOEsForUser(It.IsAny<string>())).Returns(a.GetInstance<MyDesignTimeDataService_Process>().Get_ListOEsForUser("IT"));
            myProcMock.Setup(x => x.Get_ListPostProcesses()).Returns(a.GetInstance<MyDesignTimeDataService_Process>().Get_ListPostProcesses());
            myProcMock.Setup(x => x.Get_ListPreProcesses()).Returns(a.GetInstance<MyDesignTimeDataService_Process>().Get_ListPreProcesses());
            myAppMock.Setup(x => x.Get_HistoryProcessApplicationForApplication(It.IsAny<int>())).Returns(a.GetInstance<MyDesignTimeDataService_Application>().Get_HistoryProcessApplicationForApplication(1));
            myProcMock.Setup(x => x.Get_HistoryProcessApplicationForProcess(It.IsAny<int>())).Returns(a.GetInstance<MyDesignTimeDataService_Process>().Get_HistoryProcessApplicationForProcess(1));
            myProcMock.Setup(x => x.Get_AllProcesses(It.IsAny<DateTime>())).Returns(a.GetInstance<MyDesignTimeDataService_Process>().Get_ActiveProcesses());
            myProcMock.Setup(x => x.Get_AllProcesses(null)).Returns(a.GetInstance<MyDesignTimeDataService_Process>().Get_ActiveProcesses());
            myProcMock.Setup(x => x.Get_ProcessesByOE(It.IsAny<ObservableCollection<string>>())).Returns(a.GetInstance<MyDesignTimeDataService_Process>().Get_ActiveProcesses());
            myProcMock.Setup(x => x.Get_ProcessHistory(It.IsAny<int>())).Returns(a.GetInstance<MyDesignTimeDataService_Process>().Get_ActiveProcesses());
            myProcMock.Setup(x => x.Get_ProcessModelFromDB(It.IsAny<int>())).Returns(a.GetInstance<MyDesignTimeDataService_Process>().Get_ProcessModelFromDB(1));
            myProcMock.Setup(x => x.Get_ListProcessOwner()).Returns(a.GetInstance<MyDesignTimeDataService_Process>().Get_ListProcessOwner());
            myAppMock.Setup(x => x.Get_ListRechenzentrum()).Returns(a.GetInstance<MyDesignTimeDataService_Application>().Get_ListRechenzentrum());
            myISMock.Setup(x => x.Get_SegmentModelFromDB(It.IsAny<int>())).Returns(a.GetInstance<MyDesignTimeDataService_IS_Attribute>().Get_SegmentModelFromDB(1));
            myAppMock.Setup(x => x.Get_ListServer()).Returns(a.GetInstance<MyDesignTimeDataService_Application>().Get_ListServer());
            mySettMock.Setup(x => x.Get_Settings()).Returns(a.GetInstance<MyDesignTimeDataService_Setting>().Get_Settings());
            mySettMock.Setup(x => x.Get_SettingsModelFromDB()).Returns(a.GetInstance<MyDesignTimeDataService_Setting>().Get_SettingsModelFromDB());
            myAppMock.Setup(x => x.Get_ListTypes()).Returns(a.GetInstance<MyDesignTimeDataService_Application>().Get_ListTypes());
            myAppMock.Setup(x => x.Get_ListVirtuelle_Maschine()).Returns(a.GetInstance<MyDesignTimeDataService_Application>().Get_ListVirtuelle_Maschine());
            myLockMock.Setup(x => x.Lock_Object(It.IsAny<Table_Lock_Flags>(), It.IsAny<int>())).Returns(a.GetInstance<MyDesignTimeDataService_Lock>().Lock_Object(Table_Lock_Flags.Process, 1));
            myAppMock.Setup(x => x.Map_ApplicationModelToDB(It.IsAny<Application_Model>())).Returns(a.GetInstance<MyDesignTimeDataService_Application>().Map_ApplicationModelToDB(a.GetInstance<MyDesignTimeDataService_Application>().Get_ApplicationModelFromDB(1)));
            myISMock.Setup(x => x.Map_AttributeModelToDB(It.IsAny<InformationSegmentAttribute_Model>())).Returns();
            myProcMock.Setup(x => x.Map_ProcessModelToDB(It.IsAny<Process_Model>())).Returns(a.GetInstance<MyDesignTimeDataService_Process>().Map_ProcessModelToDB(resultPM));
            myISMock.Setup(x => x.Map_SegmentModelToDB(It.IsAny<InformationSegment_Model>())).Returns(a.GetInstance<MyDesignTimeDataService_IS_Attribute>().Map_SegmentModelToDB(resultSM));
            mySettMock.Setup(x => x.Map_SettingsModelToDB(It.IsAny<Settings_Model>())).Returns(a.GetInstance<MyDesignTimeDataService_Setting>().Map_SettingsModelToDB(resultSetM));
            myLockMock.Setup(x => x.Unlock_AllObjects()).Returns(a.GetInstance<MyDesignTimeDataService_Lock>().Unlock_AllObjects());
            myLockMock.Setup(x => x.Unlock_AllObjectsForUserOnMachine()).Returns(a.GetInstance<MyDesignTimeDataService_Lock>().Unlock_AllObjectsForUserOnMachine());
            myLockMock.Setup(x => x.Unlock_Object(It.IsAny<Table_Lock_Flags>(), It.IsAny<int>())).Returns(a.GetInstance<MyDesignTimeDataService_Lock>().Unlock_Object(Table_Lock_Flags.Process, 1));

            myProcMock.Setup(x => x.Delete_Process(It.IsAny<ISB_BIA_Prozesse>())).Returns(a.GetInstance<MyDesignTimeDataService_Process>().Delete_Process(resultProcess));
            myAppMock.Setup(x => x.Delete_Application(It.IsAny<ISB_BIA_Applikationen>())).Returns(a.GetInstance<MyDesignTimeDataService_Application>().Delete_Application(resultApplication));
            myOEMock.Setup(x => x.Delete_OELink(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            myOEMock.Setup(x => x.Delete_OEName(It.IsAny<string>())).Returns(true);
            myOEMock.Setup(x => x.Delete_OENumber(It.IsAny<string>())).Returns(true);
            myAppMock.Setup(x => x.Insert_AllApplications(It.IsAny<ObservableCollection<ISB_BIA_Applikationen>>())).Returns(a.GetInstance<MyDesignTimeDataService_Application>().Insert_AllApplications(resultApplications));
            myProcMock.Setup(x => x.Insert_AllProcesses(It.IsAny<ObservableCollection<ISB_BIA_Prozesse>>())).Returns(a.GetInstance<MyDesignTimeDataService_Process>().Insert_AllProcesses(resultProcesses));
            myAppMock.Setup(x => x.Insert_Application(It.IsAny<Application_Model>(), It.IsAny<ProcAppMode>())).Returns(true);
            myProcMock.Setup(x => x.Insert_ProcessAndRelations(It.IsAny<Process_Model>(), It.IsAny<ProcAppMode>(), It.IsAny<ObservableCollection<ISB_BIA_Applikationen>>(), It.IsAny<ObservableCollection<ISB_BIA_Applikationen>>())).Returns(true);
            myISMock.Setup(x => x.Insert_Segment(It.IsAny<InformationSegment_Model>(), It.IsAny<InformationSegment_Model>())).Returns(true);
            myISMock.Setup(x => x.Insert_Attribute(It.IsAny<ObservableCollection<InformationSegmentAttribute_Model>>())).Returns(true);
            myOEMock.Setup(x => x.Insert_OELink(It.IsAny<ISB_BIA_OEs>(), It.IsAny<ISB_BIA_OEs>())).Returns(resultOE);
            myOEMock.Setup(x => x.Insert_NewOEName(It.IsAny<string>())).Returns(a.GetInstance<MyDesignTimeDataService_OE>().GetDummyOEs().FirstOrDefault());
            myOEMock.Setup(x => x.Insert_NewOENumber(It.IsAny<string>(), It.IsAny<ISB_BIA_OEs>())).Returns(resultOE);
            mySettMock.Setup(x => x.Insert_Settings(It.IsAny<ISB_BIA_Settings>(), It.IsAny<ISB_BIA_Settings>())).Returns(true);
            myDMMock.Setup(x => x.Create(It.IsAny<DataTable>(), It.IsAny<DataTable>(), It.IsAny<DataTable>(), It.IsAny<DataTable>(), It.IsAny<DataTable>())).Returns(true);


        }

        private void SharedServiceSetup()
        {
            MyDesignTimeSharedResourceService dsSharedService = new MyDesignTimeSharedResourceService();
            sharedServiceMock.Setup(x => x.Admin).Returns(dsSharedService.Admin);
            sharedServiceMock.Setup(x => x.ConstructionMode).Returns(dsSharedService.ConstructionMode);
            sharedServiceMock.Setup(x => x.CurrentEnvironment).Returns(dsSharedService.CurrentEnvironment);
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
            exportServiceMock.Setup(x => x.ExportActiveApplications()).Returns(true);
            exportServiceMock.Setup(x => x.ExportActiveProcesses()).Returns(true);
            exportServiceMock.Setup(x => x.ExportAllApplications()).Returns(true);
            exportServiceMock.Setup(x => x.ExportApplications(It.IsAny<ObservableCollection<ISB_BIA_Applikationen>>(), It.IsAny<string>(), It.IsAny<int>())).Returns(true);
            exportServiceMock.Setup(x => x.ExportDeltaAnalysis(It.IsAny<ObservableCollection<ISB_BIA_Delta_Analyse>>())).Returns(true);
            exportServiceMock.Setup(x => x.Log_ExportLog(It.IsAny<ObservableCollection<ISB_BIA_Log>>())).Returns(true);
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
            Messenger.Default.Register<NotificationMessage<ViewModelBase>>(this, MessageToken.ChangeCurrentVM, s => { received = true; });

            Main_ViewModel mvm = new Main_ViewModel(diaServiceMock.Object, naviServiceMock.Object, sharedServiceMock.Object, dataServiceMock.Object);
            Assert.IsFalse(received);
            Assert.IsTrue(mvm.ViewModelCurrent == null);
            Assert.IsTrue(naviServiceMock.Object.VMHistory.Count == 1);
            mvm.AdminSelectGroupCommand.Execute((int)UserGroups.Admin);
            //Assert that CurrentViewModel change message is received
            Assert.IsTrue(received);
            //Assert that CurrentViewModel was changed
            Assert.IsTrue(mvm.ViewModelCurrent is TargetViewModel);
            //Assert that CurrentViewModel was added to Navigation History
            Assert.IsTrue(naviServiceMock.Object.VMHistory.FirstOrDefault() is TargetViewModel);
        }

        [TestMethod]
        public void Test_Menu_ViewModel()
        {
            Messenger.Default.Register<string>(this, "a", p => { Console.WriteLine("rec"); });

            Menu_ViewModel mvmenu = new Menu_ViewModel(diaServiceMock.Object, naviServiceMock.Object, exportServiceMock.Object, dataServiceMock.Object, sharedServiceMock.Object);
            mvmenu.Cmd_ChangeTextSize.Execute(null);
            //Console.WriteLine(mvmenu.CountAllProcesses.ToString());
            mvmenu.Cmd_NavToLastDelta.Execute(null);
            //Console.WriteLine(mvmenu.CountEditProcesses.ToString());
            Messenger.Default.Send("a", "a");
            //mvmenu.e
        }

        [TestMethod]
        public void TestMethod3()
        {
            ProcessView_ViewModel mvmenu = new ProcessView_ViewModel(diaServiceMock.Object, naviServiceMock.Object, exportServiceMock.Object, dataServiceMock.Object, sharedServiceMock.Object, mailServiceMock.Object);
            //mvmenu.CmdNavBack.Execute(null);
            //Console.WriteLine(mvmenu.CountEditProcesses.ToString());
            //Messenger.Default.Send("a", "a");
            //mvmenu.e
        }

        [TestMethod]
        public void Test_Process_ViewModel_Change()
        {
            bool received = false;
            Messenger.Default.Register<NotificationMessage<ViewModelBase>>(this, MessageToken.ChangeCurrentVM, s => { received = true; });
            ProcessView_ViewModel mvprocessview = new ProcessView_ViewModel(diaServiceMock.Object, naviServiceMock.Object, exportServiceMock.Object, dataServiceMock.Object, sharedServiceMock.Object, mailServiceMock.Object);
            SourceViewModel mvsource = new SourceViewModel();
            naviServiceMock.Object.VMHistory.Insert(0, mvprocessview);
            Assert.IsTrue(naviServiceMock.Object.VMHistory.Count == 2);

            //Process Count of Dummy Processes
            Assert.IsTrue(mvprocessview.List_Processes.Count == 20);
            //Set Change Mode
            mvprocessview.Mode = ProcAppListMode.Change;
            Assert.IsTrue(mvprocessview.Cmd_DeleteProc.CanExecute(null) == false);
            Assert.IsTrue(mvprocessview.Cmd_NavToProcess.CanExecute(null) == true);
            Assert.AreEqual(mvprocessview.Cmd_RowDoubleClick.GetHashCode(), mvprocessview.Cmd_NavToProcess.GetHashCode());
            Assert.AreNotEqual(mvprocessview.Cmd_RowDoubleClick.GetHashCode(), mvprocessview.Cmd_DeleteProc.GetHashCode());

            Assert.IsTrue(mvprocessview.SelectedItem == null);
            mvprocessview.Cmd_NavToProcess.Execute(null);
            Assert.IsFalse(received);
            Assert.IsTrue(naviServiceMock.Object.VMHistory.Count == 2);

            mvprocessview.SelectedItem = mvprocessview.List_Processes.FirstOrDefault();
            mvprocessview.Cmd_NavToProcess.Execute(null);
            Assert.IsTrue(received);
            Assert.IsTrue(naviServiceMock.Object.VMHistory.Count == 3);
            Assert.IsTrue(naviServiceMock.Object.VMHistory.FirstOrDefault() is TargetViewModel);

            received = false;
            mvprocessview.Cmd_NavBack.Execute(null);
            Assert.IsTrue(received);
            Assert.IsTrue(naviServiceMock.Object.VMHistory.Count == 2);
            Assert.IsTrue(naviServiceMock.Object.VMHistory.FirstOrDefault() is ProcessView_ViewModel);

            received = false;
            mvprocessview.Cmd_NavBack.Execute(null);
            Assert.IsTrue(received);
            Assert.IsTrue(naviServiceMock.Object.VMHistory.Count == 1);
            Assert.IsTrue(naviServiceMock.Object.VMHistory.FirstOrDefault() is SourceViewModel);

            Assert.IsTrue(mvprocessview.Vis_ButtonMultiSave == Visibility.Visible);
            Assert.AreEqual(mvprocessview.Str_Instruction, "Doppelklick auf einen Prozess, den Sie ändern möchten, oder Prozesse anhaken, welche ohne Änderungen gespeichert werden sollen.");
            Assert.AreEqual(mvprocessview.Str_Header, "Prozesse bearbeiten");
        }

        [TestMethod]
        public void Test_Process_ViewModel_Delete()
        {
            ProcessView_ViewModel mvprocessview = new ProcessView_ViewModel(diaServiceMock.Object, naviServiceMock.Object, exportServiceMock.Object, dataServiceMock.Object, sharedServiceMock.Object, mailServiceMock.Object);
            //Process Count of Dummy Processes
            Assert.IsTrue(mvprocessview.List_Processes.Count == 20);

            //Set Delete Mode
            mvprocessview.Mode = ProcAppListMode.Delete;
            Assert.IsTrue(mvprocessview.Cmd_SaveSelectedProcesses.CanExecute(null) == false);
            Assert.IsTrue(mvprocessview.Cmd_DeleteProc.CanExecute(null) == true);
            Assert.IsTrue(mvprocessview.Cmd_NavToProcess.CanExecute(null) == false);
            Assert.AreNotEqual(mvprocessview.Cmd_RowDoubleClick.GetHashCode(), mvprocessview.Cmd_NavToProcess.GetHashCode());
            Assert.AreEqual(mvprocessview.Cmd_RowDoubleClick.GetHashCode(), mvprocessview.Cmd_DeleteProc.GetHashCode());

            //Delete Process with SelectedItem = 0
            Assert.IsTrue(mvprocessview.SelectedItem == null);
            mvprocessview.Cmd_DeleteProc.Execute(null);
            //Set SelectedItem for Deletion
            mvprocessview.SelectedItem = mvprocessview.List_Processes.FirstOrDefault();
            //Delete Process
            mvprocessview.Cmd_DeleteProc.Execute(null);
            //Assert that following Methods were called once
            myLockMock.Verify(mock => mock.Get_ObjectIsLocked(It.IsAny<Table_Lock_Flags>(), It.IsAny<int>()), Times.Once);
            myProcMock.Verify(mock => mock.Delete_Process(It.IsAny<ISB_BIA_Prozesse>()), Times.Once);

            mvprocessview.SelectedItem = new ISB_BIA_Prozesse() { Prozess_Id = 1, Aktiv = 0 };
            mvprocessview.Cmd_DeleteProc.Execute(null);
            myLockMock.Verify(mock => mock.Get_ObjectIsLocked(It.IsAny<Table_Lock_Flags>(), It.IsAny<int>()), Times.Exactly(2));
            myProcMock.Verify(mock => mock.Delete_Process(It.IsAny<ISB_BIA_Prozesse>()), Times.Exactly(1));

            Assert.IsTrue(mvprocessview.Vis_ButtonMultiSave == Visibility.Collapsed);
            Assert.AreEqual(mvprocessview.Str_Instruction, "Doppelklick auf einen Prozess, den Sie löschen möchten");
            Assert.AreEqual(mvprocessview.Str_Header, "Prozesse löschen");
        }

    }
    */
}
