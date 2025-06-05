namespace CustomControls;

using System;
using System.Globalization;
using System.Windows.Data;

/// <summary>
/// Represents a converter from an array of indentation levels to an object from a collection.
/// </summary>
public class TreeViewLevelToObjectConverter : IMultiValueConverter
{
    /// <summary>
    /// Converts an array of indentation levels to an object from a collection.
    /// </summary>
    /// <param name="values">The array of values that the source bindings in the <see cref="MultiBinding"/> produces.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>A converted value.</returns>
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values is not null && values.Length > 2 && (values[0] is int Level) && (values[1] is bool IsRootAlwaysExpanded) && (values[2] is int ChildCount))
            if (parameter is CompositeCollection CollectionOfItems && CollectionOfItems.Count > 2)
            {
                if (Level == 0 && IsRootAlwaysExpanded)
                    return CollectionOfItems[0];
                else if (ChildCount == 0)
                    return CollectionOfItems[1];

                return CollectionOfItems[2];
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(parameter));
            }
        else
            throw new ArgumentOutOfRangeException(nameof(parameter));
    }

    /// <summary>
    /// This method is not used.
    /// </summary>
    /// <param name="value">The value that the binding target produces.</param>
    /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>An array of values that have been converted from the target value back to the source values.</returns>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotSupportedException();
}
