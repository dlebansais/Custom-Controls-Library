namespace EnumRadioButton.Demo;

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
    /// <inheritdoc cref="IValueConverter.Convert(object, Type, object, CultureInfo)" />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null)
            return value!;
        else if (value is TestEnum1)
            return value;
        else
            return TestEnum1.X;
    }

    /// <inheritdoc cref="IValueConverter.ConvertBack(object, Type, object, CultureInfo)" />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value;
}
