using CustomControls;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Converters
{
    public class ButtonCommandToToolTipConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Length > 2 && (values[0] is ICommand Command) && (values[1] is FrameworkElement Source) && (values[2] is string ApplicationName))
                return GetItemToolTip(Command, Source, ApplicationName);
            else
                throw new ArgumentOutOfRangeException(nameof(values));
        }

        protected virtual string GetItemToolTip(ICommand command, FrameworkElement source, string applicationName)
        {
            string CommandText = DocumentHelper.GetToolTipText(command, source);
            string GestureText = GestureHelper.GetGestureText(command, source);

            if (CommandText.Contains(LocalizedRoutedCommand.ApplicationNamePattern) && applicationName != null)
                CommandText = CommandText.Replace(LocalizedRoutedCommand.ApplicationNamePattern, applicationName);

            string ItemToolTip;

            if (CommandText != null)
                if (GestureText != null)
                    ItemToolTip = CommandText + " " + "(" + GestureText + ")";
                else
                    ItemToolTip = CommandText;
            else
                ItemToolTip = string.Empty;

            return ItemToolTip;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return Array.Empty<object>();
        }
    }
}
