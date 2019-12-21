using System;
using System.Globalization;
using System.Windows.Data;

namespace Converters
{
    /// <summary>
    /// Converter from an enum to an array of values it can have. The actual value is ignored.
    /// </summary>
    [ValueConversion(typeof(object), typeof(Array))]
    internal class EnumToItemsConverter : IValueConverter
    {
        /// <summary>
        /// Converter from an enum to an array of values it can have. The actual value is ignored.
        /// </summary>
        /// <param name="value">Any enum. Only its type is used, not its actual value.</param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">This parameter is not used.</param>
        /// <param name="culture">This parameter is not used.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            Type EnumType = value.GetType();
            if (EnumType.IsEnum)
                return EnumType.GetEnumValues();
            else
                return Array.CreateInstance(EnumType, 0);
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
            return null;
        }
    }
}
