using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ISB_BIA_IMPORT1.Converter
{
    /// <summary>
    /// Converter, der Grün zurückgibt, falls Datumsjahr dem aktuellen entspricht und weiß, falls nicht
    /// </summary>
    public class DateToBrushConverter : IValueConverter
    {
        /// <summary>
        /// Konvertiert Datum zu grün, falls aktuelles Jahr und weiss, falls nicht
        /// </summary>
        /// <param name="value"> Datumsstring </param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns> Farbe </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (DateTime.TryParse(value.ToString(), out DateTime date))
            {
                if (date.Year == DateTime.Now.Year)
                {
                    return Brushes.LightGreen;
                }
                else
                {
                    return Brushes.White;
                }
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
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
