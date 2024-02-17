namespace CustomControls;

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

/// <summary>
/// Represents a converter from an array of margins to an object from a collection.
/// </summary>
public class TreeViewMarginConverter : IMultiValueConverter
{
    /// <summary>
    /// Converts an array of margins to an object from a collection.
    /// </summary>
    /// <param name="values">The array of values that the source bindings in the <see cref="MultiBinding"/> produces.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>A converted value.</returns>
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values is not null && values.Length > 3 && (values[0] is int Level) && (values[1] is bool IsRootAlwaysExpanded) && (values[2] is double IndentationWidth) && (values[3] is double ExpandButtonWidth))
        {
            double LeftMargin = Level * IndentationWidth;
            if (IsRootAlwaysExpanded && Level > 0)
                LeftMargin -= ExpandButtonWidth;

            return new Thickness(LeftMargin, 0, 0, 0);
        }

        return new Thickness(0);
    }

    /// <summary>
    /// Converts a binding target value to the source binding values.
    /// </summary>
    /// <param name="value">The value that the binding target produces.</param>
    /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>An array of values that have been converted from the target value back to the source values.</returns>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        return Array.Empty<object>();
    }
}
