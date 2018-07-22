using System;
using System.Globalization;
using System.Windows.Data;

namespace Manager.Converters
{
    public class InvertBoolConverter : IValueConverter
    {
        public bool IsNullable { get; set; } = true;
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null && IsNullable)
                return null;
            
            var flag = true.Equals(value);
            return !flag;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }
}