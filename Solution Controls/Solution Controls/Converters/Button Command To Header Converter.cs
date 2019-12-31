namespace Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Input;
    using CustomControls;

    /// <summary>
    /// Represents a converter from a button command to a menu header.
    /// </summary>
    public class ButtonCommandToHeaderConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts from a button command to a menu header.
        /// </summary>
        /// <param name="values">The array of values to convert.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>The converted value.</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Length > 0 && (values[0] is ICommand Command))
            {
                IDocument? ActiveDocument = null;
                string? ApplicationName = null;

                for (int i = 1; i < values.Length; i++)
                {
                    if (values[i] is IDocument)
                        ActiveDocument = values[i] as IDocument;

                    if (values[i] is string)
                        ApplicationName = values[i] as string;
                }

                return GetItemHeader(Command, ActiveDocument, ApplicationName);
            }
            else
                throw new ArgumentOutOfRangeException(nameof(values));
        }

        /// <summary>
        /// Gets the menu header of a command for a source element.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="activeDocument">The active document.</param>
        /// <param name="applicationName">The application name.</param>
        /// <returns>The header.</returns>
        protected virtual string GetItemHeader(ICommand command, IDocument? activeDocument, string? applicationName)
        {
            string ItemHeader;

            switch (command)
            {
                case ActiveDocumentRoutedCommand AsActiveDocumentCommand:
                    if (activeDocument == null)
                        ItemHeader = AsActiveDocumentCommand.InactiveMenuHeader;
                    else
                    {
                        string CommandTextFormat = AsActiveDocumentCommand.MenuHeader;
                        ItemHeader = string.Format(CultureInfo.CurrentCulture, CommandTextFormat, activeDocument.Path.HeaderName);
                    }
                    break;

                case LocalizedRoutedCommand AsLocalizedRoutedCommand:
                    ItemHeader = AsLocalizedRoutedCommand.MenuHeader;
                    if (ItemHeader.Contains(LocalizedRoutedCommand.ApplicationNamePattern) && applicationName != null)
                        ItemHeader = ItemHeader.Replace(LocalizedRoutedCommand.ApplicationNamePattern, applicationName);
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
        /// Converts a binding target value to the source binding values.
        /// </summary>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to.</param>
        /// <param name="parameter">The converter parameter to use..</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>The converted value.</returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return Array.Empty<object>();
        }
    }
}
