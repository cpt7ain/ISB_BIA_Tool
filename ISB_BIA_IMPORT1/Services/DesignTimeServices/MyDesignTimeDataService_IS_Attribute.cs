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
    public class MyDesignTimeDataService_IS_Attribute : IMyDataService_IS_Attribute
    {
        private ObservableCollection<ISB_BIA_Informationssegmente> SegmentDummyList;

        public MyDesignTimeDataService_IS_Attribute()
        {
            SegmentDummyList = GetDummySegments();
        }

        #region Informationssegmente
        public ObservableCollection<ISB_BIA_Informationssegmente> GetDummySegments()
        {
            Random r = new Random();
            var people = from n in Enumerable.Range(1, 25)
                         select new ISB_BIA_Informationssegmente
                         {
                             Informationssegment_Id = n,
                             Name = "IS" + n,
                             Segment = "Segment" + n,
                             Beschreibung = "Beschreibung " + n,
                             Mögliche_Segmentinhalte = "Inhalt " + n,
                             Attribut_1 = "P",
                             Attribut_2 = "O",
                             Attribut_3 = "O",
                             Attribut_4 = "O",
                             Attribut_5 = "P",
                             Attribut_6 = "P",
                             Attribut_7 = "O",
                             Attribut_8 = "O",
                             Attribut_9 = "O",
                             Attribut_10 = "O",
                             Datum = DateTime.Now,
                             Benutzer = "Test"
                         };
            return new ObservableCollection<ISB_BIA_Informationssegmente>(people.ToList());
        }
        public ObservableCollection<ISB_BIA_Informationssegmente_Attribute> GetDummyAttributes()
        {
            Random r = new Random();
            var people = from n in Enumerable.Range(1, 100)
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
        public InformationSegment_Model Get_SegmentModelFromDB(int id)
        {
            ISB_BIA_Informationssegmente linqIS = SegmentDummyList.FirstOrDefault();

            InformationSegment_Model result = new InformationSegment_Model()
            {
                Informationssegment_Id = linqIS.Informationssegment_Id,
                Name = linqIS.Name,
                Segment = linqIS.Segment,
                Beschreibung = linqIS.Beschreibung,
                Mögliche_Segmentinhalte = linqIS.Mögliche_Segmentinhalte,
                Attribut1 = (linqIS.Attribut_1 == "P") ? true : false,
                Attribut2 = (linqIS.Attribut_2 == "P") ? true : false,
                Attribut3 = (linqIS.Attribut_3 == "P") ? true : false,
                Attribut4 = (linqIS.Attribut_4 == "P") ? true : false,
                Attribut5 = (linqIS.Attribut_5 == "P") ? true : false,
                Attribut6 = (linqIS.Attribut_6 == "P") ? true : false,
                Attribut7 = (linqIS.Attribut_7 == "P") ? true : false,
                Attribut8 = (linqIS.Attribut_8 == "P") ? true : false,
                Attribut9 = (linqIS.Attribut_9 == "P") ? true : false,
                Attribut10 = (linqIS.Attribut_10 == "P") ? true : false
            };
            return result;
        }
        public ISB_BIA_Informationssegmente Map_SegmentModelToDB(InformationSegment_Model i)
        {
            return null;
        }
        public InformationSegmentAttribute_Model Get_AttributeModelFromDB(int id)
        {
            ISB_BIA_Informationssegmente_Attribute linqAttribute = GetDummyAttributes()[id - 1];

            InformationSegmentAttribute_Model result = new InformationSegmentAttribute_Model()
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
        public ISB_BIA_Informationssegmente_Attribute Map_AttributeModelToDB(InformationSegmentAttribute_Model ia)
        {
            return null;
        }
        public ObservableCollection<ISB_BIA_Informationssegmente> Get_Segments_All()
        {
            return new ObservableCollection<ISB_BIA_Informationssegmente>(
                SegmentDummyList.GroupBy(a => a.Informationssegment_Id)
                .Select(g => g.OrderByDescending(p => p.Datum).FirstOrDefault()).OrderBy(x => x.Informationssegment_Id).ToList());
        }
        public ObservableCollection<ISB_BIA_Informationssegmente> Get_Segments_Enabled()
        {
            return new ObservableCollection<ISB_BIA_Informationssegmente>(
                SegmentDummyList.Where(x => x.Segment != "Lorem ipsum").GroupBy(a => a.Informationssegment_Id)
                .Select(g => g.OrderByDescending(p => p.Datum).FirstOrDefault()).OrderBy(x => x.Informationssegment_Id).ToList());
        }
        public List<ISB_BIA_Informationssegmente> Get_5SegmentsForCalculation(Process_Model process)
        {
            process = new Process_Model { Relevantes_IS_1 = SegmentDummyList[0].Name, Relevantes_IS_2 = SegmentDummyList[1].Name };
            //Zutreffende Segmente auswählen
            return Get_Segments_Enabled().Where(x =>
            x.Name == process.Relevantes_IS_1 ||
            x.Name == process.Relevantes_IS_2 ||
            x.Name == process.Relevantes_IS_3 ||
            x.Name == process.Relevantes_IS_4 ||
            x.Name == process.Relevantes_IS_5).ToList();
        }
        public ISB_BIA_Informationssegmente Get_ISByISName(string iSName)
        {
            return SegmentDummyList.Where(y => y.Name == iSName).
                GroupBy(a => a.Informationssegment_Id).Select(g => g.OrderByDescending(p => p.Datum).FirstOrDefault()).FirstOrDefault();
        }
        public ObservableCollection<ISB_BIA_Informationssegmente_Attribute> Get_Attributes()
        {
            return new ObservableCollection<ISB_BIA_Informationssegmente_Attribute>(
                GetDummyAttributes().GroupBy(a => a.Attribut_Id)
                .Select(g => g.OrderByDescending(p => p.Datum).FirstOrDefault()).OrderBy(x => x.Attribut_Id).ToList());
        }
        public ObservableCollection<string> Get_List_AttributeNamesAndInfoForIS()
        {
            return new ObservableCollection<string>(
                GetDummyAttributes().GroupBy(x => x.Attribut_Id).
                Select(h => h.OrderByDescending(g => g.Datum).FirstOrDefault()).
                ToList().OrderBy(b => b.Attribut_Id).Select(s => String.Concat(s.Name, " ", s.Info)));
        }
        public ObservableCollection<string> Get_List_AttributeNamesForHeader()
        {
            return new ObservableCollection<string>(
                GetDummyAttributes().GroupBy(x => x.Attribut_Id).
                Select(h => h.OrderByDescending(g => g.Datum).FirstOrDefault()).
                ToList().OrderBy(b => b.Attribut_Id).Select(s => s.Name));
        }
        public bool Insert_Segment(InformationSegment_Model newIS, InformationSegment_Model oldIS)
        {
            return true;
        }
        public bool Insert_Attribute(ObservableCollection<InformationSegmentAttribute_Model> newAttributeList)
        {
            return true;
        }
        public Tuple<List<ISB_BIA_Informationssegmente>, List<ISB_BIA_Informationssegmente>, List<ISB_BIA_Informationssegmente_Attribute>, List<ISB_BIA_Informationssegmente_Attribute>> Get_ISAndISAttForExport()
        {
            return null;
        }
        #endregion

    }
}
