using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using ISB_BIA_IMPORT1.LinqDataContext;
using Microsoft.Win32;
using NPOI.HSSF.UserModel;

namespace ISB_BIA_IMPORT1.Services
{
    class MyExportService : IMyExportService
    {
        IMyDialogService myDia;
        IMyDataService myData;
        IMySharedResourceService myShared;

        ISB_BIA_Settings Setting { get; set; }
        ObservableCollection<ISB_BIA_Informationssegmente> ISList { get; set; }

        //!Es werden nur aktive Prozesse exportiert
        public MyExportService(IMyDialogService myDia, IMyDataService myDataService, IMySharedResourceService mySharedResourceService)
        {
            this.myDia = myDia;
            this.myData = myDataService;
            this.myShared = mySharedResourceService;
            if (!myShared.ConstructionMode) Setting = myData.GetSettings();
            if (!myShared.ConstructionMode) ISList = myData.GetEnabledSegments();
        }

        //Excel Datei mit Liste aller Applikationen exportieren (für Anwendungsansicht)
        public bool AllApplicationsExport()
        {
            //Alle Applikationen abrufen
            ObservableCollection<ISB_BIA_Applikationen> queryApplications;
            queryApplications = myData.GetApplications();
            if (queryApplications != null)
                return ExportApplications(queryApplications);
            else
                return false;
        }

        //Excel Datei mit Liste aller aktiven Applikationen exportieren(für SBA Ansicht)
        public bool AllActiveApplicationsExport()
        {
            //Alle Applikationen abrufen
            ObservableCollection<ISB_BIA_Applikationen> queryApplications;
            queryApplications = myData.GetActiveApplications();
            if (queryApplications != null)
                return ExportApplications(queryApplications, "SBA_");
            else
                return false;
        }

