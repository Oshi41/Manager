using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Manager.Converters
{
    /// <summary>
    /// Конвертируем числовые значения в цвет.
    /// Используется для конвертации номер месяца => цвет  
    /// </summary>
    public class IntColorConverter : IValueConverter
    {
        private static readonly List<SolidColorBrush> _colors = new List<SolidColorBrush>
        {
            Brushes.Black,
            Brushes.DarkGoldenrod,
            Brushes.LightSeaGreen,
            Brushes.DarkBlue,

        };
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (double.TryParse(value?.ToString(), out var val))
            {
                return _colors[(int) Math.Abs((val % 4))];
            }

            return _colors[0];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}