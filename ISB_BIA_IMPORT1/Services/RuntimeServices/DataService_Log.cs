﻿using ISB_BIA_IMPORT1.LINQ2SQL;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Data;
using ISB_BIA_IMPORT1.Services.Interfaces;

namespace ISB_BIA_IMPORT1.Services
{
    class DataService_Log : IDataService_Log
    {
        readonly IDialogService _myDia;
        readonly ISharedResourceService _myShared;

        public DataService_Log(IDialogService myDia, ISharedResourceService myShared)
        {
            this._myDia = myDia;
            this._myShared = myShared;
        }
        #region Log
        public ObservableCollection<ISB_BIA_Log> Get_List_Log()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
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
