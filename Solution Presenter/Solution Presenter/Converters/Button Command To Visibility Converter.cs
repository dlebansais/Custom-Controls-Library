namespace Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;
    using CustomControls;

    public class ButtonCommandToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Length > 1 && (values[0] is ICommand Command) && (values[1] is bool CanShow))
            {
                bool IsVisible;

                if (values.Length > 2)
                {
                    if (values[2] is IDocument ActiveDocument)
                        IsVisible = GetItemVisibility(Command, ActiveDocument);
                    else
                        throw new ArgumentOutOfRangeException(nameof(values));
                }
                else
                    IsVisible = true;

                return CanShow && IsVisible ? Visibility.Visible : Visibility.Collapsed;
            }
            else
                throw new ArgumentOutOfRangeException(nameof(values));
        }

        protected virtual bool GetItemVisibility(ICommand command, object activeDocument)
        {
            switch (command)
            {
                default:
                case ActiveDocumentRoutedCommand AsActiveDocumentRoutedCommand:
                    return true;

                case ExtendedRoutedCommand AsExtendedCommand:
                    return AsExtendedCommand.CommandGroup.IsEnabled;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return Array.Empty<object>();
        }
    }
}
