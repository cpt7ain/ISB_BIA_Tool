using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ISB_BIA_IMPORT1.Converter
{
    class InfoVisConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converter, der 6 Farben abruft und Sichtbarkeit zurückgibt falls mindestens eine "LightSalmon"
        /// (Für Sichtbarkeit des Legenden-Labels wenn Unterdeckung der Schutzziele zutrifft)
        /// </summary>
        /// <param name="value"> Integer Wert 1 </param>
        /// <param name="targetType"></param>
        /// <param name="parameter"> Integer Wert 2 </param>
        /// <param name="culture"></param>
        /// <returns> Rot, wenn value[0] > value[1], Transparent wenn nicht  </returns>
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value[0] != null && value[1] != null && value[2] != null && value[3] != null && value[4] != null && value[5] != null)
            {
                try
                {
                    Brush b1 = (Brush)(value[0]);
                    Brush b2 = (Brush)(value[1]);
                    Brush b3 = (Brush)(value[2]);
                    Brush b4 = (Brush)(value[3]);
                    Brush b5 = (Brush)(value[4]);
                    Brush b6 = (Brush)(value[5]);

                    if (b1 == Brushes.LightSalmon ||
                        b2 == Brushes.LightSalmon ||
                        b3 == Brushes.LightSalmon ||
                        b4 == Brushes.LightSalmon ||
                        b5 == Brushes.LightSalmon ||
                        b6 == Brushes.LightSalmon)
                    {
                        return Visibility.Visible;
                    }
                }
                catch
                {
                    return Visibility.Collapsed;
                }
            }
            return Visibility.Collapsed;
        }

        /// <summary>
        /// Nicht benötigt da nur für OneWay-Gebrauch
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
