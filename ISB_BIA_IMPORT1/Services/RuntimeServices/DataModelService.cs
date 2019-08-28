using ISB_BIA_IMPORT1.LINQ2SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using ISB_BIA_IMPORT1.Services.Interfaces;

namespace ISB_BIA_IMPORT1.Services
{
    class DataModelService : IDataModelService
    {
        readonly IDialogService _myDia;
        readonly ISharedResourceService _myShared;

        public DataModelService(IDialogService myDia, ISharedResourceService myShared)
        {
            this._myDia = myDia;
            this._myShared = myShared;
        }

        #region Datenmodell erstellen
        public bool Create(DataTable dt_Processes, DataTable dt_Applications, DataTable dt_Relation, DataTable dt_InformationSegments, DataTable dt_InformationSegmentAttributes)
        {
            //------
            string _SqlDropProc_vPnP = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'" + _myShared.Tbl_Proz_vPnP + "') DROP Table " + _myShared.Tbl_Proz_vPnP;
            string _SqlCreaProc_vPnP =
                "    CREATE TABLE [dbo].[" + _myShared.Tbl_Proz_vPnP + "] (" +
                "    [Prozess_Id] INT NOT NULL," +
                "    [Datum_Prozess] DATETIME NOT NULL," +
                "    [Ref_Prozess_Id] INT NOT NULL," +
                "    [Datum_Ref_Prozess] DATETIME NOT NULL," +
                "    [Typ] INT NOT NULL," + //1=vP 2=nP
                "    [Relation] INT NOT NULL," + //aktiv/inaktiv
                "    [Datum] DATETIME NOT NULL," +
                "    [Benutzer] NVARCHAR(50) NOT NULL," +
                "    PRIMARY KEY(Prozess_Id, Ref_Prozess_Id, Typ, Relation, Datum)," +
                "    Foreign Key(Prozess_Id, Datum_Prozess) references [" + _myShared.Tbl_Prozesse + "](Prozess_Id,Datum)," +
                "    Foreign Key(Ref_Prozess_Id, Datum_Ref_Prozess) references [" + _myShared.Tbl_Prozesse + "](Prozess_Id,Datum)," +
                "); ";
            //------

