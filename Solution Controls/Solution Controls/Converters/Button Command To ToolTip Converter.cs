using CustomControls;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Verification;

namespace Converters
{
    public class ButtonCommandToToolTipConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Assert.ValidateReference(values);

            if (values.Length > 1 && (values[0] is ICommand) && (values[1] is FrameworkElement))
            {
                ICommand Command = (ICommand)values[0];
                FrameworkElement Source = (FrameworkElement)values[1];
                string ApplicationName = (string)values[2];

                return GetItemToolTip(Command, Source, ApplicationName);
            }

            return null;
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
                ItemToolTip = null;

            return ItemToolTip;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
