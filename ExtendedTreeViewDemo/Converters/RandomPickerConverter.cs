namespace Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Controls;
    using System.Windows.Data;

    [ValueConversion(typeof(int), typeof(object))]
    public class RandomPickerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int IndexValue = (int)value;

            if (parameter is CompositeCollection ArrayParameter && ArrayParameter.Count > 0)
            {
                Random r = new Random(IndexValue);
                int Index = r.Next(ArrayParameter.Count);
                object Result = ArrayParameter[Index];

                if (Result is Image AsImage)
                    return AsImage.Source;
                else
                    return Result;
            }
            else
                throw new ArgumentOutOfRangeException(nameof(parameter));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
