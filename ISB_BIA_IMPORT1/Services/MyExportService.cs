using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ISB_BIA_IMPORT1.LINQ2SQL;
using Microsoft.Win32;
using NPOI.HSSF.UserModel;

namespace ISB_BIA_IMPORT1.Services
{
    class MyExportService : IMyExportService
    {
        readonly IMyDialogService _myDia;
        readonly IMyDataService _myData;
        readonly IMySharedResourceService _myShared;

        ISB_BIA_Settings Setting { get; set; }
        ObservableCollection<ISB_BIA_Informationssegmente> ISList { get; set; }

        public MyExportService(IMyDialogService myDia, IMyDataService myDataService, IMySharedResourceService mySharedResourceService)
        {
            this._myDia = myDia;
            this._myData = myDataService;
            this._myShared = mySharedResourceService;
            if (!_myShared.ConstructionMode) Setting = _myData.GetSettings();
            if (!_myShared.ConstructionMode) ISList = _myData.GetEnabledSegments();
        }

        public bool AllApplicationsExport()
        {
            //Alle Applikationen abrufen
            ObservableCollection<ISB_BIA_Applikationen> queryApplications;
            queryApplications = _myData.GetApplications();
            if (queryApplications != null)
                return ExportApplications(queryApplications);
            else
                return false;
        }

        public bool AllActiveApplicationsExport()
        {
            //Alle Applikationen abrufen
            ObservableCollection<ISB_BIA_Applikationen> queryApplications;
            queryApplications = _myData.GetActiveApplications();
            if (queryApplications != null)
                return ExportApplications(queryApplications, "SBA_");
            else
                return false;
        }

