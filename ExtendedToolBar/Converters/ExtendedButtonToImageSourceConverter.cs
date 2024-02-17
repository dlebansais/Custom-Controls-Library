namespace Converters;

using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using CustomControls;

/// <summary>
/// Converter from a <see cref="ExtendedToolBarButton"/> to an image source.
/// </summary>
[ValueConversion(typeof(ExtendedToolBarButton), typeof(ImageSource))]
public class ExtendedButtonToImageSourceConverter : IValueConverter
{
    /// <summary>
    /// Converts from a <see cref="ExtendedToolBarButton"/> to an image source.
    /// </summary>
    /// <param name="value">The <see cref="ExtendedToolBarButton"/> object to convert.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>
    /// If <paramref name="value"/> is a valid <see cref="ExtendedToolBarButton"/> object, the converter returns the image source associated to its command.
    /// Otherwise, this method returns null.
    /// </returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is ExtendedToolBarButton Button)
            return GetItemImageSource(Button.Command, Button.Reference);
        else
            throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Converts from a <see cref="ExtendedToolBarButton"/> to an image source.
    /// Uses the reference from <paramref name="command"/> first, and if null uses the <paramref name="reference"/> object.
    /// <returns>
    /// If both are null, returns null.
    /// </returns>
    /// </summary>
    private static ImageSource GetItemImageSource(ICommand command, CommandResourceReference reference)
    {
        switch (command)
        {
            case ExtendedRoutedCommand AsExtendedCommand:
                return AsExtendedCommand.ImageSource;

            case RoutedUICommand AsUICommand:
                Assembly ReferenceAssembly = Assembly.Load(reference.AssemblyName);
                ImageSource ItemImageSource = ThemeIcons.GetImageSource(ReferenceAssembly, AsUICommand.Name);
                return ItemImageSource;

            default:
                throw new ArgumentOutOfRangeException(nameof(command));
        }
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
