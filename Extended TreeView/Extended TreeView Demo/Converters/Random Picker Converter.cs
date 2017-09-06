using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace Converters
{
    [ValueConversion(typeof(int), typeof(object))]
    public class RandomPickerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int IndexValue = (int)value;

            CompositeCollection ArrayParameter = parameter as CompositeCollection;
            if (ArrayParameter != null && ArrayParameter.Count > 0)
            {
                Random r = new Random(IndexValue);
                int Index = r.Next(ArrayParameter.Count);
                object Result = ArrayParameter[Index];

                Image AsImage;
                if ((AsImage = Result as Image) != null)
                    return AsImage.Source;
                else
                    return Result;
            }
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
