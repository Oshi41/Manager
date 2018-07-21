using System;
using System.Globalization;
using System.Windows.Data;

namespace Manager.Converters
{
    public class MathConverter : IValueConverter
    {
        public double Add { get; set; } = 0;
        public double Deduct { get; set; } = 0;
        public double Multiply { get; set; } = 1;
        public double Divide { get; set; } = 1;
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (double.TryParse(value?.ToString(), out var val))
            {
                return val * Multiply / Divide + Add - Deduct;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}