using System.Collections.Generic;
using System.Collections.ObjectModel;
using ISB_BIA_IMPORT1.LinqEntityContext;

namespace ISB_BIA_IMPORT1.Services
{
    public class MyDesignTimeExportService : IMyExportService
    {
        public bool AllActiveApplicationsExport()
        {
            return true;
        }

        public bool AllActiveProcessesExport()
        {
            return true;
        }

        public bool AllApplicationsExport()
        {
            return true;
        }

        public bool ExportApplications(ObservableCollection<ISB_BIA_Applikationen> appList, string title, int id=0)
        {
            return true;
        }

        public bool ExportDeltaAnalysis(ObservableCollection<ISB_BIA_Delta_Analyse> DeltaList)
        {
            return true;
        }

        public bool ExportLog(ObservableCollection<ISB_BIA_Log> Log)
        {
            return true;
        }

        public bool ExportProcesses(ObservableCollection<ISB_BIA_Prozesse> procList, int id = 0)
        {
            return true;
        }

        public bool ExportSettings(List<ISB_BIA_Settings> Settings)
        {
            return true;
        }
    }
}
