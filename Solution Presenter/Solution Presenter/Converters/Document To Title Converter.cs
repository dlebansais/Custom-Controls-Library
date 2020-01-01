namespace Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Represents a converter from a document to a title.
    /// </summary>
    public class DocumentToTitleConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts from a document to a title.
        /// </summary>
        /// <param name="values">The array of values to convert.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>The converted value.</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Length > 1 && (values[0] is string FriendlyName) && (values[1] is bool IsDirty))
                return FriendlyName + (IsDirty ? "*" : string.Empty);
            else
                throw new ArgumentNullException(nameof(values));
        }

        /// <summary>
        /// Converts a binding target value to the source binding values.
        /// </summary>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to.</param>
        /// <param name="parameter">The converter parameter to use..</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>The converted value.</returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return Array.Empty<object>();
        }
    }
}
