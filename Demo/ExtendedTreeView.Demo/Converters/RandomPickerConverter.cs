namespace Converters;

using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Windows.Controls;
using System.Windows.Data;

/// <summary>
/// Converter to a value to an image from an array of images.
/// </summary>
[ValueConversion(typeof(int), typeof(object))]
internal class RandomPickerConverter : IValueConverter
{
    /// <inheritdoc cref="IValueConverter.Convert(object, Type, object, CultureInfo)" />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        int IndexValue = (int)value;

        if (parameter is CompositeCollection ArrayParameter && ArrayParameter.Count > 0)
        {
            // int Index = RandomNumberGenerator.GetInt32(ArrayParameter.Count);
            int Index = IndexValue % ArrayParameter.Count;
            object Result = ArrayParameter[Index];

            if (Result is Image AsImage)
                return AsImage.Source;
            else
                return Result;
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(parameter));
        }
    }

    /// <inheritdoc cref="IValueConverter.ConvertBack(object, Type, object, CultureInfo)" />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value;
}
