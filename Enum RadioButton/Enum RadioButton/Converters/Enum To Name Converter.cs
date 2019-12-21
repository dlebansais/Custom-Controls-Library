using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Windows.Data;
using Verification;

namespace Converters
{
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
        /// Convert from an enum value to its localized name.
        /// </summary>
        /// <param name="value">The enum value to convert.</param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">A string representing the name of some type. Resources will be taken from the assembly where that type is declared.</param>
        /// <param name="culture">This parameter is not used.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Assert.ValidateReference(value);

            Type EnumType = value.GetType();
            if (EnumType.IsEnum)
            {
                Assembly ResourceAssembly = EnumType.Assembly;
                Type ResourceSource = ResourceAssembly.GetType(parameter as string);
                ResourceManager Manager = new ResourceManager(ResourceSource);
                string ResourceName = value.ToString();

                return Manager.GetString(ResourceName, CultureInfo.CurrentCulture);
            }

            return null;
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
