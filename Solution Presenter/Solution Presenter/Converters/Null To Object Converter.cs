using System;
using System.Globalization;
using System.Windows.Data;

namespace Converters
{
    [ValueConversion(typeof(object), typeof(object))]
    public class NullToObjectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CompositeCollection CollectionOfItems = parameter as CompositeCollection;

            return value != null ? CollectionOfItems[1] : CollectionOfItems[0];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
