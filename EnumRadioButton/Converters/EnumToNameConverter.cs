namespace Converters;

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Windows.Data;
using Contracts;

/// <summary>
/// Converter from an enum value to its localized name.
/// This class implements localization as follow:
/// . Resources are taken from an assembly specified by the converter parameter.
/// . In that assembly, a string resource with key equal to the enum value token name (the name used by the compiler for the value) must exist.
/// . The converter returns the localized string associated to that key as per standard resource lookup rules.
/// Clients that use a custom localization mechanism must implement their own converter.
/// </summary>
[ValueConversion(typeof(object), typeof(string))]
internal partial class EnumToNameConverter : IValueConverter
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
    [Access("public")]
    [Require("value.GetType().IsEnum")]
    [RequireNotNull(nameof(parameter), Type = "object")]
    private static object ConvertVerified(object value, Type targetType, string parameter, CultureInfo culture)
    {
        Assembly ResourceAssembly = value.GetType().Assembly;
        Type ResourceSource = Contract.AssertNotNull(ResourceAssembly.GetType(parameter));

        ResourceManager Manager = new(ResourceSource);
        string ResourceName = Contract.AssertNotNull(value.ToString());

        // This converter converts the enum to its name in a resx file. The name must exist.
        string? Resource = Manager.GetString(ResourceName, culture);
        object Result = Contract.AssertNotNull(Resource);

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
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
}
