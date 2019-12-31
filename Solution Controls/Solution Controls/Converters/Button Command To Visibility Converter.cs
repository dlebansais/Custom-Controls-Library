namespace Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;
    using CustomControls;

    /// <summary>
    /// Represents a converter from a button command to a visibility object.
    /// </summary>
    public class ButtonCommandToVisibilityConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts from a button command to a visibility object.
        /// </summary>
        /// <param name="values">The array of values to convert.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>The converted value.</returns>
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

        /// <summary>
        /// Gets the visibility of a command for a source element.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="activeDocument">The active document.</param>
        /// <returns>True if visible; otherwise, false.</returns>
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
