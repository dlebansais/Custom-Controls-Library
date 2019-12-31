namespace Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class DocumentToTitleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Length > 1 && (values[0] is string FriendlyName) && (values[1] is bool IsDirty))
                return FriendlyName + (IsDirty ? "*" : string.Empty);
            else
                throw new ArgumentNullException(nameof(values));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return Array.Empty<object>();
        }
    }
}
