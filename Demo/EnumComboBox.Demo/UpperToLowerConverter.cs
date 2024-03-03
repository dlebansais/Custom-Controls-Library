namespace TestEnumComboBox
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    [ValueConversion(typeof(string), typeof(string))]
    internal class UpperToLowerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((string)value).ToLower();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((string)value).ToUpper();
        }
    }
}
