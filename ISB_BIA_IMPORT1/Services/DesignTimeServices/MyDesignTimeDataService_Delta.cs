using ISB_BIA_IMPORT1.Model;
using ISB_BIA_IMPORT1.ViewModel;
using ISB_BIA_IMPORT1.LINQ2SQL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Data;
using ISB_BIA_IMPORT1.Services.Interfaces;

namespace ISB_BIA_IMPORT1.Services
{
    public class MyDesignTimeDataService_Delta : IMyDataService_Delta
    {
        #region Delta
        public ObservableCollection<ISB_BIA_Delta_Analyse> GetDummyDelta()
        {
            Random r = new Random();
            var people = from n in Enumerable.Range(1, 20)
                select new ISB_BIA_Delta_Analyse
                {
                    Prozess_Id = n,
                    Prozess = "Pro" + n,
                    Sub_Prozess = "Sub " + n,
                    Datum_Prozess = DateTime.Now,
                    Applikation_Id = n,
                    Applikation = "App" + n,
                    Datum_Applikation = DateTime.Now,
                    SZ_1 = 3,
                    SZ_2 = 3,
                    SZ_3 = 3,
                    SZ_4 = 3,
                    SZ_5 = 3,
                    SZ_6 = 3,
                    Datum = DateTime.Now,
                };
            return new ObservableCollection<ISB_BIA_Delta_Analyse>(people.ToList());
        }

        public ObservableCollection<ISB_BIA_Delta_Analyse> Get_DeltaAnalysis()
        {
            return new ObservableCollection<ISB_BIA_Delta_Analyse>(
                GetDummyDelta().OrderBy(x => x.Prozess_Id).ToList());
        }

        public ObservableCollection<ISB_BIA_Delta_Analyse> Initiate_DeltaAnalysis(DateTime d)
        {
            return GetDummyDelta();
        }

        public ObservableCollection<ISB_BIA_Delta_Analyse> ComputeAndGet_DeltaAnalysis_Date(DateTime date, bool toDB, List<ISB_BIA_Prozesse_Applikationen> proc_App)
        {
            return GetDummyDelta();
        }
        #endregion
    }
}
