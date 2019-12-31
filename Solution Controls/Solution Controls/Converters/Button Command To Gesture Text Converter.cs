namespace Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;
    using CustomControls;

    /// <summary>
    /// Represents a converter from a button command to a gesture text.
    /// </summary>
    public class ButtonCommandToGestureTextConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts from a button command to a gesture text.
        /// </summary>
        /// <param name="values">The array of values to convert.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>The converted value.</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Length > 1 && (values[0] is ICommand Command) && (values[1] is FrameworkElement Source))
                return GetItemGestureText(Command, Source);
            else
                throw new ArgumentOutOfRangeException(nameof(values));
        }

        /// <summary>
        /// Gets the gesture text of a command for a source element.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="source">The source element.</param>
        /// <returns>The gesture text.</returns>
        protected virtual string GetItemGestureText(ICommand command, FrameworkElement source)
        {
            string GestureText = GestureHelper.GetGestureText(command, source);

            return GestureText;
        }

        /// <summary>
        /// Converts a binding target value to the source binding values.
        /// </summary>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to.</param>
        /// <param name="parameter">The converter parameter to use..</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>The converted value.</returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return Array.Empty<object>();
        }
    }
}
