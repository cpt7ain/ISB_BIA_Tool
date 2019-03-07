using ISB_BIA_IMPORT1.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ISB_BIA_IMPORT1.LINQ2SQL;

namespace ISB_BIA_IMPORT1.View
{
    /// <summary>
    /// Interaktionslogik für ProcessView_ViewMVVM.xaml
    /// </summary>
    public partial class ProcessView_View : UserControl
    {
        /// <summary>
        /// Vew des <see cref="ProcessView_ViewModel"/>
        /// </summary>
        public ProcessView_View()
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
        private IEnumerable<ISB_BIA_Prozesse> searchResultList;
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
                if (ProcessDataGrid.ItemsSource != null)
                {
                    IEnumerable<ISB_BIA_Prozesse> all = ProcessDataGrid.ItemsSource.Cast<ISB_BIA_Prozesse>();
                    searchResultList = all.Where(x => x.Prozess.IndexOf(SearchBox.Text, StringComparison.CurrentCultureIgnoreCase) >= 0 || x.Sub_Prozess.IndexOf(SearchBox.Text, StringComparison.CurrentCultureIgnoreCase) >= 0 || x.OE_Filter.IndexOf(SearchBox.Text, StringComparison.CurrentCultureIgnoreCase) >= 0 || x.Benutzer.IndexOf(SearchBox.Text, StringComparison.CurrentCultureIgnoreCase) >= 0 || x.Datum.ToString().IndexOf(SearchBox.Text, StringComparison.CurrentCultureIgnoreCase) >= 0);

                    ISB_BIA_Prozesse n = searchResultList.FirstOrDefault();
                    ProcessDataGrid.SelectedItem = n;
                    if (ProcessDataGrid.SelectedItem != null)
                        ProcessDataGrid.ScrollIntoView(ProcessDataGrid.SelectedItem);
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
                    int lastResultId = searchResultList.FirstOrDefault().Prozess_Id;
                    searchResultList = searchResultList.Where(b => b.Prozess_Id != lastResultId);
                    ISB_BIA_Prozesse n = null;
                    if (searchResultList.Any())
                    {
                        n = searchResultList.FirstOrDefault();
                        ProcessDataGrid.SelectedItem = n;
                        if (ProcessDataGrid.SelectedItem != null)
                            ProcessDataGrid.ScrollIntoView(ProcessDataGrid.SelectedItem);
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
