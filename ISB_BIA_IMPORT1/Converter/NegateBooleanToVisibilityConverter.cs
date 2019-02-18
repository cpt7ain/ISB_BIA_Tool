using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ISB_BIA_IMPORT1.Converter
{
    /// <summary>
    /// Universeller Converter, der den False nach Sichtbar und True nach Unsichtbar konvertiert
    /// </summary>
    public class NegateBooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Wandelt true in Sichtbarkeit und false in Unsichtbarkeit
        /// </summary>
        /// <param name="value"> Wahrheitswert </param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns> Sichtbarkeit </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                return b? Visibility.Hidden:Visibility.Visible;
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
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
