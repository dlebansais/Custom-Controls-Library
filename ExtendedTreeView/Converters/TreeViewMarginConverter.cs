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
    /// <inheritdoc cref="IMultiValueConverter.Convert(object[], Type, object, CultureInfo)" />
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

    /// <inheritdoc cref="IMultiValueConverter.ConvertBack(object, Type[], object, CultureInfo)" />
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotSupportedException();
}
