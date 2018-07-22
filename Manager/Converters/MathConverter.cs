using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Manager.Converters
{
    public class MathConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty MultiplyProperty = DependencyProperty.Register(
                                                        "Multiply", typeof(object), typeof(MathConverter), new PropertyMetadata(1));

        public object Multiply
        {
            get { return (object) GetValue(MultiplyProperty); }
            set { SetValue(MultiplyProperty, value); }
        }
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (double.TryParse(value?.ToString(), out var val)
                && double.TryParse(Multiply?.ToString(), out var param))
            {
                return val * param;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}