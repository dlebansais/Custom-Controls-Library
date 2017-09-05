using System;
using System.Globalization;
using System.Windows.Data;

namespace Converters
{
    [ValueConversion(typeof(object), typeof(int))]
    public class EnumToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                Array Values = value.GetType().GetEnumValues();
                for (int i = 0; i < Values.Length; i++)
                {
                    object EnumValue = Values.GetValue(i);
                    if (value.Equals(EnumValue))
                        return i;
                }
            }

            return -1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != null && targetType.IsEnum)
            {
                int IndexValue = (int)value;
                Array Values = targetType.GetEnumValues();
                return Values.GetValue(IndexValue);
            }
            else
                return null;
        }
    }
}
