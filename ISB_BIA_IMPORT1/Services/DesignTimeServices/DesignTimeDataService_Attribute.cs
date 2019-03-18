using ISB_BIA_IMPORT1.Model;
using ISB_BIA_IMPORT1.LINQ2SQL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Data;
using ISB_BIA_IMPORT1.Services.Interfaces;

namespace ISB_BIA_IMPORT1.Services
{
    public class DesignTimeDataService_Attribute : IDataService_Attribute
    {
        private ObservableCollection<ISB_BIA_Informationssegmente> SegmentDummyList;
        private ObservableCollection<ISB_BIA_Informationssegmente_Attribute> AttributetDummyList;

        public DesignTimeDataService_Attribute()
        {
            AttributetDummyList = GetDummyAttributes();
        }

        #region Informationssegmente
        public ObservableCollection<ISB_BIA_Informationssegmente_Attribute> GetDummyAttributes()
        {
            Random r = new Random();
            var people = from n in Enumerable.Range(1, 10)
                         select new ISB_BIA_Informationssegmente_Attribute
                         {
                             Attribut_Id = n,
                             Name = "Att" + n,
                             Info = "Info " + n,
                             SZ_1 = r.Next(0, 5),
                             SZ_2 = r.Next(0, 5),
                             SZ_3 = r.Next(0, 5),
                             SZ_4 = r.Next(0, 5),
                             SZ_5 = r.Next(0, 5),
                             SZ_6 = r.Next(0, 5),
                             Datum = DateTime.Now,
                             Benutzer = "Test"
                         };
            return new ObservableCollection<ISB_BIA_Informationssegmente_Attribute>(people.ToList());
        }
        public Attributes_Model Get_Model_FromDB(int id)
        {
            ISB_BIA_Informationssegmente_Attribute linqAttribute = AttributetDummyList.First();

            Attributes_Model result = new Attributes_Model()
            {
                Attribut_Id = linqAttribute.Attribut_Id,
                Name = linqAttribute.Name,
                Info = linqAttribute.Info,
                SZ_1 = linqAttribute.SZ_1.ToString(),
                SZ_2 = linqAttribute.SZ_2.ToString(),
                SZ_3 = linqAttribute.SZ_3.ToString(),
                SZ_4 = linqAttribute.SZ_4.ToString(),
                SZ_5 = linqAttribute.SZ_5.ToString(),
                SZ_6 = linqAttribute.SZ_6.ToString()
            };
            return result;
        }
        public ISB_BIA_Informationssegmente_Attribute Map_Model_ToDB(Attributes_Model ia)
        {
            return null;
        }
        public ObservableCollection<ISB_BIA_Informationssegmente_Attribute> Get_List_Attributes()
        {
            return new ObservableCollection<ISB_BIA_Informationssegmente_Attribute>(
                AttributetDummyList.GroupBy(a => a.Attribut_Id)
                .Select(g => g.OrderByDescending(p => p.Datum).FirstOrDefault()).OrderBy(x => x.Attribut_Id).ToList());
        }
        public bool Insert_Attribute(ObservableCollection<Attributes_Model> newAttributeList)
        {
            return true;
        }
        #endregion

    }
}
