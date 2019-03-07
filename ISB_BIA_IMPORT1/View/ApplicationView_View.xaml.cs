using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ISB_BIA_IMPORT1.LINQ2SQL;
using ISB_BIA_IMPORT1.ViewModel;

namespace ISB_BIA_IMPORT1.View
{
    /// <summary>
    /// Interaktionslogik für ApplicationView_ViewMVVM.xaml
    /// </summary>
    public partial class ApplicationView_View : UserControl
    {
        /// <summary>
        /// View des <see cref="ApplicationView_ViewModel"/>
        /// </summary>
        public ApplicationView_View()
        {
            InitializeComponent();
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
            if (!searchOn)
            {
                searchOn = true;
                if (ApplicationDataGrid.ItemsSource != null)
                {
                    IEnumerable<ISB_BIA_Applikationen> all = ApplicationDataGrid.ItemsSource.Cast<ISB_BIA_Applikationen>();
                    searchResultList = all.Where(x => x.IT_Anwendung_System.IndexOf(SearchBox.Text, StringComparison.CurrentCultureIgnoreCase) >= 0 || x.IT_Betriebsart.IndexOf(SearchBox.Text, StringComparison.CurrentCultureIgnoreCase) >= 0 || x.Benutzer.IndexOf(SearchBox.Text, StringComparison.CurrentCultureIgnoreCase) >= 0 || x.Datum.ToString().IndexOf(SearchBox.Text, StringComparison.CurrentCultureIgnoreCase) >= 0);

                    ISB_BIA_Applikationen n = searchResultList.FirstOrDefault();
                    ApplicationDataGrid.SelectedItem = n;
                    if (ApplicationDataGrid.SelectedItem != null)
                        ApplicationDataGrid.ScrollIntoView(ApplicationDataGrid.SelectedItem);
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
                        ApplicationDataGrid.SelectedItem = n;
                        if (ApplicationDataGrid.SelectedItem != null)
                            ApplicationDataGrid.ScrollIntoView(ApplicationDataGrid.SelectedItem);
                    }

                }
                else
                {
                    MessageBox.Show("Keine weiteren Ergebnisse gefunden");
                    searchOn = false;
                }
            }
        }
        #endregion
    }
}
