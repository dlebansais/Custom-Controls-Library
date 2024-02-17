namespace Converters;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Data;

/// <summary>
/// Converter from a positive double to the equivalent percentage.
/// </summary>
[ValueConversion(typeof(double), typeof(string))]
[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Instanciated in Xaml")]
internal class DoubleToPercentageConverter : IValueConverter
{
    /// <summary>
    /// Converter from a positive double to the equivalent percentage.
    /// </summary>
    /// <param name="value">The double value to convert.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>
    /// If <paramref name="value"/> is a positive double, or zero, the converter returns <paramref name="value"/> multiplied by 100 and followed by the % sign.
    /// Otherwise, returns null.
    /// </returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double AsDoubleValue)
            if (AsDoubleValue >= 0)
                return (AsDoubleValue * 100).ToString(CultureInfo.InvariantCulture) + "%";

        throw new ArgumentOutOfRangeException(nameof(value));
    }

    /// <summary>
    /// This method is not used and will always return null.
    /// </summary>
    /// <param name="value">The value that is produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>The converted value.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }
}
