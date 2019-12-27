using System;
using System.Globalization;
using System.Windows.Data;

namespace Converters
{
    [ValueConversion(typeof(double), typeof(object))]
    public class DoubleToObjectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double AsDoubleValue)
                if (parameter is CompositeCollection CollectionOfItems && CollectionOfItems.Count > 1)
                    return AsDoubleValue == 0 ? CollectionOfItems[0] : CollectionOfItems[1];
                else
                    throw new ArgumentOutOfRangeException(nameof(parameter));
            else
                throw new ArgumentOutOfRangeException(nameof(value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
