﻿namespace Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

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
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value.</returns>
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
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
