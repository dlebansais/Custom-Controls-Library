using System;
using System.Globalization;
using System.Windows.Data;

namespace Converters
{
    [ValueConversion(typeof(bool), typeof(object))]
    public class BooleanToObjectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool BooleanValue;

            if (value is bool)
                BooleanValue = (bool)value;
            else if (value is bool?)
                BooleanValue = ((bool?)value).HasValue ? ((bool?)value).Value : false;
            else
                BooleanValue = false;

            CompositeCollection CollectionOfItems = parameter as CompositeCollection;

            return BooleanValue ? CollectionOfItems[1] : CollectionOfItems[0];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
