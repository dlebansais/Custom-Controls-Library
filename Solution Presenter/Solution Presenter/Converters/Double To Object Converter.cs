namespace Converters
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Converter from a boolean or nullable boolean to the first or second object of a collection.
    /// </summary>
    [ValueConversion(typeof(double), typeof(object))]
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Instanciated in Xaml")]
    public class DoubleToObjectConverter : IValueConverter
    {
        /// <summary>
        /// Converter from a double to the first or second object of a collection.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// If <paramref name="value"/> is a double and equal to zero, and <paramref name="parameter"/> is a collection with at least two objects, the converter returns the first object in the collection.
        /// Otherwise, this method throws an exception.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double AsDoubleValue)
                if (parameter is CompositeCollection CollectionOfItems && CollectionOfItems.Count > 1)
                    return AsDoubleValue == 0 ? CollectionOfItems[0] : CollectionOfItems[1];
                else
                    throw new ArgumentOutOfRangeException(nameof(parameter));
            else
                throw new ArgumentOutOfRangeException(nameof(value));
        }

        /// <summary>
        /// Converts an object to a <see cref="double"/> instance in a binding.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0.0;
        }
    }
}
