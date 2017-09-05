using CustomControls;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;

namespace Converters
{
    [ValueConversion(typeof(ICommand), typeof(string))]
    public class ButtonCommandToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ICommand AsCommand;
            if ((AsCommand = value as ICommand) != null)
                return GetItemText(AsCommand);
            else
                return null;
        }

        protected virtual string GetItemText(ICommand command)
        {
            string ItemHeader = null;

            ActiveDocumentRoutedCommand AsActiveDocumentCommand;
            ExtendedRoutedCommand AsExtendedRoutedCommand;
            RoutedUICommand AsUICommand;

            if ((AsActiveDocumentCommand = command as ActiveDocumentRoutedCommand) != null)
                ItemHeader = AsActiveDocumentCommand.InactiveMenuHeader;

            else if ((AsExtendedRoutedCommand = command as ExtendedRoutedCommand) != null)
                ItemHeader = AsExtendedRoutedCommand.MenuHeader;

            else if ((AsUICommand = command as RoutedUICommand) != null)
                ItemHeader = AsUICommand.Text;

            return ItemHeader;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
