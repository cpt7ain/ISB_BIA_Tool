using GalaSoft.MvvmLight;
using System;

namespace ISB_BIA_IMPORT1.Model
{
    /// <summary>
    /// Model eines Informationssegments
    /// </summary>
    public class InformationSegment_Model: ObservableObject
    {
        #region Backing-Fields
        private int _informationssegment_Id;
        private string _name="";
        private string _segment="";
        private string _beschreibung="";
        private string _mögliche_Segmentinhalte="";
        private bool _attribut_1;
        private bool _attribut_2;
        private bool _attribut_3;
        private bool _attribut_4;
        private bool _attribut_5;
        private bool _attribut_6;
        private bool _attribut_7;
        private bool _attribut_8;
        private bool _attribut_9;
        private bool _attribut_10;
        private DateTime _datum;
        private string _benutzer="";
        #endregion

        #region Propteries der Informationssegmente (für DataBinding)
        /// <summary>
        /// Id des Segments
        /// </summary>
        public int Informationssegment_Id
        {
            get => _informationssegment_Id;
            set => Set(()=>Informationssegment_Id,ref _informationssegment_Id, value);
        }
        /// <summary>
        /// Kurzname des Segments
        /// </summary>
        public string Name
        {
            get => _name;
            set => Set(() => Name, ref _name, value);
        }
        /// <summary>
        /// Name des Segments
        /// </summary>
        public string Segment
        {
            get => _segment;
            set => Set(() => Segment, ref _segment, value);
        }
        /// <summary>
        /// Beschreibung des Segments
        /// </summary>
        public string Beschreibung
        {
            get => _beschreibung;
            set => Set(() => Beschreibung, ref _beschreibung, value);
        }
        /// <summary>
        /// Weitere Informationen über das Segment
        /// </summary>
        public string Mögliche_Segmentinhalte
        {
            get => _mögliche_Segmentinhalte;
            set => Set(() => Mögliche_Segmentinhalte, ref _mögliche_Segmentinhalte, value);
        }
        #region Attribute mit Warheitswert, ob auf das Segment zutreffend oder nicht
        /// <summary>
        /// Wahrheitswert für das Zutreffen des Attribut 1
        /// </summary>
        public bool Attribut1
        {
            get => _attribut_1;
            set => Set(() => Attribut1, ref _attribut_1, value);
        }
        /// <summary>
        /// Wahrheitswert für das Zutreffen des Attribut 2
        /// </summary>
        public bool Attribut2
        {
            get => _attribut_2;
            set => Set(() => Attribut2, ref _attribut_2, value);
        }
        /// <summary>
        /// Wahrheitswert für das Zutreffen des Attribut 3
        /// </summary>
        public bool Attribut3
        {
            get => _attribut_3;
            set => Set(() => Attribut3, ref _attribut_3, value);
        }
        /// <summary>
        /// Wahrheitswert für das Zutreffen des Attribut 4
        /// </summary>
        public bool Attribut4
        {
            get => _attribut_4;
            set => Set(() => Attribut4, ref _attribut_4, value);
        }
        /// <summary>
        /// Wahrheitswert für das Zutreffen des Attribut 5
        /// </summary>
        public bool Attribut5
        {
            get => _attribut_5;
            set => Set(() => Attribut5, ref _attribut_5, value);
        }
        /// <summary>
        /// Wahrheitswert für das Zutreffen des Attribut 6
        /// </summary>
        public bool Attribut6
        {
            get => _attribut_6;
            set => Set(() => Attribut6, ref _attribut_6, value);
        }
        /// <summary>
        /// Wahrheitswert für das Zutreffen des Attribut 7
        /// </summary>
        public bool Attribut7
        {
            get => _attribut_7;
            set => Set(() => Attribut7, ref _attribut_7, value);
        }
        /// <summary>
        /// Wahrheitswert für das Zutreffen des Attribut 8
        /// </summary>
        public bool Attribut8
        {
            get => _attribut_8;
            set => Set(() => Attribut8, ref _attribut_8, value);
        }
        /// <summary>
        /// Wahrheitswert für das Zutreffen des Attribut 9
        /// </summary>
        public bool Attribut9
        {
            get => _attribut_9;
            set => Set(() => Attribut9, ref _attribut_9, value);
        }
        /// <summary>
        /// Wahrheitswert für das Zutreffen des Attribut 10
        /// </summary>
        public bool Attribut10
        {
            get => _attribut_10;
            set => Set(() => Attribut10, ref _attribut_10, value);
        }
        #endregion
        /// <summary>
        /// Bearbeitungsdatum des Datensatzes
        /// </summary>
        public DateTime Datum
        {
            get => _datum;
            set => Set(() => Datum, ref _datum, value);
        }
        /// <summary>
        /// Bearbeitender Nutzer
        /// </summary>
        public string Benutzer
        {
            get => _benutzer;
            set => Set(() => Benutzer, ref _benutzer, value);
        }
        #endregion

    }
}
