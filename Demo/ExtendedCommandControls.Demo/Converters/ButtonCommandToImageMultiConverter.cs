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
    /// <inheritdoc />
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

    /// <inheritdoc />
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotSupportedException();
}
