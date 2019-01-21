using System;
using System.Globalization;
using System.Windows.Data;

namespace ISB_BIA_IMPORT1.Converter
{
    class MaxValuesToEnabledConverter : IValueConverter
    {
        /// <summary>
        /// Converter, der 2 Werte vergleicht und je nachdem true oder false liefert
        /// Aktiviert/Deaktiviert Controls, wenn der (int)value unter/über dem (int) parameter Wert liegt 
        /// </summary>
        /// <param name="value">  Mindesteinstufung des Schutzziels </param>
        /// <param name="targetType"></param>
        /// <param name="parameter"> Wert des Radio Buttons </param>
        /// <param name="culture"></param>
        /// <returns> Wahrheitswert </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int && int.TryParse(parameter.ToString(), out int para))
            {
                return ((int)value > para) ? false : true;
            }
            return null;
        }

        /// <summary>
        /// Not supported
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
