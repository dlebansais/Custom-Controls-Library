namespace Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    [ValueConversion(typeof(object), typeof(object))]
    public class NullToObjectConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is CompositeCollection CollectionOfItems)
                return value != null ? CollectionOfItems[1] : CollectionOfItems[0];
            else
                throw new ArgumentOutOfRangeException(nameof(parameter));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
