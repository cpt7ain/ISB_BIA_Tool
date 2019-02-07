using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using ISB_BIA_IMPORT1.LinqDataContext;
using ISB_BIA_IMPORT1.Services;
using Microsoft.Win32;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Excel = Microsoft.Office.Interop.Excel;

namespace ISB_BIA_IMPORT1.ViewModel
{
    /// <summary>
    /// VM zur Darstellung Datenmodelleinstellungen
    /// </summary>
    public class DataModel_ViewModel : ViewModelBase
    {
        #region Backing-Fields
        private MyRelayCommand _chooseFile;
        private MyRelayCommand _save;
        //Hier die Daten angeben, welche für den DatenImport benötigt werden (Name des Excel-Sheets, Zellenrange)
        private string _p_Sheet = "3 - BIA";
        private string _a_Sheet = "4 - SBA";
        private string _p_Range = "B6:AC776";
        private string _a_Range = "B6:O402";
        private string _p_A_Matrix_Sheet = "3 - BIA";
        private string _p_A_Matrix_Range = "AD7:PI776";
        private string _i_Range = "B8:P43";
        private string _i_Sheet = "Informationssegmente";
        private string _i_A_Range = "Y8:AG18";
        private string _i_A_Sheet = "Informationssegmente";
        #endregion

        #region Sheet-Namen und Sheet-Ranges zum definieren der einzulesenden Datenbereiche
        /// <summary>
        /// Name des Sheets mit der Prozessliste
        /// </summary>
        public string P_Sheet
        {
            get => _p_Sheet;
            set => Set(() => P_Sheet, ref _p_Sheet, value);
        }
        /// <summary>
        /// Bereich der Prozessdaten auf dem Sheet
        /// </summary>
        public string P_Range
        {
            get => _p_Range;
            set => Set(() => P_Range, ref _p_Range, value);

        }
        /// <summary>
        /// Name des Sheets mit der Anwendungsliste
        /// </summary>
        public string A_Sheet
        {
            get => _a_Sheet;
            set => Set(() => A_Sheet, ref _a_Sheet, value);

        }
        /// <summary>
        /// Bereich der Anwendungsdaten auf dem Sheet
        /// </summary>
        public string A_Range
        {
            get => _a_Range;
            set => Set(() => A_Range, ref _a_Range, value);

        }
        /// <summary>
        /// Name des Sheets mit der Prozess-Anwendungs-Matrix
        /// </summary>
        public string P_A_Matrix_Sheet
        {
            get => _p_A_Matrix_Sheet;
            set => Set(() => P_A_Matrix_Sheet, ref _p_A_Matrix_Sheet, value);

        }
        /// <summary>
        /// Bereich der Prozess-Anwendungsdaten auf dem Sheet
        /// </summary>
        public string P_A_Matrix_Range
        {
            get => _p_A_Matrix_Range;
            set => Set(() => P_A_Matrix_Range, ref _p_A_Matrix_Range, value);

        }
        /// <summary>
        /// Name des Sheets mit den Informationssegmenten
        /// </summary>
        public string I_Sheet
        {
            get => _i_Sheet;
            set => Set(() => I_Sheet, ref _i_Sheet, value);

        }
        /// <summary>
        /// Bereich der Informationssegmentdaten auf dem Sheet
        /// </summary>
        public string I_Range
        {
            get => _i_Range;
            set => Set(() => I_Range, ref _i_Range, value);

        }
        /// <summary>
        /// Name des Sheets mit den Informationssegment-Attribut-Daten
        /// </summary>
        public string I_A_Sheet
        {
            get => _i_A_Sheet;
            set => Set(() => I_A_Sheet, ref _i_A_Sheet, value);

        }
        /// <summary>
        /// Bereich der Informationssegment-Attribut-Daten auf dem Sheet
        /// </summary>
        public string I_A_Range
        {
            get => _i_A_Range;
            set => Set(() => I_A_Range, ref _i_A_Range, value);

        }
        #endregion


        #region Strings der Dateien und SQL-Drop Befehle (abhängig von den Daten in myShared im Konstruktor erzeugt)
        //Original & Arbeitsdatei der Quelldaten
        private string originalFile;
        private string workFile;
        //SQL
        private readonly string sqlDropProc_App;
        private readonly string sqlDropProcesses;
        private readonly string sqlDropIS;
        private readonly string sqlDropISAttribut;
        private readonly string sqlDropSBA;
        private readonly string sqlDropDelta;
        private readonly string sqlDropLog;
        private readonly string sqlDropOEs;
        private readonly string sqlDropSettings;
        private readonly string sqlDropLock;
        #endregion

