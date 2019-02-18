using System;
using System.Windows;
using System.Windows.Data;

namespace ISB_BIA_IMPORT1.Converter
{
    /// <summary>
    /// Converter, der die Buchstaben "P" und "O" (=DB Format) in "✓" und "✗" umwandelt
    /// </summary>
    public class AlphToNumConverter : IValueConverter
    {
        /// <summary>
        /// Konvertermethode, welche "P" und "O" (=DB Format) in "✓" und "✗" umwandelt
        /// </summary>
        /// <param name="value"> zu konvertierender String </param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns> "✓" oder "✗" </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string s)
            {
                if (s == String.Empty)
                    return null;

                if (s == "O") return "✗";
                else if  (s == "P") return "✓";
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
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
