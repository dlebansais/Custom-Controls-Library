using CustomControls;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Converters
{
    public class ButtonCommandToGestureTextConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Length > 1 && (values[0] is ICommand) && (values[1] is FrameworkElement))
            {
                ICommand Command = (ICommand)values[0];
                FrameworkElement Source = (FrameworkElement)values[1];

                return GetItemGestureText(Command, Source);
            }

            return null;
        }

        protected virtual string GetItemGestureText(ICommand command, FrameworkElement source)
        {
            string GestureText = GestureHelper.GetGestureText(command, source);

            return GestureText;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
