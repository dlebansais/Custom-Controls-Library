namespace TestEnumComboBox
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    [ValueConversion(typeof(object), typeof(object))]
    internal class LazyConverter : IValueConverter
    {
#pragma warning disable CS8603 // Possible null reference return
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value is TestEnum)
                return value;
            else
                return TestEnum.X;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
#pragma warning restore CS8603 // Possible null reference return
    }
}
