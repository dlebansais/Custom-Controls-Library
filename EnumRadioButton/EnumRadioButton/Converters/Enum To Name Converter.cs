namespace Converters
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;
    using System.Resources;
    using System.Windows.Data;

    /// <summary>
    /// Converter from an enum value to its localized name.
    /// This class implements localization as follow:
    /// . Resources are taken from an assembly specified by the converter parameter.
    /// . In that assembly, a string resource with key equal to the enum value token name (the name used by the compiler for the value) must exist.
    /// . The converter returns the localized string associated to that key as per standard resource lookup rules.
    /// Clients that use a custom localization mechanism must implement their own converter.
    /// </summary>
    [ValueConversion(typeof(object), typeof(string))]
    internal class EnumToNameConverter : IValueConverter
    {
        /// <summary>
        /// Converts from an enum value to its localized name.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">A string representing the name of some type. Resources will be taken from the assembly where that type is declared.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// The converted value.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Type EnumType = value.GetType();
            Debug.Assert(EnumType.IsEnum);

            Assembly ResourceAssembly = EnumType.Assembly;
            Type ResourceSource = ResourceAssembly.GetType(parameter as string);
            ResourceManager Manager = new ResourceManager(ResourceSource);
            string ResourceName = value.ToString();

            object Result;

            Result = Manager.GetString(ResourceName, CultureInfo.CurrentCulture);
            Debug.Assert(Result == ConvertBack(Result, typeof(object), parameter, culture));

            return Result;
        }

        /// <summary>
        /// This method is not used.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// The converted value.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
