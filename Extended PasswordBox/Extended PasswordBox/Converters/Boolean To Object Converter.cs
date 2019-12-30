namespace Converters
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Converter from a boolean or nullable boolean to the first or second objects in a collection.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(object))]
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Instanciated in Xaml")]
    internal class BooleanToObjectConverter : IValueConverter
    {
        /// <summary>
        /// Converter from a boolean or nullable boolean to the first or second objects in a collection.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// If <paramref name="value"/> is a bool and equal to True, or a bool? and also equal to true, and <paramref name="parameter"/> is a collection with at least two objects, the converter returns the second object in the collection.
        /// Otherwise, if collection has at least two objects, the converter returns the first object in the collection.
        /// Otherwise, this method throws an exception.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool BooleanValue;

            if (value is bool)
                BooleanValue = (bool)value;
            else if (value is bool?)
                BooleanValue = ((bool?)value) ?? false;
            else
                throw new ArgumentOutOfRangeException(nameof(value));

            if (parameter is CompositeCollection CollectionOfItems && CollectionOfItems.Count > 1)
                return BooleanValue ? CollectionOfItems[1] : CollectionOfItems[0];
            else
                throw new ArgumentOutOfRangeException(nameof(parameter));
        }

        /// <summary>
        /// This method is not used and will always return null.
        /// </summary>
        /// <param name="value">This parameter is not used.</param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">This parameter is not used.</param>
        /// <param name="culture">This parameter is not used.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
