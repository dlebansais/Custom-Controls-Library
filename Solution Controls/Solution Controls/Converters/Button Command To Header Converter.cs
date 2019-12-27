using CustomControls;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;

namespace Converters
{
    public class ButtonCommandToHeaderConverter : IMultiValueConverter
    {
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

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return Array.Empty<object>();
        }
    }
}
