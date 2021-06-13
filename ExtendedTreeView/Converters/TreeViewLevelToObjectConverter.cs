namespace CustomControls
{
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
            if (values != null && values.Length > 2 && (values[0] is int) && (values[1] is bool) && (values[2] is int))
                if (parameter is CompositeCollection CollectionOfItems && CollectionOfItems.Count > 2)
                {
                    int Level = (int)values[0];
                    bool IsRootAlwaysExpanded = (bool)values[1];
                    int ChildCount = (int)values[2];

                    if (Level == 0 && IsRootAlwaysExpanded)
                        return CollectionOfItems[0];
                    else if (ChildCount == 0)
                        return CollectionOfItems[1];

                    return CollectionOfItems[2];
                }
                else
                    throw new ArgumentOutOfRangeException(nameof(parameter));
            else
                throw new ArgumentOutOfRangeException(nameof(parameter));
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
}
