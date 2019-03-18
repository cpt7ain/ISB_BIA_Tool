using ISB_BIA_IMPORT1.Model;
using ISB_BIA_IMPORT1.ViewModel;
using ISB_BIA_IMPORT1.LINQ2SQL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Net;
using ISB_BIA_IMPORT1.Services.Interfaces;

namespace ISB_BIA_IMPORT1.Services
{
    public class DesignTimeDataService_DataModel : IDataModelService
    {
        #region Datenmodell erstellen
        public bool Create(DataTable dt_Processes, DataTable dt_Applications, DataTable dt_Relation, DataTable dt_InformationSegments, DataTable dt_InformationSegmentAttributes)
        {
            return true;
        }
        #endregion
    }
}
