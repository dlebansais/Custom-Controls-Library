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
public class RandomPickerConverter : IValueConverter
{
    /// <summary>
    /// Converts a value to an image from an array of images.
    /// </summary>
    /// <param name="value">An integer value.</param>
    /// <param name="targetType">The type of the binding target property (ignored).</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter (ignored).</param>
    /// <returns>A converted value.</returns>
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
            throw new ArgumentOutOfRangeException(nameof(parameter));
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
