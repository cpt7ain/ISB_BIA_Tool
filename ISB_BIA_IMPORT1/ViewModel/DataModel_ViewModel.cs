using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Win32;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using ISB_BIA_IMPORT1.Helpers;
using ISB_BIA_IMPORT1.Services.Interfaces;

namespace ISB_BIA_IMPORT1.ViewModel
{
    /// <summary>
    /// VM zur Darstellung Datenmodelleinstellungen
    /// </summary>
    public class DataModel_ViewModel : ViewModelBase
    {
        #region Backing-Fields
        private MyRelayCommand _cmd_ChooseFile;
        private MyRelayCommand _cmd_Save;
        //Hier die Daten angeben, welche für den DatenImport benötigt werden (Name des Excel-Sheets, Zellenrange)
        private string _str_Sheet_Process = "3 - BIA";
        private string _str_Sheet_App = "4 - SBA";
        private string _str_Range_Process = "B6:AB776";
        private string _str_Range_App = "B6:O466";
        private string _str_Sheet_Process_App_Matrix = "3 - BIA";
        private string _str_Range_Process_App_Matrix = "AC7:RT776";
        private string _str_Range_InformationSegment = "B8:P43";
        private string _str_Sheet_InformationSegment = "Informationssegmente";
        private string _str_Range_Attribute = "Y8:AG18";
        private string _str_Sheet_Attribute = "Informationssegmente";
        #endregion

        #region Strings der Dateien und SQL-Drop Befehle (abhängig von den Daten in myShared im Konstruktor erzeugt)
        //Original & Arbeitsdatei der Quelldaten
        private string _str_SourceFile;
        private string _str_OriginalFile;
        private string _str_WorkFile;
        #endregion

        #region Sheet-Namen und Sheet-Ranges zum definieren der einzulesenden Datenbereiche
        /// <summary>
        /// Name des Sheets mit der Prozessliste
        /// </summary>
        public string Str_Sheet_Process
        {
            get => _str_Sheet_Process;
            set => Set(() => Str_Sheet_Process, ref _str_Sheet_Process, value);
        }
        /// <summary>
        /// Bereich der Prozessdaten auf dem Sheet
        /// </summary>
        public string Str_Range_Process
        {
            get => _str_Range_Process;
            set => Set(() => Str_Range_Process, ref _str_Range_Process, value);
        }
        /// <summary>
        /// Name des Sheets mit der Anwendungsliste
        /// </summary>
        public string Str_Sheet_App
        {
            get => _str_Sheet_App;
            set => Set(() => Str_Sheet_App, ref _str_Sheet_App, value);

        }
        /// <summary>
        /// Bereich der Anwendungsdaten auf dem Sheet
        /// </summary>
        public string Str_Range_App
        {
            get => _str_Range_App;
            set => Set(() => Str_Range_App, ref _str_Range_App, value);

        }
        /// <summary>
        /// Name des Sheets mit der Prozess-Anwendungs-Matrix
        /// </summary>
        public string Str_Sheet_Process_App_Matrix
        {
            get => _str_Sheet_Process_App_Matrix;
            set => Set(() => Str_Sheet_Process_App_Matrix, ref _str_Sheet_Process_App_Matrix, value);

        }
        /// <summary>
        /// Bereich der Prozess-Anwendungsdaten auf dem Sheet
        /// </summary>
        public string Str_Range_Process_App_Matrix
        {
            get => _str_Range_Process_App_Matrix;
            set => Set(() => Str_Range_Process_App_Matrix, ref _str_Range_Process_App_Matrix, value);

        }
        /// <summary>
        /// Name des Sheets mit den Informationssegmenten
        /// </summary>
        public string Str_Sheet_InformationSegment
        {
            get => _str_Sheet_InformationSegment;
            set => Set(() => Str_Sheet_InformationSegment, ref _str_Sheet_InformationSegment, value);

        }
        /// <summary>
        /// Bereich der Informationssegmentdaten auf dem Sheet
        /// </summary>
        public string Str_Range_InformationSegment
        {
            get => _str_Range_InformationSegment;
            set => Set(() => Str_Range_InformationSegment, ref _str_Range_InformationSegment, value);

        }
        /// <summary>
        /// Name des Sheets mit den Informationssegment-Attribut-Daten
        /// </summary>
        public string Str_Sheet_Attribute
        {
            get => _str_Sheet_Attribute;
            set => Set(() => Str_Sheet_Attribute, ref _str_Sheet_Attribute, value);
        }
        /// <summary>
        /// Bereich der Informationssegment-Attribut-Daten auf dem Sheet
        /// </summary>
        public string Str_Range_Attribute
        {
            get => _str_Range_Attribute;
            set => Set(() => Str_Range_Attribute, ref _str_Range_Attribute, value);
        }
        #endregion

