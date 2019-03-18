using System.Collections.Generic;
using System.Collections.ObjectModel;
using ISB_BIA_IMPORT1.LINQ2SQL;
using ISB_BIA_IMPORT1.Services.Interfaces;

namespace ISB_BIA_IMPORT1.Services
{
    public class DesignTimeExportService : IExportService
    {
        public bool Export_Applications_Active()
        {
            return true;
        }

        public bool Export_Processes_Active()
        {
            return true;
        }

        public bool Export_Applications_All()
        {
            return true;
        }

        public bool Export_Applications(ObservableCollection<ISB_BIA_Applikationen> appList, string title, int id=0)
        {
            return true;
        }

        public bool Export_IS_Attr_History()
        {
            return true;
        }

        public bool Export_DeltaAnalysis(ObservableCollection<ISB_BIA_Delta_Analyse> DeltaList)
        {
            return true;
        }

        public bool Export_Log(ObservableCollection<ISB_BIA_Log> Log)
        {
            return true;
        }

        public bool Export_Processes(ObservableCollection<ISB_BIA_Prozesse> procList, int id = 0)
        {
            return true;
        }

        public bool Export_Settings(List<ISB_BIA_Settings> Settings)
        {
            return true;
        }
    }
}
