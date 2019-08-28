using ISB_BIA_IMPORT1.LINQ2SQL;
using ISB_BIA_IMPORT1.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ISB_BIA_IMPORT1.View
{
    /// <summary>
    /// Interaktionslogik für Process_ViewMVVM.xaml
    /// </summary>
    public partial class Process_View : UserControl
    {
        bool first = true;
        /// <summary>
        /// View des <see cref="Process_ViewModel"/>
        /// </summary>
        public Process_View()
        {
            InitializeComponent();           
        }
        /// <summary>
        /// Benachrichtigung bei Kritischer Einstufung des Prozesses
        /// </summary>
        public string Crit_Ntf
        {
            get => "Dieser Prozess wird als kritisch eingestuft!\n"
                   + "\nEin Prozess wird als 'Kritischer Prozess' eingestuft wenn mindestens eine der folgenden Bedingungen zutrifft:"
                   + "\n- mindestens 3 der Schutzziele werden als mindestens 'Hoch' eingestuft oder mindestens 2 als 'Sehr Hoch'"
                   + "\n- Max. tolerierbare Ausfallzeit (MTA Normalbetrieb) wurde auf 1 Tag festgelegtt"
                   + "\n- Das Ergebnis der Berechnung für die Kritikalität des Prozesses ist 'Sehr hoch'\n"
                   + "\nKritische Prozesse müssen identifiziert und dokumentiert werden!"
                   + "\nErläuterung zur Dokumentation: "
                   + "\nDie Dokumentation muss folgende Anforderungen erfüllen:"
                   + "\n- kurze Beschreibung des Prozesses(Prozessbezeichnung, was bewirkt der Prozess)"
                   + "\n- Begründung, warum der Prozess ein zentraler/kritischer Prozess bzw.ein Prozess mit hohem Schadenspotential ist"
                   + "\n- Benennung des Prozessverantwortlichen"
                   + "\n- Maximal tolerierbare Ausfallzeit des Prozesses"
                   + "\n- Auswirkung auf Folgeprozesse"
                   + "\n- ggf. Workaround bei Prozessstörung"
                   + "\n- eine Risikoanalyse muss durchgeführt werden";
        }
        /// <summary>
        /// Wenn Kritischer-Prozess Label sich ändert => Meldung bringen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CritLabel_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            Label a=(Label) e.TargetObject;           
            if (a.Content.ToString() == "Ja" && !first)
            {
                MessageBox.Show(Crit_Ntf, "Warnung", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            first = false;
        }
        /// <summary>
        /// Durch <see cref="ButtonAddApplication_Click"/> ausgelöste Fokussierung des hinzugefügten Elements => Scroll zu dem Element
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TargetDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid d = (DataGrid) sender;
            if(d != null)
            {
                if (d.SelectedItem != null)
                {
                    d.Focus();
                    d.ScrollIntoView(d.SelectedItem);
                }
            }
        }
        /// <summary>
        /// Click Event bei hinzufügen einer Anwendung zum Prozess => Prozess wird fokussiert
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonAddApplication_Click(object sender, RoutedEventArgs e)
        {
            if (TargetDataGrid != null)
            {
                TargetDataGrid.Focus();
                if (TargetDataGrid.SelectedItem != null)
                    TargetDataGrid.ScrollIntoView(TargetDataGrid.SelectedItem);
            }
        }
        /// <summary>
        /// Click Event bei hinzufügen eines vorgelagerten Prozesses => Prozess wird fokussiert
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonAddvP_Click(object sender, RoutedEventArgs e)
        {
            if(TargetDataGrid1 != null)
            {
                TargetDataGrid1.Focus();
                if (TargetDataGrid1.SelectedItem != null)
                    TargetDataGrid1.ScrollIntoView(TargetDataGrid1.SelectedItem);
            }
        }
        /// <summary>
        /// Click Event bei hinzufügen eines nachgelagerten Prozesses => Prozess wird fokussiert
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonAddnP_Click(object sender, RoutedEventArgs e)
        {
            if (TargetDataGrid2 != null)
            {
                TargetDataGrid2.Focus();
                if (TargetDataGrid2.SelectedItem != null)
                    TargetDataGrid2.ScrollIntoView(TargetDataGrid2.SelectedItem);
            }
        }
        /// <summary>
        /// Click Event bei hinzufügen einer Anwendung zum Prozess => Prozess wird fokussiert
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Crit_Reset_Combo_Click(object sender, RoutedEventArgs e)
        {
            if(Combo_Dmg != null && Combo_Freq != null)
            {
                Combo_Dmg.SelectedIndex = -1;
                Combo_Freq.SelectedIndex = -1;
            }
        }

        #region Suche in DataGrid
        /// <summary>
        /// Wert, ob erstes Suchergebnis oder nicht
        /// </summary>
        private bool searchOn = false;
        /// <summary>
        /// Liste der Suchergebnisse
        /// </summary>
        private IEnumerable<ISB_BIA_Applikationen> searchResultList;
        /// <summary>
        /// Neue Suche (Setzen von searchOn = false)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchBox_KeyUp(object sender, KeyEventArgs e)
        {
            searchOn = false;
        }
        /// <summary>
        /// Erneuern der Suchergebnisliste falls neue Suche (searchOn = false) und Durchlaufen/Springen zu den Ergebnissen falls vorhanden
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if(SourceDataGrid != null)
            {
                if (!searchOn)
                {
                    searchOn = true;
                    if (SourceDataGrid.ItemsSource != null)
                    {
                        IEnumerable<ISB_BIA_Applikationen> all = SourceDataGrid.Items.Cast<ISB_BIA_Applikationen>();
                        searchResultList = all.Where(x => x.IT_Anwendung_System.IndexOf(SearchBox.Text, StringComparison.CurrentCultureIgnoreCase) >= 0);

                        ISB_BIA_Applikationen n = searchResultList.FirstOrDefault();
                        SourceDataGrid.SelectedItem = n;
                        if (SourceDataGrid.SelectedItem != null)
                            SourceDataGrid.ScrollIntoView(SourceDataGrid.SelectedItem);
                        else
                        {
                            MessageBox.Show("Keine Ergebnisse gefunden");
                            searchOn = false;
                        }
                    }
                }
                else
                {
                    if (searchResultList != null && searchResultList.Count() > 1)
                    {
                        int lastResultId = searchResultList.FirstOrDefault().Applikation_Id;
                        searchResultList = searchResultList.Where(b => b.Applikation_Id != lastResultId);
                        ISB_BIA_Applikationen n = null;
                        if (searchResultList.Any())
                        {
                            n = searchResultList.FirstOrDefault();
                            SourceDataGrid.SelectedItem = n;
                            if (SourceDataGrid.SelectedItem != null)
                                SourceDataGrid.ScrollIntoView(SourceDataGrid.SelectedItem);
                        }

                    }
                    else
                    {
                        MessageBox.Show("Keine weiteren Ergebnisse gefunden");
                        searchOn = false;
                    }
                }
            }
        }
        #endregion

        #region Suche in DataGrid1
        /// <summary>
        /// Wert, ob erstes Suchergebnis oder nicht
        /// </summary>
        private bool searchOn1 = false;
        /// <summary>
        /// Liste der Suchergebnisse
        /// </summary>
        private IEnumerable<ISB_BIA_Prozesse> searchResultList1;
        /// <summary>
        /// Neue Suche (Setzen von searchOn = false)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchBox1_KeyUp(object sender, KeyEventArgs e)
        {
            searchOn = false;
        }
        /// <summary>
        /// Erneuern der Suchergebnisliste falls neue Suche (searchOn = false) und Durchlaufen/Springen zu den Ergebnissen falls vorhanden
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchButton1_Click(object sender, RoutedEventArgs e)
        {
            if(SourceDataGrid1 != null)
            {
                if (!searchOn1)
                {
                    searchOn1 = true;
                    if (SourceDataGrid1.ItemsSource != null)
                    {
                        IEnumerable<ISB_BIA_Prozesse> all = SourceDataGrid1.Items.Cast<ISB_BIA_Prozesse>();
                        searchResultList1 = all.Where(x => x.Prozess.IndexOf(SearchBox1.Text, StringComparison.CurrentCultureIgnoreCase) >= 0 || x.Sub_Prozess.IndexOf(SearchBox1.Text, StringComparison.CurrentCultureIgnoreCase) >= 0);

                        ISB_BIA_Prozesse n = searchResultList1.FirstOrDefault();
                        SourceDataGrid1.SelectedItem = n;
                        if (SourceDataGrid1.SelectedItem != null)
                            SourceDataGrid1.ScrollIntoView(SourceDataGrid1.SelectedItem);
                        else
                        {
                            MessageBox.Show("Keine Ergebnisse gefunden");
                            searchOn1 = false;
                        }
                    }
                }
                else
                {
                    if (searchResultList1 != null && searchResultList1.Count() > 1)
                    {
                        int lastResultId = searchResultList1.FirstOrDefault().Prozess_Id;
                        searchResultList1 = searchResultList1.Where(b => b.Prozess_Id != lastResultId);
                        ISB_BIA_Prozesse n = null;
                        if (searchResultList1.Any())
                        {
                            n = searchResultList1.FirstOrDefault();
                            SourceDataGrid1.SelectedItem = n;
                            if (SourceDataGrid1.SelectedItem != null)
                                SourceDataGrid1.ScrollIntoView(SourceDataGrid1.SelectedItem);
                        }

                    }
                    else
                    {
                        MessageBox.Show("Keine weiteren Ergebnisse gefunden");
                        searchOn1 = false;
                    }
                }
            }
        }
        #endregion
    }
}