        /// <summary>
        /// Dateipfad für Quelldatei
        /// </summary>
        public string Str_SourceFile
        {
            get => _str_SourceFile;
            set => Set(() => Str_SourceFile, ref _str_SourceFile, value);
        }

        #region Commands
        /// <summary>
        /// Quelldatei Auswählen
        /// </summary>
        public MyRelayCommand Cmd_ChooseFile
        {
            get => _cmd_ChooseFile
                  ?? (_cmd_ChooseFile = new MyRelayCommand(() =>
                  {
                      OpenFileDialog openFileDialog = new OpenFileDialog
                      {
                          DefaultExt = ".xlsx",
                          InitialDirectory = _myShared.Dir_InitialDirectory,
                          FileName = _myShared.Dir_InitialDirectory + @"\ISB_BIA-SBA.xlsx",
                          Filter = "Excel Files (*.xlsm;*.xlsx)|*.xlsm;*xlsx"
                      };
                      bool? result = _myDia.Open(openFileDialog);

                      if (result == true)
                      {
                          _str_OriginalFile = openFileDialog.FileName;
                          _str_WorkFile = openFileDialog.InitialDirectory + @"\ISB_BIA-SBA_tmp.xlsx";
                          Str_SourceFile = _str_OriginalFile;
                      }
                  }));
        }
        /// <summary>
        /// Erneuerung des Datenmodells Ausführen und schliessen
        /// </summary>
        public MyRelayCommand Cmd_Save
        {
            get => _cmd_Save
                  ?? (_cmd_Save = new MyRelayCommand(() =>
                  {
                      if (CheckForCreation())
                      {
                          Cleanup();
                          _myNavi.NavigateBack();
                      }
                  }));
        }
        /// <summary>
        /// Command zum Zurückkehren zum vorherigen VM
        /// </summary>
        public MyRelayCommand Cmd_NavBack
        {
            get
            {
                return new MyRelayCommand(() =>
                {
                    if (_myDia.CancelDecision())
                    {
                        Cleanup();
                        _myNavi.NavigateBack();
                    }
                });
            }
        }
        #endregion

        #region Services
        private readonly IDataModelService _myDM;
        private readonly INavigationService _myNavi;
        private readonly IDialogService _myDia;
        private readonly ISharedResourceService _myShared;
        #endregion

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="myDM"></param>
        /// <param name="myDia"></param>
        /// <param name="myNavi"></param>
        /// <param name="myShared"></param>
        public DataModel_ViewModel(IDataModelService myDM,IDialogService myDia, 
            INavigationService myNavi, ISharedResourceService myShared)
        {
            #region Services
            _myDM = myDM;
            _myNavi = myNavi;
            _myDia = myDia;
            _myShared = myShared;
            #endregion
            Str_SourceFile = _myShared.Dir_Source;
            _str_OriginalFile = _myShared.Dir_InitialDirectory + @"\ISB_BIA-SBA.xlsx";
            _str_WorkFile = _myShared.Dir_InitialDirectory + @"\ISB_BIA-SBA_tmp.xlsx";
        }

