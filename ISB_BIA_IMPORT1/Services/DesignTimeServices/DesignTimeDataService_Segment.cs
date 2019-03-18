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
    public class DesignTimeDataService_Segment : IDataService_Segment
    {
        private ObservableCollection<ISB_BIA_Informationssegmente> SegmentDummyList;
        private ObservableCollection<ISB_BIA_Informationssegmente_Attribute> AttributetDummyList;

        public DesignTimeDataService_Segment()
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
        public Segment_Model Get_Model_FromDB(int id)
        {
            ISB_BIA_Informationssegmente linqIS = SegmentDummyList.FirstOrDefault();

            Segment_Model result = new Segment_Model()
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
        public ISB_BIA_Informationssegmente Map_Model_ToDB(Segment_Model i)
        {
            return null;
        }
        public ObservableCollection<ISB_BIA_Informationssegmente> Get_List_Segments_All()
        {
            return new ObservableCollection<ISB_BIA_Informationssegmente>(
                SegmentDummyList.GroupBy(a => a.Informationssegment_Id)
                .Select(g => g.OrderByDescending(p => p.Datum).FirstOrDefault()).OrderBy(x => x.Informationssegment_Id).ToList());
        }
        public ObservableCollection<ISB_BIA_Informationssegmente> Get_List_Segments_Enabled()
        {
            return new ObservableCollection<ISB_BIA_Informationssegmente>(
                SegmentDummyList.Where(x => x.Segment != "Lorem ipsum").GroupBy(a => a.Informationssegment_Id)
                .Select(g => g.OrderByDescending(p => p.Datum).FirstOrDefault()).OrderBy(x => x.Informationssegment_Id).ToList());
        }
        public List<ISB_BIA_Informationssegmente> Get_5SegmentsForCalculation(Process_Model process)
        {
            process = new Process_Model { Relevantes_IS_1 = SegmentDummyList[0].Name, Relevantes_IS_2 = SegmentDummyList[1].Name };
            //Zutreffende Segmente auswählen
            return Get_List_Segments_Enabled().Where(x =>
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
        public ObservableCollection<string> Get_StringList_AttributeNamesAndInfo()
        {
            return new ObservableCollection<string>(
                AttributetDummyList.GroupBy(x => x.Attribut_Id).
                Select(h => h.OrderByDescending(g => g.Datum).FirstOrDefault()).
                ToList().OrderBy(b => b.Attribut_Id).Select(s => String.Concat(s.Name, " ", s.Info)));
        }
        public ObservableCollection<string> Get_StringList_AttributeNames()
        {
            return new ObservableCollection<string>(
                AttributetDummyList.GroupBy(x => x.Attribut_Id).
                Select(h => h.OrderByDescending(g => g.Datum).FirstOrDefault()).
                ToList().OrderBy(b => b.Attribut_Id).Select(s => s.Name));
        }
        public bool Insert_Segment(Segment_Model newIS, Segment_Model oldIS)
        {
            return true;
        }
        public Tuple<List<ISB_BIA_Informationssegmente>, List<ISB_BIA_Informationssegmente>, List<ISB_BIA_Informationssegmente_Attribute>, List<ISB_BIA_Informationssegmente_Attribute>> Get_History_SegmentsAndAttributes()
        {
            return null;
        }
        #endregion

    }
}
