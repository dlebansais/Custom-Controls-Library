namespace EnumComboBoxDemo;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Data;

/// <summary>
/// Converter for tests.
/// </summary>
[ValueConversion(typeof(object), typeof(object))]
[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Instantiated in Xaml")]
internal class LazyConverter : IValueConverter
{
    /// <summary>
    /// Converter for tests.
    /// </summary>
    /// <param name="value">Any value.</param>
    /// <param name="targetType">The type of the binding target property (ignored).</param>
    /// <param name="parameter">The converter parameter to use (ignored).</param>
    /// <param name="culture">The culture to use in the converter (ignored).</param>
    /// <returns>A converted value.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null)
            return value!;
        else if (value is TestEnum1)
            return value;
        else
            return TestEnum1.X;
    }

    /// <summary>
    /// This method is not used.
    /// </summary>
    /// <param name="value">The value that is produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>A converted value.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }
}
