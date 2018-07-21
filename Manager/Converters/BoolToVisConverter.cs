using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Manager.Converters
{
    public class BoolToVisConverter : IValueConverter
    {
        public bool IsHidden { get; set; }
        public bool IsInverse { get; set; }
        public Visibility? NullValue { get; set; }
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var flag = value as bool?;
            
            if (!flag.HasValue && NullValue.HasValue)
            {
                return NullValue.Value;
            }

            if (IsInverse)
                flag = !flag;

            var falseVisibility = IsHidden ? Visibility.Hidden : Visibility.Collapsed;

            return flag.Value ? Visibility.Visible : falseVisibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}