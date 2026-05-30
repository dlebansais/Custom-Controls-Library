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
    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        object Result = value;

        Debug.Assert(Result == ConvertBack(value, typeof(object), parameter, culture));

        return Result;
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value;
}
