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
    /// <summary>
    /// Converts an integer value to <see langword="true"/> if it's equal to the provided parameter.
    /// </summary>
    /// <param name="value">An integer value.</param>
    /// <param name="targetType">The type of the binding target property (ignored).</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter (ignored).</param>
    /// <returns>A converted value.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        int IndexValue = (int)value;
        int ExpectedIndex = int.Parse((string)parameter, CultureInfo.InvariantCulture);
        return IndexValue == ExpectedIndex;
    }

    /// <summary>
    /// This method is not used.
    /// </summary>
    /// <param name="value">The value that is produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>A converted value.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
}
