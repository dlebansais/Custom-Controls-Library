using CustomControls;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;

namespace Converters
{
    /// <summary>
    ///     Converter from a <see cref="ICommand"/> to a menu header string.
    /// </summary>
    [ValueConversion(typeof(ICommand), typeof(string))]
    public class ButtonCommandToTextConverter : IValueConverter
    {
        /// <summary>
        ///     Converts from a <see cref="ICommand"/> to a menu header string.
        /// </summary>
        /// <param name="value">The <see cref="ICommand"/> object to convert.</param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">This parameter is not used.</param>
        /// <param name="culture">This parameter is not used.</param>
        /// <returns>
        ///     If <paramref name="value"/> is a valid <see cref="ICommand"/> object, the converter returns its menu header as a string.
        ///     Otherwise, this method returns null.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ICommand AsCommand)
                return GetItemText(AsCommand);
            else
                throw new ArgumentOutOfRangeException(nameof(value));
        }

        /// <summary>
        ///     Converts from a <see cref="ICommand"/> to a menu header string.
        /// </summary>
        /// <param name="command">The <see cref="ICommand"/> object to convert.</param>
        /// <returns>
        ///     If <paramref name="command"/> is an instance of one of the types that have an associated key to obtain a menu header, it returns this string.
        ///     Otherwise, if the command is a <see cref="RoutedUICommand"/> it returns its text (already localized by the system).
        ///     Otherwise, this method returns null.
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
                    if (ItemHeader.Contains(LocalizedRoutedCommand.ApplicationNamePattern))
                        ItemHeader = ItemHeader.Replace(LocalizedRoutedCommand.ApplicationNamePattern, "");
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
        ///     This method is not used and will always return null.
        /// </summary>
        /// <param name="value">This parameter is not used.</param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">This parameter is not used.</param>
        /// <param name="culture">This parameter is not used.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
