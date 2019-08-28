using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ISB_BIA_IMPORT1.Converter
{
    /// <summary>
    /// Converter, der Grün zurückgibt, falls aktuelles Jahr und nicht Erstanlage, ansonsten weiss
    /// </summary>
    public class DateFirstToBrushConverter : IMultiValueConverter
    {
        /// <summary>
        /// Konvertiert zu grün, falls aktuelles Jahr und nicht Erstanlage, ansonsten weiss
        /// </summary>
        /// <param name="values"> Array von Werten (Datum, String) </param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns> Farbe </returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is DateTime d && values[1] is string)
            {
                if (d.Year == DateTime.Now.Year && values[1].ToString() == "Nein") return Brushes.LightGreen;
                return Brushes.White;
            }
            return DependencyProperty.UnsetValue;
        }

        /// <summary>
        /// Nicht benötigt da nur für OneWay-Gebrauch
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
