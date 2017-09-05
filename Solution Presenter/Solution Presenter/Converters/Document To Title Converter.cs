using System;
using System.Globalization;
using System.Windows.Data;
using Verification;

namespace Converters
{
    public class DocumentToTitleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Assert.ValidateReference(values);

            if (values.Length > 1 && (values[0] is string) && (values[1] is bool))
            {
                string FriendlyName = (string)values[0];
                bool IsDirty = (bool)values[1];

                return FriendlyName + (IsDirty ? "*" : "");
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