        /// <summary>
        /// Dateipfad für Quelldatei
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Quelldatei Auswählen
        /// </summary>
        public MyRelayCommand ChooseFile
        {
            get => _chooseFile
                  ?? (_chooseFile = new MyRelayCommand(() =>
                  {
                      OpenFileDialog openFileDialog = new OpenFileDialog
                      {
                          DefaultExt = ".xlsx",
                          InitialDirectory = myShared.InitialDirectory,
                          FileName = myShared.InitialDirectory + @"\ISB_BIA-SBA.xlsx",
                          Filter = "Excel Files (*.xlsm;*.xlsx)|*.xlsm;*xlsx"
                      };
                      bool? result = myDia.Open(openFileDialog);

                      if (result == true)
                      {
                          originalFile = openFileDialog.FileName;
                          workFile = openFileDialog.InitialDirectory + @"\ISB_BIA-SBA_tmp.xlsx";
                      }
                  }));
        }

        /// <summary>
        /// Erneuerung des Datenmodells Ausführen und schliessen
        /// </summary>
        public MyRelayCommand Save
        {
            get => _save
                  ?? (_save = new MyRelayCommand(() =>
                  {
                      if (CheckForCreation())
                      {
                          Cleanup();
                          myNavi.NavigateBack();
                      }
                  }));
        }

        /// <summary>
        /// Command zum Zurückkehren zum vorherigen VM
        /// </summary>
        public MyRelayCommand NavBack
        {
            get
            {
                return new MyRelayCommand(() =>
                {
                    if (myDia.CancelDecision())
                    {
                        Cleanup();
                        myNavi.NavigateBack();
                    }
                });
            }
        }

