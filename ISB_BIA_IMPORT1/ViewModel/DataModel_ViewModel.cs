using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using ISB_BIA_IMPORT1.Services;
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
        private string _originalFile;
        private string _workFile;
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
                          InitialDirectory = _myShared.InitialDirectory,
                          FileName = _myShared.InitialDirectory + @"\ISB_BIA-SBA.xlsx",
                          Filter = "Excel Files (*.xlsm;*.xlsx)|*.xlsm;*xlsx"
                      };
                      bool? result = _myDia.Open(openFileDialog);

                      if (result == true)
                      {
                          _originalFile = openFileDialog.FileName;
                          _workFile = openFileDialog.InitialDirectory + @"\ISB_BIA-SBA_tmp.xlsx";
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
                          _myNavi.NavigateBack();
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
                    if (_myDia.CancelDecision())
                    {
                        Cleanup();
                        _myNavi.NavigateBack();
                    }
                });
            }
        }

        #region Services
        private readonly IMyDataService _myData;
        private readonly IMyNavigationService _myNavi;
        private readonly IMyDialogService _myDia;
        private readonly IMySharedResourceService _myShared;
        #endregion

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="myDataService"></param>
        /// <param name="myDialogService"></param>
        /// <param name="myNavigationService"></param>
        /// <param name="mySharedResourceService"></param>
        public DataModel_ViewModel(IMyDataService myDataService,IMyDialogService myDialogService, IMyNavigationService myNavigationService, IMySharedResourceService mySharedResourceService)
        {
            #region Services
            _myData = myDataService;
            _myNavi = myNavigationService;
            _myDia = myDialogService;
            _myShared = mySharedResourceService;
            Source = _myShared.Source;
            #endregion
            _originalFile = _myShared.InitialDirectory + @"\ISB_BIA-SBA.xlsx";
            _workFile = _myShared.InitialDirectory + @"\ISB_BIA-SBA_tmp.xlsx";

        }


        #region Datenmodell erstellen
        /// <summary>
        /// Methode zur Überprüfung der angegebenen Datenbereiche und sonstigen Angaben
        /// </summary>
        /// <returns></returns>
        private bool CheckForCreation()
        {
            string rangePattern = "[A-Z]+[1-9]+:[A-Z]+[1-9]+";
            if (File.Exists(_originalFile))
            {
                if (File.Exists(_workFile)) File.Delete(_workFile);
                File.Copy(_originalFile, _workFile);
                IWorkbook workbook;
                using (FileStream stream = new FileStream(_workFile, FileMode.Open, FileAccess.Read))
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
                        if (GetDataForDatamodel(workbook))
                        {
                            if (File.Exists(_workFile)) File.Delete(_workFile);
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
                    if (File.Exists(_workFile)) File.Delete(_workFile);
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
            DataTable dt_Applications = GetApplicationDataFromXLSX(workbook);
            DataTable dt_Processes = GetProcessDataFromXLSX(workbook);
            DataTable dt_InformationSegments = GetInformationSegmentDataFromXLSX(workbook);
            DataTable dt_InformationSegmentAttributes = GetInformationSegmentAttributDataFromXLSX(workbook);
            DataTable dt_Relation = FillSQLRelationFromXLSX(workbook);

            if (dt_Applications == null || dt_Processes == null || dt_InformationSegments == null || dt_InformationSegmentAttributes == null || dt_Relation == null)
            {
                _myDia.ShowWarning("Datenodell konnte nicht erzeugt werden!\nÜberprüfen Sie die Datenquelle.");
                return false;
            }
            else
            {


                //Datenbank Operation durch Service übernommen
                return _myData.CreateDataModel(dt_Processes, dt_Applications, dt_Relation, dt_InformationSegments, dt_InformationSegmentAttributes);               
            }
        }
        #endregion

        #region Excel Import
        /// <summary>
        /// Funktion um Prozessdaten aus Excel zu lesen und in DataTable zu speichern
        /// </summary>
        /// <param name="workbook"> Workbook Objekt, auf dem Operationen durchgeführt werden können </param>
        /// <returns> Datatable, der die gelesenen Daten enthält </returns>
        private DataTable GetProcessDataFromXLSX(IWorkbook workbook)
        {
            ISheet sheet = workbook.GetSheet(P_Sheet);
            if (sheet != null)
            {
                DataTable dt = new DataTable(sheet.SheetName);
                CellRangeAddress cellRange = CellRangeAddress.ValueOf(P_Range);
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
                _myDia.ShowWarning("Excel-Quelldatei hat nicht das passende Format!\n(Prozesse)\nVoraussetzungen:\nSheet-Name: '" + P_Sheet + "'\nRange: '" + P_Range + "'");
                return null;
            }
        }

        /// <summary>
        /// Funktion um Anwendungsdaten aus Excel zu lesen und in DataTable zu speichern
        /// </summary>
        /// <param name="workbook"> Workbook Objekt, auf dem Operationen durchgeführt werden können </param>
        /// <returns> Datatable, der die gelesenen Daten enthält </returns>
        private DataTable GetApplicationDataFromXLSX(IWorkbook workbook)
        {
            ISheet sheet = workbook.GetSheet(A_Sheet);
            if (sheet != null)
            {
                DataTable dt = new DataTable(sheet.SheetName);
                CellRangeAddress cellRange = CellRangeAddress.ValueOf(A_Range);
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
                _myDia.ShowWarning("Excel-Quelldatei hat nicht das passende Format!\n(Applikationen)\nVoraussetzungen:\nSheet-Name: '" + A_Sheet + "'\nRange: '" + A_Range + "'");
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
            ISheet sheet = workbook.GetSheet(P_A_Matrix_Sheet);
            if (sheet != null)
            {
                CellRangeAddress cellRange = CellRangeAddress.ValueOf(P_A_Matrix_Range);
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
                _myDia.ShowWarning("Excel-Quelldatei hat nicht das passende Format!\n(Matrix)\nVoraussetzungen:\nSheet-Name: '" + P_Sheet + "'\nRange: '" + P_A_Matrix_Range + "'");
                return null;
            }
        }

        /// <summary>
        /// Funktion um Informationssegmentdaten aus Excel zu lesen und in DataTable zu speichern
        /// </summary>
        /// <param name="workbook"> Workbook Objekt, auf dem Operationen durchgeführt werden können </param>
        /// <returns> Datatable, der die gelesenen Daten enthält </returns>
        private DataTable GetInformationSegmentDataFromXLSX(IWorkbook workbook)
        {
            ISheet sheet = workbook.GetSheet(I_Sheet);
            if (sheet != null)
            {
                DataTable dt = new DataTable(sheet.SheetName);
                CellRangeAddress cellRange = CellRangeAddress.ValueOf(I_Range);
                for (var i = cellRange.FirstRow; i <= cellRange.LastRow; i++)
                {
                    var row = sheet.GetRow(i);
                    if (i == cellRange.FirstRow)
                    {
                        for (var j = cellRange.FirstColumn; j <= cellRange.LastColumn; j++)
                        {
                            var headerCell = row.GetCell(j).ToString();
                            dt.Columns.Add(headerCell);
                        }
                    }
                    else
                    {
                        DataRow dataRow = dt.NewRow();
                        int index = 0;
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
                _myDia.ShowWarning("Excel-Quelldatei hat nicht das passende Format!\n(Informationssegmente)\nVoraussetzungen:\nSheet-Name: '" + I_Sheet + "'\nRange: '" + I_Range + "'");
                return null;
            }
        }

        /// <summary>
        /// Funktion um Informationssegmentattributdaten aus Excel zu lesen und in DataTable zu speichern
        /// </summary>
        /// <param name="workbook"> Workbook Objekt, auf dem Operationen durchgeführt werden können </param>
        /// <returns> Datatable, der die gelesenen Daten enthält </returns>
        private DataTable GetInformationSegmentAttributDataFromXLSX(IWorkbook workbook)
        {
            ISheet sheet = workbook.GetSheet(I_A_Sheet);
            if (sheet != null)
            {
                DataTable dt = new DataTable(sheet.SheetName);
                CellRangeAddress cellRange = CellRangeAddress.ValueOf(I_A_Range);
                for (var i = cellRange.FirstRow; i <= cellRange.LastRow; i++)
                {
                    var row = sheet.GetRow(i);
                    if (i == cellRange.FirstRow)
                    {
                        for (var j = cellRange.FirstColumn; j <= cellRange.LastColumn; j++)
                        {
                            var headerCell = row.GetCell(j).ToString();
                            dt.Columns.Add(headerCell);
                        }
                    }
                    else
                    {
                        DataRow dataRow = dt.NewRow();
                        int index = 0;
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
                            _myDia.ShowError("Fehler", ex);
                            return null;
                        }
                    }
                }
                return dt;
            }
            else
            {
                _myDia.ShowWarning("Excel-Quelldatei hat nicht das passende Format!\n(Informationssegment-Attribute)\nVoraussetzungen:\nSheet-Name: '" + I_A_Sheet + "'\nRange: '" + I_A_Range + "'");
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
