namespace Converters;

using System;
using System.Globalization;
using System.Windows.Data;

/// <summary>
/// Converter for an integer value to <see langword="true"/> if it's equal to the provided parameter.
/// </summary>
[ValueConversion(typeof(int), typeof(bool))]
internal class DiscreteIndexConverter : IValueConverter
{
    /// <inheritdoc cref="IValueConverter.Convert(object, Type, object, CultureInfo)" />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        int IndexValue = (int)value;
        int ExpectedIndex = int.Parse((string)parameter, CultureInfo.InvariantCulture);
        return IndexValue == ExpectedIndex;
    }

    /// <inheritdoc cref="IValueConverter.ConvertBack(object, Type, object, CultureInfo)" />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
}
