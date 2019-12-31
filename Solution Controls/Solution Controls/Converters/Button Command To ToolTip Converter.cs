namespace Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;
    using CustomControls;

    /// <summary>
    /// Represents a converter from a button command to a tooltip.
    /// </summary>
    public class ButtonCommandToToolTipConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts from a button command to a tooltip.
        /// </summary>
        /// <param name="values">The array of values to convert.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>The converted value.</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Length > 2 && (values[0] is ICommand Command) && (values[1] is FrameworkElement Source) && (values[2] is string ApplicationName))
                return GetItemToolTip(Command, Source, ApplicationName);
            else
                throw new ArgumentOutOfRangeException(nameof(values));
        }

        /// <summary>
        /// Gets the tooltip of a command for a source element.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="source">The source document.</param>
        /// <param name="applicationName">The application name.</param>
        /// <returns>The tooltip.</returns>
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
