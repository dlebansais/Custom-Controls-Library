namespace Converters;

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;
using CustomControls;

/// <summary>
/// Converter from a <see cref="ICommand"/> to a menu header string.
/// </summary>
[ValueConversion(typeof(ICommand), typeof(string))]
public class ButtonCommandToTextConverter : IValueConverter
{
    /// <summary>
    /// Converts from a <see cref="ICommand"/> to a menu header string.
    /// </summary>
    /// <param name="value">The <see cref="ICommand"/> object to convert.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>
    /// If <paramref name="value"/> is a valid <see cref="ICommand"/> object, the converter returns its menu header as a string.
    /// Otherwise, this method returns null.
    /// </returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is ICommand AsCommand)
            return GetItemText(AsCommand);
        else
            throw new ArgumentOutOfRangeException(nameof(value));
    }

    /// <summary>
    /// Converts from a <see cref="ICommand"/> to a menu header string.
    /// </summary>
    /// <param name="command">The <see cref="ICommand"/> object to convert.</param>
    /// <returns>
    /// If <paramref name="command"/> is an instance of one of the types that have an associated key to obtain a menu header, it returns this string.
    /// Otherwise, if the command is a <see cref="RoutedUICommand"/> it returns its text (already localized by the system).
    /// Otherwise, this method returns null.
    /// </returns>
    private static string GetItemText(ICommand command)
    {
        string ItemHeader;

        switch (command)
        {
            case ActiveDocumentRoutedCommand AsActiveDocumentCommand:
                ItemHeader = AsActiveDocumentCommand.InactiveMenuHeader;
                break;

            case LocalizedRoutedCommand AsLocalizedRoutedCommand:
                ItemHeader = AsLocalizedRoutedCommand.MenuHeader;
#if NETFRAMEWORK
                if (ItemHeader.Contains(LocalizedRoutedCommand.ApplicationNamePattern))
                    ItemHeader = ItemHeader.Replace(LocalizedRoutedCommand.ApplicationNamePattern, string.Empty);
#else
                if (ItemHeader.Contains(LocalizedRoutedCommand.ApplicationNamePattern, StringComparison.InvariantCulture))
                    ItemHeader = ItemHeader.Replace(LocalizedRoutedCommand.ApplicationNamePattern, string.Empty, StringComparison.InvariantCulture);
#endif
                break;

            case ExtendedRoutedCommand AsExtendedRoutedCommand:
                ItemHeader = AsExtendedRoutedCommand.MenuHeader;
                break;

            case RoutedUICommand AsUICommand:
                ItemHeader = AsUICommand.Text;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(command));
        }

        return ItemHeader;
    }

    /// <summary>
    /// This method is not used.
    /// </summary>
    /// <param name="value">The value that is produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>A converted value.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