        public bool ExportApplications(ObservableCollection<ISB_BIA_Applikationen> apps, string title="", string id="")
        {
            //SaveFileDialog öffnen mit default Ordner "Dokumente"
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                OverwritePrompt = true,
                FileName = title+"Anwendungsübersicht"+id+ "_Export_" + DateTime.Now.ToShortDateString() + ".xls",
                Filter = "Excel-Datei xls|*.xls",
                Title = "Speichern der Anwendungsübersicht",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            bool? result = myDia.Save(saveFileDialog);
            if (result == true)
            {
                try
                {
                    //Excel Sheet anlegen und Spaltenheader erstellen
                    var workbook = new HSSFWorkbook();
                    var sheet = workbook.CreateSheet("Anwendungsübersicht");
                    var headerRow = sheet.CreateRow(0);
                    var row = sheet.CreateRow(0);

                    var cell = row.CreateCell(0);
                    cell.SetCellValue("IT Anwendung System");
                    cell = row.CreateCell(1);
                    cell.SetCellValue("IT Betriebsart");
                    cell = row.CreateCell(2);
                    cell.SetCellValue("Rechenzentrum");
                    cell = row.CreateCell(3);
                    cell.SetCellValue("Server");
                    cell = row.CreateCell(4);
                    cell.SetCellValue("Virtuelle Maschine");
                    cell = row.CreateCell(5);
                    cell.SetCellValue("Typ");
                    cell = row.CreateCell(6);
                    cell.SetCellValue("Wichtiges Anwendungssystem");
                    cell = row.CreateCell(7);
                    cell.SetCellValue(Setting.SZ_1_Name);
                    cell = row.CreateCell(8);
                    cell.SetCellValue(Setting.SZ_2_Name);
                    cell = row.CreateCell(9);
                    cell.SetCellValue(Setting.SZ_3_Name);
                    cell = row.CreateCell(10);
                    cell.SetCellValue(Setting.SZ_4_Name);
                    cell = row.CreateCell(11);
                    cell.SetCellValue(Setting.SZ_5_Name);
                    cell = row.CreateCell(12);
                    cell.SetCellValue(Setting.SZ_6_Name);
                    cell = row.CreateCell(13);
                    cell.SetCellValue("Datum");
                    cell = row.CreateCell(14);
                    cell.SetCellValue("Benutzer");
                    cell = row.CreateCell(15);
                    cell.SetCellValue("Aktiv");

                    //Zellen befüllen
                    for (int i = 0; i < apps.Count; i++)
                    {
                        var rowIndex = i + 1;
                        row = sheet.CreateRow(rowIndex);
                        cell = row.CreateCell(0);
                        cell.SetCellValue(apps[i].IT_Anwendung_System);
                        cell = row.CreateCell(1);
                        cell.SetCellValue(apps[i].IT_Betriebsart);
                        cell = row.CreateCell(2);
                        cell.SetCellValue(apps[i].Rechenzentrum);
                        cell = row.CreateCell(3);
                        cell.SetCellValue(apps[i].Server);
                        cell = row.CreateCell(4);
                        cell.SetCellValue(apps[i].Virtuelle_Maschine);
                        cell = row.CreateCell(5);
                        cell.SetCellValue(apps[i].Typ);
                        cell = row.CreateCell(6);
                        cell.SetCellValue((apps[i].Wichtiges_Anwendungssystem == "x") ? "Ja" : "Nein");
                        cell = row.CreateCell(7);
                        cell.SetCellValue(apps[i].SZ_1);
                        cell = row.CreateCell(8);
                        cell.SetCellValue(apps[i].SZ_2);
                        cell = row.CreateCell(9);
                        cell.SetCellValue(apps[i].SZ_3);
                        cell = row.CreateCell(10);
                        cell.SetCellValue(apps[i].SZ_4);
                        cell = row.CreateCell(11);
                        cell.SetCellValue(apps[i].SZ_5);
                        cell = row.CreateCell(12);
                        cell.SetCellValue(apps[i].SZ_6);
                        cell = row.CreateCell(13);
                        cell.SetCellValue(apps[i].Datum.ToString());
                        cell = row.CreateCell(14);
                        cell.SetCellValue(apps[i].Benutzer);
                        cell = row.CreateCell(15);
                        cell.SetCellValue(apps[i].Aktiv);
                    }

                    // MemoryStream variable um in das Sheet zu schreiben
                    var stream = new MemoryStream();
                    workbook.Write(stream);

                    string FilePath = saveFileDialog.FileName;

                    //wenn Datei existiert löschen (Frage im FileDialog)
                    if (File.Exists(FilePath))
                    {
                        File.Delete(FilePath);
                    }
                    FileStream file = new FileStream(FilePath, FileMode.CreateNew, FileAccess.Write);
                    stream.WriteTo(file);
                    file.Close();
                    stream.Close();
                    //Erfolg
                    return true;
                }
                catch (Exception ex)
                {
                    myDia.ShowError("Fehler beim Speichern der Datei.\n", ex);
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
            ps = myData.GetActiveProcesses();
            if (ps != null)
                return ExportProcesses(ps);
            else
                return false;
        }

        public bool ExportProcesses(ObservableCollection<ISB_BIA_Prozesse> procs, int id=0)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog()
            {
                OverwritePrompt = true,
                FileName = (id==0)? "BIA_Prozessübersicht_Export_" + DateTime.Now.ToShortDateString() + ".xls" : "BIA_Prozess_" + id + "_Export.xls",
                Filter = "Excel-Datei xls|*.xls",
                Title = (id == 0) ? "Export der Prozessübersicht": "Export eines Prozesses",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)

            };
            bool? result = myDia.Save(saveFileDialog1);

            if (result == true)
            {
                try
                {
                    // Declare HSSFWorkbook object for create sheet  
                    var workbook = new HSSFWorkbook();
                    var sheet = workbook.CreateSheet("Prozessübersicht");
                    var headerRow = sheet.CreateRow(0);
                    var row = sheet.CreateRow(0);

                    #region Process
                    var cell = row.CreateCell(0);
                    cell.SetCellValue("Prozess");
                    cell = row.CreateCell(1);
                    cell.SetCellValue("Sub-Prozess");
                    cell = row.CreateCell(2);
                    cell.SetCellValue("OE");
                    cell = row.CreateCell(3);
                    cell.SetCellValue("Prozessverantwortlicher");
                    cell = row.CreateCell(4);
                    cell.SetCellValue("Kritischer Prozess");
                    cell = row.CreateCell(5);
                    cell.SetCellValue("Kritikalität des Prozesses");
                    cell = row.CreateCell(6);
                    cell.SetCellValue("Reifegrad des Prozesses");
                    cell = row.CreateCell(7);
                    cell.SetCellValue("Regulatorisch");
                    cell = row.CreateCell(8);
                    cell.SetCellValue("Reputatorisch");
                    cell = row.CreateCell(9);
                    cell.SetCellValue("Finanziell");
                    cell = row.CreateCell(10);
                    cell.SetCellValue(Setting.SZ_1_Name);
                    cell = row.CreateCell(11);
                    cell.SetCellValue(Setting.SZ_2_Name);
                    cell = row.CreateCell(12);
                    cell.SetCellValue(Setting.SZ_3_Name);
                    cell = row.CreateCell(13);
                    cell.SetCellValue(Setting.SZ_4_Name);
                    cell = row.CreateCell(14);
                    cell.SetCellValue(Setting.SZ_5_Name);
                    cell = row.CreateCell(15);
                    cell.SetCellValue(Setting.SZ_6_Name);
                    cell = row.CreateCell(16);
                    cell.SetCellValue("Vorgelagerte Prozesse");
                    cell = row.CreateCell(17);
                    cell.SetCellValue("Nachgelagerte Prozesse");
                    cell = row.CreateCell(18);
                    cell.SetCellValue("Servicezeiten");
                    cell = row.CreateCell(19);
                    cell.SetCellValue("RPO Datenverlustzeit");
                    cell = row.CreateCell(20);
                    cell.SetCellValue("RTO_Wiederanlaufzeit");
                    cell = row.CreateCell(21);
                    cell.SetCellValue("RTO_Wiederanlaufzeit");
                    cell = row.CreateCell(22);
                    cell.SetCellValue("Relevantes Informationssegment 1");
                    cell = row.CreateCell(23);
                    cell.SetCellValue("Relevantes Informationssegment 2");
                    cell = row.CreateCell(24);
                    cell.SetCellValue("Relevantes Informationssegment 3");
                    cell = row.CreateCell(25);
                    cell.SetCellValue("Relevantes Informationssegment 4");
                    cell = row.CreateCell(26);
                    cell.SetCellValue("Relevantes Informationssegment 5");
                    cell = row.CreateCell(27);
                    cell.SetCellValue("Datum");
                    cell = row.CreateCell(28);
                    cell.SetCellValue("Benutzer");
                    cell = row.CreateCell(29);
                    cell.SetCellValue("Aktiv");

                    for (int i = 0; i < procs.Count; i++)
                    {
                        var rowIndex = i + 1;
                        row = sheet.CreateRow(rowIndex);
                        cell = row.CreateCell(0);
                        cell.SetCellValue(procs[i].Prozess);
                        cell = row.CreateCell(1);
                        cell.SetCellValue(procs[i].Sub_Prozess);
                        cell = row.CreateCell(2);
                        cell.SetCellValue(procs[i].OE_Filter);
                        cell = row.CreateCell(3);
                        cell.SetCellValue(procs[i].Prozessverantwortlicher);
                        cell = row.CreateCell(4);
                        cell.SetCellValue((procs[i].Kritischer_Prozess == "x") ? "Ja" : "Nein");
                        cell = row.CreateCell(5);
                        cell.SetCellValue(procs[i].Kritikalität_des_Prozesses);
                        cell = row.CreateCell(6);
                        cell.SetCellValue(procs[i].Reifegrad_des_Prozesses);
                        cell = row.CreateCell(7);
                        cell.SetCellValue(procs[i].Regulatorisch);
                        cell = row.CreateCell(8);
                        cell.SetCellValue(procs[i].Reputatorisch);
                        cell = row.CreateCell(9);
                        cell.SetCellValue(procs[i].Finanziell);
                        cell = row.CreateCell(10);
                        cell.SetCellValue(procs[i].SZ_1);
                        cell = row.CreateCell(11);
                        cell.SetCellValue(procs[i].SZ_2);
                        cell = row.CreateCell(12);
                        cell.SetCellValue(procs[i].SZ_3);
                        cell = row.CreateCell(13);
                        cell.SetCellValue(procs[i].SZ_4);
                        cell = row.CreateCell(14);
                        cell.SetCellValue(procs[i].SZ_5);
                        cell = row.CreateCell(15);
                        cell.SetCellValue(procs[i].SZ_6);
                        cell = row.CreateCell(16);
                        cell.SetCellValue(procs[i].Vorgelagerte_Prozesse);
                        cell = row.CreateCell(17);
                        cell.SetCellValue(procs[i].Nachgelagerte_Prozesse);
                        cell = row.CreateCell(18);
                        cell.SetCellValue(procs[i].Servicezeit_Helpdesk);
                        cell = row.CreateCell(19);
                        cell.SetCellValue(procs[i].RPO_Datenverlustzeit_Recovery_Point_Objective + " Stunden");
                        cell = row.CreateCell(20);
                        cell.SetCellValue(procs[i].RTO_Wiederanlaufzeit_Recovery_Time_Objective + " Tage");
                        cell = row.CreateCell(21);
                        cell.SetCellValue(procs[i].RTO_Wiederanlaufzeit_Recovery_Time_Objective_Notfall + " Tage");
                        cell = row.CreateCell(22);
                        cell.SetCellValue((procs[i].Relevantes_IS_1 == "") ? "" : "(" + procs[i].Relevantes_IS_1 + ") " + ISList.Where(x => x.Name == procs[i].Relevantes_IS_1).Select(y => y.Segment).FirstOrDefault().ToString());
                        cell = row.CreateCell(23);
                        cell.SetCellValue((procs[i].Relevantes_IS_2 == "") ? "" : "(" + procs[i].Relevantes_IS_2 + ") " + ISList.Where(x => x.Name == procs[i].Relevantes_IS_2).Select(y => y.Segment).FirstOrDefault().ToString());
                        cell = row.CreateCell(24);
                        cell.SetCellValue((procs[i].Relevantes_IS_3 == "") ? "" : "(" + procs[i].Relevantes_IS_3 + ") " + ISList.Where(x => x.Name == procs[i].Relevantes_IS_3).Select(y => y.Segment).FirstOrDefault().ToString());
                        cell = row.CreateCell(25);
                        cell.SetCellValue((procs[i].Relevantes_IS_4 == "") ? "" : "(" + procs[i].Relevantes_IS_4 + ") " + ISList.Where(x => x.Name == procs[i].Relevantes_IS_4).Select(y => y.Segment).FirstOrDefault().ToString());
                        cell = row.CreateCell(26);
                        cell.SetCellValue((procs[i].Relevantes_IS_5 == "") ? "" : "(" + procs[i].Relevantes_IS_5 + ") " + ISList.Where(x => x.Name == procs[i].Relevantes_IS_5).Select(y => y.Segment).FirstOrDefault().ToString());
                        cell = row.CreateCell(27);
                        cell.SetCellValue(procs[i].Datum.ToString());
                        cell = row.CreateCell(28);
                        cell.SetCellValue(procs[i].Benutzer);
                        cell = row.CreateCell(29);
                        cell.SetCellValue(procs[i].Aktiv);
                    }
                    #endregion

                    if (id!=0)
                    {
                        ObservableCollection<ISB_BIA_Delta_Analyse> historyList = myData.GetProcessApplicationHistoryForProcess(id);

                        var sheet1 = workbook.CreateSheet("Prozess-Applikation Historie");
                        var headerRow1 = sheet1.CreateRow(0);

                        //Spaltenheader
                        List<string> HeaderList = new List<string>() { "Prozess Id", "Prozess", "Sub-Prozess", "Applikation Id", "Applikation", "Relation (1 = verknüpft)", "Datum" };
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
                            cell1.SetCellValue(historyList[i].SZ_1);
                            cell1 = row1.CreateCell(6);
                            cell1.SetCellValue(historyList[i].Datum.ToString());
                        }
                    }

                    // Declare one MemoryStream variable for write file in stream  
                    var stream = new MemoryStream();
                    workbook.Write(stream);

                    string FilePath = saveFileDialog1.FileName;

                    //Write to file using file stream  
                    if (File.Exists(FilePath))
                    {
                        File.Delete(FilePath);
                    }
                    FileStream file = new FileStream(FilePath, FileMode.CreateNew, FileAccess.Write);
                    stream.WriteTo(file);
                    file.Close();
                    stream.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    myDia.ShowError("Fehler beim Speichern.",ex);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        //Öffnet einen FIleDialog zum Speichern einer Excel-Datei, welche die Delta-Analyse als Export enthält (nur kritische EInträge)
        public bool ExportDeltaAnalysis(ObservableCollection<ISB_BIA_Delta_Analyse> DeltaList)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog()
            {
                OverwritePrompt = true,
                FileName = "BIA_Delta-Analyse_Export_" + DateTime.Now.ToShortDateString() + ".xls",
                Filter = "Excel-Datei xls|*.xls",
                Title = "Speichern der Prozessübersicht",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            bool? result = myDia.Save(saveFileDialog1);

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
                        cell.SetCellValue(HeaderList[i].ToString());
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
                        cell.SetCellValue(DeltaList[i].Datum.ToString());
                    }

                    // Declare one MemoryStream variable for write file in stream  
                    var stream = new MemoryStream();
                    workbook.Write(stream);

                    string FilePath = saveFileDialog1.FileName;

                    //Write to file using file stream  
                    if (File.Exists(FilePath))
                    {
                        File.Delete(FilePath);
                    }
                    FileStream file = new FileStream(FilePath, FileMode.CreateNew, FileAccess.Write);
                    stream.WriteTo(file);
                    file.Close();
                    stream.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    myDia.ShowError("Fehler beim Speichern.", ex);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        //Excel Datei mit Log exportieren
        public bool ExportLog(ObservableCollection<ISB_BIA_Log> Log)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog()
            {
                OverwritePrompt = true,
                FileName = "BIA_Log_Export_" + DateTime.Now.ToShortDateString() + ".xls",
                Filter = "Excel-Datei xls|*.xls",
                Title = "Speichern des Anwendungs-Logs",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            bool? result = myDia.Save(saveFileDialog1);

            if (result == true)
            {
                try
                {
                    var workbook = new HSSFWorkbook();
                    var sheet = workbook.CreateSheet("Log");
                    var headerRow = sheet.CreateRow(0);

                    //Spaltenheader
                    List<string> HeaderList = Log.First().GetType().GetProperties().Select(p => p.Name).ToList();

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

                    string FilePath = saveFileDialog1.FileName;

                    //Überschreiben falls vorhanden 
                    if (File.Exists(FilePath))
                    {
                        File.Delete(FilePath);
                    }
                    FileStream file = new FileStream(FilePath, FileMode.CreateNew, FileAccess.Write);
                    stream.WriteTo(file);
                    file.Close();
                    stream.Close();
                    myDia.ShowMessage("Export erfolgreich.");
                    return true;
                }
                catch (Exception ex)
                {
                    myDia.ShowError("Export fehlgeschlagen.\n", ex);
                    return false;
                }
            }
            return false;
        }

        //Excel Datei mit Einstellungshistorie exportieren
        public bool ExportSettings(List<ISB_BIA_Settings> Log)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog()
            {
                OverwritePrompt = true,
                FileName = "BIA_Settings_History_Export_"+DateTime.Now.ToShortDateString()+".xls",
                Filter = "Excel-Datei xls|*.xls",
                Title = "Speichern des Anwendungs-EInstellungshistorie",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            bool? result = myDia.Save(saveFileDialog1);

            if (result == true)
            {
                try
                {
                    var workbook = new HSSFWorkbook();
                    var sheet = workbook.CreateSheet("Einstellungshistorie");
                    var headerRow = sheet.CreateRow(0);

                    //Spaltenheader
                    List<string> HeaderList = Log.First().GetType().GetProperties().Select(p => p.Name).ToList();

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

                    string FilePath = saveFileDialog1.FileName;

                    //Überschreiben falls vorhanden 
                    if (File.Exists(FilePath))
                    {
                        File.Delete(FilePath);
                    }
                    FileStream file = new FileStream(FilePath, FileMode.CreateNew, FileAccess.Write);
                    stream.WriteTo(file);
                    file.Close();
                    stream.Close();
                    myDia.ShowMessage("Export erfolgreich.");
                    return true;
                }
                catch (Exception ex)
                {
                    myDia.ShowError("Export fehlgeschlagen.\n", ex);
                    return false;
                }
            }
            return false;
        }
    }
}