        #region Services
        IMyDataService myData;
        IMyNavigationService myNavi;
        IMyDialogService myDia;
        IMySharedResourceService myShared;
        #endregion

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="myDialogService"></param>
        /// <param name="myNavigationService"></param>
        /// <param name="mySharedResourceService"></param>
        public DataModel_ViewModel(IMyDataService myDataService,IMyDialogService myDialogService, IMyNavigationService myNavigationService, IMySharedResourceService mySharedResourceService)
        {
            #region Services
            myData = myDataService;
            myNavi = myNavigationService;
            myDia = myDialogService;
            myShared = mySharedResourceService;
            Source = myShared.Source;
            #endregion
            originalFile = myShared.InitialDirectory + @"\ISB_BIA-SBA.xlsx";
            workFile = myShared.InitialDirectory + @"\ISB_BIA-SBA_tmp.xlsx";
            #region Tabellen Löschungs SQL Anweisungen
            sqlDropProc_App = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'" + myShared.Tbl_Proz_App + "') DROP Table " +myShared.Tbl_Proz_App;
            sqlDropProcesses = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'" +myShared.Tbl_Prozesse + "') DROP Table " +myShared.Tbl_Prozesse;
            sqlDropIS = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'" +myShared.Tbl_IS + "') DROP Table " +myShared.Tbl_IS;
            sqlDropISAttribut = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'" +myShared.Tbl_IS_Attribute + "') DROP Table " +myShared.Tbl_IS_Attribute;
            sqlDropSBA = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'" +myShared.Tbl_Applikationen + "') DROP Table " +myShared.Tbl_Applikationen;
            sqlDropDelta = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'" +myShared.Tbl_Delta + "') DROP Table " +myShared.Tbl_Delta;
            sqlDropLog = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'" +myShared.Tbl_Log + "') DROP Table " +myShared.Tbl_Log;
            sqlDropOEs = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'" +myShared.Tbl_OEs + "') DROP Table " +myShared.Tbl_OEs;
            sqlDropSettings = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'" +myShared.Tbl_Settings + "') DROP Table " +myShared.Tbl_Settings;
            sqlDropLock = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'" +myShared.Tbl_Lock + "') DROP Table " +myShared.Tbl_Lock;
            #endregion
        }


        #region Datenmodell erstellen
        /// <summary>
        /// Methode zur Überprüfung der angegebenen Datenbereiche und sonstigen Angaben
        /// </summary>
        /// <returns></returns>
        private bool CheckForCreation()
        {
            IWorkbook workbook;
            string rangePattern = "[A-Z]+[1-9]+:[A-Z]+[1-9]+";
            if (File.Exists(originalFile))
            {
                if (File.Exists(workFile)) File.Delete(workFile);
                File.Copy(originalFile, workFile);
                using (FileStream stream = new FileStream(workFile, FileMode.Open, FileAccess.Read))
                {
                    workbook = new XSSFWorkbook(stream);
                }
                if (workbook.GetSheet(P_Sheet) != null &&
                   workbook.GetSheet(A_Sheet) != null &&
                   workbook.GetSheet(I_Sheet) != null &&
                   workbook.GetSheet(I_A_Sheet) != null &&
                   workbook.GetSheet(P_A_Matrix_Sheet) != null &&
                   Regex.IsMatch(P_Range, rangePattern) &&
                   Regex.IsMatch(A_Range, rangePattern) &&
                   Regex.IsMatch(I_Range, rangePattern) &&
                   Regex.IsMatch(I_A_Range, rangePattern) &&
                   Regex.IsMatch(P_A_Matrix_Range, rangePattern))
                {
                    try
                    {
                        if (GetDataForDatamodel(workFile, workbook))
                        {
                            if (File.Exists(workFile)) File.Delete(workFile);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        myDia.ShowError("Fehler beim erneuern des Datenmodells.", ex);
                        return false;
                    }
                }
                else
                {
                    if (File.Exists(workFile)) File.Delete(workFile);
                    myDia.ShowInfo("Bitte überprüfen Sie Ihre Eingaben!");
                    return false;
                }
            }
            else
            {
                myDia.ShowError("Datei nicht gefunden!");
                return false;
            }
        }

        /// <summary>
        /// Aktionen welche zur Erstellung des Datenmodells aus der bisherigen Excel Datei dienen
        /// erstellen der Tabellen, füllen der Tabellen mit den Prozess/Anwendungsdaten (etc.) des Excel Sheets        
        /// /// </summary>
        /// <param name="workFile"> File, der als Datenquelle dienen soll </param>
        /// <param name="workbook"> Workbook Objekt, auf dem Operationen durchgeführt werden können </param>
        /// <returns> Erfolg </returns>
        private bool GetDataForDatamodel(string workFile, IWorkbook workbook)
        {
            //Funktionen ausführen, um daten aus der Excel-Datei zu erhalten
            DataTable dt_Applications = GetApplicationDataFromXLSX(workFile, workbook);
            DataTable dt_Processes = GetProcessDataFromXLSX(workFile, workbook);
            DataTable dt_InformationSegments = GetInformationSegmentDataFromXLSX(workFile, workbook);
            DataTable dt_InformationSegmentAttributes = GetInformationSegmentAttributDataFromXLSX(workFile, workbook);
            DataTable dt_Relation = FillSQLRelationFromXLSX(workFile, workbook);

            if (dt_Applications == null || dt_Processes == null || dt_InformationSegments == null || dt_InformationSegmentAttributes == null || dt_Relation == null)
            {
                myDia.ShowWarning("Datenodell konnte nicht erzeugt werden!\nÜberprüfen Sie die Datenquelle.");
                return false;
            }
            else
            {
                #region SQL Strings für Erstellen der Tabellen mit Headern analog zu Excel (!Trotzdem nicht ändern, da im Code per Linq2SQL Klassenmember aufegrufen werden)

                string sqlCreaProc_App =
                    "    CREATE TABLE [dbo].[" + myShared.Tbl_Proz_App + "] (" +
                    "    [Id]          INT IDENTITY(1, 1) NOT NULL PRIMARY KEY," +
                    "    [Prozess] INT NOT NULL," +
                    "    [Applikation] INT NOT NULL," +
                    "    [Relation] INT NOT NULL," +
                    "    [Datum] DATETIME NOT NULL DEFAULT(CONVERT(VARCHAR(19), '2018-12-31 23:59:59',120))," +
                    "    [Benutzer] NVARCHAR(50) NOT NULL DEFAULT('')," +
                    "    UNIQUE(Prozess,Applikation,Datum)," +
                    "); ";

                //Tabellen erstellen, deren Spaltennamen den Spaltennamen des Quell-Excel-Sheets entsprechen
                //Wenn Spaltennamen in Excel geändert werden und das Datenmodell erneuert werden soll, müssen Anpassungen
                //im Code vorgenommen werden (Linq-to-SQL Modell erneuern, Variablenaufrufe im Code ändern)
                string sqlCreaSBA =
                    "CREATE TABLE[dbo].[" + myShared.Tbl_Applikationen + "](" +
                    "    [Id]                  INT IDENTITY(1, 1) NOT NULL PRIMARY KEY," +
                    "    [" + dt_Applications.Columns[0] + "] INT NOT NULL," +
                    "    [" + dt_Applications.Columns[1] + "] NVARCHAR(350) NOT NULL," +
                    "    [" + dt_Applications.Columns[2] + "] NVARCHAR(350) NOT NULL," +
                    "    [" + dt_Applications.Columns[3] + "] NVARCHAR(350) NOT NULL," +
                    "    [" + dt_Applications.Columns[4] + "] NVARCHAR(350) NOT NULL," +
                    "    [" + dt_Applications.Columns[5] + "] NVARCHAR(350) NOT NULL," +
                    "    [" + dt_Applications.Columns[6] + "] NVARCHAR(1000) NOT NULL," +
                    "    [" + dt_Applications.Columns[7] + "] NVARCHAR(20) NOT NULL," +
                    "    [" + dt_Applications.Columns[8] + "] INT NOT NULL," +
                    "    [" + dt_Applications.Columns[9] + "] INT NOT NULL," +
                    "    [" + dt_Applications.Columns[10] + "] INT NOT NULL," +
                    "    [" + dt_Applications.Columns[11] + "] INT NOT NULL," +
                    "    [" + dt_Applications.Columns[12] + "] INT NOT NULL," +
                    "    [" + dt_Applications.Columns[13] + "] INT NOT NULL," +
                    "    [Aktiv] INT NOT NULL DEFAULT(1)," +
                    "    [Datum] DATETIME NOT NULL DEFAULT(CONVERT(VARCHAR(19), '2018-12-31 23:59:59',120))," +
                    "    [Benutzer] NVARCHAR(50) NOT NULL DEFAULT('')," +
                    "    Unique (" + dt_Applications.Columns[0] + ",Datum)" +
                    ");";

                string sqlCreaProcesses =
                    "CREATE TABLE[dbo].[" + myShared.Tbl_Prozesse + "](" +
                    "    [Id]                              INT IDENTITY(1, 1) NOT NULL PRIMARY KEY," +
                    "    [" + dt_Processes.Columns[0] + "] INT NOT NULL," +
                    "    [" + dt_Processes.Columns[1] + "] NVARCHAR(350) NOT NULL," +
                    "    [" + dt_Processes.Columns[2] + "] NVARCHAR(350) NOT NULL," +
                    "    [" + dt_Processes.Columns[3] + "] NVARCHAR(350) NOT NULL," +
                    "    [" + dt_Processes.Columns[4] + "] NVARCHAR(50) NOT NULL," +
                    "    [" + dt_Processes.Columns[5] + "] NVARCHAR(50) NOT NULL," +
                    "    [" + dt_Processes.Columns[6] + "] NVARCHAR(50) NOT NULL," +
                    "    [" + dt_Processes.Columns[7] + "] NVARCHAR(50) NOT NULL," +
                    "    [" + dt_Processes.Columns[8] + "] NVARCHAR(50) NOT NULL," +
                    "    [" + dt_Processes.Columns[9] + "] NVARCHAR(50) NOT NULL," +
                    "    [" + dt_Processes.Columns[10] + "] NVARCHAR(50) NOT NULL," +
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
                    "    [" + dt_Processes.Columns[23] + "] NVARCHAR(50) NOT NULL," +
                    "    [" + dt_Processes.Columns[24] + "] NVARCHAR(50) NOT NULL," +
                    "    [" + dt_Processes.Columns[25] + "] NVARCHAR(50) NOT NULL," +
                    "    [" + dt_Processes.Columns[26] + "] NVARCHAR(50) NOT NULL," +
                    "    [" + dt_Processes.Columns[27] + "] NVARCHAR(50) NOT NULL," +
                    "    [Aktiv] INT NOT NULL DEFAULT(1)," +
                    "    [Datum] DATETIME NOT NULL DEFAULT(CONVERT(VARCHAR(19), '2018-12-31 23:59:59' ,120))," +
                    "    [Benutzer] NVARCHAR(50) NOT NULL DEFAULT('')," +
                    "    Unique (" + dt_Processes.Columns[0] + ",Datum)" +
                ");";

                string sqlCreaIS =
                    "CREATE TABLE[dbo].[" + myShared.Tbl_IS + "](" +
                    "    [Id]                  INT IDENTITY(1, 1) NOT NULL PRIMARY KEY," +
                    "    [" + dt_InformationSegments.Columns[0] + "] INT NOT NULL," +
                    "    [" + dt_InformationSegments.Columns[1] + "] VARCHAR(50) NOT NULL," +
                    "    [" + dt_InformationSegments.Columns[2] + "] VARCHAR(1000) NOT NULL," +
                    "    [" + dt_InformationSegments.Columns[3] + "] VARCHAR(1000) NOT NULL," +
                    "    [" + dt_InformationSegments.Columns[4] + "] VARCHAR(1000) NOT NULL," +
                    "    [" + dt_InformationSegments.Columns[5] + "] VARCHAR(1000) NOT NULL," +
                    "    [" + dt_InformationSegments.Columns[6] + "] VARCHAR(1000) NOT NULL," +
                    "    [" + dt_InformationSegments.Columns[7] + "] VARCHAR(1000) NOT NULL," +
                    "    [" + dt_InformationSegments.Columns[8] + "] VARCHAR(1000) NOT NULL," +
                    "    [" + dt_InformationSegments.Columns[9] + "] VARCHAR(1000) NOT NULL," +
                    "    [" + dt_InformationSegments.Columns[10] + "] VARCHAR(1000) NOT NULL," +
                    "    [" + dt_InformationSegments.Columns[11] + "] VARCHAR(1000) NOT NULL," +
                    "    [" + dt_InformationSegments.Columns[12] + "] VARCHAR(1000) NOT NULL," +
                    "    [" + dt_InformationSegments.Columns[13] + "] VARCHAR(1000) NOT NULL," +
                    "    [" + dt_InformationSegments.Columns[14] + "] VARCHAR(1000) NOT NULL," +
                    "    [Datum] DATETIME NOT NULL DEFAULT(CONVERT(VARCHAR(19), '2018-12-31 23:59:59',120))," +
                    "    [Benutzer] NVARCHAR(50) NOT NULL DEFAULT('')," +
                    "    Unique (" + dt_InformationSegments.Columns[0] + ", Id)" +
                    ");";

                string sqlCreaISAttribut =
                    "CREATE TABLE[dbo].[" + myShared.Tbl_IS_Attribute + "](" +
                    "    [Id]                  INT IDENTITY(1, 1) NOT NULL PRIMARY KEY," +
                    "    [" + dt_InformationSegmentAttributes.Columns[0] + "] INT NOT NULL," +
                    "    [" + dt_InformationSegmentAttributes.Columns[1] + "] VARCHAR(200) NOT NULL," +
                    "    [" + dt_InformationSegmentAttributes.Columns[2] + "] VARCHAR(200) NOT NULL," +
                    "    [" + dt_InformationSegmentAttributes.Columns[3] + "] INT NOT NULL," +
                    "    [" + dt_InformationSegmentAttributes.Columns[4] + "] INT NOT NULL," +
                    "    [" + dt_InformationSegmentAttributes.Columns[5] + "] INT NOT NULL," +
                    "    [" + dt_InformationSegmentAttributes.Columns[6] + "] INT NOT NULL," +
                    "    [" + dt_InformationSegmentAttributes.Columns[7] + "] INT NOT NULL," +
                    "    [" + dt_InformationSegmentAttributes.Columns[8] + "] INT NOT NULL," +
                    "    [Datum] DATETIME NOT NULL DEFAULT(CONVERT(VARCHAR(19), '2018-12-31 23:59:59',120))," +
                    "    [Benutzer] NVARCHAR(50) NOT NULL DEFAULT('')," +
                    "    Unique (" + dt_InformationSegmentAttributes.Columns[0] + ", Id)" +
                    ");";

                string sqlCreaDelta =
                    "CREATE TABLE[dbo].[" + myShared.Tbl_Delta + "](" +
                    "    [Id]                  INT IDENTITY(1, 1) NOT NULL PRIMARY KEY," +
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
                    "    [Datum] DATETIME NOT NULL DEFAULT(CONVERT(VARCHAR(19), '2018-12-31 23:59:59',120))," +
                    "    Unique (Prozess_Id, Applikation_Id)," +
                    "    Foreign Key(Prozess_Id,Datum_Prozess) references [" + myShared.Tbl_Prozesse + "](Prozess_Id,Datum)," +
                    "    Foreign Key(Applikation_Id,Datum_Applikation) references [" + myShared.Tbl_Applikationen + "](Applikation_Id,Datum)" +
                    ");";

                string sqlCreaLog =
                    "CREATE TABLE[dbo].[" + myShared.Tbl_Log + "] (" +
                    "    [Id]                  INT IDENTITY(1, 1) NOT NULL PRIMARY KEY," +
                    "    [Action] VARCHAR(200) NOT NULL," +
                    "    [Tabelle] VARCHAR(200) NOT NULL," +
                    "    [Details] VARCHAR(1000) NOT NULL," +
                    "    [Id_1] INT NOT NULL," +
                    "    [Id_2] INT NOT NULL," +
                    "    [Datum] DATETIME NOT NULL DEFAULT(CONVERT(VARCHAR(19), '2018-12-31 23:59:59',120))," +
                    "    [Benutzer] NVARCHAR(50) NOT NULL DEFAULT('')," +
                    ");";

                string sqlCreaOEs =
                    "CREATE TABLE[dbo].[" + myShared.Tbl_OEs + "] (" +
                    "    [Id]                  INT IDENTITY(1, 1) NOT NULL PRIMARY KEY," +
                    "    [OE_Name] VARCHAR(200) NOT NULL," +
                    "    [OE_Nummer] VARCHAR(200) NOT NULL," +
                    "    [Datum] DATETIME NOT NULL DEFAULT(CONVERT(VARCHAR(19), '2018-12-31 23:59:59',120))," +
                    "    [Benutzer] NVARCHAR(50) NOT NULL DEFAULT('')," +
                    "    Unique (OE_Name, OE_Nummer)" +
                    ");";

                string sqlCreaSettings =
                    "CREATE TABLE[dbo].[" + myShared.Tbl_Settings + "] (" +
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
                    "    [Datum] DATETIME NOT NULL DEFAULT(CONVERT(VARCHAR(19), '2018-12-31 23:59:59',120))," +
                    "    [Benutzer] NVARCHAR(50) NOT NULL," +
                    ");";

                string sqlCreaLock =
                    "CREATE TABLE[dbo].[" + myShared.Tbl_Lock + "] (" +
                    "    [Id]                  INT IDENTITY(1, 1) NOT NULL PRIMARY KEY," +
                    "    [Table_Flag] INT NOT NULL," +
                    "    [Object_Id] INT NOT NULL," +
                    "    [Datum] DATETIME NOT NULL," +
                    "    [BenutzerNnVn] NVARCHAR(50) NOT NULL DEFAULT('')," +
                    "    [Benutzer] NVARCHAR(50) NOT NULL DEFAULT('')," +
                    ");";
                #endregion
                List<string> sqlCommandList = new List<string>();
                sqlCommandList.Add(sqlDropLog);
                sqlCommandList.Add(sqlCreaLog);
                sqlCommandList.Add(sqlDropDelta);
                sqlCommandList.Add(sqlDropProc_App);
                sqlCommandList.Add(sqlDropSBA);
                sqlCommandList.Add(sqlCreaSBA);
                sqlCommandList.Add(sqlDropProcesses);
                sqlCommandList.Add(sqlCreaProcesses);
                sqlCommandList.Add(sqlCreaProc_App);
                sqlCommandList.Add(sqlCreaDelta);
                sqlCommandList.Add(sqlDropIS);
                sqlCommandList.Add(sqlCreaIS);
                sqlCommandList.Add(sqlDropISAttribut);
                sqlCommandList.Add(sqlCreaISAttribut);
                sqlCommandList.Add(sqlDropOEs);
                sqlCommandList.Add(sqlCreaOEs);
                sqlCommandList.Add(sqlDropSettings);
                sqlCommandList.Add(sqlCreaSettings);
                sqlCommandList.Add(sqlDropLock);
                sqlCommandList.Add(sqlCreaLock);

                //Datenbank Operation durch Service übernommen
                return myData.CreateDataModel(sqlCommandList, dt_Processes, dt_Applications, dt_Relation, dt_InformationSegments, dt_InformationSegmentAttributes);               
            }
        }
        #endregion

        #region Excel Import
        private Excel.Workbook Open(Excel.Application excelInstance,
        string fileName, bool readOnly = false,
        bool editable = true, bool updateLinks = true)
        {
            Excel.Workbook book = excelInstance.Workbooks.Open(
                fileName, updateLinks, readOnly,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, editable, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing);
            return book;
        }

        /// <summary>
        /// Funktion um Prozessdaten aus Excel zu lesen und in DataTable zu speichern
        /// </summary>
        /// <param name="xlsxPath"> File, der als Datenquelle dienen soll </param>
        /// <param name="workbook"> Workbook Objekt, auf dem Operationen durchgeführt werden können </param>
        /// <returns> Datatable, der die gelesenen Daten enthält </returns>
        private DataTable GetProcessDataFromXLSX(string xlsxPath, IWorkbook workbook)
        {
            ISheet sheet = workbook.GetSheet(P_Sheet);
            if (sheet != null)
            {
                DataTable dt = new DataTable(sheet.SheetName);
                CellRangeAddress cellRange = CellRangeAddress.ValueOf(P_Range);
                int index = 0;
                for (var i = cellRange.FirstRow; i <= cellRange.LastRow; i++)
                {
                    var row = sheet.GetRow(i);
                    if (i == cellRange.FirstRow)
                    {
                        for (var j = cellRange.FirstColumn; j <= cellRange.LastColumn; j++)
                        {
                            var headerCell = row.GetCell(j);
                            dt.Columns.Add(headerCell.ToString());
                        }
                    }
                    else
                    {
                        DataRow dataRow = dt.NewRow();
                        index = 0;
                        for (var j = cellRange.FirstColumn; j <= cellRange.LastColumn; j++)
                        {
                            dataRow[index] = row.GetCell(j).ToString();
                            index++;
                        }
                        try
                        {
                            dt.Rows.Add(dataRow);
                        }
                        catch (Exception ex)
                        {
                            myDia.ShowError("Fehler beim Import", ex);
                            return null;
                        }
                    }
                }
                return dt;
            }
            else
            {
                myDia.ShowWarning("Excel-Quelldatei hat nicht das passende Format!\n(Prozesse)\nVoraussetzungen:\nSheet-Name: '" + P_Sheet + "'\nRange: '" + P_Range + "'");
                return null;
            }
        }

        /// <summary>
        /// Funktion um Anwendungsdaten aus Excel zu lesen und in DataTable zu speichern
        /// </summary>
        /// <param name="xlsxPath"> File, der als Datenquelle dienen soll </param>
        /// <param name="workbook"> Workbook Objekt, auf dem Operationen durchgeführt werden können </param>
        /// <returns> Datatable, der die gelesenen Daten enthält </returns>
        private DataTable GetApplicationDataFromXLSX(string xlsxPath, IWorkbook workbook)
        {
            ISheet sheet = workbook.GetSheet(A_Sheet);
            if (sheet != null)
            {
                DataTable dt = new DataTable(sheet.SheetName);
                CellRangeAddress cellRange = CellRangeAddress.ValueOf(A_Range);
                int index = 0;
                for (var i = cellRange.FirstRow; i <= cellRange.LastRow; i++)
                {
                    var row = sheet.GetRow(i);
                    if (i == cellRange.FirstRow)
                    {
                        for (var j = cellRange.FirstColumn; j <= cellRange.LastColumn; j++)
                        {
                            var headerCell = row.GetCell(j);
                            dt.Columns.Add(headerCell.ToString());
                        }
                    }
                    else
                    {
                        DataRow dataRow = dt.NewRow();
                        index = 0;
                        for (var j = cellRange.FirstColumn; j <= cellRange.LastColumn; j++)
                        {
                            dataRow[index] = row.GetCell(j).ToString();
                            index++;
                        }
                        try
                        {
                            dt.Rows.Add(dataRow);
                        }
                        catch (Exception ex)
                        {
                            myDia.ShowError("Fehler beim Import", ex);
                            return null;
                        }
                    }
                }
                return dt;
            }
            else
            {
                myDia.ShowWarning("Excel-Quelldatei hat nicht das passende Format!\n(Applikationen)\nVoraussetzungen:\nSheet-Name: '" + A_Sheet + "'\nRange: '" + A_Range + "'");
                return null;
            }
        }

        /// <summary>
        /// Funktion um DataTable zu erzeugen, welcher die Relationen zwischen Prozessen und Anwendungen anzeigt
        /// </summary>
        /// <param name="xlsxPath"> File, der als Datenquelle dienen soll </param>
        /// <param name="workbook"> Workbook Objekt, auf dem Operationen durchgeführt werden können </param>
        /// <returns> Datatable, der die gelesenen Daten enthält </returns>
        private DataTable FillSQLRelationFromXLSX(string xlsxPath, IWorkbook workbook)
        {
            DataTable dt = new DataTable();
            ISheet sheet = workbook.GetSheet(P_A_Matrix_Sheet);
            if (sheet != null)
            {
                CellRangeAddress cellRange = CellRangeAddress.ValueOf(P_A_Matrix_Range);
                int rows = cellRange.LastRow - cellRange.FirstRow + 1;
                int cols = cellRange.LastColumn - cellRange.FirstColumn + 1;
                dt.Columns.Add("Prozess");
                dt.Columns.Add("Applikation");
                dt.Columns.Add("Relation");
                for (var i = cellRange.FirstRow; i <= cellRange.LastRow; i++)
                {
                    var row = sheet.GetRow(i);
                    for (var j = cellRange.FirstColumn; j <= cellRange.LastColumn; j++)
                    {
                        if (row.GetCell(j).ToString() == "1")
                        {
                            DataRow newRow = dt.NewRow();
                            newRow[0] = i + 1 - cellRange.FirstRow;
                            newRow[1] = j + 1 - cellRange.FirstColumn;
                            newRow[2] = 1;
                            try
                            {
                                dt.Rows.Add(newRow);
                            }
                            catch (Exception ex)
                            {
                                myDia.ShowError("Fehler beim Import", ex);
                                return null;
                            }
                        }
                    }
                }
                return dt;
            }
            else
            {
                myDia.ShowWarning("Excel-Quelldatei hat nicht das passende Format!\n(Matrix)\nVoraussetzungen:\nSheet-Name: '" + P_Sheet + "'\nRange: '" + P_A_Matrix_Range + "'");
                return null;
            }
        }

        /// <summary>
        /// Funktion um Informationssegmentdaten aus Excel zu lesen und in DataTable zu speichern
        /// </summary>
        /// <param name="xlsxPath"> File, der als Datenquelle dienen soll </param>
        /// <param name="workbook"> Workbook Objekt, auf dem Operationen durchgeführt werden können </param>
        /// <returns> Datatable, der die gelesenen Daten enthält </returns>
        private DataTable GetInformationSegmentDataFromXLSX(string xlsxPath, IWorkbook workbook)
        {
            ISheet sheet = workbook.GetSheet(I_Sheet);
            if (sheet != null)
            {
                DataTable dt = new DataTable(sheet.SheetName);
                CellRangeAddress cellRange = CellRangeAddress.ValueOf(I_Range);
                int index = 0;
                for (var i = cellRange.FirstRow; i <= cellRange.LastRow; i++)
                {
                    var row = sheet.GetRow(i);
                    if (i == cellRange.FirstRow)
                    {
                        for (var j = cellRange.FirstColumn; j <= cellRange.LastColumn; j++)
                        {
                            var headerCell = row.GetCell(j).ToString();
                            dt.Columns.Add(headerCell.ToString());
                        }
                    }
                    else
                    {
                        DataRow dataRow = dt.NewRow();
                        index = 0;
                        for (var j = cellRange.FirstColumn; j <= cellRange.LastColumn; j++)
                        {
                            dataRow[index] = row.GetCell(j).ToString();
                            index++;
                        }
                        try
                        {
                            dt.Rows.Add(dataRow);
                        }
                        catch (Exception ex)
                        {
                            myDia.ShowError("Fehler beim Import", ex);
                            return null;
                        }
                    }
                }
                return dt;
            }
            else
            {
                myDia.ShowWarning("Excel-Quelldatei hat nicht das passende Format!\n(Informationssegmente)\nVoraussetzungen:\nSheet-Name: '" + I_Sheet + "'\nRange: '" + I_Range + "'");
                return null;
            }
        }

        /// <summary>
        /// Funktion um Informationssegmentattributdaten aus Excel zu lesen und in DataTable zu speichern
        /// </summary>
        /// <param name="xlsxPath"> File, der als Datenquelle dienen soll </param>
        /// <param name="workbook"> Workbook Objekt, auf dem Operationen durchgeführt werden können </param>
        /// <returns> Datatable, der die gelesenen Daten enthält </returns>
        private DataTable GetInformationSegmentAttributDataFromXLSX(string xlsxPath, IWorkbook workbook)
        {
            ISheet sheet = workbook.GetSheet(I_A_Sheet);
            if (sheet != null)
            {
                DataTable dt = new DataTable(sheet.SheetName);
                CellRangeAddress cellRange = CellRangeAddress.ValueOf(I_A_Range);
                int index = 0;

                for (var i = cellRange.FirstRow; i <= cellRange.LastRow; i++)
                {
                    var row = sheet.GetRow(i);
                    if (i == cellRange.FirstRow)
                    {
                        for (var j = cellRange.FirstColumn; j <= cellRange.LastColumn; j++)
                        {
                            var headerCell = row.GetCell(j).ToString();
                            dt.Columns.Add(headerCell.ToString());
                        }
                    }
                    else
                    {
                        DataRow dataRow = dt.NewRow();
                        index = 0;
                        for (var j = cellRange.FirstColumn; j <= cellRange.LastColumn; j++)
                        {
                            dataRow[index] = row.GetCell(j).ToString();
                            index++;
                        }
                        try
                        {
                            dt.Rows.Add(dataRow);
                        }
                        catch (Exception ex)
                        {
                            myDia.ShowError("Fehler", ex);
                            return null;
                        }
                    }
                }
                return dt;
            }
            else
            {
                myDia.ShowWarning("Excel-Quelldatei hat nicht das passende Format!\n(Informationssegment-Attribute)\nVoraussetzungen:\nSheet-Name: '" + I_A_Sheet + "'\nRange: '" + I_A_Range + "'");
                return null;
            }
        }
        #endregion

        /// <summary>
        /// VM bereinigen
        /// </summary>
        override public void Cleanup()
        {
            Messenger.Default.Unregister(this);
            SimpleIoc.Default.Unregister(this);
            base.Cleanup();
        }
    }
}
