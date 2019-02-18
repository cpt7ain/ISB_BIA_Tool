using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ISB_BIA_IMPORT1.Converter
{
    /// <summary>
    /// Converter, der die Zellen im Datagrid je nach Wert Grün(=P) oder Rot(=O) färbt
    /// </summary>
    public class CellColorConverter : IValueConverter
    {
        /// <summary>
        /// Konvertiert Strings "P" und "O" in Farbwerte grün und rot 
        /// </summary>
        /// <param name="value"> zu konvertierender String </param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns> Farbe </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value is string)
            {
                if (value.ToString() == String.Empty)
                    return null;
                if (value.ToString() == "O") return Brushes.LightSalmon;
                else if (value.ToString() == "P") return Brushes.LightGreen;
            }
            return null;
        }

        /// <summary>
        /// Nicht benötigt da nur für OneWay-Gebrauch
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
