using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ISB_BIA_IMPORT1.Converter
{
    /// <summary>
    /// Converter, der abhängig von 2 boolschen Werten Sichtbarkeit zurückgibt
    /// benutzt um Textblock unsichbar zu machen, wenn TextBox Fokus erhält oder Text enthält (Watermark-Effekt)
    /// </summary>
    public class TextInputToVisibilityConverter : IMultiValueConverter
    {
        /// <summary>
        /// Konvertiert 2 Wahheitswerte nach einem bestimmten Schema in Sichtbarkeit
        /// </summary>
        /// <param name="values"> Array von Wahheitswerten </param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns> Sichtbarkeit </returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is bool && values[1] is bool)
            {
                bool isFilled = !(bool)values[0];
                bool isFocused = (bool)values[1];
                //Wenn Textbox fokussiert oder Textbox text enthält
                if (isFocused || isFilled)
                    return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        /// <summary>
        /// Not supported
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}