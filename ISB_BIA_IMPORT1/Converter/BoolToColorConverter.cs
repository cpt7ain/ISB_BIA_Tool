using System;
using System.Windows.Data;
using System.Windows.Media;

namespace ISB_BIA_IMPORT1.Converter
{
    /// <summary>
    /// Converter, der die Zellen im Datagrid je nach Wert Grün (=true) oder Rot(=false) färbt
    /// </summary>
    public class BoolToColorConverter : IValueConverter
    {
        /// <summary>
        /// Konvertertiert Wahrheitswerte in Farben (true-> grün, false->rot)
        /// </summary>
        /// <param name="value"> Wahrheitswert </param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns> Farbe </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value is bool)
            {
                if ((bool)value)
                    return Brushes.LightGreen;
                else
                    return Brushes.LightSalmon;
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