        public bool ExportApplications(ObservableCollection<ISB_BIA_Applikationen> apps, string title="", int id=0)
        {
            //SaveFileDialog öffnen mit default Ordner "Dokumente"
            string a = (id == 0) ? "": id.ToString();
            SaveFileDialog sfd = new SaveFileDialog()
            {
                OverwritePrompt = true,
                FileName = title+"Anwendungsübersicht"+ a + "_Export_" + DateTime.Now.ToShortDateString() + ".xls",
                Filter = "Excel-Datei xls|*.xls",
                Title = "Speichern der Anwendungsübersicht",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            bool? result = _myDia.Save(sfd);
            if (result == true)
            {
                try
                {
                    //Excel Sheet anlegen und Spaltenheader erstellen
                    var workbook = new HSSFWorkbook();
                    var sheet = workbook.CreateSheet("Anwendungsübersicht");
                    
                    //Spaltenheader
                    var row = sheet.CreateRow(0);
                    List<string> HeaderList = new List<string>() { "Applikation Id", "IT Anwendung System", "IT Betriebsart", "Server", "Virtuelle Maschine", "Typ", "Wichtiges Anwendungssystem", Setting.SZ_1_Name, Setting.SZ_2_Name, Setting.SZ_3_Name, Setting.SZ_4_Name, Setting.SZ_5_Name , Setting.SZ_6_Name, "Datum", "Benutzer", "Aktiv" };
                    for (int i = 0; i < HeaderList.Count; i++)
                    {
                        var cell = row.CreateCell(i);
                        cell.SetCellValue(HeaderList[i].ToString());
                    }                    

                    //Zellen befüllen
                    for (int i = 0; i < apps.Count; i++)
                    {
                        var rowIndex = i + 1;
                        row = sheet.CreateRow(rowIndex);
                        var cell = row.CreateCell(0);
                        cell.SetCellValue(apps[i].Applikation_Id);
                        cell = row.CreateCell(1);
                        cell.SetCellValue(apps[i].IT_Anwendung_System);
                        cell = row.CreateCell(2);
                        cell.SetCellValue(apps[i].IT_Betriebsart);
                        cell = row.CreateCell(3);
                        cell.SetCellValue(apps[i].Rechenzentrum);
                        cell = row.CreateCell(4);
                        cell.SetCellValue(apps[i].Server);
                        cell = row.CreateCell(5);
                        cell.SetCellValue(apps[i].Virtuelle_Maschine);
                        cell = row.CreateCell(6);
                        cell.SetCellValue(apps[i].Typ);
                        cell = row.CreateCell(7);
                        cell.SetCellValue((apps[i].Wichtiges_Anwendungssystem == "x") ? "Ja" : "Nein");
                        cell = row.CreateCell(8);
                        cell.SetCellValue(apps[i].SZ_1);
                        cell = row.CreateCell(9);
                        cell.SetCellValue(apps[i].SZ_2);
                        cell = row.CreateCell(10);
                        cell.SetCellValue(apps[i].SZ_3);
                        cell = row.CreateCell(11);
                        cell.SetCellValue(apps[i].SZ_4);
                        cell = row.CreateCell(12);
                        cell.SetCellValue(apps[i].SZ_5);
                        cell = row.CreateCell(13);
                        cell.SetCellValue(apps[i].SZ_6);
                        cell = row.CreateCell(14);
                        cell.SetCellValue(apps[i].Datum.ToString());
                        cell = row.CreateCell(15);
                        cell.SetCellValue(apps[i].Benutzer);
                        cell = row.CreateCell(16);
                        cell.SetCellValue((apps[i].Aktiv == 1)?"Ja":"Nein");
                    }

                    if (id != 0)
                    {
                        ObservableCollection<ISB_BIA_Delta_Analyse> historyList = _myData.GetProcessApplicationHistoryForApplication(id);

                        var sheet1 = workbook.CreateSheet("Prozess-Applikation Historie");
                        var headerRow1 = sheet1.CreateRow(0);

                        //Spaltenheader
                        HeaderList = new List<string>() { "Prozess Id", "Prozess", "Sub-Prozess", "Applikation Id", "Applikation", "Relation (Verknüpfung)", "Aktuell", "Datum" };
                        for (int i = 0; i < HeaderList.Count; i++)
                        {
                            var cell1 = headerRow1.CreateCell(i);
                            cell1.SetCellValue(HeaderList[i].ToString());
                        }

                        for (int i = 0; i < historyList.Count; i++)
                        {
                            var rowIndex = i + 1;
                            var row1 = sheet1.CreateRow(rowIndex);
                            var cell1 = row1.CreateCell(0);
                            cell1.SetCellValue(historyList[i].Prozess_Id);
                            cell1 = row1.CreateCell(1);
                            cell1.SetCellValue(historyList[i].Prozess);
                            cell1 = row1.CreateCell(2);
                            cell1.SetCellValue(historyList[i].Sub_Prozess);
                            cell1 = row1.CreateCell(3);
                            cell1.SetCellValue(historyList[i].Applikation_Id);
                            cell1 = row1.CreateCell(4);
                            cell1.SetCellValue(historyList[i].Applikation);
                            cell1 = row1.CreateCell(5);
                            cell1.SetCellValue((historyList[i].SZ_1 == 1) ? "Ja" : "Nein");
                            cell1 = row1.CreateCell(6);
                            cell1.SetCellValue((historyList[i].SZ_2 == 1) ? "Ja" : "Nein");
                            cell1 = row1.CreateCell(7);
                            cell1.SetCellValue(historyList[i].Datum.ToString());
                        }
                    }

                    // MemoryStream variable um in das Sheet zu schreiben
                    var stream = new MemoryStream();
                    workbook.Write(stream);

                    string FilePath = sfd.FileName;

                    //wenn Datei existiert löschen (Frage im FileDialog)
                    if (File.Exists(FilePath))
                    {
                        File.Delete(FilePath);
                    }
                    FileStream file = new FileStream(FilePath, FileMode.CreateNew, FileAccess.Write);
                    stream.WriteTo(file);
                    file.Close();
                    stream.Close();
                    if (_myDia.ShowQuestion("Export erfolgreich\nMöchten Sie die Datei nun öffen?", "Export öffnen?"))
                    {
                        if(File.Exists(sfd.FileName))
                            Process.Start(sfd.FileName);
                        else 
                            _myDia.ShowError("Datei existiert nicht mehr");
                    }
                    //Erfolg
                    return true;
                }
                catch (Exception ex)
                {
                    _myDia.ShowError("Fehler beim Speichern der Datei.\n", ex);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool AllActiveProcessesExport()
        {
            ObservableCollection<ISB_BIA_Prozesse> ps;
            ps = _myData.GetActiveProcesses();
            if (ps != null)
                return ExportProcesses(ps);
            else
                return false;
        }

        public bool ExportProcesses(ObservableCollection<ISB_BIA_Prozesse> procs, int id=0)
        {
            SaveFileDialog sfd = new SaveFileDialog()
            {
                OverwritePrompt = true,
                FileName = (id==0)? "BIA_Prozessübersicht_Export_" + DateTime.Now.ToShortDateString() + ".xls" : "BIA_Prozess_" + id + "_Export.xls",
                Filter = "Excel-Datei xls|*.xls",
                Title = (id == 0) ? "Export der Prozessübersicht": "Export eines Prozesses",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)

            };
            bool? result = _myDia.Save(sfd);

            if (result == true)
            {
                try
                {
                    // Declare HSSFWorkbook object for create sheet  
                    var workbook = new HSSFWorkbook();
                    var sheet = workbook.CreateSheet("Prozessübersicht");

                    #region Process
                    //Spaltenheader
                    var row = sheet.CreateRow(0);
                    List<string> HeaderList = new List<string>() { "Prozess Id", "Prozess", "Sub-Prozess", "OE", "Prozessverantwortlicher", "Kritischer Prozess", "Kritikalität", "Reifegrad", "Regulatorisch", "Reputatorisch", "Finanziell", Setting.SZ_1_Name, Setting.SZ_2_Name, Setting.SZ_3_Name, Setting.SZ_4_Name, Setting.SZ_5_Name, Setting.SZ_6_Name, "Datum", "Benutzer", "Aktiv" };
                    for (int i = 0; i < HeaderList.Count; i++)
                    {
                        var cell = row.CreateCell(i);
                        cell.SetCellValue(HeaderList[i].ToString());
                    }

                    for (int i = 0; i < procs.Count; i++)
                    {
                        var rowIndex = i + 1;
                        row = sheet.CreateRow(rowIndex);
                        var cell = row.CreateCell(0);
                        cell = row.CreateCell(0);
                        cell.SetCellValue(procs[i].Prozess_Id);
                        cell = row.CreateCell(1);
                        cell.SetCellValue(procs[i].Prozess);
                        cell = row.CreateCell(2);
                        cell.SetCellValue(procs[i].Sub_Prozess);
                        cell = row.CreateCell(3);
                        cell.SetCellValue(procs[i].OE_Filter);
                        cell = row.CreateCell(4);
                        cell.SetCellValue(procs[i].Prozessverantwortlicher);
                        cell = row.CreateCell(5);
                        cell.SetCellValue((procs[i].Kritischer_Prozess == "x") ? "Ja" : "Nein");
                        cell = row.CreateCell(6);
                        cell.SetCellValue(procs[i].Kritikalität_des_Prozesses);
                        cell = row.CreateCell(7);
                        cell.SetCellValue(procs[i].Reifegrad_des_Prozesses);
                        cell = row.CreateCell(8);
                        cell.SetCellValue(procs[i].Regulatorisch);
                        cell = row.CreateCell(9);
                        cell.SetCellValue(procs[i].Reputatorisch);
                        cell = row.CreateCell(10);
                        cell.SetCellValue(procs[i].Finanziell);
                        cell = row.CreateCell(11);
                        cell.SetCellValue(procs[i].SZ_1);
                        cell = row.CreateCell(12);
                        cell.SetCellValue(procs[i].SZ_2);
                        cell = row.CreateCell(13);
                        cell.SetCellValue(procs[i].SZ_3);
                        cell = row.CreateCell(14);
                        cell.SetCellValue(procs[i].SZ_4);
                        cell = row.CreateCell(15);
                        cell.SetCellValue(procs[i].SZ_5);
                        cell = row.CreateCell(16);
                        cell.SetCellValue(procs[i].SZ_6);
                        cell = row.CreateCell(17);
                        cell.SetCellValue(procs[i].Vorgelagerte_Prozesse);
                        cell = row.CreateCell(18);
                        cell.SetCellValue(procs[i].Nachgelagerte_Prozesse);
                        cell = row.CreateCell(19);
                        cell.SetCellValue(procs[i].Servicezeit_Helpdesk);
                        cell = row.CreateCell(20);
                        cell.SetCellValue(procs[i].RPO_Datenverlustzeit_Recovery_Point_Objective + " Stunden");
                        cell = row.CreateCell(21);
                        cell.SetCellValue(procs[i].RTO_Wiederanlaufzeit_Recovery_Time_Objective + " Tage");
                        cell = row.CreateCell(22);
                        cell.SetCellValue(procs[i].RTO_Wiederanlaufzeit_Recovery_Time_Objective_Notfall + " Tage");
                        cell = row.CreateCell(23);
                        cell.SetCellValue((procs[i].Relevantes_IS_1 == "") ? "" : "(" + procs[i].Relevantes_IS_1 + ") " + ISList.Where(x => x.Name == procs[i].Relevantes_IS_1).Select(y => y.Segment).FirstOrDefault().ToString());
                        cell = row.CreateCell(24);
                        cell.SetCellValue((procs[i].Relevantes_IS_2 == "") ? "" : "(" + procs[i].Relevantes_IS_2 + ") " + ISList.Where(x => x.Name == procs[i].Relevantes_IS_2).Select(y => y.Segment).FirstOrDefault().ToString());
                        cell = row.CreateCell(25);
                        cell.SetCellValue((procs[i].Relevantes_IS_3 == "") ? "" : "(" + procs[i].Relevantes_IS_3 + ") " + ISList.Where(x => x.Name == procs[i].Relevantes_IS_3).Select(y => y.Segment).FirstOrDefault().ToString());
                        cell = row.CreateCell(26);
                        cell.SetCellValue((procs[i].Relevantes_IS_4 == "") ? "" : "(" + procs[i].Relevantes_IS_4 + ") " + ISList.Where(x => x.Name == procs[i].Relevantes_IS_4).Select(y => y.Segment).FirstOrDefault().ToString());
                        cell = row.CreateCell(27);
                        cell.SetCellValue((procs[i].Relevantes_IS_5 == "") ? "" : "(" + procs[i].Relevantes_IS_5 + ") " + ISList.Where(x => x.Name == procs[i].Relevantes_IS_5).Select(y => y.Segment).FirstOrDefault().ToString());
                        cell = row.CreateCell(28);
                        cell.SetCellValue(procs[i].Datum.ToString());
                        cell = row.CreateCell(29);
                        cell.SetCellValue(procs[i].Benutzer);
                        cell = row.CreateCell(30);
                        cell.SetCellValue((procs[i].Aktiv == 1) ? "Ja" : "Nein");
                    }
                    #endregion

                    if (id!=0)
                    {
                        ObservableCollection<ISB_BIA_Delta_Analyse> historyList = _myData.GetProcessApplicationHistoryForProcess(id);

                        sheet = workbook.CreateSheet("Prozess-Applikation Historie");
                        row = sheet.CreateRow(0);

                        //Spaltenheader
                        HeaderList = new List<string>() { "Prozess Id", "Prozess", "Sub-Prozess", "Applikation Id", "Applikation", "Relation (Verknüpfung)", "Aktuell", "Datum" };
                        for (int i = 0; i < HeaderList.Count; i++)
                        {
                            var cell = row.CreateCell(i);
                            cell.SetCellValue(HeaderList[i].ToString());
                        }

                        for (int i = 0; i < historyList.Count; i++)
                        {
                            var rowIndex = i + 1;
                            row = sheet.CreateRow(rowIndex);
                            var cell = row.CreateCell(0);
                            cell = row.CreateCell(0);
                            cell.SetCellValue(historyList[i].Prozess_Id);
                            cell = row.CreateCell(1);
                            cell.SetCellValue(historyList[i].Prozess);
                            cell = row.CreateCell(2);
                            cell.SetCellValue(historyList[i].Sub_Prozess);
                            cell = row.CreateCell(3);
                            cell.SetCellValue(historyList[i].Applikation_Id);
                            cell = row.CreateCell(4);
                            cell.SetCellValue(historyList[i].Applikation);
                            cell = row.CreateCell(5);
                            cell.SetCellValue((historyList[i].SZ_1 == 1) ? "Ja" : "Nein");
                            cell = row.CreateCell(6);
                            cell.SetCellValue((historyList[i].SZ_2 == 1) ? "Ja" : "Nein");
                            cell = row.CreateCell(7);
                            cell.SetCellValue(historyList[i].Datum.ToString());
                        }
                    }

                    // Declare one MemoryStream variable for write file in stream  
                    var stream = new MemoryStream();
                    workbook.Write(stream);

                    string FilePath = sfd.FileName;

                    //Write to file using file stream  
                    if (File.Exists(FilePath))
                    {
                        File.Delete(FilePath);
                    }
                    FileStream file = new FileStream(FilePath, FileMode.CreateNew, FileAccess.Write);
                    stream.WriteTo(file);
                    file.Close();
                    stream.Close();
                    if (_myDia.ShowQuestion("Export erfolgreich\nMöchten Sie die Datei nun öffen?", "Export öffnen?"))
                    {
                        if (File.Exists(sfd.FileName))
                            Process.Start(sfd.FileName);
                        else
                            _myDia.ShowError("Datei existiert nicht mehr");
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    _myDia.ShowError("Fehler beim Speichern.",ex);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool ExportSegmentAndAttributeHistory()
        {
            Tuple<List<ISB_BIA_Informationssegmente>, List<ISB_BIA_Informationssegmente>, List<ISB_BIA_Informationssegmente_Attribute>, List<ISB_BIA_Informationssegmente_Attribute>> tuple =_myData.GetISAndISAttForExport();
            if (tuple != null)
            {
                SaveFileDialog sfd = new SaveFileDialog()
                {
                    OverwritePrompt = true,
                    FileName = "BIA_Segmente_und_Attribute_Übersicht_" + DateTime.Now.ToShortDateString()+".xls",
                    Filter = "Excel-Datei xls|*.xls",
                    Title = "Export der Segmente und Attribute",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                };
                bool? result = _myDia.Save(sfd);

                if (result == true)
                {
                    try
                    {
                        // Declare HSSFWorkbook object for create sheet  
                        var workbook = new HSSFWorkbook();
                        var sheet = workbook.CreateSheet("Segmentübersicht");

                        #region Segment
                        //Spaltenheader
                        var row = sheet.CreateRow(0);
                        List<string> HeaderList = new List<string>() { "Segment Id", "Bezeichnung", "Segment", "Beschreibung", "Mögliche Inhalte", "Attribut 1", "Attribut 2", "Attribut 3", "Attribut 4", "Attribut 5", "Attribut 6", "Attribut 7", "Attribut 8", "Attribut 9", "Attribut 10", "Datum", "Aktuell", "Benutzer" };
                        for (int i = 0; i < HeaderList.Count; i++)
                        {
                            var cell = row.CreateCell(i);
                            cell.SetCellValue(HeaderList[i].ToString());
                        }

                        var segmentList = tuple.Item1;
                        var currentSegmentList = tuple.Item2;
                        for (int i = 0; i < segmentList.Count; i++)
                        {
                            var rowIndex = i + 1;
                            row = sheet.CreateRow(rowIndex);
                            var cell = row.CreateCell(0);
                            cell = row.CreateCell(0);
                            cell.SetCellValue(segmentList[i].Informationssegment_Id);
                            cell = row.CreateCell(1);
                            cell.SetCellValue(segmentList[i].Name);
                            cell = row.CreateCell(2);
                            cell.SetCellValue(segmentList[i].Segment);
                            cell = row.CreateCell(3);
                            cell.SetCellValue(segmentList[i].Beschreibung);
                            cell = row.CreateCell(4);
                            cell.SetCellValue(segmentList[i].Mögliche_Segmentinhalte);
                            cell = row.CreateCell(5);
                            cell.SetCellValue(segmentList[i].Attribut_1 == "P" ? "Ja" : "Nein");
                            cell = row.CreateCell(6);
                            cell.SetCellValue(segmentList[i].Attribut_2 == "P" ? "Ja" : "Nein");
                            cell = row.CreateCell(7);
                            cell.SetCellValue(segmentList[i].Attribut_3 == "P" ? "Ja" : "Nein");
                            cell = row.CreateCell(8);
                            cell.SetCellValue(segmentList[i].Attribut_4 == "P" ? "Ja" : "Nein");
                            cell = row.CreateCell(9);
                            cell.SetCellValue(segmentList[i].Attribut_5 == "P" ? "Ja" : "Nein");
                            cell = row.CreateCell(10);
                            cell.SetCellValue(segmentList[i].Attribut_6 == "P" ? "Ja" : "Nein");
                            cell = row.CreateCell(11);
                            cell.SetCellValue(segmentList[i].Attribut_7 == "P" ? "Ja" : "Nein");
                            cell = row.CreateCell(12);
                            cell.SetCellValue(segmentList[i].Attribut_8 == "P" ? "Ja" : "Nein");
                            cell = row.CreateCell(13);
                            cell.SetCellValue(segmentList[i].Attribut_9 == "P" ? "Ja" : "Nein");
                            cell = row.CreateCell(14);
                            cell.SetCellValue(segmentList[i].Attribut_10 == "P" ? "Ja" : "Nein");
                            cell = row.CreateCell(15);
                            cell.SetCellValue(segmentList[i].Datum.ToString());
                            cell = row.CreateCell(16);
                            cell.SetCellValue((currentSegmentList.Count(x=>x.Datum == segmentList[i].Datum && x.Informationssegment_Id == segmentList[i].Informationssegment_Id) == 1)?"Ja":"Nein");
                            cell = row.CreateCell(17);
                            cell.SetCellValue(segmentList[i].Benutzer);                        
                        }
                        #endregion

                        #region Attribute
                        sheet = workbook.CreateSheet("Attributübersicht");
                        //Spaltenheader
                        row = sheet.CreateRow(0);
                        HeaderList = new List<string>() { "Attribut Id", "Attributname", "Info", Setting.SZ_1_Name, Setting.SZ_2_Name, Setting.SZ_3_Name, Setting.SZ_4_Name, Setting.SZ_5_Name, Setting.SZ_6_Name, "Datum", "Aktuell", "Benutzer" };
                        for (int i = 0; i < HeaderList.Count; i++)
                        {
                            var cell = row.CreateCell(i);
                            cell.SetCellValue(HeaderList[i].ToString());
                        }

                        var attributeList = tuple.Item3;
                        var currentAttributeList = tuple.Item4;
                        for (int i = 0; i < attributeList.Count; i++)
                        {
                            var rowIndex = i + 1;
                            row = sheet.CreateRow(rowIndex);
                            var cell = row.CreateCell(0);
                            cell = row.CreateCell(0);
                            cell.SetCellValue(attributeList[i].Attribut_Id);
                            cell = row.CreateCell(1);
                            cell.SetCellValue(attributeList[i].Name);
                            cell = row.CreateCell(2);
                            cell.SetCellValue(attributeList[i].Info);
                            cell = row.CreateCell(3);
                            cell.SetCellValue(attributeList[i].SZ_1);
                            cell = row.CreateCell(4);
                            cell.SetCellValue(attributeList[i].SZ_2);
                            cell = row.CreateCell(5);
                            cell.SetCellValue(attributeList[i].SZ_3);
                            cell = row.CreateCell(6);
                            cell.SetCellValue(attributeList[i].SZ_4);
                            cell = row.CreateCell(7);
                            cell.SetCellValue(attributeList[i].SZ_5);
                            cell = row.CreateCell(8);
                            cell.SetCellValue(attributeList[i].SZ_6);
                            cell = row.CreateCell(9);
                            cell.SetCellValue(attributeList[i].Datum.ToString());
                            cell = row.CreateCell(10);
                            cell.SetCellValue((currentAttributeList.Count(x => x.Datum == attributeList[i].Datum && x.Attribut_Id == attributeList[i].Attribut_Id) == 1) ? "Ja" : "Nein");
                            cell = row.CreateCell(11);
                            cell.SetCellValue(segmentList[i].Benutzer);
                        }
                        #endregion

                        // Declare one MemoryStream variable for write file in stream  
                        var stream = new MemoryStream();
                        workbook.Write(stream);

                        string FilePath = sfd.FileName;

                        //Write to file using file stream  
                        if (File.Exists(FilePath))
                        {
                            File.Delete(FilePath);
                        }
                        FileStream file = new FileStream(FilePath, FileMode.CreateNew, FileAccess.Write);
                        stream.WriteTo(file);
                        file.Close();
                        stream.Close();
                        if (_myDia.ShowQuestion("Export erfolgreich\nMöchten Sie die Datei nun öffen?", "Export öffnen?"))
                        {
                            if (File.Exists(sfd.FileName))
                                Process.Start(sfd.FileName);
                            else
                                _myDia.ShowError("Datei existiert nicht mehr");
                        }
                        return true;
                    }
                    catch (Exception ex)
                    {
                        _myDia.ShowError("Fehler beim Speichern.", ex);
                        return false;
                    }
                }
            }
            return false;
        }

        public bool ExportDeltaAnalysis(ObservableCollection<ISB_BIA_Delta_Analyse> DeltaList)
        {
            SaveFileDialog sfd = new SaveFileDialog()
            {
                OverwritePrompt = true,
                FileName = "BIA_Delta-Analyse_Export_" + DeltaList.First().Datum.ToShortDateString() + ".xls",
                Filter = "Excel-Datei xls|*.xls",
                Title = "Speichern der Prozessübersicht",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            bool? result = _myDia.Save(sfd);

            if (result == true)
            {
                try
                {
                    var workbook = new HSSFWorkbook();
                    var sheet = workbook.CreateSheet("Delta Analyse");
                    var headerRow = sheet.CreateRow(0);

                    //Spaltenheader
                    List<string> HeaderList = new List<string>() { "Prozess Id", "Prozess", "Sub-Prozess","Datum Prozess","Applikation Id","Applikation","Datum Applikation",Setting.SZ_1_Name, Setting.SZ_2_Name, Setting.SZ_3_Name, Setting.SZ_4_Name, Setting.SZ_5_Name, Setting.SZ_6_Name, "Datum" };

                    for (int i = 0; i < HeaderList.Count; i++)
                    {
                        var cell = headerRow.CreateCell(i);
                        cell.SetCellValue(HeaderList[i]);
                    }

                    for (int i = 0; i < DeltaList.Count; i++)
                    {
                        var rowIndex = i + 1;
                        var row = sheet.CreateRow(rowIndex);
                        var cell = row.CreateCell(0);
                        cell.SetCellValue(DeltaList[i].Prozess_Id);
                        cell = row.CreateCell(1);
                        cell.SetCellValue(DeltaList[i].Prozess);
                        cell = row.CreateCell(2);
                        cell.SetCellValue(DeltaList[i].Sub_Prozess);
                        cell = row.CreateCell(3);
                        cell.SetCellValue(DeltaList[i].Datum_Prozess.ToString());
                        cell = row.CreateCell(4);
                        cell.SetCellValue(DeltaList[i].Applikation_Id);
                        cell = row.CreateCell(5);
                        cell.SetCellValue(DeltaList[i].Applikation);
                        cell = row.CreateCell(6);
                        cell.SetCellValue(DeltaList[i].Datum_Applikation.ToString());
                        cell = row.CreateCell(7);
                        cell.SetCellValue(DeltaList[i].SZ_1);
                        cell = row.CreateCell(8);
                        cell.SetCellValue(DeltaList[i].SZ_2);
                        cell = row.CreateCell(9);
                        cell.SetCellValue(DeltaList[i].SZ_3);
                        cell = row.CreateCell(10);
                        cell.SetCellValue(DeltaList[i].SZ_4);
                        cell = row.CreateCell(11);
                        cell.SetCellValue(DeltaList[i].SZ_5);
                        cell = row.CreateCell(12);
                        cell.SetCellValue(DeltaList[i].SZ_6);
                        cell = row.CreateCell(13);
                        cell.SetCellValue(DeltaList[i].Datum.ToShortDateString());
                    }

                    // Declare one MemoryStream variable for write file in stream  
                    var stream = new MemoryStream();
                    workbook.Write(stream);

                    string FilePath = sfd.FileName;

                    //Write to file using file stream  
                    if (File.Exists(FilePath))
                    {
                        File.Delete(FilePath);
                    }
                    FileStream file = new FileStream(FilePath, FileMode.CreateNew, FileAccess.Write);
                    stream.WriteTo(file);
                    file.Close();
                    stream.Close();
                    if (_myDia.ShowQuestion("Export erfolgreich\nMöchten Sie die Datei nun öffen?", "Export öffnen?"))
                    {
                        if (File.Exists(sfd.FileName))
                            Process.Start(sfd.FileName);
                        else
                            _myDia.ShowError("Datei existiert nicht mehr");
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    _myDia.ShowError("Fehler beim Speichern.", ex);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool ExportLog(ObservableCollection<ISB_BIA_Log> Log)
        {
            SaveFileDialog sfd = new SaveFileDialog()
            {
                OverwritePrompt = true,
                FileName = "BIA_Log_Export_" + DateTime.Now.ToShortDateString() + ".xls",
                Filter = "Excel-Datei xls|*.xls",
                Title = "Speichern des Anwendungs-Logs",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            bool? result = _myDia.Save(sfd);

            if (result == true)
            {
                try
                {
                    var workbook = new HSSFWorkbook();
                    var sheet = workbook.CreateSheet("Log");
                    var headerRow = sheet.CreateRow(0);

                    //Spaltenheader
                    List<string> HeaderList = Log.FirstOrDefault().GetType().GetProperties().Select(p => p.Name).ToList();

                    for (int i = 0; i < HeaderList.Count; i++)
                    {
                        var cell = headerRow.CreateCell(i);
                        cell.SetCellValue(HeaderList[i].ToString());
                    }

                    for (int i = 0; i < Log.Count; i++)
                    {
                        var rowIndex = i + 1;
                        var row = sheet.CreateRow(rowIndex);
                        var cell = row.CreateCell(0);
                        cell.SetCellValue(Log[i].Id);
                        cell = row.CreateCell(1);
                        cell.SetCellValue(Log[i].Action);
                        cell = row.CreateCell(2);
                        cell.SetCellValue(Log[i].Tabelle);
                        cell = row.CreateCell(3);
                        cell.SetCellValue(Log[i].Details);
                        cell = row.CreateCell(4);
                        cell.SetCellValue(Log[i].Id_1);
                        cell = row.CreateCell(5);
                        cell.SetCellValue(Log[i].Id_2);
                        cell = row.CreateCell(6);
                        cell.SetCellValue(Log[i].Datum.ToString());
                        cell = row.CreateCell(7);
                        cell.SetCellValue(Log[i].Benutzer);
                    }

                    var stream = new MemoryStream();
                    workbook.Write(stream);

                    string FilePath = sfd.FileName;

                    //Überschreiben falls vorhanden 
                    if (File.Exists(FilePath))
                    {
                        File.Delete(FilePath);
                    }
                    FileStream file = new FileStream(FilePath, FileMode.CreateNew, FileAccess.Write);
                    stream.WriteTo(file);
                    file.Close();
                    stream.Close();
                    if (_myDia.ShowQuestion("Export erfolgreich\nMöchten Sie die Datei nun öffen?", "Export öffnen?"))
                    {
                        if (File.Exists(sfd.FileName))
                            Process.Start(sfd.FileName);
                        else
                            _myDia.ShowError("Datei existiert nicht mehr");
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    _myDia.ShowError("Export fehlgeschlagen.\n", ex);
                    return false;
                }
            }
            return false;
        }

        public bool ExportSettings(List<ISB_BIA_Settings> Log)
        {
            SaveFileDialog sfd = new SaveFileDialog()
            {
                OverwritePrompt = true,
                FileName = "BIA_Settings_History_Export_"+DateTime.Now.ToShortDateString()+".xls",
                Filter = "Excel-Datei xls|*.xls",
                Title = "Speichern des Anwendungs-Einstellungshistorie",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            bool? result = _myDia.Save(sfd);

            if (result == true)
            {
                try
                {
                    var workbook = new HSSFWorkbook();
                    var sheet = workbook.CreateSheet("Einstellungshistorie");
                    var headerRow = sheet.CreateRow(0);

                    //Spaltenheader
                    List<string> HeaderList = Log.FirstOrDefault().GetType().GetProperties().Select(p => p.Name).ToList();

                    for (int i = 0; i < HeaderList.Count; i++)
                    {
                        var cell = headerRow.CreateCell(i);
                        cell.SetCellValue(HeaderList[i].ToString());
                    }

                    for (int i = 0; i < Log.Count; i++)
                    {
                        var rowIndex = i + 1;
                        var row = sheet.CreateRow(rowIndex);
                        var cell = row.CreateCell(0);
                        cell.SetCellValue(Log[i].Id);
                        cell = row.CreateCell(1);
                        cell.SetCellValue(Log[i].SZ_1_Name);
                        cell = row.CreateCell(2);
                        cell.SetCellValue(Log[i].SZ_2_Name);
                        cell = row.CreateCell(3);
                        cell.SetCellValue(Log[i].SZ_3_Name);
                        cell = row.CreateCell(4);
                        cell.SetCellValue(Log[i].SZ_4_Name);
                        cell = row.CreateCell(5);
                        cell.SetCellValue(Log[i].SZ_5_Name);
                        cell = row.CreateCell(6);
                        cell.SetCellValue(Log[i].SZ_6_Name);
                        cell = row.CreateCell(7);
                        cell.SetCellValue(Log[i].Neue_Schutzziele_aktiviert);
                        cell = row.CreateCell(8);
                        cell.SetCellValue(Log[i].BIA_abgeschlossen);
                        cell = row.CreateCell(9);
                        cell.SetCellValue(Log[i].SBA_abgeschlossen);
                        cell = row.CreateCell(10);
                        cell.SetCellValue(Log[i].Delta_abgeschlossen);
                        cell = row.CreateCell(11);
                        cell.SetCellValue(Log[i].Attribut9_aktiviert);
                        cell = row.CreateCell(12);
                        cell.SetCellValue(Log[i].Attribut10_aktiviert);
                        cell = row.CreateCell(13);
                        cell.SetCellValue(Log[i].Multi_Save);
                        cell = row.CreateCell(14);
                        cell.SetCellValue(Log[i].Datum.ToString());
                        cell = row.CreateCell(15);
                        cell.SetCellValue(Log[i].Benutzer);
                    }

                    var stream = new MemoryStream();
                    workbook.Write(stream);

                    string FilePath = sfd.FileName;

                    //Überschreiben falls vorhanden 
                    if (File.Exists(FilePath))
                    {
                        File.Delete(FilePath);
                    }
                    FileStream file = new FileStream(FilePath, FileMode.CreateNew, FileAccess.Write);
                    stream.WriteTo(file);
                    file.Close();
                    stream.Close();
                    if (_myDia.ShowQuestion("Export erfolgreich\nMöchten Sie die Datei nun öffen?", "Export öffnen?"))
                    {
                        if (File.Exists(sfd.FileName))
                            Process.Start(sfd.FileName);
                        else
                            _myDia.ShowError("Datei existiert nicht mehr");
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    _myDia.ShowError("Export fehlgeschlagen.\n", ex);
                    return false;
                }
            }
            return false;
        }
    }
}
