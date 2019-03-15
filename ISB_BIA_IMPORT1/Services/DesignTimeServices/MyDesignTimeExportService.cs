using System.Collections.Generic;
using System.Collections.ObjectModel;
using ISB_BIA_IMPORT1.LINQ2SQL;
using ISB_BIA_IMPORT1.Services.Interfaces;

namespace ISB_BIA_IMPORT1.Services
{
    public class MyDesignTimeExportService : IMyExportService
    {
        public bool App_ExportActiveApplications()
        {
            return true;
        }

        public bool Proc_ExportActiveProcesses()
        {
            return true;
        }

        public bool App_ExportAllApplications()
        {
            return true;
        }

        public bool App_ExportApplications(ObservableCollection<ISB_BIA_Applikationen> appList, string title, int id=0)
        {
            return true;
        }

        public bool IS_Attr_ExportSegmentAndAttributeHistory()
        {
            return true;
        }

        public bool Delta_ExportDeltaAnalysis(ObservableCollection<ISB_BIA_Delta_Analyse> DeltaList)
        {
            return true;
        }

        public bool Log_ExportLog(ObservableCollection<ISB_BIA_Log> Log)
        {
            return true;
        }

        public bool Proc_ExportProcesses(ObservableCollection<ISB_BIA_Prozesse> procList, int id = 0)
        {
            return true;
        }

        public bool Set_ExportSettings(List<ISB_BIA_Settings> Settings)
        {
            return true;
        }
    }
}
