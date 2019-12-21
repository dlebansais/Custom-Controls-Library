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
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

            double DoubleValue = (double)value;
            CompositeCollection CollectionOfItems = parameter as CompositeCollection;

            return DoubleValue == 0 ? CollectionOfItems[0] : CollectionOfItems[1];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
