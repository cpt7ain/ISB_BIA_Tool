using System;
using System.Windows.Data;

namespace ISB_BIA_IMPORT1.Converter
{
    /// <summary>
    /// Converter, der die Zahl 1 in "Ja" wandelt und andere Werte in "Nein"
    /// Verwendet für Datagridzellen, in denen 1 für "Ja" steht und 0 für "Nein"
    /// </summary>
    public class NumToAlphBinaryConverter : IValueConverter
    {
        /// <summary>
        /// Wandelt 1 in "Ja" und ander Werte in "Nein"
        /// </summary>
        /// <param name="value"> Integer Wert </param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns> "Ja" oder "Nein" </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int)
            {
                if ((int)value == 1)
                    return "Ja";
                else
                    return "Nein";
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
