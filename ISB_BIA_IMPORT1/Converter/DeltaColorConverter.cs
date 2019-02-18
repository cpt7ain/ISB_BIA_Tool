﻿using System;
using System.Windows;
using System.Windows.Data;

namespace ISB_BIA_IMPORT1.Converter
{
    /// <summary>
    /// Converter, der bei negativen Werten Rot und bei positiven Werten Grün zurückgibt
    /// </summary>
    public class DeltaColorConverter : IValueConverter
    {
        /// <summary>
        /// Konvertiert negative Werte zu Rot, positive zu grün
        /// </summary>
        /// <param name="value"> Integer Wert </param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns> Farbe </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value is int i)
            {
                return (i < 0) ? "LightSalmon" : "LightGreen";
            }
            return DependencyProperty.UnsetValue;
        }

        /// <summary>
        /// Nicht benötigt da nur für OneWay-Gebrauch
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
