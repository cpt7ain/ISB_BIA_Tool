using ISB_BIA_IMPORT1.Model;
using ISB_BIA_IMPORT1.LINQ2SQL;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Data;
using ISB_BIA_IMPORT1.Services.Interfaces;

namespace ISB_BIA_IMPORT1.Services
{
    class DataService_Attribute : IDataService_Attribute
    {
        readonly IDialogService _myDia;
        readonly ISharedResourceService _myShared;

        public DataService_Attribute(IDialogService myDia, ISharedResourceService myShared)
        {
            this._myDia = myDia;
            this._myShared = myShared;
        }

        #region Informationssegmente
        public Attributes_Model Get_Model_FromDB(int id)
        {
            try
            {
                ISB_BIA_Informationssegmente_Attribute linqAttribute;
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    linqAttribute = db.ISB_BIA_Informationssegmente_Attribute.Where(c => c.Attribut_Id == id)
                        .OrderByDescending(p => p.Datum).FirstOrDefault();
                }
                if (linqAttribute == null)
                {
                    _myDia.ShowError("Attributdaten konnten nicht abgerufen werden");
                    return null;
                }
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
            catch (Exception ex)
            {
                _myDia.ShowError("Attributdaten konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public ISB_BIA_Informationssegmente_Attribute Map_Model_ToDB(Attributes_Model ia)
        {
            Int32.TryParse(ia.SZ_1, out int sz1);
            Int32.TryParse(ia.SZ_2, out int sz2);
            Int32.TryParse(ia.SZ_3, out int sz3);
            Int32.TryParse(ia.SZ_4, out int sz4);
            Int32.TryParse(ia.SZ_5, out int sz5);
            Int32.TryParse(ia.SZ_6, out int sz6);

            ISB_BIA_Informationssegmente_Attribute map = new ISB_BIA_Informationssegmente_Attribute()
            {
                Attribut_Id = ia.Attribut_Id,
                Name = ia.Name,
                Info = ia.Info,
                SZ_1 = sz1,
                SZ_2 = sz2,
                SZ_3 = sz3,
                SZ_4 = sz4,
                SZ_5 = sz5,
                SZ_6 = sz6,
            };
            return map;
        }
        public ObservableCollection<ISB_BIA_Informationssegmente_Attribute> Get_List_Attributes()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    return new ObservableCollection<ISB_BIA_Informationssegmente_Attribute>(
                        db.ISB_BIA_Informationssegmente_Attribute.GroupBy(a => a.Attribut_Id)
                        .Select(g => g.OrderByDescending(p => p.Datum).FirstOrDefault()).OrderBy(x => x.Attribut_Id).ToList());
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Segment-Attribute konnten nicht abgerufen werden.", ex);
                return null;
            }
        }
        public bool Insert_Attribute(ObservableCollection<Attributes_Model> newAttributeList)
        {
            if (newAttributeList.Select(x => x.Name).Distinct().Count() != newAttributeList.Count)
            {
                _myDia.ShowMessage("Attribut-Namen müssen einzigartig sein.");
                return false;
            }
            //Indikator für Änderung
            bool change = false;
            ObservableCollection<ISB_BIA_Informationssegmente_Attribute> oldAttributeList;
            oldAttributeList = Get_List_Attributes();
            if (oldAttributeList == null)
            {
                _myDia.ShowError("Fehler beim Speichern der Attribute!");
                return false;
            }

            //Schreiben in Datenbank
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    foreach (Attributes_Model isx in newAttributeList)
                    {
                        if (isx.Name == oldAttributeList.Where(x => x.Attribut_Id == isx.Attribut_Id).Select(n => n.Name).FirstOrDefault() &&
                            isx.Info == oldAttributeList.Where(x => x.Attribut_Id == isx.Attribut_Id).Select(n => n.Info).FirstOrDefault() &&
                            isx.SZ_1 == oldAttributeList.Where(x => x.Attribut_Id == isx.Attribut_Id).Select(n => n.SZ_1.ToString()).FirstOrDefault() &&
                            isx.SZ_2 == oldAttributeList.Where(x => x.Attribut_Id == isx.Attribut_Id).Select(n => n.SZ_2.ToString()).FirstOrDefault() &&
                            isx.SZ_3 == oldAttributeList.Where(x => x.Attribut_Id == isx.Attribut_Id).Select(n => n.SZ_3.ToString()).FirstOrDefault() &&
                            isx.SZ_4 == oldAttributeList.Where(x => x.Attribut_Id == isx.Attribut_Id).Select(n => n.SZ_4.ToString()).FirstOrDefault() &&
                            isx.SZ_5 == oldAttributeList.Where(x => x.Attribut_Id == isx.Attribut_Id).Select(n => n.SZ_5.ToString()).FirstOrDefault() &&
                            isx.SZ_6 == oldAttributeList.Where(x => x.Attribut_Id == isx.Attribut_Id).Select(n => n.SZ_6.ToString()).FirstOrDefault()
                            )
                        {
                            continue;
                        }

                        // mindestens eine Änderung
                        change = true;
                        ISB_BIA_Informationssegmente_Attribute isMapped = Map_Model_ToDB(isx);
                        isMapped.Datum = DateTime.Now;
                        isMapped.Benutzer = _myShared.User.Username;
                        db.ISB_BIA_Informationssegmente_Attribute.InsertOnSubmit(isMapped);

                        //Log Eintrag für erfolgreiches schreiben in Datenbank
                        ISB_BIA_Log logEntry = new ISB_BIA_Log
                        {
                            Aktion = "Ändern der Informationssegment-Attribute",
                            Tabelle = _myShared.Tbl_IS_Attribute,
                            Details = "Id = " + isx.Attribut_Id + ", Name = '" + isx.Name + "'",
                            Id_1 = isx.Attribut_Id,
                            Id_2 = 0,
                            Datum = DateTime.Now,
                            Benutzer = _myShared.User.Username
                        };
                        db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                    }
                    if (!change) _myDia.ShowInfo("Keine Änderungen entdeckt");
                    else
                    {
                        db.SubmitChanges();
                        _myDia.ShowInfo("Attribute gespeichert");
                    }
                }
                return true;
            }
            catch (Exception ex1)
            {
                //LogEntry bei Fehler erstellen + Schreiben in Datenbank
                try
                {
                    using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                    {
                        ISB_BIA_Log logEntry = new ISB_BIA_Log
                        {
                            Datum = DateTime.Now,
                            Aktion = "Fehler: Ändern der Informationssegment-Attributstabelle",
                            Tabelle = _myShared.Tbl_Prozesse,
                            Details = ex1.Message,
                            Id_1 = 0,
                            Id_2 = 0,
                            Benutzer = _myShared.User.Username
                        };
                        db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                        db.SubmitChanges();
                        _myDia.ShowError("Fehler beim Speichern der Attributliste", ex1);
                        return false;
                    }
                }
                catch (Exception ex2)
                {
                    _myDia.ShowError("Fehler beim Speichern der Attributliste", ex2);
                    return false;
                }
            }
        }
        #endregion

    }
}
