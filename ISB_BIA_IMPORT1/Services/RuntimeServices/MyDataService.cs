using ISB_BIA_IMPORT1.Model;
using ISB_BIA_IMPORT1.ViewModel;
using ISB_BIA_IMPORT1.LINQ2SQL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Net;
using ISB_BIA_IMPORT1.Services.Interfaces;

namespace ISB_BIA_IMPORT1.Services
{
    class MyDataService : IMyDataService
    {
        readonly IMyDialogService _myDia;
        readonly IMySharedResourceService _myShared;
        readonly IMyMailNotificationService _myMail;

        public MyDataService(IMyDialogService myDia, IMySharedResourceService myShared, IMyMailNotificationService myMail)
        {
            this._myDia = myDia;
            this._myShared = myShared;
            this._myMail = myMail;
        }

        #region Datenmodell erstellen
        public bool CheckDBConnection()
        {
            string s = "";
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
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
        public bool CreateDataModel(DataTable dt_Processes, DataTable dt_Applications, DataTable dt_Relation, DataTable dt_InformationSegments, DataTable dt_InformationSegmentAttributes)
        {

            #region SQL Strings für Erstellen der Tabellen mit Headern analog zu Excel (!Trotzdem nicht ändern, da im Code per Linq2SQL Klassenmember aufegrufen werden)
            #region Tabellen Löschungs SQL Anweisungen
            string _sqlDropProc_App = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'" + _myShared.Tbl_Proz_App + "') DROP Table " + _myShared.Tbl_Proz_App;
            string _sqlDropProcesses = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'" + _myShared.Tbl_Prozesse + "') DROP Table " + _myShared.Tbl_Prozesse;
            string _sqlDropIS = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'" + _myShared.Tbl_IS + "') DROP Table " + _myShared.Tbl_IS;
            string _sqlDropISAttribut = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'" + _myShared.Tbl_IS_Attribute + "') DROP Table " + _myShared.Tbl_IS_Attribute;
            string _sqlDropSBA = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'" + _myShared.Tbl_Applikationen + "') DROP Table " + _myShared.Tbl_Applikationen;
            string _sqlDropDelta = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'" + _myShared.Tbl_Delta + "') DROP Table " + _myShared.Tbl_Delta;
            string _sqlDropLog = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'" + _myShared.Tbl_Log + "') DROP Table " + _myShared.Tbl_Log;
            string _sqlDropOEs = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'" + _myShared.Tbl_OEs + "') DROP Table " + _myShared.Tbl_OEs;
            string _sqlDropSettings = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'" + _myShared.Tbl_Settings + "') DROP Table " + _myShared.Tbl_Settings;
            string _sqlDropLock = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'" + _myShared.Tbl_Lock + "') DROP Table " + _myShared.Tbl_Lock;
            #endregion
            string sqlCreaProcApp =
                "    CREATE TABLE [dbo].[" + _myShared.Tbl_Proz_App + "] (" +
                "    [Prozess_Id] INT NOT NULL," +
                "    [Datum_Prozess] DATETIME NOT NULL DEFAULT(CONVERT(VARCHAR(23), '2018-12-31 23:59:59.500',121))," +
                "    [Applikation_Id] INT NOT NULL," +
                "    [Datum_Applikation] DATETIME NOT NULL DEFAULT(CONVERT(VARCHAR(23), '2018-12-31 23:59:59.500',121))," +
                "    [Relation] INT NOT NULL," +
                "    [Datum] DATETIME NOT NULL DEFAULT(CONVERT(VARCHAR(23), '2018-12-31 23:59:59.500',121))," +
                "    [Benutzer] NVARCHAR(50) NOT NULL DEFAULT('')," +
                "    PRIMARY KEY(Prozess_Id, Applikation_Id, Datum)," +
                "    Foreign Key(Prozess_Id, Datum_Prozess) references [" + _myShared.Tbl_Prozesse + "](Prozess_Id,Datum)," +
                "    Foreign Key(Applikation_Id, Datum_Applikation) references [" + _myShared.Tbl_Applikationen + "](Applikation_Id,Datum)" +
                "); ";

            //Tabellen erstellen, deren Spaltennamen den Spaltennamen des Quell-Excel-Sheets entsprechen
            //Wenn Spaltennamen in Excel geändert werden und das Datenmodell erneuert werden soll, müssen Anpassungen
            //im Code vorgenommen werden (Linq-to-SQL Modell erneuern, Variablenaufrufe im Code ändern)
            string sqlCreaSBA =
                "CREATE TABLE[dbo].[" + _myShared.Tbl_Applikationen + "](" +
                "    [" + dt_Applications.Columns[0] + "] INT NOT NULL," +
                "    [" + dt_Applications.Columns[1] + "] NVARCHAR(350) NOT NULL," +
                "    [" + dt_Applications.Columns[2] + "] NVARCHAR(350) NOT NULL," +
                "    [" + dt_Applications.Columns[3] + "] NVARCHAR(350) NOT NULL," +
                "    [" + dt_Applications.Columns[4] + "] NVARCHAR(350) NOT NULL," +
                "    [" + dt_Applications.Columns[5] + "] NVARCHAR(350) NOT NULL," +
                "    [" + dt_Applications.Columns[6] + "] NVARCHAR(350) NOT NULL," +
                "    [" + dt_Applications.Columns[7] + "] NVARCHAR(20) NOT NULL," +
                "    [" + dt_Applications.Columns[8] + "] INT NOT NULL," +
                "    [" + dt_Applications.Columns[9] + "] INT NOT NULL," +
                "    [" + dt_Applications.Columns[10] + "] INT NOT NULL," +
                "    [" + dt_Applications.Columns[11] + "] INT NOT NULL," +
                "    [" + dt_Applications.Columns[12] + "] INT NOT NULL," +
                "    [" + dt_Applications.Columns[13] + "] INT NOT NULL," +
                "    [Aktiv] INT NOT NULL DEFAULT(1)," +
                "    [Datum] DATETIME NOT NULL DEFAULT(CONVERT(VARCHAR(23), '2018-12-31 23:59:59.500',121))," +
                "    [Benutzer] NVARCHAR(50) NOT NULL DEFAULT('')," +
                "    PRIMARY KEY(" + dt_Applications.Columns[0] + ",Datum)" +
                ");";

            string sqlCreaProcesses =
                "CREATE TABLE[dbo].[" + _myShared.Tbl_Prozesse + "](" +
                "    [" + dt_Processes.Columns[0] + "] INT NOT NULL," +
                "    [" + dt_Processes.Columns[1] + "] NVARCHAR(200) NOT NULL," +
                "    [" + dt_Processes.Columns[2] + "] NVARCHAR(350) NOT NULL," +
                "    [" + dt_Processes.Columns[3] + "] NVARCHAR(350) NOT NULL," +
                "    [" + dt_Processes.Columns[4] + "] NVARCHAR(50) NOT NULL," +
                "    [" + dt_Processes.Columns[5] + "] NVARCHAR(10) NOT NULL," +
                "    [" + dt_Processes.Columns[6] + "] NVARCHAR(20) NOT NULL," +
                "    [" + dt_Processes.Columns[7] + "] NVARCHAR(20) NOT NULL," +
                "    [" + dt_Processes.Columns[8] + "] NVARCHAR(10) NOT NULL," +
                "    [" + dt_Processes.Columns[9] + "] NVARCHAR(10) NOT NULL," +
                "    [" + dt_Processes.Columns[10] + "] NVARCHAR(10) NOT NULL," +
                "    [" + dt_Processes.Columns[11] + "] INT NOT NULL," +
                "    [" + dt_Processes.Columns[12] + "] INT NOT NULL," +
                "    [" + dt_Processes.Columns[13] + "] INT NOT NULL," +
                "    [" + dt_Processes.Columns[14] + "] INT NOT NULL," +
                "    [" + dt_Processes.Columns[15] + "] INT NOT NULL," +
                "    [" + dt_Processes.Columns[16] + "] INT NOT NULL," +
                "    [" + dt_Processes.Columns[17] + "] NVARCHAR(350) NOT NULL," +
                "    [" + dt_Processes.Columns[18] + "] NVARCHAR(350) NOT NULL," +
                "    [" + dt_Processes.Columns[19] + "] NVARCHAR(350) NOT NULL," +
                "    [" + dt_Processes.Columns[20] + "] INT NOT NULL," +
                "    [" + dt_Processes.Columns[21] + "] INT NOT NULL," +
                "    [" + dt_Processes.Columns[22] + "] INT NOT NULL," +
                "    [" + dt_Processes.Columns[23] + "] VARCHAR(10) NOT NULL," +
                "    [" + dt_Processes.Columns[24] + "] VARCHAR(10) NOT NULL," +
                "    [" + dt_Processes.Columns[25] + "] VARCHAR(10) NOT NULL," +
                "    [" + dt_Processes.Columns[26] + "] VARCHAR(10) NOT NULL," +
                "    [" + dt_Processes.Columns[27] + "] VARCHAR(10) NOT NULL," +
                "    [Aktiv] INT NOT NULL DEFAULT(1)," +
                "    [Datum] DATETIME NOT NULL DEFAULT(CONVERT(VARCHAR(23), '2018-12-31 23:59:59.500',121))," +
                "    [Benutzer] NVARCHAR(50) NOT NULL DEFAULT('')," +
                "    PRIMARY KEY(" + dt_Processes.Columns[0] + ",Datum)," +
            ");";

            string sqlCreaIS =
                "CREATE TABLE[dbo].[" + _myShared.Tbl_IS + "](" +
                "    [" + dt_InformationSegments.Columns[0] + "] INT NOT NULL," +
                "    [" + dt_InformationSegments.Columns[1] + "] VARCHAR(10) NOT NULL," +
                "    [" + dt_InformationSegments.Columns[2] + "] VARCHAR(200) NOT NULL," +
                "    [" + dt_InformationSegments.Columns[3] + "] VARCHAR(2000) NOT NULL," +
                "    [" + dt_InformationSegments.Columns[4] + "] VARCHAR(2000) NOT NULL," +
                "    [" + dt_InformationSegments.Columns[5] + "] VARCHAR(10) NOT NULL," +
                "    [" + dt_InformationSegments.Columns[6] + "] VARCHAR(10) NOT NULL," +
                "    [" + dt_InformationSegments.Columns[7] + "] VARCHAR(10) NOT NULL," +
                "    [" + dt_InformationSegments.Columns[8] + "] VARCHAR(10) NOT NULL," +
                "    [" + dt_InformationSegments.Columns[9] + "] VARCHAR(10) NOT NULL," +
                "    [" + dt_InformationSegments.Columns[10] + "] VARCHAR(10) NOT NULL," +
                "    [" + dt_InformationSegments.Columns[11] + "] VARCHAR(10) NOT NULL," +
                "    [" + dt_InformationSegments.Columns[12] + "] VARCHAR(10) NOT NULL," +
                "    [" + dt_InformationSegments.Columns[13] + "] VARCHAR(10) NOT NULL," +
                "    [" + dt_InformationSegments.Columns[14] + "] VARCHAR(10) NOT NULL," +
                "    [Datum] DATETIME NOT NULL DEFAULT(CONVERT(VARCHAR(23), '2018-12-31 23:59:59.500',121))," +
                "    [Benutzer] NVARCHAR(50) NOT NULL DEFAULT('')," +
                "    PRIMARY KEY(" + dt_InformationSegments.Columns[0] + ", Datum)" +
                ");";

            string sqlCreaISAttribut =
                "CREATE TABLE[dbo].[" + _myShared.Tbl_IS_Attribute + "](" +
                "    [" + dt_InformationSegmentAttributes.Columns[0] + "] INT NOT NULL," +
                "    [" + dt_InformationSegmentAttributes.Columns[1] + "] VARCHAR(200) NOT NULL," +
                "    [" + dt_InformationSegmentAttributes.Columns[2] + "] VARCHAR(200) NOT NULL," +
                "    [" + dt_InformationSegmentAttributes.Columns[3] + "] INT NOT NULL," +
                "    [" + dt_InformationSegmentAttributes.Columns[4] + "] INT NOT NULL," +
                "    [" + dt_InformationSegmentAttributes.Columns[5] + "] INT NOT NULL," +
                "    [" + dt_InformationSegmentAttributes.Columns[6] + "] INT NOT NULL," +
                "    [" + dt_InformationSegmentAttributes.Columns[7] + "] INT NOT NULL," +
                "    [" + dt_InformationSegmentAttributes.Columns[8] + "] INT NOT NULL," +
                "    [Datum] DATETIME NOT NULL DEFAULT(CONVERT(VARCHAR(23), '2018-12-31 23:59:59.500',121))," +
                "    [Benutzer] NVARCHAR(50) NOT NULL DEFAULT('')," +
                "    PRIMARY KEY(" + dt_InformationSegmentAttributes.Columns[0] + ", Datum)" +
                ");";

            string sqlCreaDelta =
                "CREATE TABLE[dbo].[" + _myShared.Tbl_Delta + "](" +
                "    [Prozess_Id] INT NOT NULL," +
                "    [Prozess] VARCHAR(350) NOT NULL," +
                "    [Sub_Prozess] VARCHAR(350) NOT NULL," +
                "    [Datum_Prozess] DATETIME NOT NULL," +
                "    [Applikation_Id] INT NOT NULL," +
                "    [Applikation] VARCHAR(350) NOT NULL," +
                "    [Datum_Applikation] DATETIME NOT NULL," +
                "    [SZ_1] INT NOT NULL," +
                "    [SZ_2] INT NOT NULL," +
                "    [SZ_3] INT NOT NULL," +
                "    [SZ_4] INT NOT NULL," +
                "    [SZ_5] INT NOT NULL," +
                "    [SZ_6] INT NOT NULL," +
                "    [Datum] DATETIME NOT NULL DEFAULT(CONVERT(VARCHAR(23), '2018-12-31 23:59:59.500',121))," +
                "    PRIMARY KEY(Prozess_Id, Applikation_Id)," +
                "    Foreign Key(Prozess_Id,Datum_Prozess) references [" + _myShared.Tbl_Prozesse + "](Prozess_Id,Datum)," +
                "    Foreign Key(Applikation_Id,Datum_Applikation) references [" + _myShared.Tbl_Applikationen + "](Applikation_Id,Datum)" +
                ");";

            string sqlCreaLog =
                "CREATE TABLE[dbo].[" + _myShared.Tbl_Log + "] (" +
                "    [Id]                  INT IDENTITY(1, 1) NOT NULL PRIMARY KEY," +
                "    [Action] VARCHAR(200) NOT NULL," +
                "    [Tabelle] VARCHAR(200) NOT NULL," +
                "    [Details] VARCHAR(1000) NOT NULL," +
                "    [Id_1] INT NOT NULL," +
                "    [Id_2] INT NOT NULL," +
                "    [Datum] DATETIME NOT NULL DEFAULT(CONVERT(VARCHAR(23), '2018-12-31 23:59:59.500',121))," +
                "    [Benutzer] NVARCHAR(50) NOT NULL DEFAULT('')," +
                ");";

            string sqlCreaOEs =
                "CREATE TABLE[dbo].[" + _myShared.Tbl_OEs + "] (" +
                "    [Id]                  INT IDENTITY(1, 1) NOT NULL PRIMARY KEY," +
                "    [OE_Name] VARCHAR(200) NOT NULL," +
                "    [OE_Nummer] VARCHAR(200) NOT NULL," +
                "    [Datum] DATETIME NOT NULL DEFAULT(CONVERT(VARCHAR(23), '2018-12-31 23:59:59.500',121))," +
                "    [Benutzer] NVARCHAR(50) NOT NULL DEFAULT('')," +
                "    Unique (OE_Name, OE_Nummer)" +
                ");";

            string sqlCreaSettings =
                "CREATE TABLE[dbo].[" + _myShared.Tbl_Settings + "] (" +
                "    [Id]                  INT IDENTITY(1, 1) NOT NULL PRIMARY KEY," +
                "    [SZ_1_Name] VARCHAR(100) NOT NULL," +
                "    [SZ_2_Name] VARCHAR(100) NOT NULL," +
                "    [SZ_3_Name] VARCHAR(100) NOT NULL," +
                "    [SZ_4_Name] VARCHAR(100) NOT NULL," +
                "    [SZ_5_Name] VARCHAR(100) NOT NULL," +
                "    [SZ_6_Name] VARCHAR(100) NOT NULL," +
                "    [Neue_Schutzziele_aktiviert] VARCHAR(10) NOT NULL," +
                "    [BIA_abgeschlossen] VARCHAR(10) NOT NULL," +
                "    [SBA_abgeschlossen] VARCHAR(10) NOT NULL," +
                "    [Delta_abgeschlossen] VARCHAR(10) NOT NULL," +
                "    [Attribut9_aktiviert] VARCHAR(10) NOT NULL," +
                "    [Attribut10_aktiviert] VARCHAR(10) NOT NULL," +
                "    [Multi_Save] VARCHAR(10) NOT NULL," +
                "    [Datum] DATETIME NOT NULL DEFAULT(CONVERT(VARCHAR(23), '2018-12-31 23:59:59.500',121))," +
                "    [Benutzer] NVARCHAR(50) NOT NULL," +
                ");";

            string sqlCreaLock =
                "CREATE TABLE[dbo].[" + _myShared.Tbl_Lock + "] (" +
                "    [Id]                  INT IDENTITY(1, 1) NOT NULL PRIMARY KEY," +
                "    [Table_Flag] INT NOT NULL," +
                "    [Object_Id] INT NOT NULL," +
                "    [Datum] DATETIME NOT NULL DEFAULT(CONVERT(VARCHAR(23), '2018-12-31 23:59:59.500',121))," +
                "    [BenutzerNnVn] NVARCHAR(50) NOT NULL DEFAULT('')," +
                "    [Benutzer] NVARCHAR(50) NOT NULL DEFAULT('')," +
                "    [ComputerName] NVARCHAR(50) NOT NULL DEFAULT('')"+
                ");";
            #endregion
            List<string> sqlCommandList = new List<string>();
            sqlCommandList.Add(_sqlDropLog);
            sqlCommandList.Add(sqlCreaLog);
            sqlCommandList.Add(_sqlDropOEs);
            sqlCommandList.Add(sqlCreaOEs);
            sqlCommandList.Add(_sqlDropSettings);
            sqlCommandList.Add(sqlCreaSettings);
            sqlCommandList.Add(_sqlDropLock);
            sqlCommandList.Add(sqlCreaLock);
            sqlCommandList.Add(_sqlDropISAttribut);
            sqlCommandList.Add(sqlCreaISAttribut);

            sqlCommandList.Add(_sqlDropDelta);
            sqlCommandList.Add(_sqlDropProc_App);
            sqlCommandList.Add(_sqlDropProcesses);
            sqlCommandList.Add(_sqlDropSBA);
            sqlCommandList.Add(sqlCreaSBA);

            sqlCommandList.Add(_sqlDropIS);
            sqlCommandList.Add(sqlCreaIS);

            sqlCommandList.Add(sqlCreaProcesses);
            sqlCommandList.Add(sqlCreaProcApp);
            sqlCommandList.Add(sqlCreaDelta);


            try
            {
                //Löschen der Tabellen wenn vorhanden und neu erstellen
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {                  
                    foreach (string s in sqlCommandList)
                    {
                        db.ExecuteCommand(s);
                    }
                    db.SubmitChanges();
                }

                //Schreiben der DataTables in die Datenbank (Initialer Stand der Daten)
                using (SqlConnection con = new SqlConnection(_myShared.ConnectionString))
                {
                    SQLBulkCopy(_myShared.Tbl_Prozesse, con, dt_Processes);
                    SQLBulkCopy(_myShared.Tbl_Applikationen, con, dt_Applications);
                    SQLBulkCopy(_myShared.Tbl_Proz_App, con, dt_Relation);
                    SQLBulkCopy(_myShared.Tbl_IS, con, dt_InformationSegments);
                    SQLBulkCopy(_myShared.Tbl_IS_Attribute, con, dt_InformationSegmentAttributes);

                    dt_Applications.Dispose();
                    dt_Processes.Dispose();
                    dt_InformationSegments.Dispose();
                    dt_InformationSegmentAttributes.Dispose();
                    dt_Relation.Dispose();
                }
                
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {

                    //Initialer Inhalt der Tabelle für OE-Gruppenbezeichnungen wird aus Prozesstabelle entnommen
                    List<ISB_BIA_Prozesse> query = db.ISB_BIA_Prozesse.GroupBy(x => x.OE_Filter).Select(group => group.FirstOrDefault()).ToList();
                    foreach (ISB_BIA_Prozesse p in query)
                    {
                        ISB_BIA_OEs oe = new ISB_BIA_OEs()
                        {
                            OE_Name = p.OE_Filter,
                            OE_Nummer = "",
                            Datum = DateTime.Now,
                            Benutzer = ""
                        };
                        db.ISB_BIA_OEs.InsertOnSubmit(oe);
                    }

                    //Initiale Einstellungen werden gesetzt
                    ISB_BIA_Settings initial_settings = new ISB_BIA_Settings()
                    {
                        SZ_1_Name = "Verfügbarkeit",
                        SZ_2_Name = "Integrität",
                        SZ_3_Name = "Vertraulichkeit",
                        SZ_4_Name = "Authentizität",
                        SZ_5_Name = "Verbindlichkeit",
                        SZ_6_Name = "Zurechenbarkeit",
                        Neue_Schutzziele_aktiviert = "Nein",
                        BIA_abgeschlossen = "Nein",
                        SBA_abgeschlossen = "Nein",
                        Delta_abgeschlossen = "Nein",
                        Attribut9_aktiviert = "Nein",
                        Attribut10_aktiviert = "Nein",
                        Multi_Save = "Nein",
                        Datum = DateTime.Now,
                        Benutzer = Environment.UserName
                    };
                    db.ISB_BIA_Settings.InsertOnSubmit(initial_settings);
                    db.SubmitChanges();

                }
                
                _myDia.ShowInfo("Datenmodell erfolgreich erneuert!");
                return true;
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Fehler beim erstellen des Datenmodells.", ex);
                return false;
            }
        }
        public void SQLBulkCopy(string tableName, SqlConnection con, DataTable dt)
        {
            try
            {
                using (var bulkCopy = new SqlBulkCopy(con.ConnectionString, SqlBulkCopyOptions.KeepIdentity))
                {
                    bulkCopy.BulkCopyTimeout = 600;
                    bulkCopy.DestinationTableName = tableName;
                    
                    foreach (DataColumn col in dt.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                    }
                    
                    bulkCopy.WriteToServer(dt);
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Fehler beim Schreiben der Quelldaten in das Datenmodell.", ex);
            }
        }
        #endregion

        #region Datensatz-Lock Operationen
        public string GetObjectLocked(Table_Lock_Flags table_Flag, int id)
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    ISB_BIA_Lock lockObj = (db.ISB_BIA_Lock.Where(x => x.Table_Flag == (int)table_Flag && x.Object_Id == id).FirstOrDefault());
                    return (lockObj != null) ? lockObj.BenutzerNnVn + " (" + lockObj.Benutzer + ")" : "";
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Datensatz-Lock konnte nicht abgerufen werden.", ex);
                return "";
            }
        }
        public bool LockObject(Table_Lock_Flags table_Flag, int id)
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    ISB_BIA_Lock lockObject = new ISB_BIA_Lock()
                    {
                        Table_Flag = (int)table_Flag,
                        Object_Id = id,
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
        public bool UnlockObject(Table_Lock_Flags table_Flag, int id)
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    db.ISB_BIA_Lock.DeleteAllOnSubmit(db.ISB_BIA_Lock.Where(x => x.Table_Flag == (int)table_Flag && x.Object_Id == id).ToList());
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
        public bool UnlockAllObjectsForUserOnMachine()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    db.ISB_BIA_Lock.DeleteAllOnSubmit(db.ISB_BIA_Lock.Where(x => x.Benutzer == Environment.UserName && x.ComputerName == Dns.GetHostEntry("").HostName).ToList());
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
        public bool UnlockAllObjects()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    List<ISB_BIA_Lock> list = db.ISB_BIA_Lock.ToList();
                    db.ISB_BIA_Lock.DeleteAllOnSubmit(list);
                    //Logeintrag erzeugen
                    ISB_BIA_Log logEntry = new ISB_BIA_Log
                    {
                        Action = "Entfernen aller User-Locks durch Admin",
                        Tabelle = _myShared.Tbl_Lock,
                        Details = list.Count + " Locks entfernt",
                        Id_1 = 0,
                        Id_2 = 0,
                        Datum = DateTime.Now,
                        Benutzer = Environment.UserName
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

        #region Process
        public Process_Model GetProcessModelFromDB(int id)
        {
            try
            {
                //Letzte Version des Prozesses abrufen
                ISB_BIA_Prozesse linqProc;
                ObservableCollection<ISB_BIA_Applikationen> linqApps= new ObservableCollection<ISB_BIA_Applikationen>();
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    //Prozess
                    linqProc = db.ISB_BIA_Prozesse.Where(c => c.Prozess_Id == id).OrderByDescending(p => p.Datum).FirstOrDefault();
                    //Applikationsliste
                    List<ISB_BIA_Prozesse_Applikationen> proc_AppCurrent = db.ISB_BIA_Prozesse_Applikationen.Where(x => x.Prozess_Id == id).GroupBy(y => y.Applikation_Id).Select(z => z.OrderByDescending(q => q.Datum).FirstOrDefault()).ToList();
                    List<int> listIdCurrentApplications = proc_AppCurrent.Where(x => x.Relation == 1).Select(y => y.Applikation_Id).ToList();
                    if (listIdCurrentApplications.Count == 0)
                        linqApps = new ObservableCollection<ISB_BIA_Applikationen>();
                    else
                        linqApps = new ObservableCollection<ISB_BIA_Applikationen>(db.ISB_BIA_Applikationen.Where(x => listIdCurrentApplications.Contains(x.Applikation_Id)).GroupBy(y => y.Applikation_Id).Select(z => z.OrderByDescending(q => q.Datum).FirstOrDefault()).OrderBy(k => k.Applikation_Id).ToList());

                }
                //Falls existiert
                if (linqProc != null && linqApps != null)
                {
                    Process_Model result = new Process_Model()
                    {
                        //"Kritischer Prozess" im Process_Model initial auf "Nein" gesetzt, wird berechnet in den Settern der anderen Eigenschaften
                        //Kritischer_Prozess = (linqProc.Kritischer_Prozess == "x") ? "Ja" : "Nein",
                        Prozess_Id = linqProc.Prozess_Id,
                        OE_Filter = linqProc.OE_Filter,
                        Prozess = linqProc.Prozess,
                        Sub_Prozess = linqProc.Sub_Prozess,
                        Prozessverantwortlicher = linqProc.Prozessverantwortlicher,
                        Kritikalität_des_Prozesses = linqProc.Kritikalität_des_Prozesses,
                        Reifegrad_des_Prozesses = linqProc.Reifegrad_des_Prozesses,
                        Regulatorisch = (linqProc.Regulatorisch == "x") ? true : false,
                        Reputatorisch = (linqProc.Reputatorisch == "x") ? true : false,
                        Finanziell = (linqProc.Finanziell == "x") ? true : false,
                        SZ_1 = (SZ_Values)linqProc.SZ_1,
                        SZ_2 = (SZ_Values)linqProc.SZ_2,
                        SZ_3 = (SZ_Values)linqProc.SZ_3,
                        SZ_4 = (SZ_Values)linqProc.SZ_4,
                        SZ_5 = (SZ_Values)linqProc.SZ_5,
                        SZ_6 = (SZ_Values)linqProc.SZ_6,
                        Vorgelagerte_Prozesse = linqProc.Vorgelagerte_Prozesse,
                        Nachgelagerte_Prozesse = linqProc.Nachgelagerte_Prozesse,
                        Servicezeit_Helpdesk = linqProc.Servicezeit_Helpdesk,
                        RPO_Datenverlustzeit_Recovery_Point_Objective = linqProc.RPO_Datenverlustzeit_Recovery_Point_Objective,
                        RTO_Wiederanlaufzeit_Recovery_Time_Objective = linqProc.RTO_Wiederanlaufzeit_Recovery_Time_Objective,
                        RTO_Wiederanlaufzeit_Recovery_Time_Objective_Notfall = linqProc.RTO_Wiederanlaufzeit_Recovery_Time_Objective_Notfall,
                        Relevantes_IS_1 = linqProc.Relevantes_IS_1,
                        Relevantes_IS_2 = linqProc.Relevantes_IS_2,
                        Relevantes_IS_3 = linqProc.Relevantes_IS_3,
                        Relevantes_IS_4 = linqProc.Relevantes_IS_4,
                        Relevantes_IS_5 = linqProc.Relevantes_IS_5,
                        Aktiv = linqProc.Aktiv,
                        Datum = linqProc.Datum,
                        Benutzer = linqProc.Benutzer,
                        ApplicationList = linqApps,
                    };
                    return result;
                }
                else
                {
                    _myDia.ShowError("Prozessdaten konnten nicht abgerufen werden");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Prozessdaten konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public ISB_BIA_Prozesse MapProcessModelToDB(Process_Model p)
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext())
                {
                    List<ISB_BIA_Informationssegmente> ISListForDates = db.ISB_BIA_Informationssegmente
                        .GroupBy(x => x.Name)
                        .Select(c => c.OrderByDescending(v => v.Datum).FirstOrDefault()).ToList();
                    ISB_BIA_Prozesse res = new ISB_BIA_Prozesse()
                    {
                        Prozess_Id = p.Prozess_Id,
                        OE_Filter = p.OE_Filter,
                        Prozess = p.Prozess,
                        Sub_Prozess = p.Sub_Prozess,
                        Prozessverantwortlicher = p.Prozessverantwortlicher,
                        Kritikalität_des_Prozesses = p.Kritikalität_des_Prozesses,
                        Kritischer_Prozess = (p.Kritischer_Prozess == "Ja") ? "x" : "",
                        Reifegrad_des_Prozesses = p.Reifegrad_des_Prozesses,
                        Regulatorisch = (p.Regulatorisch) ? "x" : "",
                        Reputatorisch = (p.Reputatorisch) ? "x" : "",
                        Finanziell = (p.Finanziell) ? "x" : "",
                        SZ_1 = (int) p.SZ_1,
                        SZ_2 = (int) p.SZ_2,
                        SZ_3 = (int) p.SZ_3,
                        SZ_4 = (int) p.SZ_4,
                        SZ_5 = (int) p.SZ_5,
                        SZ_6 = (int) p.SZ_6,
                        Vorgelagerte_Prozesse = p.Vorgelagerte_Prozesse,
                        Nachgelagerte_Prozesse = p.Nachgelagerte_Prozesse,
                        Servicezeit_Helpdesk = p.Servicezeit_Helpdesk,
                        RPO_Datenverlustzeit_Recovery_Point_Objective = p.RPO_Datenverlustzeit_Recovery_Point_Objective,
                        RTO_Wiederanlaufzeit_Recovery_Time_Objective = p.RTO_Wiederanlaufzeit_Recovery_Time_Objective,
                        RTO_Wiederanlaufzeit_Recovery_Time_Objective_Notfall =
                            p.RTO_Wiederanlaufzeit_Recovery_Time_Objective_Notfall,
                        Relevantes_IS_1 = p.Relevantes_IS_1,
                        Relevantes_IS_2 = p.Relevantes_IS_2,
                        Relevantes_IS_3 = p.Relevantes_IS_3,
                        Relevantes_IS_4 = p.Relevantes_IS_4,
                        Relevantes_IS_5 = p.Relevantes_IS_5,
                        Aktiv = p.Aktiv,
                        Datum = p.Datum,
                        Benutzer = p.Benutzer
                    };
                    return res;
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Fehler beim Mappen des Prozesses", ex);
                return null;
            }
        }
        public Dictionary<string, string> GetISList()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    List<ISB_BIA_Informationssegmente> listIS = db.ISB_BIA_Informationssegmente.Where(x => x.Segment != "Lorem ipsum").GroupBy(y => y.Informationssegment_Id).Select(c => c.OrderByDescending(d => d.Datum).FirstOrDefault()).ToList();
                    listIS.Insert(0, new ISB_BIA_Informationssegmente() { Name = "", Segment = "<leer>" });
                    return new Dictionary<string, string>(listIS.Select(t => new { t.Name, t.Segment }).ToDictionary(t => t.Name, t => t.Segment));
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Liste der Segmente konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public ObservableCollection<ISB_BIA_Prozesse> GetProcessesByOE(ObservableCollection<string> listOE)
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    return new ObservableCollection<ISB_BIA_Prozesse>(
                        db.ISB_BIA_Prozesse.Where(n => listOE.Contains(n.OE_Filter)).GroupBy(p => p.Prozess_Id)
                        .Select(g => g.OrderByDescending(p => p.Datum).FirstOrDefault()).Where(v => v.Aktiv == 1).OrderBy(x => x.Prozess_Id).ToList());
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Prozesse konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public ObservableCollection<ISB_BIA_Prozesse> GetProcesses(DateTime? date = null)
        {
            try
            {
                if (!date.HasValue) date = DateTime.Now;

                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    return new ObservableCollection<ISB_BIA_Prozesse>(
                        db.ISB_BIA_Prozesse.Where(d => d.Datum <= date).GroupBy(p => p.Prozess_Id)
                        .Select(g => g.OrderByDescending(p => p.Datum).FirstOrDefault()).
                        OrderBy(x => x.Prozess_Id).ToList());
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Prozesse konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public ObservableCollection<ISB_BIA_Prozesse> GetActiveProcesses()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    return new ObservableCollection<ISB_BIA_Prozesse>(
                        db.ISB_BIA_Prozesse.GroupBy(p => p.Prozess_Id)
                        .Select(g => g.OrderByDescending(p => p.Datum).FirstOrDefault()).Where(v => v.Aktiv == 1).
                        OrderBy(x => x.Prozess_Id).ToList());
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Prozesse konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public ObservableCollection<string> GetProcessOwner()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    return new ObservableCollection<string>(db.ISB_BIA_Prozesse.Where(n => n.Prozessverantwortlicher != "" && n.Prozessverantwortlicher != " ").Select(p => p.Prozessverantwortlicher).Distinct());
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Prozesseigentümer konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public ObservableCollection<string> GetOEsForUser(string userOE)
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    return new ObservableCollection<string>(db.ISB_BIA_OEs.Where(x => x.OE_Nummer.StartsWith(userOE)).Select(p => p.OE_Name).Distinct());
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("OE's konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public ObservableCollection<string> GetOEs()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    return new ObservableCollection<string>(db.ISB_BIA_OEs.Select(p => p.OE_Name).Distinct());
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("OE's konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public ObservableCollection<string> GetPreProcesses()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    return new ObservableCollection<string>(db.ISB_BIA_Prozesse.Select(p => p.Vorgelagerte_Prozesse).ToList().Distinct(StringComparer.Ordinal).OrderBy(a => a));
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Vorgelagerte Prozesse konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public ObservableCollection<string> GetPostProcesses()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    return new ObservableCollection<string>(db.ISB_BIA_Prozesse.Select(p => p.Nachgelagerte_Prozesse).ToList().Distinct(StringComparer.Ordinal).OrderBy(a => a));
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Nachgelagerte Prozesse konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public ObservableCollection<ISB_BIA_Prozesse> GetProcessHistory(int process_id)
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    return new ObservableCollection<ISB_BIA_Prozesse>(
                        db.ISB_BIA_Prozesse.Where(p => p.Prozess_Id == process_id)
                        .OrderByDescending(p => p.Datum).ToList());
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Prozesse konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public bool InsertProcessAndRelations(Process_Model p, ProcAppMode mode, ObservableCollection<ISB_BIA_Applikationen> add, ObservableCollection<ISB_BIA_Applikationen> remove)
        {
            if (p.IsValid)
            {
                //Bei Bearbeitung eines inaktiven Prozesses (durch Admin oder CISO) beim Speichern fragen, ob er auf aktiv gesetzt werden soll
                if (p.Aktiv == 0)
                {
                    p.Aktiv = (_myDia.ShowQuestion("Der Prozess ist momentan auf inaktiv gesetzt. Möchten Sie den Prozess auf aktiv setzen?", "Prozess aktivieren")) ? 1 : 0;
                }
                try
                {
                    //In Datenbank schreiben
                    using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                    {
                        DateTime d = DateTime.Now;
                        //Bei Neuanlage neue ID berechnen
                        if (mode == ProcAppMode.New)
                            p.Prozess_Id = db.ISB_BIA_Prozesse.Max(x => x.Prozess_Id) + 1;
                        p.Benutzer = Environment.UserName;
                        p.Datum = d;
                        //Nach DB-Format Mappen und einfügen
                        db.ISB_BIA_Prozesse.InsertOnSubmit(MapProcessModelToDB(p));
                        //Logeintrag erzeugen
                        ISB_BIA_Log logEntry = new ISB_BIA_Log
                        {
                            Action = "Ändern/Erstellen eines Prozesses",
                            Tabelle = _myShared.Tbl_Prozesse,
                            Details = "Id = " + p.Prozess_Id + ", Name = '" + p.Prozess + "', Sub-Name = '" + p.Sub_Prozess + "'",
                            Id_1 = p.Prozess_Id,
                            Id_2 = 0,
                            Datum = d,
                            Benutzer = p.Benutzer
                        };
                        db.ISB_BIA_Log.InsertOnSubmit(logEntry);

                        #region relation
                        int k = add.Count;
                        int i = 0;
                        List<ISB_BIA_Applikationen> result = add.Concat(remove).ToList();
                        foreach (ISB_BIA_Applikationen a in result)
                        {
                            i++;
                            ISB_BIA_Prozesse_Applikationen proc_app = new ISB_BIA_Prozesse_Applikationen
                            {
                                Applikation_Id = a.Applikation_Id,
                                Datum_Applikation = a.Datum,
                                Prozess_Id = p.Prozess_Id,
                                Datum_Prozess = d,
                                Relation = (i <= k) ? 1 : 0,
                                Datum = d,
                                Benutzer = Environment.UserName
                            };

                            //Schreiben in Datenbank
                            db.ISB_BIA_Prozesse_Applikationen.InsertOnSubmit(proc_app);

                            string s = (proc_app.Relation == 1) ? "Verknüpfung" : "Trennung";

                            ISB_BIA_Log logEntryRelation = new ISB_BIA_Log
                            {
                                Action = "Ändern einer Prozesses-Applikations-Relation: " + s,
                                Tabelle = _myShared.Tbl_Proz_App,
                                Details = "Prozess Id = " + proc_app.Prozess_Id + ", App. Id = " + proc_app.Applikation_Id + ", App. Name = '" + a.IT_Anwendung_System + "'",
                                Id_1 = proc_app.Prozess_Id,
                                Id_2 = proc_app.Applikation_Id,
                                Datum = d,
                                Benutzer = p.Benutzer,
                            };
                            db.ISB_BIA_Log.InsertOnSubmit(logEntryRelation);
                        }
                        #endregion

                        #region Notification-Mail senden
                        string subject;
                        if (mode == ProcAppMode.Change)
                        {
                            subject = "BIA-Tool Auto-Notification: Prozess bearbeitet";
                        }
                        else
                        {
                            subject = "BIA-Tool Auto-Notification: Prozess erstellt";
                        }
                        string body = "Prozess-ID: " + p.Prozess_Id + Environment.NewLine +
                            "Prozess Name: " + p.Prozess + Environment.NewLine +
                            "OE: " + p.OE_Filter + Environment.NewLine +
                            "Prozessverantwortlicher: " + p.Prozessverantwortlicher + Environment.NewLine +
                            "Datum: " + d;
                        _myMail.Send_NotificationMail(subject, body, _myShared.Current_Environment);
                        #endregion
                        db.SubmitChanges();
                    }
                    _myDia.ShowInfo("Prozess gespeichert");
                    return true;
                }
                catch (Exception ex1)
                {
                    //LogEntry bei Fehler erstellen + Schreiben in Datenbank
                    try
                    {
                        using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                        {
                            ISB_BIA_Log logEntry = new ISB_BIA_Log
                            {
                                Action = "Fehler: Ändern/Erstellen eines Prozesses",
                                Tabelle = _myShared.Tbl_Prozesse,
                                Details = ex1.Message,
                                Id_1 = p.Prozess_Id,
                                Id_2 = 0,
                                Datum = DateTime.Now,
                                Benutzer = Environment.UserName
                            };

                            db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                            db.SubmitChanges();
                            _myDia.ShowError("Fehler beim Speichern des Prozesses!\nEin Log Eintrag wurde erzeugt.", ex1);
                            return false;
                        }
                    }
                    catch (Exception ex2)
                    {
                        _myDia.ShowError("Fehler beim Speichern des Prozesses!\nEin Log Eintrag konnte ebenfalls nicht erzeugt werden.", ex2);
                        return false;
                    }
                }
            }
            else
            {
                _myDia.ShowWarning("Bitte füllen Sie alle Pflichtfelder aus.\nPflichtfelder sind fett markiert und werden für Sie nun rot gekennzeichnet.");
                return false;
            }
        }
        #region Prozess löschen (Setzen des Aktiv Flags in der Datenbank auf 0)
        public ISB_BIA_Prozesse DeleteProcess(ISB_BIA_Prozesse p)
        {
            ISB_BIA_Prozesse toDelete = new ISB_BIA_Prozesse
            {
                Prozess_Id = p.Prozess_Id,
                OE_Filter = p.OE_Filter,
                Prozess = p.Prozess,
                Sub_Prozess = p.Sub_Prozess,
                Prozessverantwortlicher = p.Prozessverantwortlicher,
                Kritischer_Prozess = p.Kritischer_Prozess,
                Kritikalität_des_Prozesses = p.Kritikalität_des_Prozesses,
                Reifegrad_des_Prozesses = p.Reifegrad_des_Prozesses,
                Regulatorisch = p.Regulatorisch,
                Reputatorisch = p.Reputatorisch,
                Finanziell = p.Finanziell,
                SZ_1 = p.SZ_1,
                SZ_2 = p.SZ_2,
                SZ_3 = p.SZ_3,
                SZ_4 = p.SZ_4,
                SZ_5 = p.SZ_5,
                SZ_6 = p.SZ_6,
                Vorgelagerte_Prozesse = p.Vorgelagerte_Prozesse,
                Nachgelagerte_Prozesse = p.Nachgelagerte_Prozesse,
                Servicezeit_Helpdesk = p.Servicezeit_Helpdesk,
                RPO_Datenverlustzeit_Recovery_Point_Objective = p.RPO_Datenverlustzeit_Recovery_Point_Objective,
                RTO_Wiederanlaufzeit_Recovery_Time_Objective = p.RTO_Wiederanlaufzeit_Recovery_Time_Objective,
                RTO_Wiederanlaufzeit_Recovery_Time_Objective_Notfall = p.RTO_Wiederanlaufzeit_Recovery_Time_Objective_Notfall,
                Relevantes_IS_1 = p.Relevantes_IS_1,
                Relevantes_IS_2 = p.Relevantes_IS_2,
                Relevantes_IS_3 = p.Relevantes_IS_3,
                Relevantes_IS_4 = p.Relevantes_IS_4,
                Relevantes_IS_5 = p.Relevantes_IS_5,
                Datum = DateTime.Now,
                Benutzer = Environment.UserName,
                Aktiv = 0
            };
            bool res = _myDia.ShowQuestion("Möchten Sie den Prozess wirklich löschen?", "Prozess löschen");
            return (res) ? TryInsert(toDelete) : null;
        }
        public ISB_BIA_Prozesse TryInsert(ISB_BIA_Prozesse toDelete)
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    toDelete.Datum = DateTime.Now;
                    db.ISB_BIA_Prozesse.InsertOnSubmit(toDelete);
                    //Logeintrag erzeugen
                    ISB_BIA_Log logEntry = new ISB_BIA_Log
                    {
                        Action = "Löschen eines Prozesses (Setzen auf inaktiv)",
                        Tabelle = _myShared.Tbl_Prozesse,
                        Details = "Id = " + toDelete.Prozess_Id + ", Name = '" + toDelete.Prozess + "', Sub-Name = '" + toDelete.Sub_Prozess + "'",
                        Id_1 = toDelete.Prozess_Id,
                        Id_2 = 0,
                        Datum = toDelete.Datum,
                        Benutzer = Environment.UserName
                    };
                    db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                    db.SubmitChanges();
                    #region Mail Notification senden
                    string subject = "BIA-Tool Auto-Notification: Prozess gelöscht";
                    string body = "Prozess-ID: " + toDelete.Prozess_Id + Environment.NewLine +
                        "Prozess Name: " + toDelete.Prozess + Environment.NewLine +
                        "OE: " + toDelete.OE_Filter + Environment.NewLine +
                        "Prozessverantwortlicher: " + toDelete.Prozessverantwortlicher + Environment.NewLine +
                        "Datum: " + toDelete.Datum;
                    _myMail.Send_NotificationMail(subject, body, _myShared.Current_Environment);
                    #endregion
                }
                _myDia.ShowMessage("Prozess gelöscht");
                return toDelete;
            }
            catch (Exception ex1)
            {
                //Logeintrag bei Fehler
                try
                {
                    using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                    {
                        ISB_BIA_Log logEntry = new ISB_BIA_Log
                        {
                            Action = "Fehler: Löschen eines Prozesses",
                            Tabelle = _myShared.Tbl_Prozesse,
                            Details = ex1.Message,
                            Id_1 = toDelete.Prozess_Id,
                            Id_2 = 0,
                            Datum = toDelete.Datum,
                            Benutzer = Environment.UserName
                        };
                        db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                        db.SubmitChanges();
                        _myDia.ShowError("Prozess konnte nicht gelöscht werden.\n", ex1);
                        return null;
                    }
                }
                catch (Exception ex2)
                {
                    _myDia.ShowError("Prozess konnte nicht gelöscht werden!\nEin Log Eintrag konnte ebenfalls nicht erzeugt werden.\n", ex2);
                    return null;
                }
            }
        }
        #endregion
        public bool SaveAllProcesses(ObservableCollection<ISB_BIA_Prozesse> pList)
        {
            List<ISB_BIA_Prozesse> refreshedProcesses = new List<ISB_BIA_Prozesse>();

            bool res = _myDia.ShowQuestion("Möchten Sie die ausgewählten Prozesse wirklich ohne Änderungen aktualisieren?", "Auswahl bestätigen");
            if (res)
            {
                //Liste der gesperrten Prozesse
                List<ISB_BIA_Prozesse> lockedList = new List<ISB_BIA_Prozesse>();
                //String für die Nachricht welcher Prozess durch welchen User gesperrt ist
                string lockedListStringMsg = "";
                foreach (ISB_BIA_Prozesse p in pList)
                {
                    string user = GetObjectLocked(Table_Lock_Flags.Process, p.Prozess_Id);
                    if (user != "")
                    {
                        //Wenn Prozess gesperrt zur Liste hinzufügen
                        lockedList.Add(p);
                        lockedListStringMsg = lockedListStringMsg + p.Prozess + " (geöffnet von: " + user + ")\n";
                    }
                }
                //Falls kein Prozess gesperrt ist
                if (lockedList.Count == 0)
                {
                    //Schreiben in DB
                    try
                    {
                        using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                        {
                            foreach (ISB_BIA_Prozesse process_old in pList)
                            {
                                //zu speichernden Prozess in neues Objekt "kopieren"
                                ISB_BIA_Prozesse process_refresh = new ISB_BIA_Prozesse
                                {
                                    Prozess_Id = process_old.Prozess_Id,
                                    Aktiv = process_old.Aktiv,
                                    OE_Filter = process_old.OE_Filter,
                                    Prozess = process_old.Prozess,
                                    Sub_Prozess = process_old.Sub_Prozess,
                                    Prozessverantwortlicher = process_old.Prozessverantwortlicher,
                                    Kritikalität_des_Prozesses = process_old.Kritikalität_des_Prozesses,
                                    Kritischer_Prozess = process_old.Kritischer_Prozess,
                                    Reifegrad_des_Prozesses = process_old.Reifegrad_des_Prozesses,
                                    Regulatorisch = process_old.Regulatorisch,
                                    Reputatorisch = process_old.Reputatorisch,
                                    Finanziell = process_old.Finanziell,
                                    SZ_1 = process_old.SZ_1,
                                    SZ_2 = process_old.SZ_2,
                                    SZ_3 = process_old.SZ_3,
                                    SZ_4 = process_old.SZ_4,
                                    SZ_5 = process_old.SZ_5,
                                    SZ_6 = process_old.SZ_6,
                                    Vorgelagerte_Prozesse = process_old.Vorgelagerte_Prozesse,
                                    Nachgelagerte_Prozesse = process_old.Nachgelagerte_Prozesse,
                                    Servicezeit_Helpdesk = process_old.Servicezeit_Helpdesk,
                                    RPO_Datenverlustzeit_Recovery_Point_Objective = process_old.RPO_Datenverlustzeit_Recovery_Point_Objective,
                                    RTO_Wiederanlaufzeit_Recovery_Time_Objective = process_old.RTO_Wiederanlaufzeit_Recovery_Time_Objective,
                                    RTO_Wiederanlaufzeit_Recovery_Time_Objective_Notfall = process_old.RTO_Wiederanlaufzeit_Recovery_Time_Objective_Notfall,
                                    Relevantes_IS_1 = process_old.Relevantes_IS_1,
                                    Relevantes_IS_2 = process_old.Relevantes_IS_2,
                                    Relevantes_IS_3 = process_old.Relevantes_IS_3,
                                    Relevantes_IS_4 = process_old.Relevantes_IS_4,
                                    Relevantes_IS_5 = process_old.Relevantes_IS_5,
                                };
                                process_refresh.Benutzer = Environment.UserName;
                                process_refresh.Datum = DateTime.Now;
                                db.ISB_BIA_Prozesse.InsertOnSubmit(process_refresh);
                                //Logeintrag erzeugen
                                ISB_BIA_Log logEntry = new ISB_BIA_Log
                                {
                                    Datum = process_refresh.Datum,
                                    Action = "Aktualisieren eines Prozesses ohne Änderungen",
                                    Tabelle = _myShared.Tbl_Prozesse,
                                    Details = "Id = " + process_refresh.Prozess_Id + ", Name = '" + process_refresh.Prozess + "', Sub-Name = '" + process_refresh.Sub_Prozess + "'",
                                    Id_1 = process_refresh.Prozess_Id,
                                    Id_2 = 0,
                                    Benutzer = Environment.UserName
                                };
                                db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                                refreshedProcesses.Add(process_refresh);
                            }
                            db.SubmitChanges();
                            #region Mail Notification senden
                            string body = "Benutzer: " + Environment.UserName + "\nErfolgreich: " + refreshedProcesses.Count + "\nFolgende Prozesse wurden ohne Änderungen aktualisiert:";
                            string subject = "BIA-Tool Auto-Notification: Aktualisieren von Prozessen";
                            body += Environment.NewLine;
                            foreach (ISB_BIA_Prozesse h in refreshedProcesses)
                            {
                                body += Environment.NewLine;
                                body += h.Prozess_Id + ", " + h.Prozess + ", " + h.Sub_Prozess;
                            }
                            _myMail.Send_NotificationMail(subject, body, _myShared.Current_Environment);
                            #endregion
                            _myDia.ShowInfo("Prozesse erfolgreich gespeichert.");
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        //Bei Fehler Logeintrag
                        try
                        {
                            using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                            {
                                ISB_BIA_Log logEntry = new ISB_BIA_Log
                                {
                                    Datum = DateTime.Now,
                                    Action = "Fehler: Aktualisieren von Prozessen ohne Änderungen",
                                    Tabelle = _myShared.Tbl_Prozesse,
                                    Details = ex.Message,
                                    Id_1 = 0,
                                    Id_2 = 0,
                                    Benutzer = Environment.UserName
                                };
                                db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                                db.SubmitChanges();
                            }
                            _myDia.ShowError("Die ausgewählten Prozesse konnten nicht gespeichert werden.", ex);
                            return false;
                        }
                        catch (Exception ex1)
                        {
                            _myDia.ShowError("Die ausgewählten Prozesse konnten nicht gespeichert werden.", ex1);
                            return false;
                        }
                    }
                }
                else
                {
                    string msg = "In der Auswahl befinden sich Prozesse, die momentan durch andere User geöffnet sind und deshalb nicht gespeichert werden können.\nBitte warten Sie, bis die Bearbeitung beendet ist oder deselektieren Sie betroffene Prozesse.\n\n";
                    _myDia.ShowWarning(msg + lockedListStringMsg);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Application
        public Application_Model GetApplicationModelFromDB(int id)
        {
            try
            {
                ISB_BIA_Applikationen linqApp;
                ObservableCollection<ISB_BIA_Prozesse> linqProcs;
                //Letzte Version der Anwendung abrufen
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    linqApp = db.ISB_BIA_Applikationen.Where(c => c.Applikation_Id == id)
                        .OrderByDescending(p => p.Datum).FirstOrDefault();

                    List<ISB_BIA_Prozesse_Applikationen> proc_AppCurrent =
                        db.ISB_BIA_Prozesse_Applikationen.Where(x => x.Applikation_Id == id).
                            GroupBy(y => y.Prozess_Id).Select(z => z.OrderByDescending(q => q.Datum).FirstOrDefault()).ToList();
                    //Aktive Relationen filtern
                    List<int> listIdCurrentProcesses =
                        proc_AppCurrent.Where(x => x.Relation == 1).Select(y => y.Prozess_Id).ToList();
                    if (listIdCurrentProcesses.Count == 0)
                        linqProcs = new ObservableCollection<ISB_BIA_Prozesse>();
                    else
                        //
                        linqProcs = new ObservableCollection<ISB_BIA_Prozesse>(db.ISB_BIA_Prozesse.Where(x => listIdCurrentProcesses.Contains(x.Prozess_Id)).GroupBy(y => y.Prozess_Id).Select(z => z.OrderByDescending(q => q.Datum).FirstOrDefault()).OrderBy(k => k.Prozess_Id).ToList());
                }
                //Falls existiert
                if (linqApp != null)
                {
                    Application_Model result = new Application_Model
                    {
                        Applikation_Id = linqApp.Applikation_Id,
                        Rechenzentrum = linqApp.Rechenzentrum,
                        Server = linqApp.Server,
                        Virtuelle_Maschine = linqApp.Virtuelle_Maschine,
                        Typ = linqApp.Typ,
                        IT_Betriebsart = linqApp.IT_Betriebsart,
                        IT_Anwendung_System = linqApp.IT_Anwendung_System,
                        Wichtiges_Anwendungssystem = (linqApp.Wichtiges_Anwendungssystem != ""),
                        SZ_1 = (SZ_Values)linqApp.SZ_1,
                        SZ_2 = (SZ_Values)linqApp.SZ_2,
                        SZ_3 = (SZ_Values)linqApp.SZ_3,
                        SZ_4 = (SZ_Values)linqApp.SZ_4,
                        SZ_5 = (SZ_Values)linqApp.SZ_5,
                        SZ_6 = (SZ_Values)linqApp.SZ_6,
                        Aktiv = linqApp.Aktiv,
                        ProcessList = linqProcs
                    };
                    return result;
                }
                else
                {
                    _myDia.ShowError("Anwendungsdaten konnten nicht abgerufen werden");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Anwendungsdaten konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public ISB_BIA_Applikationen MapApplicationModelToDB(Application_Model a)
        {
            return new ISB_BIA_Applikationen()
            {
                Applikation_Id = a.Applikation_Id,
                Rechenzentrum = a.Rechenzentrum,
                Server = a.Server,
                Virtuelle_Maschine = a.Virtuelle_Maschine,
                Typ = a.Typ,
                IT_Betriebsart = a.IT_Betriebsart,
                IT_Anwendung_System = a.IT_Anwendung_System,
                Wichtiges_Anwendungssystem = (a.Wichtiges_Anwendungssystem) ? "x" : "",
                SZ_1 = (int)a.SZ_1,
                SZ_2 = (int)a.SZ_2,
                SZ_3 = (int)a.SZ_3,
                SZ_4 = (int)a.SZ_4,
                SZ_5 = (int)a.SZ_5,
                SZ_6 = (int)a.SZ_6,
                Aktiv = a.Aktiv,
                Datum = a.Datum,
                Benutzer = a.Benutzer
            };
        }
        public ObservableCollection<string> GetRechenzentrum()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    return new ObservableCollection<string>(db.ISB_BIA_Applikationen.Select(p => p.Rechenzentrum).Distinct());
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Rechenzentren konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public ObservableCollection<string> GetServer()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    return new ObservableCollection<string>(db.ISB_BIA_Applikationen.Select(p => p.Server).Distinct());
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Server konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public ObservableCollection<string> GetVirtuelle_Maschine()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    return new ObservableCollection<string>(db.ISB_BIA_Applikationen.Select(p => p.Virtuelle_Maschine).Distinct());
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Virtuelle Maschinen konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public ObservableCollection<string> GetTypes()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    return new ObservableCollection<string>(db.ISB_BIA_Applikationen.Select(p => p.Typ).Distinct());
                }
            }
            catch
            {
                return null;
            }
        }
        public ObservableCollection<string> GetBetriebsart()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    return new ObservableCollection<string>(db.ISB_BIA_Applikationen.Select(p => p.IT_Betriebsart).Distinct());
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Kategorien konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public ObservableCollection<ISB_BIA_Applikationen> GetApplications(DateTime? date = null)
        {
            try
            {
                if (!date.HasValue) date = DateTime.Now;
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    return new ObservableCollection<ISB_BIA_Applikationen>(
                        db.ISB_BIA_Applikationen.Where(d => d.Datum <= date).GroupBy(a => a.Applikation_Id)
                        .Select(g => g.OrderByDescending(p => p.Datum).FirstOrDefault()).
                        OrderBy(x => x.Applikation_Id).ToList());
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Anwendungen konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public ObservableCollection<ISB_BIA_Applikationen> GetApplicationHistory(int applikation_Id)
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    return new ObservableCollection<ISB_BIA_Applikationen>(
                        db.ISB_BIA_Applikationen.Where(a => a.Applikation_Id == applikation_Id).
                        OrderByDescending(p => p.Datum).ToList());
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Anwendungen konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public bool InsertApplication(Application_Model a, ProcAppMode mode)
        {
            //Pflichtfelder (Anwendungsname & Kategorie)
            if (a.IT_Anwendung_System != ""
                && a.IT_Betriebsart != "")
            {
                //Wenn Anwendung momentan inaktiv ist => Frage bzgl. Aktivierung
                if (a.Aktiv == 0)
                {
                    a.Aktiv = (_myDia.ShowQuestion("Die Anwendung ist momentan auf inaktiv gesetzt. Möchten Sie die Anwendung auf aktiv setzen?", "Anwendung aktivieren")) ? 1 : 0;
                }
                try
                {
                    using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                    {
                        //neue Anwendungs ID erzeugen im Falle eines neuen Prozesses
                        if (mode == ProcAppMode.New)
                            a.Applikation_Id = db.ISB_BIA_Applikationen.Select(x => x.Applikation_Id).Max() + 1;

                        a.Benutzer = Environment.UserName;
                        a.Datum = DateTime.Now;
                        //Mappen und Einfügen
                        db.ISB_BIA_Applikationen.InsertOnSubmit(MapApplicationModelToDB(a));
                        //Logeintrag erstellen
                        ISB_BIA_Log logEntry = new ISB_BIA_Log
                        {
                            Action = "Ändern/Erstellen einer Anwendung",
                            Tabelle = _myShared.Tbl_Prozesse,
                            Details = "Id = " + a.Applikation_Id + ", Name = '" + a.IT_Anwendung_System + "'",
                            Id_1 = a.Applikation_Id,
                            Id_2 = 0,
                            Datum = DateTime.Now,
                            Benutzer = a.Benutzer
                        };
                        db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                        db.SubmitChanges();
                    }
                    _myDia.ShowMessage("Anwendung eingefügt");
                    return true;
                }
                catch (Exception ex1)
                {
                    //Logeintrag bei Fehler erstellen
                    try
                    {
                        using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                        {
                            ISB_BIA_Log logEntry = new ISB_BIA_Log
                            {
                                Action = "Fehler: Ändern/Erstellen einer Anwendung",
                                Tabelle = _myShared.Tbl_Prozesse,
                                Details = ex1.Message,
                                Id_1 = a.Applikation_Id,
                                Id_2 = 0,
                                Datum = DateTime.Now,
                                Benutzer = Environment.UserName
                            };

                            db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                            db.SubmitChanges();
                            _myDia.ShowError("Fehler beim Speichern der Anwendung!\nEin Log Eintrag wurde erzeugt.", ex1);
                            return false;
                        }
                    }
                    catch (Exception ex2)
                    {
                        _myDia.ShowError("Fehler beim Speichern der Anwendung!\nEin Log Eintrag konnte ebenfalls nicht erzeugt werden.", ex2);
                        return false;
                    }
                }
            }
            else
            {
                _myDia.ShowInfo("Bitte Alle Pflichtfelder korrekt ausfüllen.\nPflichtfelder sind fett markiert.");
                return false;
            }
        }
        #region Applikation löschen (Setzen des Aktiv Flags in der Datenbank auf 0)
        public ISB_BIA_Applikationen DeleteApplication(ISB_BIA_Applikationen a)
        {
            ISB_BIA_Applikationen toDelete = new ISB_BIA_Applikationen
            {
                Applikation_Id = a.Applikation_Id,
                IT_Anwendung_System = a.IT_Anwendung_System,
                IT_Betriebsart = a.IT_Betriebsart,
                Wichtiges_Anwendungssystem = a.Wichtiges_Anwendungssystem,
                Rechenzentrum = a.Rechenzentrum,
                Server = a.Server,
                Virtuelle_Maschine = a.Virtuelle_Maschine,
                Typ = a.Typ,
                SZ_1 = a.SZ_1,
                SZ_2 = a.SZ_2,
                SZ_3 = a.SZ_3,
                SZ_4 = a.SZ_4,
                SZ_5 = a.SZ_5,
                SZ_6 = a.SZ_6,
                Benutzer = Environment.UserName,
                Aktiv = 0
            };
            bool res = _myDia.ShowQuestion("Möchten Sie die Anwendung wirklich löschen (auf inaktiv setzen)?", "Anwendung löschen");
            return (res) ? TryInsert(toDelete) : null;
        }
        public ISB_BIA_Applikationen TryInsert(ISB_BIA_Applikationen toDelete)
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    toDelete.Datum = DateTime.Now;
                    db.ISB_BIA_Applikationen.InsertOnSubmit(toDelete);
                    ISB_BIA_Log logEntry = new ISB_BIA_Log
                    {
                        Action = "Löschen einer Anwendung (Setzen auf inaktiv)",
                        Tabelle = _myShared.Tbl_Applikationen,
                        Details = "Id = " + toDelete.Applikation_Id + ", Name = '" + toDelete.IT_Anwendung_System + "'",
                        Id_1 = toDelete.Applikation_Id,
                        Id_2 = 0,
                        Datum = toDelete.Datum,
                        Benutzer = Environment.UserName
                    };
                    db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                    db.SubmitChanges();
                }
                _myDia.ShowMessage("Anwendung gelöscht");
                return toDelete;
            }
            catch (Exception ex1)
            {
                try
                {
                    using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                    {
                        ISB_BIA_Log logEntry = new ISB_BIA_Log
                        {
                            Action = "Fehler: Löschen einer Anwendung",
                            Tabelle = _myShared.Tbl_Applikationen,
                            Details = ex1.Message,
                            Id_1 = toDelete.Applikation_Id,
                            Id_2 = 0,
                            Datum = toDelete.Datum,
                            Benutzer = Environment.UserName
                        };
                        db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                        db.SubmitChanges();
                        _myDia.ShowError("Applikation konnte nicht gelöscht werden.(1)\n", ex1);
                        return null;
                    }
                }
                catch (Exception ex2)
                {
                    _myDia.ShowError("Applikation konnte nicht gelöscht werden!\nEin Log Eintrag konnte ebenfalls nicht erzeugt werden.(2)\n", ex2);
                    return null;
                }
            }
        }
        #endregion
        public bool SaveAllApplications(ObservableCollection<ISB_BIA_Applikationen> aList)
        {
            bool res = _myDia.ShowQuestion("Möchten Sie die ausgewählten Anwendungen wirklich ohne Änderungen aktualisieren?", "Auswahl bestätigen");
            if (res)
            {
                //Liste der gesperrten Anwendungen
                List<ISB_BIA_Applikationen> lockedList = new List<ISB_BIA_Applikationen>();
                //String für die Nachricht welche Anwendung durch welchen User gesperrt ist
                string lockedListStringMsg = "";
                foreach (ISB_BIA_Applikationen a in aList)
                {
                    string user = GetObjectLocked(Table_Lock_Flags.Application, a.Applikation_Id);
                    if (user != "")
                    {
                        //Wenn Anwendung gesperrt zur Liste hinzufügen
                        lockedList.Add(a);
                        lockedListStringMsg = lockedListStringMsg + a.IT_Anwendung_System + " (geöffnet von: " + user + ")\n";
                    }
                }
                //Falls keine Anwendung gesperrt ist
                if (lockedList.Count == 0)
                {
                    try
                    {
                        using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                        {
                            foreach (ISB_BIA_Applikationen application_old in aList)
                            {
                                //zu speichernde Anwendung in neues Objekt "kopieren"
                                ISB_BIA_Applikationen application_refresh = new ISB_BIA_Applikationen
                                {
                                    Applikation_Id = application_old.Applikation_Id,
                                    Aktiv = application_old.Aktiv,
                                    Rechenzentrum = application_old.Rechenzentrum,
                                    Server = application_old.Server,
                                    Virtuelle_Maschine = application_old.Virtuelle_Maschine,
                                    Typ = application_old.Typ,
                                    IT_Betriebsart = application_old.IT_Betriebsart,
                                    IT_Anwendung_System = application_old.IT_Anwendung_System,
                                    Wichtiges_Anwendungssystem = application_old.Wichtiges_Anwendungssystem,
                                    SZ_1 = application_old.SZ_1,
                                    SZ_2 = application_old.SZ_2,
                                    SZ_3 = application_old.SZ_3,
                                    SZ_4 = application_old.SZ_4,
                                    SZ_5 = application_old.SZ_5,
                                    SZ_6 = application_old.SZ_6,
                                };
                                application_refresh.Benutzer = Environment.UserName;
                                application_refresh.Datum = DateTime.Now;
                                db.ISB_BIA_Applikationen.InsertOnSubmit(application_refresh);
                                //Logeintrag erzeugen
                                ISB_BIA_Log logEntry = new ISB_BIA_Log
                                {
                                    Datum = application_refresh.Datum,
                                    Action = "Aktualisieren einer Anwendung ohne Änderungen",
                                    Tabelle = _myShared.Tbl_Applikationen,
                                    Details = "Id = " + application_refresh.Applikation_Id + ", Name = '" + application_refresh.IT_Anwendung_System + "', Sub-Name = '" + application_refresh.IT_Betriebsart + "'",
                                    Id_1 = application_refresh.Applikation_Id,
                                    Id_2 = 0,
                                    Benutzer = Environment.UserName
                                };
                                db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                            }
                            db.SubmitChanges();
                            _myDia.ShowInfo("Anwendungen erfolgreich gespeichert.");
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        //Bei Fehler Logeintrag
                        try
                        {
                            using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                            {
                                ISB_BIA_Log logEntry = new ISB_BIA_Log
                                {
                                    Datum = DateTime.Now,
                                    Action = "Fehler: Aktualisieren von Anwendungen ohne Änderungen",
                                    Tabelle = _myShared.Tbl_Applikationen,
                                    Details = ex.Message,
                                    Id_1 = 0,
                                    Id_2 = 0,
                                    Benutzer = Environment.UserName
                                };
                                db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                                db.SubmitChanges();
                            }
                            _myDia.ShowError("Die ausgewählten Anwendungen konnten nicht gespeichert werden.", ex);
                            return false;
                        }
                        catch (Exception ex1)
                        {
                            _myDia.ShowError("Die ausgewählten Anwendungen konnten nicht gespeichert werden.", ex1);
                            return false;
                        }
                    }
                }
                else
                {
                    string msg = "In der Auswahl befinden sich Anwendungen, die momentan durch andere User geöffnet sind und deshalb nicht gespeichert werden können.\nBitte warten Sie, bis die Bearbeitung beendet ist oder deselektieren Sie betroffene Anwendungen.\n\n";
                    _myDia.ShowWarning(msg + lockedListStringMsg);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Einstellungen
        public Settings_Model GetSettingsModelFromDB()
        {
            try
            {
                ISB_BIA_Settings linqSettings;
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    linqSettings = db.ISB_BIA_Settings
                        .OrderByDescending(p => p.Datum).FirstOrDefault();
                }
                if (linqSettings != null)
                {
                    Settings_Model result = new Settings_Model()
                    {
                        SZ_1_Name = linqSettings.SZ_1_Name,
                        SZ_2_Name = linqSettings.SZ_2_Name,
                        SZ_3_Name = linqSettings.SZ_3_Name,
                        SZ_4_Name = linqSettings.SZ_4_Name,
                        SZ_5_Name = linqSettings.SZ_5_Name,
                        SZ_6_Name = linqSettings.SZ_6_Name,

                        Neue_Schutzziele_aktiviert = (linqSettings.Neue_Schutzziele_aktiviert == "Ja") ? true : false,
                        BIA_abgeschlossen = (linqSettings.BIA_abgeschlossen == "Ja") ? true : false,
                        SBA_abgeschlossen = (linqSettings.SBA_abgeschlossen == "Ja") ? true : false,
                        Delta_abgeschlossen = (linqSettings.Delta_abgeschlossen == "Ja") ? true : false,
                        Attribut9_aktiviert = (linqSettings.Attribut9_aktiviert == "Ja") ? true : false,
                        Attribut10_aktiviert = (linqSettings.Attribut10_aktiviert == "Ja") ? true : false,
                        Multi_Save = (linqSettings.Multi_Save == "Ja") ? true : false,
                        Datum = linqSettings.Datum,
                        Benutzer = linqSettings.Benutzer
                    };
                    return result;
                }
                else
                {
                    _myDia.ShowError("Einstellungen konnten nicht abgerufen werden");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Einstellungen konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public ISB_BIA_Settings MapSettingsModelToDB(Settings_Model s)
        {
            return new ISB_BIA_Settings()
            {
                SZ_1_Name = s.SZ_1_Name,
                SZ_2_Name = s.SZ_2_Name,
                SZ_3_Name = s.SZ_3_Name,
                SZ_4_Name = s.SZ_4_Name,
                SZ_5_Name = s.SZ_5_Name,
                SZ_6_Name = s.SZ_6_Name,
                Neue_Schutzziele_aktiviert = (s.Neue_Schutzziele_aktiviert) ? "Ja" : "Nein",
                BIA_abgeschlossen = (s.BIA_abgeschlossen) ? "Ja" : "Nein",
                SBA_abgeschlossen = (s.SBA_abgeschlossen) ? "Ja" : "Nein",
                Delta_abgeschlossen = (s.Delta_abgeschlossen) ? "Ja" : "Nein",
                Attribut9_aktiviert = (s.Attribut9_aktiviert) ? "Ja" : "Nein",
                Attribut10_aktiviert = (s.Attribut10_aktiviert) ? "Ja" : "Nein",
                Multi_Save = (s.Multi_Save) ? "Ja" : "Nein",
                Datum = s.Datum,
                Benutzer = s.Benutzer
            };
        }
        public ISB_BIA_Settings GetSettings()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    return db.ISB_BIA_Settings.OrderByDescending(d => d.Datum).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Einstellungen konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public List<ISB_BIA_Settings> GetSettingsHistory()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    return db.ISB_BIA_Settings.OrderByDescending(d => d.Datum).ToList();
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Einstellungshistorie konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public bool InsertSettings(ISB_BIA_Settings newSettings, ISB_BIA_Settings oldSettings)
        {
            try
            {
                if (newSettings.SZ_1_Name != oldSettings.SZ_1_Name
                    || newSettings.SZ_2_Name != oldSettings.SZ_2_Name
                    || newSettings.SZ_3_Name != oldSettings.SZ_3_Name
                    || newSettings.SZ_4_Name != oldSettings.SZ_4_Name
                    || newSettings.SZ_5_Name != oldSettings.SZ_5_Name
                    || newSettings.SZ_6_Name != oldSettings.SZ_6_Name
                    || newSettings.Neue_Schutzziele_aktiviert != oldSettings.Neue_Schutzziele_aktiviert
                    || newSettings.BIA_abgeschlossen != oldSettings.BIA_abgeschlossen
                    || newSettings.SBA_abgeschlossen != oldSettings.SBA_abgeschlossen
                    || newSettings.Delta_abgeschlossen != oldSettings.Delta_abgeschlossen
                    || newSettings.Attribut9_aktiviert != oldSettings.Attribut9_aktiviert
                    || newSettings.Attribut10_aktiviert != oldSettings.Attribut10_aktiviert
                    || newSettings.Multi_Save != oldSettings.Multi_Save)
                {
                    using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                    {
                        newSettings.Datum = DateTime.Now;
                        newSettings.Benutzer = Environment.UserName;
                        db.ISB_BIA_Settings.InsertOnSubmit(newSettings);

                        //Logeintrag erzeugen
                        ISB_BIA_Log logEntry = new ISB_BIA_Log
                        {
                            Action = "Ändern der Einstellungen",
                            Tabelle = _myShared.Tbl_Settings,
                            Details = "Für Details exportieren Sie die Einstellungshistorie",
                            Id_1 = 0,
                            Id_2 = 0,
                            Datum = DateTime.Now,
                            Benutzer = newSettings.Benutzer
                        };
                        db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                        db.SubmitChanges();
                    }
                    _myDia.ShowMessage("Einstellungen gespeichert.");
                    return true;
                }
                else
                {
                    _myDia.ShowMessage("Keine Änderungen entdeckt.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Fehler: Einstellungen konnten nicht gespeichert werden.", ex);
                return false;
            }
        }
        #endregion

        #region Informationssegmente
        public InformationSegment_Model GetSegmentModelFromDB(int id)
        {
            try
            {
                ISB_BIA_Informationssegmente linqIS;
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    linqIS = db.ISB_BIA_Informationssegmente.Where(c => c.Informationssegment_Id == id)
                        .OrderByDescending(p => p.Datum).FirstOrDefault();
                }
                if (linqIS != null)
                {
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
                else
                {
                    _myDia.ShowError("Segmentdaten konnten nicht abgerufen werden");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Segmentdaten konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public ISB_BIA_Informationssegmente MapSegmentModelToDB(InformationSegment_Model i)
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
        public InformationSegmentAttribute_Model GetAttributeModelFromDB(int id)
        {
            try
            {
                ISB_BIA_Informationssegmente_Attribute linqAttribute;
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    linqAttribute = db.ISB_BIA_Informationssegmente_Attribute.Where(c => c.Attribut_Id == id)
                        .OrderByDescending(p => p.Datum).FirstOrDefault();
                }
                if (linqAttribute != null)
                {
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
                else
                {
                    _myDia.ShowError("Attributdaten konnten nicht abgerufen werden");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Attributdaten konnten nicht abgerufen werden", ex);
                return null;
            }
        }
        public ISB_BIA_Informationssegmente_Attribute MapAttributeModelToDB(InformationSegmentAttribute_Model ia)
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
        public ObservableCollection<ISB_BIA_Informationssegmente> GetAllSegments()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
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
        public ObservableCollection<ISB_BIA_Informationssegmente> GetEnabledSegments()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
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
        public List<ISB_BIA_Informationssegmente> Get5SegmentsForCalculation(Process_Model process)
        {
            using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
            {
                //Zutreffende Segmente auswählen
                return GetEnabledSegments().Where(x =>
                x.Name == process.Relevantes_IS_1 ||
                x.Name == process.Relevantes_IS_2 ||
                x.Name == process.Relevantes_IS_3 ||
                x.Name == process.Relevantes_IS_4 ||
                x.Name == process.Relevantes_IS_5).ToList();
            }
        }
        public ISB_BIA_Informationssegmente GetISByISName(string iSName)
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    return db.ISB_BIA_Informationssegmente.Where(y => y.Name == iSName).
                        GroupBy(a => a.Name).Select(g => g.OrderByDescending(p => p.Datum).FirstOrDefault()).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Informationssegment konnten nicht abgerufen werden.", ex);
                return null;
            }
        }
        public ObservableCollection<ISB_BIA_Informationssegmente_Attribute> GetAttributes()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
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
        public ObservableCollection<string> GetAttributeNamesAndInfoForIS()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
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
        public ObservableCollection<string> GetAttributeNamesForHeader()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
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
        public bool InsertIS(InformationSegment_Model newIS, InformationSegment_Model oldIS)
        {
            //Wenn Änderungen gemacht wurden, Datenbankeintrag erzeugen
            if (oldIS.Segment != newIS.Segment
                || oldIS.Beschreibung != newIS.Beschreibung
                || oldIS.Mögliche_Segmentinhalte != newIS.Mögliche_Segmentinhalte
                || oldIS.Attribut1 != newIS.Attribut1
                || oldIS.Attribut2 != newIS.Attribut2
                || oldIS.Attribut3 != newIS.Attribut3
                || oldIS.Attribut4 != newIS.Attribut4
                || oldIS.Attribut5 != newIS.Attribut5
                || oldIS.Attribut6 != newIS.Attribut6
                || oldIS.Attribut7 != newIS.Attribut7
                || oldIS.Attribut8 != newIS.Attribut8
                || oldIS.Attribut9 != newIS.Attribut9
                || oldIS.Attribut10 != newIS.Attribut10)
            {
                try
                {
                    using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                    {
                        //Schreiben in Datenbank
                        newIS.Datum = DateTime.Now;
                        newIS.Benutzer = Environment.UserName;
                        //Mappen und in DB einfügen
                        db.ISB_BIA_Informationssegmente.InsertOnSubmit(MapSegmentModelToDB(newIS));
                        //Logeintrag erzeugen
                        ISB_BIA_Log logEntry = new ISB_BIA_Log
                        {
                            Action = "Ändern eines Informationssegments",
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
                        using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                        {
                            ISB_BIA_Log logEntry = new ISB_BIA_Log
                            {
                                Action = "Fehler: Ändern eines Informationssegments",
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
            else
            {
                _myDia.ShowInfo("Keine Änderungen entdeckt.");
                return false;
            }
        }
        public bool InsertISAtt(ObservableCollection<InformationSegmentAttribute_Model> newAttributeList)
        {
            //Indikator für Änderung
            bool change = false;
            ObservableCollection<ISB_BIA_Informationssegmente_Attribute> oldAttributeList;
            oldAttributeList = GetAttributes();
            if (oldAttributeList == null)
                return false;
            else
                //Schreiben in Datenbank
                try
                {
                    using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                    {
                        foreach (InformationSegmentAttribute_Model isx in newAttributeList)
                        {
                            if (isx.Name != oldAttributeList.Where(x => x.Attribut_Id == isx.Attribut_Id).Select(n => n.Name).FirstOrDefault() ||
                                isx.Info != oldAttributeList.Where(x => x.Attribut_Id == isx.Attribut_Id).Select(n => n.Info).FirstOrDefault() ||
                                isx.SZ_1 != oldAttributeList.Where(x => x.Attribut_Id == isx.Attribut_Id).Select(n => n.SZ_1.ToString()).FirstOrDefault() ||
                                isx.SZ_2 != oldAttributeList.Where(x => x.Attribut_Id == isx.Attribut_Id).Select(n => n.SZ_2.ToString()).FirstOrDefault() ||
                                isx.SZ_3 != oldAttributeList.Where(x => x.Attribut_Id == isx.Attribut_Id).Select(n => n.SZ_3.ToString()).FirstOrDefault() ||
                                isx.SZ_4 != oldAttributeList.Where(x => x.Attribut_Id == isx.Attribut_Id).Select(n => n.SZ_4.ToString()).FirstOrDefault() ||
                                isx.SZ_5 != oldAttributeList.Where(x => x.Attribut_Id == isx.Attribut_Id).Select(n => n.SZ_5.ToString()).FirstOrDefault() ||
                                isx.SZ_6 != oldAttributeList.Where(x => x.Attribut_Id == isx.Attribut_Id).Select(n => n.SZ_6.ToString()).FirstOrDefault()
                                )
                            {
                                // mindestens eine Änderung
                                change = true;
                                ISB_BIA_Informationssegmente_Attribute isMapped = MapAttributeModelToDB(isx);
                                isMapped.Datum = DateTime.Now;
                                isMapped.Benutzer = Environment.UserName;
                                db.ISB_BIA_Informationssegmente_Attribute.InsertOnSubmit(isMapped);

                                //Log Eintrag für erfolgreiches schreiben in Datenbank
                                ISB_BIA_Log logEntry = new ISB_BIA_Log
                                {
                                    Action = "Ändern der Informationssegment-Attribute",
                                    Tabelle = _myShared.Tbl_IS_Attribute,
                                    Details = "Id = " + isx.Attribut_Id + ", Name = '" + isx.Name + "'",
                                    Id_1 = isx.Attribut_Id,
                                    Id_2 = 0,
                                    Datum = DateTime.Now,
                                    Benutzer = Environment.UserName
                                };
                                db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                            }
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
                        using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                        {
                            ISB_BIA_Log logEntry = new ISB_BIA_Log
                            {
                                Datum = DateTime.Now,
                                Action = "Fehler: Ändern der Informationssegment-Attributstabelle",
                                Tabelle = _myShared.Tbl_Prozesse,
                                Details = ex1.Message,
                                Id_1 = 0,
                                Id_2 = 0,
                                Benutzer = Environment.UserName
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
        public Tuple<List<ISB_BIA_Informationssegmente>, List<ISB_BIA_Informationssegmente>, List<ISB_BIA_Informationssegmente_Attribute>, List<ISB_BIA_Informationssegmente_Attribute>> GetISAndISAttForExport()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
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

        #region Prozess<-->Applikation
        public ObservableCollection<ISB_BIA_Applikationen> GetActiveApplications()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    return new ObservableCollection<ISB_BIA_Applikationen>(
                        db.ISB_BIA_Applikationen.GroupBy(y => y.Applikation_Id).
                        Select(z => z.OrderByDescending(q => q.Datum).FirstOrDefault()).
                        Where(x => x.Aktiv == 1).OrderBy(k => k.Applikation_Id).ToList());

                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Applikationen konnten nicht abgerufen werden.", ex);
                return null;
            }
        }
        public ObservableCollection<string> GetApplicationCategories()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    List<ISB_BIA_Applikationen> ocCategories = db.ISB_BIA_Applikationen.GroupBy(y => y.Applikation_Id).Select(z => z.OrderByDescending(p => p.Datum).FirstOrDefault()).Where(u => u.Aktiv == 1).GroupBy(x => x.IT_Betriebsart).Select(group => group.FirstOrDefault()).ToList();
                    //Für späteres Filtern: Eintrag für alle Kategorien
                    ocCategories.Insert(0, new ISB_BIA_Applikationen() { Applikation_Id = 0, IT_Betriebsart = "<Alle>" });
                    return new ObservableCollection<string>(ocCategories.Select(x => x.IT_Betriebsart));

                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Anwendungskategorien konnten nicht abgerufen werden.", ex);
                return null;
            }
        }
        public ObservableCollection<ISB_BIA_Delta_Analyse> GetProcessApplicationHistoryForProcess(int id)
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    ObservableCollection<ISB_BIA_Delta_Analyse> result = new ObservableCollection<ISB_BIA_Delta_Analyse>();
                    List<ISB_BIA_Prozesse_Applikationen> procAppList = db.ISB_BIA_Prozesse_Applikationen.Where(x => x.Prozess_Id == id).OrderByDescending(c => c.Datum).ToList();
                    List<ISB_BIA_Prozesse_Applikationen> procAppListCurrent = procAppList.GroupBy(x => x.Applikation_Id)
                        .Select(c => c.OrderByDescending(v => v.Datum).FirstOrDefault()).ToList();
                    List<ISB_BIA_Prozesse> processes = db.ISB_BIA_Prozesse.ToList();
                    List<ISB_BIA_Applikationen> applications = db.ISB_BIA_Applikationen.ToList();
                    foreach (ISB_BIA_Prozesse_Applikationen k in procAppList)
                    {
                        ISB_BIA_Prozesse p = processes.Where(x => x.Prozess_Id == k.Prozess_Id).GroupBy(l => l.Prozess_Id).Select(g => g.OrderByDescending(d => d.Datum).FirstOrDefault()).FirstOrDefault();
                        ISB_BIA_Applikationen a = applications.Where(x => x.Applikation_Id == k.Applikation_Id).GroupBy(l => l.Applikation_Id).Select(g => g.OrderByDescending(d => d.Datum).FirstOrDefault()).FirstOrDefault();

                        ISB_BIA_Delta_Analyse n = new ISB_BIA_Delta_Analyse()
                        {
                            Prozess_Id = p.Prozess_Id,
                            Prozess = p.Prozess,
                            Sub_Prozess = p.Sub_Prozess,
                            Applikation_Id = a.Applikation_Id,
                            Applikation = a.IT_Anwendung_System,
                            SZ_1 = k.Relation,
                            SZ_2 = (procAppListCurrent.Count(x => x.Applikation_Id == k.Applikation_Id && x.Prozess_Id == k.Prozess_Id && x.Datum == k.Datum) > 0) ? 1 : 0,
                            Datum = k.Datum
                        };
                        result.Add(n);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Anwendungskategorien konnten nicht abgerufen werden.", ex);
                return null;
            }
        }
        public ObservableCollection<ISB_BIA_Delta_Analyse> GetProcessApplicationHistoryForApplication(int id)
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    ObservableCollection<ISB_BIA_Delta_Analyse> result = new ObservableCollection<ISB_BIA_Delta_Analyse>();
                    //Alle Einträge für Prozess
                    List<ISB_BIA_Prozesse_Applikationen> procAppList = db.ISB_BIA_Prozesse_Applikationen.Where(x => x.Applikation_Id == id).OrderByDescending(c => c.Datum).ToList();
                    //Alle aktuellen Einträge für Prozess (später gekennzeichnet)
                    List<ISB_BIA_Prozesse_Applikationen> procAppListCurrent = procAppList.GroupBy(x => x.Prozess_Id)
                        .Select(c => c.OrderByDescending(v => v.Datum).FirstOrDefault()).ToList();
                    List<ISB_BIA_Prozesse> processes = db.ISB_BIA_Prozesse.ToList();
                    List<ISB_BIA_Applikationen> applications = db.ISB_BIA_Applikationen.ToList();
                    foreach (ISB_BIA_Prozesse_Applikationen k in procAppList)
                    {
                        ISB_BIA_Prozesse p = processes.Where(x => x.Prozess_Id == k.Prozess_Id).GroupBy(l => l.Prozess_Id).Select(g => g.OrderByDescending(d => d.Datum).FirstOrDefault()).FirstOrDefault();
                        ISB_BIA_Applikationen a = applications.Where(x => x.Applikation_Id == k.Applikation_Id).GroupBy(l => l.Applikation_Id).Select(g => g.OrderByDescending(d => d.Datum).FirstOrDefault()).FirstOrDefault();
                        ISB_BIA_Delta_Analyse n = new ISB_BIA_Delta_Analyse()
                        {
                            Prozess_Id = p.Prozess_Id,
                            Prozess = p.Prozess,
                            Sub_Prozess = p.Sub_Prozess,
                            Applikation_Id = a.Applikation_Id,
                            Applikation = a.IT_Anwendung_System,
                            SZ_1 = k.Relation,
                            SZ_2 = (procAppListCurrent.Count(x => x.Applikation_Id == k.Applikation_Id && x.Prozess_Id == k.Prozess_Id && x.Datum == k.Datum) > 0) ? 1 : 0,
                            Datum = k.Datum
                        };
                        result.Add(n);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Anwendungskategorien konnten nicht abgerufen werden.", ex);
                return null;
            }
        }
        #endregion

        #region Delta
        public ObservableCollection<ISB_BIA_Delta_Analyse> GetDeltaAnalysis()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    return new ObservableCollection<ISB_BIA_Delta_Analyse>(
                        db.ISB_BIA_Delta_Analyse.OrderBy(x => x.Prozess_Id).ToList());
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("Fehler beim Abrufen der Delta Analyse.\n", ex);
                return null;
            }
        }
        public ObservableCollection<ISB_BIA_Delta_Analyse> InitiateDeltaAnalysis(DateTime d)
        {
            try
            {
                List<ISB_BIA_Prozesse_Applikationen> proc_App;
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    //einen Tag aufaddieren da immer zeit 00:00 benutzt wird & sonst Fehler für spätere Einträge des Tages auftreten
                    d = d.AddDays(1);
                    //Nur aktuelle 1er Relationen bis zu dem bestimmten Datum aus Relationentabelle abfragen
                    proc_App = db.ISB_BIA_Prozesse_Applikationen.Where(q => q.Datum <= d && q.Relation == 1)
                        .GroupBy(x => new { x.Prozess_Id, x.Applikation_Id })
                        .Select(z => z.OrderBy(q => q.Prozess_Id).FirstOrDefault()).ToList();
                }
                if (proc_App.Count != 0)
                {
                    //Dialog ob Analyse in Datenbank gespeichert werden soll oder nicht
                    bool res = _myDia.ShowQuestion("Möchten Sie die letzte gespeicherte Deltaanalyse in Datenbank überschreiben?", "Deltaanalyse");
                    if (res)
                        return DateDeltaAnalysis(d, true, proc_App);
                    else
                        return DateDeltaAnalysis(d, false, proc_App);
                }
                else
                {
                    _myDia.ShowInfo("Für den gewählten Zeitpunkt existieren keine Daten für eine Delta-Analyse.");
                    return null;
                }

            }
            catch (Exception ex)
            {
                _myDia.ShowError("Fehler beim Erstellen der Tabelle \n" + _myShared.Tbl_Delta + "\n", ex);
                return null;
            }
        }
        public ObservableCollection<ISB_BIA_Delta_Analyse> DateDeltaAnalysis(DateTime date, bool toDB, List<ISB_BIA_Prozesse_Applikationen> proc_App)
        {
            List<ISB_BIA_Delta_Analyse> DeltaList = new List<ISB_BIA_Delta_Analyse>();
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    //Erstelle Liste der Prozesse und Anwendungen mit dem zu dem gewählten Zeitpunkt aktuellsten Stand
                    ObservableCollection<ISB_BIA_Prozesse> processes = GetProcesses(date);
                    ObservableCollection<ISB_BIA_Applikationen> applications = GetApplications(date);
                    if (toDB)
                    {
                        db.ISB_BIA_Delta_Analyse.DeleteAllOnSubmit(db.ISB_BIA_Delta_Analyse.ToList());
                        db.SubmitChanges();
                    }
                    //Delta-Analyse für jede Prozess-Applikation Relation
                    foreach (ISB_BIA_Prozesse_Applikationen pa in proc_App)
                    {
                        ISB_BIA_Prozesse p = processes.Where(x => x.Prozess_Id == pa.Prozess_Id).FirstOrDefault();
                        ISB_BIA_Applikationen a = applications.Where(x => x.Applikation_Id == pa.Applikation_Id).FirstOrDefault();
                        //"Gelöschte" Prozesse / Anwendungen irrelevant
                        if (a.Aktiv == 0 || p.Aktiv == 0)
                        {
                            continue;
                        }

                        ISB_BIA_Delta_Analyse d = new ISB_BIA_Delta_Analyse
                        {
                            Prozess_Id = p.Prozess_Id,
                            Prozess = p.Prozess,
                            Sub_Prozess = p.Sub_Prozess,
                            Datum_Prozess = p.Datum,
                            Applikation_Id = a.Applikation_Id,
                            Applikation = a.IT_Anwendung_System,
                            Datum_Applikation = a.Datum,
                            SZ_1 = a.SZ_1 - p.SZ_1,
                            SZ_2 = a.SZ_2 - p.SZ_2,
                            SZ_3 = a.SZ_3 - p.SZ_3,
                            SZ_4 = a.SZ_4 - p.SZ_4,
                            SZ_5 = a.SZ_5 - p.SZ_5,
                            SZ_6 = a.SZ_6 - p.SZ_6,
                            Datum = date.Subtract(TimeSpan.FromDays(1))
                        };
                        DeltaList.Add(d);
                        if (toDB) db.ISB_BIA_Delta_Analyse.InsertOnSubmit(d);
                    }
                    if (toDB)
                    {
                        //Log Eintrag für erfolgreiches schreiben in Datenbank
                        ISB_BIA_Log logEntry = new ISB_BIA_Log
                        {
                            Action = "Erstellen der Delta-Analyse",
                            Tabelle = _myShared.Tbl_Delta,
                            Details = "-",
                            Id_1 = 0,
                            Id_2 = 0,
                            Datum = DateTime.Now,
                            Benutzer = Environment.UserName
                        };
                        db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                        db.SubmitChanges();
                    }
                }
                //Rückgabe der Delta-Liste zum Anzeigen der Ergebnisse
                return new ObservableCollection<ISB_BIA_Delta_Analyse>(DeltaList);
            }
            catch (Exception ex)
            {
                try
                {
                    using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                    {
                        _myDia.ShowError("Fehler: Ertellen der Delta-Analyse.", ex);
                        ISB_BIA_Log logEntry = new ISB_BIA_Log
                        {
                            Action = "Fehler: Erstellen der Delta-Analyse",
                            Tabelle = _myShared.Tbl_Delta,
                            Details = ex.Message,
                            Id_1 = 0,
                            Id_2 = 0,
                            Datum = DateTime.Now,
                            Benutzer = Environment.UserName
                        };
                        db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                        db.SubmitChanges();
                        _myDia.ShowError("Fehler beim Speichern der Delta-Analyse!\nEin Log Eintrag wurde erzeugt." + ex.ToString(), ex);
                        return null;
                    }
                }
                catch (Exception ex1)
                {
                    _myDia.ShowError("Fehler beim Speichern der Delta-Analyse!\nEin Log Eintrag konnte ebenfalls nicht erzeugt werden.\n", ex1);
                    return null;
                }
            }
        }
        #endregion

        #region OE
        public ObservableCollection<ISB_BIA_OEs> GetOENames()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    List<ISB_BIA_OEs> queryName = db.ISB_BIA_OEs.Where(c => c.OE_Name != "").
                        GroupBy(x => x.OE_Name).Select(g => g.OrderBy(p => p.Datum).FirstOrDefault()).ToList();
                    return new ObservableCollection<ISB_BIA_OEs>(queryName);
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("OE-Gruppierungen konnten nicht abgerufen werden.", ex);
                return null;
            }
        }
        public ObservableCollection<ISB_BIA_OEs> GetOENumbers()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    List<ISB_BIA_OEs> queryKennung = db.ISB_BIA_OEs.Where(y => y.OE_Nummer != null && y.OE_Nummer != "").GroupBy(x => x.OE_Nummer).Select(g => g.OrderBy(p => p.Datum).FirstOrDefault()).ToList();
                    return new ObservableCollection<ISB_BIA_OEs>(queryKennung);
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("OE-Nummern konnten nicht abgerufen werden.", ex);
                return null;
            }
        }
        public ObservableCollection<ISB_BIA_OEs> GetOELinks()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    List<ISB_BIA_OEs> queryLink = db.ISB_BIA_OEs.Where(x => x.OE_Nummer != "").OrderBy(c => c.OE_Name).ToList();
                    return new ObservableCollection<ISB_BIA_OEs>(queryLink);
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("OE-Relationen konnten nicht abgerufen werden.", ex);
                return null;
            }
        }
        public ISB_BIA_OEs InsertOEName(string name)
        {
            try
            {
                //neu erstellen: OE Gruppe
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                {
                    //Indikator ob Name bereits existiert
                    int already_exists = db.ISB_BIA_OEs.Where(x => x.OE_Name == name).ToList().Count;
                    if (name != "" && already_exists == 0)
                    {
                        //Bei erstellung einer neuen Gruppe wird OE_Nummer leer gelassen
                        ISB_BIA_OEs new_Link = new ISB_BIA_OEs
                        {
                            OE_Name = name,
                            OE_Nummer = "",
                            Benutzer = Environment.UserName,
                            Datum = DateTime.Now
                        };
                        db.ISB_BIA_OEs.InsertOnSubmit(new_Link);
                        //Erstellen eines LogEntries und schreiben in Datenbank
                        ISB_BIA_Log logEntry = new ISB_BIA_Log
                        {
                            Action = "ADMIN/CISO: Erstellen einer OE",
                            Tabelle = _myShared.Tbl_Prozesse + ", " + _myShared.Tbl_OEs,
                            Details = "Name = '" + name + "'",
                            Id_1 = 0,
                            Id_2 = 0,
                            Datum = DateTime.Now,
                            Benutzer = Environment.UserName
                        };
                        db.ISB_BIA_Log.InsertOnSubmit(logEntry);

                        db.SubmitChanges();
                        _myDia.ShowInfo("OE wurde erstellt.");
                        return new_Link;
                    }
                    else
                    {
                        _myDia.ShowWarning("Bitte geben Sie einen gültigen Namen für eine OE ein, welche nicht bereits existiert.");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                _myDia.ShowError("OE-Gruppierung konnten nicht abgerufen werden.", ex);
                return null;
            }
        }
        public bool EditOEName(string name, string oldName)
        {
            //Ändern: OE_Name
            if (name != "" && name != oldName)
            {
                try
                {
                    using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                    {
                        //Indikator ob Name bereits existiert
                        int already_exists = db.ISB_BIA_OEs.Where(x => x.OE_Name == name).ToList().Count;
                        if (name != "" && already_exists == 0)
                        {
                            //Liste aller Prozesse, der die OE-Gruppierung zugeordnet ist
                            List<ISB_BIA_Prozesse> oldProcessList = db.ISB_BIA_Prozesse.Where(c => c.OE_Filter == oldName).GroupBy(x => x.Prozess_Id).Select(y => y.OrderByDescending(z => z.Datum).FirstOrDefault()).ToList();
                            //Prüfen ob ein Prozess gesperrt ist (es darf kein Prozess geöffnet sein, da sonst veraltete Daten verwendet werden könnten)
                            var lockedProcesses = db.ISB_BIA_Lock.Where(x => x.Table_Flag == (int)Table_Lock_Flags.Process).ToList();
                            string pl = "Prozesse sind durch folgende User geöffnet";
                            if (lockedProcesses.Count != 0)
                            {
                                foreach (ISB_BIA_Lock p in lockedProcesses)
                                {
                                    string user = GetObjectLocked(Table_Lock_Flags.Process, p.Object_Id);
                                    if (user != "")
                                    {
                                        pl = pl + "\n" + user + "\n";
                                    }
                                }
                                string msg = "OE-Namen können nur geändert werden, wenn momentan kein Prozess zur Bearbeitung geöffnet ist.\nBitte warten Sie, bis die Bearbeitung durch den entsprechenden User beendet ist.\n\n";
                                _myDia.ShowWarning(msg + pl);
                                return false;
                            }

                            //Alle OE-Einträge ändern (hier keine Historisierung)
                            db.ISB_BIA_OEs.Where(n => n.OE_Name == oldName).ToList()
                                .ForEach(x => { x.OE_Name = name; });

                            //Die OE's all dieser Prozesse ändern
                            foreach (ISB_BIA_Prozesse process_old in oldProcessList)
                            {
                                ISB_BIA_Prozesse process_new = new ISB_BIA_Prozesse()
                                {
                                    OE_Filter = name,
                                    Datum = DateTime.Now,
                                    Benutzer = Environment.UserName,

                                    Prozess_Id = process_old.Prozess_Id,
                                    Aktiv = process_old.Aktiv,
                                    Prozess = process_old.Prozess,
                                    Sub_Prozess = process_old.Sub_Prozess,
                                    Prozessverantwortlicher = process_old.Prozessverantwortlicher,
                                    Kritischer_Prozess = process_old.Kritischer_Prozess,
                                    Kritikalität_des_Prozesses = process_old.Kritikalität_des_Prozesses,
                                    Reifegrad_des_Prozesses = process_old.Reifegrad_des_Prozesses,
                                    Regulatorisch = process_old.Regulatorisch,
                                    Reputatorisch = process_old.Reputatorisch,
                                    Finanziell = process_old.Finanziell,
                                    SZ_1 = process_old.SZ_1,
                                    SZ_2 = process_old.SZ_2,
                                    SZ_3 = process_old.SZ_3,
                                    SZ_4 = process_old.SZ_4,
                                    SZ_5 = process_old.SZ_5,
                                    SZ_6 = process_old.SZ_6,
                                    Vorgelagerte_Prozesse = process_old.Vorgelagerte_Prozesse,
                                    Nachgelagerte_Prozesse = process_old.Nachgelagerte_Prozesse,
                                    Servicezeit_Helpdesk = process_old.Servicezeit_Helpdesk,
                                    RPO_Datenverlustzeit_Recovery_Point_Objective =
                                        process_old.RPO_Datenverlustzeit_Recovery_Point_Objective,
                                    RTO_Wiederanlaufzeit_Recovery_Time_Objective =
                                        process_old.RTO_Wiederanlaufzeit_Recovery_Time_Objective,
                                    RTO_Wiederanlaufzeit_Recovery_Time_Objective_Notfall =
                                        process_old.RTO_Wiederanlaufzeit_Recovery_Time_Objective_Notfall,
                                    Relevantes_IS_1 = process_old.Relevantes_IS_1,
                                    Relevantes_IS_2 = process_old.Relevantes_IS_2,
                                    Relevantes_IS_3 = process_old.Relevantes_IS_3,
                                    Relevantes_IS_4 = process_old.Relevantes_IS_4,
                                    Relevantes_IS_5 = process_old.Relevantes_IS_5
                                };
                                db.ISB_BIA_Prozesse.InsertOnSubmit(process_new);
                            }

                            //Erstellen eines LogEntries und schreiben in Datenbank
                            ISB_BIA_Log logEntry = new ISB_BIA_Log
                            {
                                Action = "ADMIN/CISO: Ändern einer OE",
                                Tabelle = _myShared.Tbl_Prozesse + ", " + _myShared.Tbl_OEs,
                                Details = "Von '" + oldName + "' zu '" + name + "'",
                                Id_1 = 0,
                                Id_2 = 0,
                                Datum = DateTime.Now,
                                Benutzer = Environment.UserName
                            };
                            db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                            db.SubmitChanges();
                            _myDia.ShowInfo("OE-Bezeichnung geändert!");
                            return true;
                        }
                        else
                        {
                            _myDia.ShowWarning("Bezeichnung existiert bereits");
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _myDia.ShowError("Fehler beim Ändern der OE-Namen.\nDies betrifft auch alle Prozesse, denen diese OE zugeordnet war.", ex);
                    return false;
                }
            }
            else
            {
                _myDia.ShowWarning("Bitte gültige Bezeichnung eingeben.");
                return false;
            }
        }
        public bool DeleteOEName(string oeName)
        {
            bool res = _myDia.ShowQuestion("Möchten Sie diesen OE-Filter wirklich löschen?\nAlle Zuordnungen werden ebenfalls gelöscht.", "OE-Filter löschen");
            if (res)
            {
                try
                {
                    using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                    {
                        //OE Gruppe kann nur gelöscht werden, wenn kein Prozess mehr dieser Gruppe zugeordnet ist
                        List<ISB_BIA_Prozesse> list1 = db.ISB_BIA_Prozesse.Where(x => x.OE_Filter == oeName).ToList();
                        if (list1.Count > 0)
                        {
                            _myDia.ShowWarning("Diesem OE-Namen sind noch Prozesse zugeordnet.\nOrdnen Sie diese Prozesse zunächst einer anderen OE zu.");
                            return false;
                        }

                        //Prüfen ob ein Prozess gesperrt ist (es darf kein Prozess geöffnet sein, da sonst veraltete Daten verwendet werden könnten)
                        var lockedProcesses = db.ISB_BIA_Lock.Where(x => x.Table_Flag == (int)Table_Lock_Flags.Process).ToList();
                        string pl = "Prozesse sind durch folgende User geöffnet";
                        if (lockedProcesses.Count != 0)
                        {
                            foreach (ISB_BIA_Lock p in lockedProcesses)
                            {
                                string user = GetObjectLocked(Table_Lock_Flags.Process, p.Object_Id);
                                if (user != "")
                                {
                                    pl = pl + "\n" + user + "\n";
                                }
                            }
                            string msg = "OE's können nur gelöscht werden, wenn momentan kein Prozess zur Bearbeitung geöffnet ist.\nBitte warten Sie, bis die Bearbeitung durch den entsprechenden User beendet ist.\n\n";
                            _myDia.ShowWarning(msg + pl);
                            return false;
                        }

                        try
                        {
                            //Löschen aller Zuordnungen mit dieser Gruppe
                            db.ISB_BIA_OEs.DeleteAllOnSubmit(db.ISB_BIA_OEs.Where(x => x.OE_Name == oeName));
                            ISB_BIA_Log logEntry = new ISB_BIA_Log
                            {
                                Action = "Löschen einer OE",
                                Details = "Name = '" + oeName + "'",
                                Tabelle = _myShared.Tbl_OEs,
                                Id_1 = 0,
                                Id_2 = 0,
                                Datum = DateTime.Now,
                                Benutzer = Environment.UserName
                            };
                            db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                            db.SubmitChanges();
                            _myDia.ShowInfo("OE wurde gelöscht.");
                            return true;
                        }
                        catch
                        {
                            _myDia.ShowError("OE konnte nicht gelöscht werden.");
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _myDia.ShowError("Fehler beim Löschen.", ex);
                    return false;
                }
            }
            else
                return false;
        }
        public bool DeleteOELink(string oeName, string oeNumber)
        {
            bool res = _myDia.ShowQuestion("Möchten Sie diesen diese OE-Zuordnung wirklich löschen?", "OE-Zuordnung löschen");
            if (res)
            {
                try
                {
                    using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                    {
                        //Löschen der angegebenen Zuordnung
                        db.ISB_BIA_OEs.DeleteAllOnSubmit(db.ISB_BIA_OEs.Where(x => x.OE_Nummer == oeNumber && x.OE_Name == oeName));
                        ISB_BIA_Log logEntry = new ISB_BIA_Log
                        {
                            Action = "Löschen einer OE-Zuordnung",
                            Tabelle = _myShared.Tbl_OEs,
                            Details = "Zuordnung = '" + oeName + "' <-> " + oeNumber,
                            Id_1 = 0,
                            Id_2 = 0,
                            Datum = DateTime.Now,
                            Benutzer = Environment.UserName
                        };
                        db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                        db.SubmitChanges();
                        _myDia.ShowInfo("OE-Zuordnung wurde gelöscht.");
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    _myDia.ShowError("OE-Zuordnung konnte nicht gelöscht werden.", ex);
                    return false;

                }
            }
            else
                return false;
        }
        public bool DeleteOENumber(string oeNumber)
        {
            bool res = _myDia.ShowQuestion("Möchten Sie diesen diese OE-Kennung wirklich löschen?\nAlle Zuordnungen werden ebenfalls gelöscht.", "OE-Kennung löschen");
            if (res)
            {
                try
                {
                    using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                    {
                        //Löschen aller Zuordnungen mit dieser OE-Nummer
                        db.ISB_BIA_OEs.DeleteAllOnSubmit(db.ISB_BIA_OEs.Where(x => x.OE_Nummer == oeNumber));
                        ISB_BIA_Log logEntry = new ISB_BIA_Log
                        {
                            Action = "Löschen einer OE-Nummer",
                            Tabelle = _myShared.Tbl_OEs,
                            Details = "Nummer = " + oeNumber,
                            Id_1 = 0,
                            Id_2 = 0,
                            Datum = DateTime.Now,
                            Benutzer = Environment.UserName
                        };
                        db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                        db.SubmitChanges();
                        _myDia.ShowInfo("OE-Nummer wurde gelöscht.");
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    _myDia.ShowError("OE-Nummer konnte nicht gelöscht werden.", ex);
                    return false;
                }
            }
            else
                return false;
        }
        public ISB_BIA_OEs InsertOELink(ISB_BIA_OEs name, ISB_BIA_OEs number)
        {
            if (name != null && number != null)
            {
                try
                {
                    using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                    {
                        //Prüfen ob Zuordnung bereits vorhanden
                        if (db.ISB_BIA_OEs.Where(x => x.OE_Name == name.OE_Name && x.OE_Nummer == number.OE_Nummer).ToList().Count == 0)
                        {
                            //Neue Zuordnung erstellen
                            ISB_BIA_OEs new_Link = new ISB_BIA_OEs
                            {
                                OE_Name = name.OE_Name,
                                OE_Nummer = number.OE_Nummer,
                                Benutzer = Environment.UserName,
                                Datum = DateTime.Now
                            };
                            db.ISB_BIA_OEs.InsertOnSubmit(new_Link);
                            //Erstellen eines LogEntries und schreiben in Datenbank
                            ISB_BIA_Log logEntry = new ISB_BIA_Log
                            {
                                Action = "ADMIN/CISO: Erstellen einer OE-Zuordnung",
                                Tabelle = _myShared.Tbl_Prozesse + ", " + _myShared.Tbl_OEs,
                                Details = "Zuordnung = '" + name.OE_Name + "' <-> " + number.OE_Nummer,
                                Id_1 = 0,
                                Id_2 = 0,
                                Datum = DateTime.Now,
                                Benutzer = Environment.UserName
                            };
                            db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                            db.SubmitChanges();
                            _myDia.ShowInfo("OE-Zuordnung wurde erstellt.");
                            return new_Link;
                        }
                        else
                        {
                            _myDia.ShowWarning("Diese Zuordnung existiert bereits.");
                            return null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _myDia.ShowError("OE-Zuordnung konnte nicht erstellt werden.", ex);
                    return null;
                }
            }
            else
            {
                _myDia.ShowWarning("Bitte geben Sie einen gültigen Namen ein.");
                return null;
            }
        }
        public ISB_BIA_OEs InsertOENumber(string number, ISB_BIA_OEs name)
        {
            if (name != null && number != "")
            {
                try
                {
                    using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                    {
                        //Prüfen ob Zuordnung bereits existiert
                        if (db.ISB_BIA_OEs.Where(x => x.OE_Name == name.OE_Name && x.OE_Nummer == number).ToList().Count == 0)
                        {
                            //Neue Zuordnung erstellen
                            ISB_BIA_OEs new_Number = new ISB_BIA_OEs
                            {
                                OE_Name = name.OE_Name,
                                OE_Nummer = number,
                                Benutzer = Environment.UserName,
                                Datum = DateTime.Now
                            };
                            db.ISB_BIA_OEs.InsertOnSubmit(new_Number);
                            //Erstellen eines LogEntries und schreiben in Datenbank
                            ISB_BIA_Log logEntry = new ISB_BIA_Log
                            {
                                Action = "ADMIN/CISO: Erstellen einer OE-Nummer",
                                Tabelle = _myShared.Tbl_Prozesse + ", " + _myShared.Tbl_OEs,
                                Details = "Nummer = " + new_Number.OE_Nummer,
                                Id_1 = 0,
                                Id_2 = 0,
                                Datum = DateTime.Now,
                                Benutzer = Environment.UserName
                            };
                            db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                            db.SubmitChanges();
                            _myDia.ShowInfo("OE-Nummer wurde erstellt.");
                            return new_Number;
                        }
                        else
                        {
                            _myDia.ShowWarning("Diese Zuordnung existiert bereits.");
                            return null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _myDia.ShowError("OE-Nummer konnte nicht erstellt werden.", ex);
                    return null;
                }
            }
            else
            {
                _myDia.ShowWarning("Bitte geben Sie einen gültigen Namen ein und wählen Sie eine OE aus.");
                return null;
            }
        }
        public bool EditOENumber(string number, string oldNumber)
        {
            if (number != "" && number != oldNumber)
            {
                try
                {
                    using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
                    {
                        //Prüfen ob Nummer bereits vorhanden
                        if (db.ISB_BIA_OEs.Where(x => x.OE_Nummer == number).ToList().Count > 0)
                        {
                            _myDia.ShowWarning("OE-Nummer existiert bereits.\nÄndern Sie ggf. zunächst die vorhandene Nummer.");
                            return false;
                        }
                        //Ändern der Nummern aller Zuordnungen, deren Nummer momentan der angegebenen entspricht
                        List<ISB_BIA_OEs> numbers = db.ISB_BIA_OEs.Where(x => x.OE_Nummer == oldNumber).ToList();
                        foreach (ISB_BIA_OEs x in numbers)
                        {
                            ISB_BIA_OEs newNumber = new ISB_BIA_OEs()
                            {
                                OE_Name = x.OE_Name,
                                OE_Nummer = number,
                                Benutzer = Environment.UserName,
                                Datum = DateTime.Now,
                            };
                            //Zuordnung mit neuer nummer einfügen
                            db.ISB_BIA_OEs.InsertOnSubmit(newNumber);
                        }
                        //Alte Zuordnungen löschen
                        db.ISB_BIA_OEs.DeleteAllOnSubmit(numbers);

                        //Erstellen eines LogEntries und schreiben in Datenbank
                        ISB_BIA_Log logEntry = new ISB_BIA_Log
                        {
                            Action = "ADMIN/CISO: Ändern einer OE-Nummer",
                            Tabelle = _myShared.Tbl_Prozesse + ", " + _myShared.Tbl_OEs,
                            Details = "Von '" + number + "' zu '" + oldNumber + "'",
                            Id_1 = 0,
                            Id_2 = 0,
                            Datum = DateTime.Now,
                            Benutzer = Environment.UserName
                        };
                        db.ISB_BIA_Log.InsertOnSubmit(logEntry);
                        db.SubmitChanges();
                        _myDia.ShowInfo("OE-Nummer wurde geändert.");
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    _myDia.ShowError("OE-Nummer konnte nicht geändert werden.", ex);
                    return false;
                }
            }
            else
            {
                _myDia.ShowWarning("Bitte geben Sie einen gültigen Namen ein.");
                return false;
            }
        }
        #endregion

        #region Log
        public ObservableCollection<ISB_BIA_Log> GetLog()
        {
            try
            {
                using (L2SDataContext db = new L2SDataContext(_myShared.ConnectionString))
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
