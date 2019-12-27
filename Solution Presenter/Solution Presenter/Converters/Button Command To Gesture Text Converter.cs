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
            if (values != null && values.Length > 1 && (values[0] is ICommand Command) && (values[1] is FrameworkElement Source))
                return GetItemGestureText(Command, Source);
            else
                throw new ArgumentOutOfRangeException(nameof(values));
        }

        protected virtual string GetItemGestureText(ICommand command, FrameworkElement source)
        {
            string GestureText = GestureHelper.GetGestureText(command, source);
            return GestureText;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return Array.Empty<object>();
        }
    }
}
