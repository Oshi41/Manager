using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Manager.Converters
{
    public class NotNullChooseConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.FirstOrDefault(x => x != null);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}