namespace EnumComboBox.Demo;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Data;
using Contracts;

/// <summary>
/// Converter from upper a case string to a lower case string.
/// </summary>
[ValueConversion(typeof(string), typeof(string))]
[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Instantiated in Xaml")]
internal partial class UpperToLowerConverter : IValueConverter
{
    /// <summary>
    /// Converts from a upper case string to a lower case string.
    /// </summary>
    /// <param name="value">The value to convert. Must be a string.</param>
    /// <param name="targetType">The type of the binding target property (ignored).</param>
    /// <param name="parameter">The converter parameter to use (ignored).</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>A converted value.</returns>
    [Access("public")]
    [RequireNotNull(nameof(value), Type = "object")]
    private static object ConvertVerified(string value, Type targetType, object parameter, CultureInfo culture)
    {
        object Result = value.ToLower(culture);
        return Result;
    }

    /// <summary>
    /// Converts from a lower case string to a upper case string.
    /// </summary>
    /// <param name="value">The value to convert. Must be a string.</param>
    /// <param name="targetType">The type of the binding target property (ignored).</param>
    /// <param name="parameter">The converter parameter to use (ignored).</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>A converted value.</returns>
    [Access("public")]
    [RequireNotNull(nameof(value), Type = "object")]
    private static object ConvertBackVerified(string value, Type targetType, object parameter, CultureInfo culture)
    {
        object Result = value.ToUpper(culture);
        return Result;
    }
}
