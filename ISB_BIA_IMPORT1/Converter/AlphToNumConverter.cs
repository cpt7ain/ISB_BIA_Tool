using System;
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
            if (value is string)
            {
                if (value.ToString() == String.Empty)
                    return null;

                string val = value.ToString();
                if (val == "O") return "✗";
                else if  (val == "P") return "✓";
            }
            return null;
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
