using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using GalaSoft.MvvmLight;


namespace ISB_BIA_IMPORT1.View
{
    /// <summary>
    /// Interaktionslogik für Process_ViewMVVM.xaml
    /// </summary>
    public partial class Process_View : UserControl
    {
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
        public string Krit_Ntf
        {
            get => "Dieser Prozess wird als kritisch eingestuft!\n"
                   + "\nEin Prozess wird als 'Kritischer Prozess' eingestuft wenn mindestens eine der folgenden Bedingungen zutrifft:"
                   + "\n- mindestens 3 der Schutzziele werden als mindestens 'Hoch' eingestuft oder mindestens 2 als 'Sehr Hoch'"
                   + "\n- Das Recover Time Objective (RTO) wurde auf 1 Tag festgelegt"
                   + "\n- Die Kritikalität des Prozesses wurde auf 'Sehr Hoch' festgelegt\n"
                   + "\nKritische Prozesse müssen identifiziert und dokumentiert werden!"
                   + "\nErläuterung zur Dokumentation: "
                   + "\nDie Dokumentation muss folgende Anforderungen erfüllen:"
                   + "\n- kurze Beschreibung des Prozesses(Prozessbezeichnung, was bewirkt der Prozess)"
                   + "\n- Begründung, warum der Prozess ein zentraler/kritischer Prozess bzw.ein Prozess mit hohem Schadenspotential ist"
                   + "\n- Benennung des Prozessverantwortlichen"
                   + "\n- Maximal tolerierbare Ausfallzeit des Prozesses"
                   + "\n- Auswirkung auf Folgeprozesse"
                   + "\n- ggf.Workaround bei Prozessstörung";
        }

        private void Label_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            Label a=(Label) e.TargetObject;
            
            if (a.Content.ToString() == "Ja") MessageBox.Show(Krit_Ntf, "Warnung", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void TargetDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid d = (DataGrid) sender;
            if (TargetDataGrid.SelectedItem != null)
            {
                d.Focus();
                d.ScrollIntoView(d.SelectedItem);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TargetDataGrid.Focus();
            if(TargetDataGrid.SelectedItem != null)
                TargetDataGrid.ScrollIntoView(TargetDataGrid.SelectedItem);
        }
    }
}
