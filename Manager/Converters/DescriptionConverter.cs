using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace Manager.Converters
{
    public class DescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Binding.DoNothing;

            var fieldInfo = value.GetType().GetField(value.ToString());

            var attributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            return attributes?.Length > 0
                ? attributes[0].Description
                : value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}