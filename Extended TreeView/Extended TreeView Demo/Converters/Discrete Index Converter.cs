using System;
using System.Globalization;
using System.Windows.Data;

namespace Converters
{
    [ValueConversion(typeof(int), typeof(bool))]
    public class DiscreteIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int IndexValue = (int)value;
            int ExpectedIndex = int.Parse(parameter as string, CultureInfo.InvariantCulture);
            return IndexValue == ExpectedIndex;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
