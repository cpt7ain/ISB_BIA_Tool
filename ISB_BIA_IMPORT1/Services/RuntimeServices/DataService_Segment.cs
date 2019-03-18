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
    class DataService_Segment : IDataService_Segment
    {
        readonly IDialogService _myDia;
        readonly ISharedResourceService _myShared;

        public DataService_Segment(IDialogService myDia, ISharedResourceService myShared)
        {
            this._myDia = myDia;
            this._myShared = myShared;
        }

        #region Informationssegmente
        public Segment_Model Get_Model_FromDB(int id)
        {
            try
            {
                ISB_BIA_Informationssegmente linqIS;
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    linqIS = db.ISB_BIA_Informationssegmente.Where(c => c.Informationssegment_Id == id)
                        .OrderByDescending(p => p.Datum).FirstOrDefault();
                }
                if (linqIS == null)
                {
                    _myDia.ShowError("Segmentdaten konnten nicht abgerufen werden");
                    return null;
                }
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
            catch (Exception ex)
            {
                _myDia.ShowError("Segmentdaten konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public ISB_BIA_Informationssegmente Map_Model_ToDB(Segment_Model i)
        {
            return new ISB_BIA_Informationssegmente()
            {
                Informationssegment_Id = i.Informationssegment_Id,
                Name = i.Name,
                Segment = i.Segment,
                Beschreibung = i.Beschreibung,
                Mögliche_Segmentinhalte = i.Mögliche_Segmentinhalte,
                Attribut_1 = (i.Attribut1) ? "P" : "O",
                Attribut_2 = (i.Attribut2) ? "P" : "O",
                Attribut_3 = (i.Attribut3) ? "P" : "O",
                Attribut_4 = (i.Attribut4) ? "P" : "O",
                Attribut_5 = (i.Attribut5) ? "P" : "O",
                Attribut_6 = (i.Attribut6) ? "P" : "O",
                Attribut_7 = (i.Attribut7) ? "P" : "O",
                Attribut_8 = (i.Attribut8) ? "P" : "O",
                Attribut_9 = (i.Attribut9) ? "P" : "O",
                Attribut_10 = (i.Attribut10) ? "P" : "O",
                Datum = i.Datum,
                Benutzer = i.Benutzer
            };
        }
        public ObservableCollection<ISB_BIA_Informationssegmente> Get_List_Segments_All()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    return new ObservableCollection<ISB_BIA_Informationssegmente>(
                        db.ISB_BIA_Informationssegmente.GroupBy(a => a.Name)
                        .Select(g => g.OrderByDescending(p => p.Datum).FirstOrDefault()).OrderBy(x => x.Informationssegment_Id).ToList());
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Segmente konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public ObservableCollection<ISB_BIA_Informationssegmente> Get_List_Segments_Enabled()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    return new ObservableCollection<ISB_BIA_Informationssegmente>(
                        db.ISB_BIA_Informationssegmente.Where(x => x.Segment != "Lorem ipsum").GroupBy(a => a.Name)
                        .Select(g => g.OrderByDescending(p => p.Datum).FirstOrDefault()).OrderBy(x => x.Informationssegment_Id).ToList());
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Informationssegmente konnten nicht abgerufen werden.\n", ex);
                return null;
            }
        }
        public ObservableCollection<string> Get_StringList_AttributeNamesAndInfo()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    return new ObservableCollection<string>(
                        db.ISB_BIA_Informationssegmente_Attribute.GroupBy(x => x.Attribut_Id).
                        Select(h => h.OrderByDescending(g => g.Datum).FirstOrDefault()).
                        ToList().OrderBy(b => b.Attribut_Id).Select(s => String.Concat(s.Name, " ", s.Info)));
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Informationssegment-Attribute konnten nicht abgerufen werden.", ex);
                return null;
            }
        }
        public ObservableCollection<string> Get_StringList_AttributeNames()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    return new ObservableCollection<string>(
                        db.ISB_BIA_Informationssegmente_Attribute.GroupBy(x => x.Attribut_Id).
                        Select(h => h.OrderByDescending(g => g.Datum).FirstOrDefault()).
                        ToList().OrderBy(b => b.Attribut_Id).Select(s => s.Name));
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Informationssegment-Attribute konnten nicht abgerufen werden.", ex);
                return null;
            }
        }
        public bool Insert_Segment(Segment_Model newIS, Segment_Model oldIS)
        {
            //Wenn keine Änderungen gemacht wurden abbrechen
            if (oldIS.Segment == newIS.Segment
                && oldIS.Beschreibung == newIS.Beschreibung
                && oldIS.Mögliche_Segmentinhalte == newIS.Mögliche_Segmentinhalte
                && oldIS.Attribut1 == newIS.Attribut1
                && oldIS.Attribut2 == newIS.Attribut2
                && oldIS.Attribut3 == newIS.Attribut3
                && oldIS.Attribut4 == newIS.Attribut4
                && oldIS.Attribut5 == newIS.Attribut5
                && oldIS.Attribut6 == newIS.Attribut6
                && oldIS.Attribut7 == newIS.Attribut7
                && oldIS.Attribut8 == newIS.Attribut8
                && oldIS.Attribut9 == newIS.Attribut9
                && oldIS.Attribut10 == newIS.Attribut10)
            {
                _myDia.ShowInfo("Keine Änderungen entdeckt.");
                return false;
            }

            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    if (db.ISB_BIA_Informationssegmente.Count(x => x.Segment == newIS.Segment) > 0)
                    {
                        _myDia.ShowInfo("Der Informationssegment-Name ist bereits vergeben.");
                        return false;
                    }
                    //Schreiben in Datenbank
                    newIS.Datum = DateTime.Now;
                    newIS.Benutzer = _myShared.User.Username;
                    //Mappen und in DB einfügen
                    db.ISB_BIA_Informationssegmente.InsertOnSubmit(Map_Model_ToDB(newIS));
                    //Logeintrag erzeugen
                    ISB_BIA_Log logEntry = new ISB_BIA_Log
                    {
                        Aktion = "Ändern eines Informationssegments",
                        Tabelle = _myShared.Tbl_IS,
                        Details = "Id = " + newIS.Informationssegment_Id + ", Name = '" + newIS.Name + "'",
                        Id_1 = newIS.Informationssegment_Id,
                        Id_2 = 0,
                        Datum = newIS.Datum,
                        Benutzer = newIS.Benutzer
                    };
                    db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                    db.SubmitChanges();
                    _myDia.ShowInfo("Informationssegment gespeichert");
                    return true;
                }
            }
            catch (Exception ex1)
            {
                //LogEntry bei Fehler erstellen & Schreiben in Datenbank
                try
                {
                    using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                    {
                        ISB_BIA_Log logEntry = new ISB_BIA_Log
                        {
                            Aktion = "Fehler: Ändern eines Informationssegments",
                            Tabelle = _myShared.Tbl_Prozesse,
                            Details = ex1.Message,
                            Id_1 = newIS.Informationssegment_Id,
                            Id_2 = 0,
                            Datum = newIS.Datum,
                            Benutzer = newIS.Benutzer
                        };
                        db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                        db.SubmitChanges();
                        _myDia.ShowError("Fehler beim Speichern des Informationssegments!\nEin Log Eintrag wurde erzeugt.", ex1);
                        return false;
                    }
                }
                catch (Exception ex2)
                {
                    _myDia.ShowError("Fehler beim Speichern des Informationssegments!\nEin Log Eintrag konnte ebenfalls nicht erzeugt werden.", ex2);
                    return false;
                }
            }
        }
        public Tuple<List<ISB_BIA_Informationssegmente>, List<ISB_BIA_Informationssegmente>, List<ISB_BIA_Informationssegmente_Attribute>, List<ISB_BIA_Informationssegmente_Attribute>> Get_History_SegmentsAndAttributes()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    var segmentsList = db.ISB_BIA_Informationssegmente.OrderBy(x=>x.Informationssegment_Id).ToList();
                    var currentSegmentList = segmentsList.GroupBy(x => x.Informationssegment_Id)
                        .Select(c => c.OrderByDescending(v => v.Datum).FirstOrDefault()).ToList();
                    var attributeList = db.ISB_BIA_Informationssegmente_Attribute.OrderBy(x => x.Attribut_Id).ToList();
                    var currentAttributeList = attributeList.GroupBy(x => x.Attribut_Id)
                        .Select(c => c.OrderByDescending(v => v.Datum).FirstOrDefault()).ToList();
                    return new Tuple<List<ISB_BIA_Informationssegmente>, List<ISB_BIA_Informationssegmente>,
                        List<ISB_BIA_Informationssegmente_Attribute>, List<ISB_BIA_Informationssegmente_Attribute>>(
                        segmentsList, currentSegmentList, attributeList, currentAttributeList);
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Informationssegmente und Attribute konnten nicht abgerufen werden.", ex);
                return null;
            }
        }
        #endregion

    }
}
