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
                IDocument ActiveDocument = (values.Length > 1) ? (IDocument)values[1] : null;

                return GetItemHeader(Command, ActiveDocument);
            }

            return null;
        }

        protected virtual string GetItemHeader(ICommand command, IDocument activeDocument)
        {
            string ItemHeader = null;

            ActiveDocumentRoutedCommand AsActiveDocumentCommand;
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

            else if ((AsExtendedRoutedCommand = command as ExtendedRoutedCommand) != null)
                //ItemHeader = AsExtendedRoutedCommand.GetLocalizedText(AsExtendedRoutedCommand.HeaderKey);
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