        #region Datenmodell erstellen
        /// <summary>
        /// Methode zur Überprüfung der angegebenen Datenbereiche und sonstigen Angaben
        /// </summary>
        /// <returns></returns>
        private bool CheckForCreation()
        {
            string rangePattern = "[A-Z]+[1-9]+:[A-Z]+[1-9]+";
            if (File.Exists(_str_OriginalFile))
            {
                if (File.Exists(_str_WorkFile)) File.Delete(_str_WorkFile);
                File.Copy(_str_OriginalFile, _str_WorkFile);
                IWorkbook workbook;
                using (FileStream stream = new FileStream(_str_WorkFile, FileMode.Open, FileAccess.Read))
                {
                    workbook = new XSSFWorkbook(stream);
                }
                if (workbook.GetSheet(Str_Sheet_Process) != null &&
                   workbook.GetSheet(Str_Sheet_App) != null &&
                   workbook.GetSheet(Str_Sheet_InformationSegment) != null &&
                   workbook.GetSheet(Str_Sheet_Attribute) != null &&
                   workbook.GetSheet(Str_Sheet_Process_App_Matrix) != null &&
                   Regex.IsMatch(Str_Range_Process, rangePattern) &&
                   Regex.IsMatch(Str_Range_App, rangePattern) &&
                   Regex.IsMatch(Str_Range_InformationSegment, rangePattern) &&
                   Regex.IsMatch(Str_Range_Attribute, rangePattern) &&
                   Regex.IsMatch(Str_Range_Process_App_Matrix, rangePattern))
                {
                    try
                    {
                        if (GetDataForDatamodel(workbook))
                        {
                            if (File.Exists(_str_WorkFile)) File.Delete(_str_WorkFile);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        _myDia.ShowError("Fehler beim erneuern des Datenmodells.", ex);
                        return false;
                    }
                }
                else
                {
                    if (File.Exists(_str_WorkFile)) File.Delete(_str_WorkFile);
                    _myDia.ShowInfo("Bitte überprüfen Sie Ihre Eingaben!");
                    return false;
                }
            }
            else
            {
                _myDia.ShowError("Datei nicht gefunden!");
                return false;
            }
        }

        /// <summary>
        /// Aktionen welche zur Erstellung des Datenmodells aus der bisherigen Excel Datei dienen
        /// erstellen der Tabellen, füllen der Tabellen mit den Prozess/Anwendungsdaten (etc.) des Excel Sheets        
        /// /// </summary>
        /// <param name="workbook"> Workbook Objekt, auf dem Operationen durchgeführt werden können </param>
        /// <returns> Erfolg </returns>
        private bool GetDataForDatamodel(IWorkbook workbook)
        {
            //Funktionen ausführen, um daten aus der Excel-Datei zu erhalten
            DataTable dt_Applications = GetDataFromXLSX(workbook, Str_Sheet_App, Str_Range_App, "Applikationen");
            DataTable dt_Processes = GetDataFromXLSX(workbook, Str_Sheet_Process, Str_Range_Process, "Prozesse");
            DataTable dt_InformationSegments = GetDataFromXLSX(workbook, Str_Sheet_InformationSegment, Str_Range_InformationSegment, "Segmente");
            DataTable dt_InformationSegmentAttributes = GetDataFromXLSX(workbook, Str_Sheet_Attribute, Str_Range_Attribute, "Attribute");
            DataTable dt_Relation = FillSQLRelationFromXLSX(workbook);

            if (dt_Applications == null || dt_Processes == null || dt_InformationSegments == null || dt_InformationSegmentAttributes == null || dt_Relation == null)
            {
                _myDia.ShowWarning("Datenodell konnte nicht erzeugt werden!\nÜberprüfen Sie die Datenquelle.");
                return false;
            }
            else
            {
                //Datenbank Operation durch Service übernommen
                return _myDM.Create(dt_Processes, dt_Applications, dt_Relation, dt_InformationSegments, dt_InformationSegmentAttributes);               
            }
        }
        #endregion

        #region Excel Import
        /// <summary>
        /// Funktion um Prozessdaten aus Excel zu lesen und in DataTable zu speichern
        /// </summary>
        /// <param name="workbook"> Workbook Objekt, auf dem Operationen durchgeführt werden können </param>
        /// <returns> Datatable, der die gelesenen Daten enthält </returns>
        private DataTable GetDataFromXLSX(IWorkbook workbook, string sheetName, string range, string subject)
        {
            ISheet sheet = workbook.GetSheet(sheetName);
            if (sheet != null)
            {
                DataTable dt = new DataTable(sheet.SheetName);
                CellRangeAddress cellRange = CellRangeAddress.ValueOf(range);
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
                        var index = 0;
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
                            _myDia.ShowError("Fehler beim Import", ex);
                            return null;
                        }
                    }
                }
                return dt;
            }
            else
            {
                _myDia.ShowWarning("Excel-Quelldatei hat nicht das passende Format!\n("+subject+")\nVoraussetzungen:\nSheet-Name: '" + sheetName + "'\nRange: '" + range + "'");
                return null;
            }
        }
        /// <summary>
        /// Funktion um DataTable zu erzeugen, welcher die Relationen zwischen Prozessen und Anwendungen anzeigt
        /// </summary>
        /// <param name="workbook"> Workbook Objekt, auf dem Operationen durchgeführt werden können </param>
        /// <returns> Datatable, der die gelesenen Daten enthält </returns>
        private DataTable FillSQLRelationFromXLSX(IWorkbook workbook)
        {
            DataTable dt = new DataTable();
            ISheet sheet = workbook.GetSheet(Str_Sheet_Process_App_Matrix);
            if (sheet != null)
            {
                CellRangeAddress cellRange = CellRangeAddress.ValueOf(Str_Range_Process_App_Matrix);
                dt.Columns.Add("Prozess_Id");
                dt.Columns.Add("Applikation_Id");
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
                                _myDia.ShowError("Fehler beim Import", ex);
                                return null;
                            }
                        }
                    }
                }
                return dt;
            }
            else
            {
                _myDia.ShowWarning("Excel-Quelldatei hat nicht das passende Format!\n(Matrix)\nVoraussetzungen:\nSheet-Name: '" + Str_Sheet_Process + "'\nRange: '" + Str_Range_Process_App_Matrix + "'");
                return null;
            }
        }
        #endregion

        /// <summary>
        /// VM bereinigen
        /// </summary>
        override public void Cleanup()
        {
            SimpleIoc.Default.Unregister(this);
            base.Cleanup();
        }
    }
}
