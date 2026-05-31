namespace CustomControls;

using System;
using System.Globalization;
using System.Windows.Data;

/// <summary>
/// Represents a converter from an array of indentation levels to an object from a collection.
/// </summary>
public class TreeViewLevelToObjectConverter : IMultiValueConverter
{
    /// <inheritdoc cref="IMultiValueConverter.Convert(object[], Type, object, CultureInfo)" />
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

    /// <inheritdoc cref="IMultiValueConverter.ConvertBack(object, Type[], object, CultureInfo)" />
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotSupportedException();
}
