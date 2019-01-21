using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ISB_BIA_IMPORT1.Converter
{
    class ISToBrushConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converter, der zwei integer Werte vergleicht.
        /// </summary>
        /// <param name="value"> Integer Wert 1 </param>
        /// <param name="targetType"></param>
        /// <param name="parameter"> Integer Wert 2 </param>
        /// <param name="culture"></param>
        /// <returns> Rot, wenn value[0] > value[1], Transparent wenn nicht  </returns>
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value[0] != null && value[1] != null)
            {
                try
                {
                    int minValue = (int)(value[0]);
                    int paramValue = (int)(value[1]);

                    if (minValue > paramValue)
                    {
                        return Brushes.LightSalmon;
                    }
                    else
                    {
                        return Brushes.Transparent;
                    }
                }
                catch
                {

                }
            }
            return Brushes.Transparent;
        }

        /// <summary>
        /// Not supported
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
