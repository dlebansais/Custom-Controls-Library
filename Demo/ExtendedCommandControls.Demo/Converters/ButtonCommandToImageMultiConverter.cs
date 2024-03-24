namespace Converters;

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using CustomControls;

/// <summary>
/// Represents the converter that converts a command to a <see cref="ImageSource"/>.
/// </summary>
[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Instantiated in Xaml")]
internal class ButtonCommandToImageMultiConverter : IMultiValueConverter
{
    /// <summary>
    /// Converts a command to a <see cref="ImageSource"/>.
    /// </summary>
    /// <param name="values">The values to convert. The first value is the command, the second value the control associated to the command.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>
    /// A System.Object that represents the converted value.
    /// </returns>
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        Debug.Assert(values.Length > 1);

        Debug.Assert(values[0] is RoutedCommand);
        RoutedCommand Command = (RoutedCommand)values[0];
        Debug.Assert(values[1] is ExtendedToolBarButton);
        ExtendedToolBarButton Ctrl = (ExtendedToolBarButton)values[1];

        return ConvertValidValues(Command, Ctrl);
    }

    private static ImageSource ConvertValidValues(RoutedCommand command, ExtendedToolBarButton ctrl)
    {
        CommandResourceReference Reference = command switch
        {
            LocalizedRoutedCommand AsLocalizedRoutedCommand => AsLocalizedRoutedCommand.Reference,
            _ => ctrl.Reference,
        };

        ImageSource Result = Reference.GetImageSource(command.Name + ".png");

        return Result;
    }

    /// <summary>
    /// This method is not used.
    /// </summary>
    /// <param name="value">The value that the binding target produces.</param>
    /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>
    /// An array of values that have been converted from the target value back to the source values.
    /// </returns>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
