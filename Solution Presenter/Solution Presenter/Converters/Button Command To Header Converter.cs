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
                if (values.Length > 1)
                {
                    IDocument ActiveDocument = (IDocument)values[1];
                    return GetItemHeader(Command, ActiveDocument);
                }
                else
                    return GetItemHeader(Command, null);
            else
                throw new ArgumentOutOfRangeException(nameof(values));
        }

        protected virtual string GetItemHeader(ICommand command, IDocument? activeDocument)
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
