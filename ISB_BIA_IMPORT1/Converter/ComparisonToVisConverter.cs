using System;
using System.Windows;
using System.Windows.Data;

namespace ISB_BIA_IMPORT1.Converter
{
    /// <summary>
    /// Converter, der 2 Werte vergleicht (zb. Enumeration und Integer) die logisch identisch sind (sichtbar wenn wahr, unsichtbar wenn falsch)
    /// </summary>
    public class ComparisonToVisConverter : IValueConverter
    {
        /// <summary>
        /// Vergleicht 2 logisch identische Werte und gibt Sichtbarkeit zurück falls wahr
        /// </summary>
        /// <param name="value"> Wert 1 </param>
        /// <param name="targetType"></param>
        /// <param name="parameter"> Wert 2 </param>
        /// <param name="culture"></param>
        /// <returns> Wahrheitswert </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
                return (value.Equals(parameter)) ? Visibility.Visible : Visibility.Collapsed;
            else
                return DependencyProperty.UnsetValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>  </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value?.Equals(true) == true ? parameter : Binding.DoNothing;
        }
    }
}
