using ISB_BIA_IMPORT1.LINQ2SQL;
using System.Collections.ObjectModel;

namespace ISB_BIA_IMPORT1.Services.Interfaces
{
    public interface IDataService_Log
    {
        #region Log
        /// <summary>
        /// Log abrufen (für Log Ansicht / Export1)
        /// </summary>
        /// <returns> Log-Liste </returns>
        ObservableCollection<ISB_BIA_Log> Get_List_Log();
        #endregion
    }
}
