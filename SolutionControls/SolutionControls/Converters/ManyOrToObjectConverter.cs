namespace Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Represents a converter from a collection of boolean to the logical OR of the collection.
    /// </summary>
    public class ManyOrToObjectConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts from a collection of boolean to the logical OR of the collection.
        /// </summary>
        /// <param name="values">The array of values that the source bindings in the <see cref="MultiBinding"/> produces.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>The logical OR of the input values.</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool BooleanValue = false;

            if (values != null)
                foreach (object o in values)
                {
                    if (o is bool)
                    {
                        bool b = (bool)o;
                        if (b)
                        {
                            BooleanValue = true;
                            break;
                        }
                    }
                    else if (o is bool?)
                    {
                        bool? b = (bool?)o;
                        if (b.HasValue && b.Value)
                        {
                            BooleanValue = true;
                            break;
                        }
                    }
                }

            if (parameter is CompositeCollection CollectionOfItems && CollectionOfItems.Count > 1)
                return BooleanValue ? CollectionOfItems[1] : CollectionOfItems[0];
            else
                throw new ArgumentOutOfRangeException(nameof(parameter));
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
