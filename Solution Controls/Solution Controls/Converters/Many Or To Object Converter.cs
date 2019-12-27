using System;
using System.Globalization;
using System.Windows.Data;

namespace Converters
{
    public class ManyOrToObjectConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool BooleanValue = false;

            if (values != null)
                foreach (object o in values)
                {
                    if (o is bool)
                    {
                        bool b = (bool)o;
                        if (b)
                        {
                            BooleanValue = true;
                            break;
                        }
                    }

                    else if (o is bool?)
                    {
                        bool? b = (bool?)o;
                        if (b.HasValue && b.Value)
                        {
                            BooleanValue = true;
                            break;
                        }
                    }
                }

            if (parameter is CompositeCollection CollectionOfItems && CollectionOfItems.Count > 1)
                return BooleanValue ? CollectionOfItems[1] : CollectionOfItems[0];
            else
                throw new ArgumentOutOfRangeException(nameof(parameter));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return Array.Empty<object>();
        }
    }
}
