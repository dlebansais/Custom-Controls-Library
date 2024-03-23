namespace EnumComboBoxDemo;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Windows.Data;

/// <summary>
/// Converter from upper a case string to a lower case string.
/// </summary>
[ValueConversion(typeof(string), typeof(string))]
[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Instantiated in Xaml")]
internal class UpperToLowerConverter : IValueConverter
{
    /// <summary>
    /// Converts from a upper case string to a lower case string.
    /// </summary>
    /// <param name="value">The value to convert. Must be a string.</param>
    /// <param name="targetType">The type of the binding target property (ignored).</param>
    /// <param name="parameter">The converter parameter to use (ignored).</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>A converted value.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Contract.Requires(value is string);
        return ((string)value).ToLower(culture);
    }

    /// <summary>
    /// Converts from a lower case string to a upper case string.
    /// </summary>
    /// <param name="value">The value to convert. Must be a string.</param>
    /// <param name="targetType">The type of the binding target property (ignored).</param>
    /// <param name="parameter">The converter parameter to use (ignored).</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>A converted value.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Contract.Requires(value is string);
        return ((string)value).ToUpper(culture);
    }
}
