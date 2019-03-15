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
    class MyDataService_Log : IMyDataService_Log
    {
        readonly IMyDialogService _myDia;
        readonly IMySharedResourceService _myShared;

        public MyDataService_Log(IMyDialogService myDia, IMySharedResourceService myShared)
        {
            this._myDia = myDia;
            this._myShared = myShared;
        }
        #region Log
        public ObservableCollection<ISB_BIA_Log> Get_Log()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    return new ObservableCollection<ISB_BIA_Log>(db.ISB_BIA_Log.OrderByDescending(x => x.Datum).ToList());
                }

            }
            catch (Exception ex)
            {
                _myDia.ShowError("Log Daten konnten nicht geladen werden.\n", ex);
                return null;
            }
        }
        #endregion  
    }
}
