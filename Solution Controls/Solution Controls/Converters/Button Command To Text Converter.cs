namespace Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Input;
    using CustomControls;

    /// <summary>
    /// Represents a converter from a button command to a text.
    /// </summary>
    [ValueConversion(typeof(ICommand), typeof(string))]
    public class ButtonCommandToTextConverter : IValueConverter
    {
        /// <summary>
        /// Converts from a button command to a text.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>The converted value.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ICommand AsCommand)
                return GetItemText(AsCommand);
            else
                throw new ArgumentOutOfRangeException(nameof(value));
        }

        /// <summary>
        /// Gets the text from the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>The text.</returns>
        protected virtual string GetItemText(ICommand command)
        {
            switch (command)
            {
                case ActiveDocumentRoutedCommand AsActiveDocumentCommand:
                    return AsActiveDocumentCommand.InactiveMenuHeader;

                case ExtendedRoutedCommand AsExtendedRoutedCommand:
                    return AsExtendedRoutedCommand.MenuHeader;

                case RoutedUICommand AsUICommand:
                    return AsUICommand.Text;

                default:
                    throw new ArgumentOutOfRangeException(nameof(command));
            }
        }

        /// <summary>
        /// Converts a binding target value to the source binding value.
        /// </summary>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use..</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>The converted value.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
