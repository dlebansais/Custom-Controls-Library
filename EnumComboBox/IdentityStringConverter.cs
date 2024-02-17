namespace Converters;

using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

/// <summary>
/// Identity string converter.
/// </summary>
[ValueConversion(typeof(string), typeof(string))]
internal class IdentityStringConverter : IValueConverter
{
    /// <summary>
    /// Identity string converter.
    /// </summary>
    /// <param name="value">The value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>
    /// The value of <paramref name="value"/>.
    /// </returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        object Result = value;

        Debug.Assert(Result == ConvertBack(value, typeof(object), parameter, culture));

        return Result;
    }

    /// <summary>
    /// Identity string back converter.
    /// </summary>
    /// <param name="value">The value that is produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>
    /// The value of <paramref name="value"/>.
    /// </returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }
}
