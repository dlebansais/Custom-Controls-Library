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
            if (value is ICommand AsCommand)
                return GetItemText(AsCommand);
            else
                throw new ArgumentOutOfRangeException(nameof(value));
        }

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

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
