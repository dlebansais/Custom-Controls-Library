namespace Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Input;
    using CustomControls;

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
            string ItemHeader;

            switch (command)
            {
                case ActiveDocumentRoutedCommand AsActiveDocumentCommand:
                    ItemHeader = AsActiveDocumentCommand.InactiveMenuHeader;
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

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
