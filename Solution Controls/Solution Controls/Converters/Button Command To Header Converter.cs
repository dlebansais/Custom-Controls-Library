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
            if (values != null && values.Length > 0 && (values[0] is ICommand))
            {
                ICommand Command = (ICommand)values[0];
                IDocument ActiveDocument = null;
                string ApplicationName = null;

                for (int i = 1; i < values.Length; i++)
                {
                    if (values[i] is IDocument)
                        ActiveDocument = values[i] as IDocument;

                    if (values[i] is string)
                        ApplicationName = values[i] as string;
                }

                return GetItemHeader(Command, ActiveDocument, ApplicationName);
            }

            return null;
        }

        protected virtual string GetItemHeader(ICommand command, IDocument activeDocument, string applicationName)
        {
            string ItemHeader = null;

            ActiveDocumentRoutedCommand AsActiveDocumentCommand;
            LocalizedRoutedCommand AsLocalizedRoutedCommand;
            ExtendedRoutedCommand AsExtendedRoutedCommand;
            RoutedUICommand AsUICommand;

            if ((AsActiveDocumentCommand = command as ActiveDocumentRoutedCommand) != null)
            {
                if (activeDocument == null)
                    ItemHeader = AsActiveDocumentCommand.InactiveMenuHeader;
                else
                {
                    string CommandTextFormat = AsActiveDocumentCommand.MenuHeader;
                    if (CommandTextFormat != null)
                        ItemHeader = string.Format(CultureInfo.CurrentCulture, CommandTextFormat, activeDocument.Path.HeaderName);
                }
            }

            else if ((AsLocalizedRoutedCommand = command as LocalizedRoutedCommand) != null)
            {
                ItemHeader = AsLocalizedRoutedCommand.MenuHeader;
                if (ItemHeader.Contains(LocalizedRoutedCommand.ApplicationNamePattern) && applicationName != null)
                    ItemHeader = ItemHeader.Replace(LocalizedRoutedCommand.ApplicationNamePattern, applicationName);
            }

            else if ((AsExtendedRoutedCommand = command as ExtendedRoutedCommand) != null)
                ItemHeader = AsExtendedRoutedCommand.MenuHeader;

            else if ((AsUICommand = command as RoutedUICommand) != null)
                ItemHeader = AsUICommand.Text;

            return ItemHeader;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
