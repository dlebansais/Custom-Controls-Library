namespace TestEnumComboBox
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    [ValueConversion(typeof(object), typeof(object))]
    internal class LazyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TestEnum)
                return value;
            else
                return TestEnum.X;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TestEnum)
                return value;
            else
                return TestEnum.X;
        }
    }
}