            #region SQL Strings für Erstellen der Tabellen mit Headern analog zu Excel (!Trotzdem nicht ändern, da im Code per Linq2SQL Klassenmember aufegrufen werden)
            #region Tabellen Löschungs SQL Anweisungen
            string _SqlDropProc_App = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'" + _myShared.Tbl_Proz_App + "') DROP Table " + _myShared.Tbl_Proz_App;
            string _SqlDropProcesses = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'" + _myShared.Tbl_Prozesse + "') DROP Table " + _myShared.Tbl_Prozesse;
            string _SqlDropIS = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'" + _myShared.Tbl_IS + "') DROP Table " + _myShared.Tbl_IS;
            string _SqlDropISAttribut = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'" + _myShared.Tbl_IS_Attribute + "') DROP Table " + _myShared.Tbl_IS_Attribute;
            string _SqlDropSBA = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'" + _myShared.Tbl_Applikationen + "') DROP Table " + _myShared.Tbl_Applikationen;
            string _SqlDropDelta = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'" + _myShared.Tbl_Delta + "') DROP Table " + _myShared.Tbl_Delta;
            string _SqlDropLog = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'" + _myShared.Tbl_Log + "') DROP Table " + _myShared.Tbl_Log;
            string _SqlDropOEs = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'" + _myShared.Tbl_OEs + "') DROP Table " + _myShared.Tbl_OEs;
            string _SqlDropSettings = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'" + _myShared.Tbl_Settings + "') DROP Table " + _myShared.Tbl_Settings;
            string _SqlDropLock = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'" + _myShared.Tbl_Lock + "') DROP Table " + _myShared.Tbl_Lock;
            #endregion
            string _SqlCreaProcApp =
                "    CREATE TABLE [dbo].[" + _myShared.Tbl_Proz_App + "] (" +
                "    [Prozess_Id] INT NOT NULL," +
                "    [Datum_Prozess] DATETIME NOT NULL DEFAULT(CONVERT(VARCHAR(23), '2018-12-31 23:59:00.000',121))," +
                "    [Applikation_Id] INT NOT NULL," +
                "    [Datum_Applikation] DATETIME NOT NULL DEFAULT(CONVERT(VARCHAR(23), '2018-12-31 23:59:00.000',121))," +
                "    [Relation] INT NOT NULL," +
                "    [Datum] DATETIME NOT NULL DEFAULT(CONVERT(VARCHAR(23), '2018-12-31 23:59:00.000',121))," +
                "    [Benutzer] NVARCHAR(50) NOT NULL DEFAULT('')," +
                "    PRIMARY KEY(Prozess_Id, Applikation_Id, Datum)," +
                "    Foreign Key(Prozess_Id, Datum_Prozess) references [" + _myShared.Tbl_Prozesse + "](Prozess_Id,Datum)," +
                "    Foreign Key(Applikation_Id, Datum_Applikation) references [" + _myShared.Tbl_Applikationen + "](Applikation_Id,Datum)" +
                "); ";

            //Tabellen erstellen, deren Spaltennamen den Spaltennamen des Quell-Excel-Sheets entsprechen
            //Wenn Spaltennamen in Excel geändert werden und das Datenmodell erneuert werden soll, müssen Anpassungen
            //im Code vorgenommen werden (Linq-to-SQL Modell erneuern, Variablenaufrufe im Code ändern)
            string _SqlCreaSBA =
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
                "    [Datum] DATETIME NOT NULL DEFAULT(CONVERT(VARCHAR(23), '2018-12-31 23:59:00.000',121))," +
                "    [Benutzer] NVARCHAR(50) NOT NULL DEFAULT('')," +
                "    [Erstanlage] NVARCHAR(10) NOT NULL DEFAULT('Ja')," +
                "    PRIMARY KEY(" + dt_Applications.Columns[0] + ",Datum)" +
                ");";

            string _SqlCreaProcesses =
                "CREATE TABLE[dbo].[" + _myShared.Tbl_Prozesse + "](" +
                "    [" + dt_Processes.Columns[0] + "] INT NOT NULL," +
                "    [" + dt_Processes.Columns[1] + "] NVARCHAR(200) NOT NULL," +
                "    [" + dt_Processes.Columns[2] + "] NVARCHAR(350) NOT NULL," +
                "    [" + dt_Processes.Columns[3] + "] NVARCHAR(350) NOT NULL," +
                "    [" + dt_Processes.Columns[4] + "] NVARCHAR(50) NOT NULL," +
                "    [" + dt_Processes.Columns[5] + "] NVARCHAR(50) NOT NULL," +
                "    [" + dt_Processes.Columns[6] + "] NVARCHAR(10) NOT NULL," +
                "    [" + dt_Processes.Columns[7] + "] NVARCHAR(20) NOT NULL," +
                "    [" + dt_Processes.Columns[8] + "] NVARCHAR(20) NOT NULL," +
                "    [" + dt_Processes.Columns[9] + "] NVARCHAR(10) NOT NULL," +
                "    [" + dt_Processes.Columns[10] + "] NVARCHAR(10) NOT NULL," +
                "    [" + dt_Processes.Columns[11] + "] NVARCHAR(10) NOT NULL," +
                "    [" + dt_Processes.Columns[12] + "] INT NOT NULL," +
                "    [" + dt_Processes.Columns[13] + "] INT NOT NULL," +
                "    [" + dt_Processes.Columns[14] + "] INT NOT NULL," +
                "    [" + dt_Processes.Columns[15] + "] INT NOT NULL," +
                "    [" + dt_Processes.Columns[16] + "] INT NOT NULL," +
                "    [" + dt_Processes.Columns[17] + "] INT NOT NULL," +
                "    [" + dt_Processes.Columns[18] + "] NVARCHAR(350) NOT NULL," +
                "    [" + dt_Processes.Columns[19] + "] INT NOT NULL," +
                "    [" + dt_Processes.Columns[20] + "] INT NOT NULL," +
                "    [" + dt_Processes.Columns[21] + "] INT NOT NULL," +
                "    [" + dt_Processes.Columns[22] + "] VARCHAR(10) NOT NULL," +
                "    [" + dt_Processes.Columns[23] + "] VARCHAR(10) NOT NULL," +
                "    [" + dt_Processes.Columns[24] + "] VARCHAR(10) NOT NULL," +
                "    [" + dt_Processes.Columns[25] + "] VARCHAR(10) NOT NULL," +
                "    [" + dt_Processes.Columns[26] + "] VARCHAR(10) NOT NULL," +
                "    [Aktiv] INT NOT NULL DEFAULT(1)," +
                "    [Datum] DATETIME NOT NULL DEFAULT(CONVERT(VARCHAR(23), '2018-12-31 23:59:00.000',121))," +
                "    [Benutzer] NVARCHAR(50) NOT NULL DEFAULT('')," +
                "    PRIMARY KEY(" + dt_Processes.Columns[0] + ",Datum)," +
            ");";

            string _SqlCreaIS =
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
                "    [Datum] DATETIME NOT NULL DEFAULT(CONVERT(VARCHAR(23), '2018-12-31 23:59:00.000',121))," +
                "    [Benutzer] NVARCHAR(50) NOT NULL DEFAULT('')," +
                "    PRIMARY KEY(" + dt_InformationSegments.Columns[0] + ", Datum)" +
                ");";

            string _SqlCreaISAttribut =
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
                "    [Datum] DATETIME NOT NULL DEFAULT(CONVERT(VARCHAR(23), '2018-12-31 23:59:00.000',121))," +
                "    [Benutzer] NVARCHAR(50) NOT NULL DEFAULT('')," +
                "    PRIMARY KEY(" + dt_InformationSegmentAttributes.Columns[0] + ", Datum)" +
                ");";

            string _SqlCreaDelta =
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
                "    [Datum] DATETIME NOT NULL DEFAULT(CONVERT(VARCHAR(23), '2018-12-31 23:59:00.000',121))," +
                "    PRIMARY KEY(Prozess_Id, Applikation_Id)," +
                "    Foreign Key(Prozess_Id,Datum_Prozess) references [" + _myShared.Tbl_Prozesse + "](Prozess_Id,Datum)," +
                "    Foreign Key(Applikation_Id,Datum_Applikation) references [" + _myShared.Tbl_Applikationen + "](Applikation_Id,Datum)" +
                ");";

            string _SqlCreaLog =
                "CREATE TABLE[dbo].[" + _myShared.Tbl_Log + "] (" +
                "    [Id]                  INT IDENTITY(1, 1) NOT NULL PRIMARY KEY," +
                "    [Aktion] VARCHAR(200) NOT NULL," +
                "    [Tabelle] VARCHAR(200) NOT NULL," +
                "    [Details] VARCHAR(1000) NOT NULL," +
                "    [Id_1] INT NOT NULL," +
                "    [Id_2] INT NOT NULL," +
                "    [Datum] DATETIME NOT NULL DEFAULT(CONVERT(VARCHAR(23), '2018-12-31 23:59:00.000',121))," +
                "    [Benutzer] NVARCHAR(50) NOT NULL DEFAULT('')," +
                ");";

            string _SqlCreaOEs =
                "CREATE TABLE[dbo].[" + _myShared.Tbl_OEs + "] (" +
                "    [Id]                  INT IDENTITY(1, 1) NOT NULL PRIMARY KEY," +
                "    [OE_Name] VARCHAR(200) NOT NULL," +
                "    [OE_Nummer] VARCHAR(200) NOT NULL," +
                "    [Prozesseigentümer] VARCHAR(200) NOT NULL," +
                "    [Datum] DATETIME NOT NULL DEFAULT(CONVERT(VARCHAR(23), '2018-12-31 23:59:00.000',121))," +
                "    [Benutzer] NVARCHAR(50) NOT NULL DEFAULT('')," +
                "    Unique (OE_Name, OE_Nummer)" +
                ");" +
                "INSERT INTO " + _myShared.Tbl_OEs + " (OE_NAME, OE_Nummer, Prozesseigentümer) " +
                "VALUES ('Vorstandsassistenz', '1', 'Ulrich Link')," +
                "('Mittelstands-, Kommunalfinanzierung', '1.1', 'Roland Wagner')," +
                "('Mittelstands-, Kommunalfinanzierung', '1.11', '')," +
                "('Mittelstands-, Kommunalfinanzierung', '1.12', '')," +
                "('Mittelstands-, Kommunalfinanzierung', '1.13', '')," +
                "('Mittelstands-, Kommunalfinanzierung', '1.14', '')," +
                "('Mittelstands-, Kommunalfinanzierung', '1.15', '')," +
                "('Mittelstands-, Kommunalfinanzierung', '1.16', '')," +
                //"('Grundsatzfragen Wirtschaftsförderung', '1.11', '')," +
                //"('Kreditfinanzierung', '1.12', '')," +
                //"('Programmkredite, Produktentwicklung', '1.13', '')," +
                //"('Kommunalfinanzierung', '1.14', '')," +
                //"('Technologieförderung', '1.15', '')," +
                //"('Kleine Zuschussprogramme', '1.16', '')," +
                "('Bürgschaften, Investitionszuschüsse', '1.2', 'Sibylle Schwalie')," +
                "('Bürgschaften, Investitionszuschüsse', '1.201', '')," +
                "('Bürgschaften, Investitionszuschüsse', '1.202', '')," +
                //"('Investitionszuschüsse', '1.201', '')," +
                //"('Bürgschaften', '1.202', '')," +
                "('Personal, Verwaltung', '1.4', 'Horst Grafen')," +
                "('Personal', '1.401', '')," +
                "('Verwaltung', '1.402', '')," +
                "('Kundenbetreuung, Beratung', '1.5', 'Folker Gratz')," +
                "('Venture Capital, Beteiligungen', '1.6', 'Mike Walber')," +
                "('Handel', '1.7', 'Dr.Ulrich Link')," +
                "('Wohnraumförderung', '2.1', 'Corden Brendel')," +
                "('Wohnraumförderung', '2.101', '')," +
                "('Wohnraumförderung', '2.102', '')," +
                "('Wohnraumförderung', '2.103', '')," +
                "('Wohnraumförderung', '2.104', '')," +
                //"('Koordination Wohnraumförderung', '2.101', '')," +
                //"('Mietwohnungsbau', '2.102', '')," +
                //"('Modernisierung, Spezialprogramme', '2.103', '')," +
                //"('Eigentumswohnungsbau', '2.104', '')," +
                "('Zuschuss-, Fördermittelverwaltung', '2.2', 'Thomas Wittig')," +
                "('Zuschuss-, Fördermittelverwaltung', '2.201', '')," +
                "('Zuschuss-, Fördermittelverwaltung', '2.202', '')," +
                "('Zuschuss-, Fördermittelverwaltung', '2.203', '')," +
                "('Zuschuss-, Fördermittelverwaltung', '2.204', '')," +
                "('Zuschuss-, Fördermittelverwaltung', '2.205', '')," +
                //"('Grundsatzfragen Zuschuss-, Fördermittelverwaltung', '2.201', '')," +
                //"('Fördermittelverwaltung', '2.202', '')," +
                //"('Koordination Zuschussverwaltung', '2.203', '')," +
                //"('Mittelabruf, Auszahlungen', '2.204', '')," +
                //"('Verwendungsnachweisprüfung', '2.205', '')," +
                "('Zweitvotum, Sanierung, Abwicklung', '2.3', 'Gerda-Trudi Oprée')," +
                "('Zweitvotum, Sanierung, Abwicklung', '2.301', '')," +
                "('Zweitvotum, Sanierung, Abwicklung', '2.302', '')," +
                //"('Sanierung, Abwicklung Wohnraumförderung', '2.301', '')," +
                //"('Sanierung, Abwicklung Wirtschaftsförderung', '2.302', '')," +
                "('Finanzen', '3.1', 'Ralf Gölz')," +
                "('Rechnungswesen', '3.12', '')," +
                "('Rechnungswesen', '3.121', '')," +
                "('Rechnungswesen', '3.122', '')," +
                //"('Darlehensbuchhaltung', '3.121', '')," +
                //"('Finanzbuchhaltung, Bilanzierung', '3.122', '')," +
                "('Treasury, Handelsabwicklung', '3.13', '')," +
                "('Controlling, Risikocontrolling', '3.14', '')," +
                "('Zentrale Daten, Meldewesen', '3.15', '')," +
                "('Zentrale Daten, Meldewesen', '3.152', '')," +
                //"('Meldewesen', '3.152', '')," +
                "('Presse', '3.2', 'Claudia Belz')," +
                "('Unternehmenskommunikation, Investorenservice, Außenhandelsförderung', '3.3', 'Antje Duwe')," +
                "('Zentrale Stelle, Compliance', '3.4', 'Karsten Drawe')," +
                "('Interne Revision, Bescheinigungsbehörde', '4.1', 'Gerhard Pulverich')," +
                "('Interne Revision, Bescheinigungsbehörde', '4.102', '')," +
                //"('Bescheinigungsbehörde', '4.102', '')," +
                "('Vorstandssekretariat, Allgemeine Organisation', '4.2', 'Monika Evelo')," +
                "('Zentrale Stelle, Compliance', '4.3', 'Karsten Drawe')," +
                "('IT', '4.4', 'Markus Engel')," +
                "('IT', '4.401', '')," +
                "('IT', '4.402', '')," +
                "('IT', '4.403', '');";
                //"('IT-Systeme', '4.401', '')," +
                //"('IT-Steuerung', '4.402', '')," +
                //"('IT-Betrieb', '4.403', '');";

            string _SqlCreaSettings =
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
                "    [Multi_Speichern] VARCHAR(10) NOT NULL," +
                "    [Datum] DATETIME NOT NULL DEFAULT(CONVERT(VARCHAR(23), '2018-12-31 23:59:00.000',121))," +
                "    [Benutzer] NVARCHAR(50) NOT NULL," +
                ");";

            string _SqlCreaLock =
                "CREATE TABLE[dbo].[" + _myShared.Tbl_Lock + "] (" +
                "    [Id]                  INT IDENTITY(1, 1) NOT NULL PRIMARY KEY," +
                "    [Tabellen_Kennzeichen] INT NOT NULL," +
                "    [Objekt_Id] INT NOT NULL," +
                "    [Datum] DATETIME NOT NULL DEFAULT(CONVERT(VARCHAR(23), '2018-12-31 23:59:00.000',121))," +
                "    [BenutzerNnVn] NVARCHAR(50) NOT NULL DEFAULT('')," +
                "    [Benutzer] NVARCHAR(50) NOT NULL DEFAULT('')," +
                "    [ComputerName] NVARCHAR(50) NOT NULL DEFAULT('')"+
                ");";

            #endregion
            List<string> sqlCommandList = new List<string>();
            //
            sqlCommandList.Add(_SqlDropProc_vPnP);

            sqlCommandList.Add(_SqlDropLog);
            sqlCommandList.Add(_SqlCreaLog);
            sqlCommandList.Add(_SqlDropOEs);
            sqlCommandList.Add(_SqlCreaOEs);
            sqlCommandList.Add(_SqlDropSettings);
            sqlCommandList.Add(_SqlCreaSettings);
            sqlCommandList.Add(_SqlDropLock);
            sqlCommandList.Add(_SqlCreaLock);
            sqlCommandList.Add(_SqlDropISAttribut);
            sqlCommandList.Add(_SqlCreaISAttribut);

            sqlCommandList.Add(_SqlDropDelta);
            sqlCommandList.Add(_SqlDropProc_App);
            sqlCommandList.Add(_SqlDropProcesses);
            sqlCommandList.Add(_SqlDropSBA);
            sqlCommandList.Add(_SqlCreaSBA);

            sqlCommandList.Add(_SqlDropIS);
            sqlCommandList.Add(_SqlCreaIS);

            sqlCommandList.Add(_SqlCreaProcesses);
            sqlCommandList.Add(_SqlCreaProcApp);
            sqlCommandList.Add(_SqlCreaDelta);

            //
            sqlCommandList.Add(_SqlCreaProc_vPnP);


            try
            {
                //Löschen der Tabellen wenn vorhanden und neu erstellen
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {                  
                    foreach (string s in sqlCommandList)
                    {
                        db.ExecuteCommand(s);
                    }
                    db.SubmitChanges();
                }
                
                //Schreiben der DataTables in die Datenbank (Initialer Stand der Daten)
                using (SqlConnection con = new SqlConnection(_myShared.Conf_ConnectionString))
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
                
                using (L2SDataContext db = new L2SDataContext(_myShared.Conf_ConnectionString))
                {

                    //Initialer Inhalt der Tabelle für OE-Gruppenbezeichnungen wird aus Prozesstabelle entnommen
                    List<ISB_BIA_Prozesse> query = db.ISB_BIA_Prozesse.GroupBy(x => x.OE_Filter).Select(group => group.FirstOrDefault()).ToList();
                    foreach (ISB_BIA_Prozesse p in query)
                    {
                        ISB_BIA_OEs oe = new ISB_BIA_OEs()
                        {
                            OE_Name = p.OE_Filter,
                            OE_Nummer = "",
                            Prozesseigentümer = "",
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
                        Multi_Speichern = "Nein",
                        Datum = DateTime.Now,
                        Benutzer =  _myShared.User.Username
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
        /// <summary>
        /// Funktion in der die SQL Bulk Copy Konfiguration bestimmt wird
        /// Wird benutzt um Prozess/Anwendungs/IS(-Attribut) Daten aus den Datatables in die Datenbank zu schreiben 
        /// </summary>
        /// <param name="tableName"> Name der Zieltabelle </param>
        /// <param name="con"> SQL Connection </param>
        /// <param name="dt"> Datatable, der die zu schreibenden Daten enthält </param>
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
    }
}
