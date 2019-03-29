using ISB_BIA_IMPORT1.LINQ2SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Net;
using ISB_BIA_IMPORT1.Services.Interfaces;

namespace ISB_BIA_IMPORT1.Services
{
    class DataService_Lock : ILockService
    {
        readonly IDialogService _myDia;
        readonly ISharedResourceService _myShared;

        public DataService_Lock(IDialogService myDia, ISharedResourceService myShared)
        {
            this._myDia = myDia;
            this._myShared = myShared;
        }

        public bool CheckDBConnection()
        {
            string s = "";
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    s = db.Connection.ConnectionString;
                    db.Connection.Open();
                }
                return true;
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Es konnte keine Verbindung zur Datenbank [" + s + "] hergestellt werden.\nDie Anwendung wird geschlossen.", ex);
                return false;
            }
        }
        #region Datensatz-Lock Operationen
        public string Get_ObjectIsLocked(Table_Lock_Flags table_Flag, int id)
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    ISB_BIA_Lock lockObj = db.ISB_BIA_Lock.Where(x => x.Tabellen_Kennzeichen == (int)table_Flag && x.Objekt_Id == id).FirstOrDefault();
                    return (lockObj != null) ? lockObj.BenutzerNnVn + " (" + lockObj.Benutzer + ")" : "";
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Datensatz-Lock konnte nicht abgerufen werden.", ex);
                return "";
            }
        }
        public bool Lock_Object(Table_Lock_Flags table_Flag, int id)
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    ISB_BIA_Lock lockObject = new ISB_BIA_Lock()
                    {
                        Tabellen_Kennzeichen = (int)table_Flag,
                        Objekt_Id = id,
                        BenutzerNnVn = _myShared.User.Surname + ", " + _myShared.User.Givenname,
                        Datum = DateTime.Now,
                        Benutzer = _myShared.User.Username,
                        ComputerName = Dns.GetHostEntry("").HostName
                    };
                    db.ISB_BIA_Lock.InsertOnSubmit(lockObject);
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Datensatz konnte nicht gelockt werden.", ex);
                return false;
            }
        }
        public bool Unlock_Object(Table_Lock_Flags table_Flag, int id)
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    db.ISB_BIA_Lock.DeleteAllOnSubmit(db.ISB_BIA_Lock.Where(x => x.Tabellen_Kennzeichen == (int)table_Flag && x.Objekt_Id == id).ToList());
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Datensatz-Lock konnte nicht entfernt werden.", ex);
                return false;
            }
        }
        public bool Unlock_AllObjectsForUserOnMachine()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    db.ISB_BIA_Lock.DeleteAllOnSubmit(db.ISB_BIA_Lock.Where(x => x.Benutzer ==  _myShared.User.Username && x.ComputerName == Dns.GetHostEntry("").HostName).ToList());
                    db.SubmitChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Es waren User-Locks vorhanden, die nicht entfernt werden konnten", ex);
                return false;
            }
        }
        public bool Unlock_AllObjects()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {
                    List<ISB_BIA_Lock> list = db.ISB_BIA_Lock.ToList();
                    db.ISB_BIA_Lock.DeleteAllOnSubmit(list);
                    //Logeintrag erzeugen
                    ISB_BIA_Log logEntry = new ISB_BIA_Log
                    {
                        Aktion = "Entfernen aller User-Locks durch Admin",
                        Tabelle = _myShared.Tbl_Lock,
                        Details = list.Count + " Locks entfernt",
                        Id_1 = 0,
                        Id_2 = 0,
                        Datum = DateTime.Now,
                        Benutzer =  _myShared.User.Username
                    };
                    db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                    db.SubmitChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Es waren User-Locks vorhanden, die nicht entfernt werden konnten", ex);
                return false;
            }
        }
        #endregion
    }
}
