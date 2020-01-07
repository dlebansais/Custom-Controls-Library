namespace Converters
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Represents the converter that converts the current margin, border and padding of the <see cref="CustomControls.EditableTextBlock"/> control to a margin.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Instantiated in Xaml")]
    internal class EditableTextBlockMarginConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts the current margin, border and padding of the <see cref="CustomControls.EditableTextBlock"/> control to a margin.
        /// </summary>
        /// <param name="values">The values to convert. The first value indicates if the control is being edited, the second and third values are the margin and padding of the TextBlock part respectively, the fourth and fifth values are the border and padding of the TextBox part respectively.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">This parameter is a string that indicates which margin is desired, either "GridMargin" or "TextBoxMargin".</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A System.Object that represents the converted value.
        /// </returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Debug.Assert(values.Length > 4);

            Debug.Assert(values[0] is bool);
            bool IsEditing = (bool)values[0];
            Debug.Assert(values[1] is Thickness);
            Thickness TextBlockMargin = (Thickness)values[1];
            Debug.Assert(values[2] is Thickness);
            Thickness TextBlockPadding = (Thickness)values[2];
            Debug.Assert(values[3] is Thickness);
            Thickness TextBoxBorder = (Thickness)values[3];
            Debug.Assert(values[4] is Thickness);
            Thickness TextBoxPadding = (Thickness)values[4];
            Debug.Assert(parameter is string);
            string ExpectedResult = (string)parameter;

            return ConvertValidValues(IsEditing, TextBlockMargin, TextBlockPadding, TextBoxBorder, TextBoxPadding, ExpectedResult);
        }

        private object ConvertValidValues(bool isEditing, Thickness textBlockMargin, Thickness textBlockPadding, Thickness textBoxBorder, Thickness textBoxPadding, string expectedResult)
        {
            object Result;

            double TextBoxLeft = (textBlockMargin.Left + textBlockPadding.Left) - (textBoxBorder.Left + textBoxPadding.Left + 3);
            double GridLeft = Math.Max(0.0, -TextBoxLeft);
            double TextBoxTop = (textBlockMargin.Top + textBlockPadding.Top) - (textBoxBorder.Top + textBoxPadding.Top);
            double GridTop = Math.Max(0.0, -TextBoxTop);

            double GridRight = isEditing ? 0 : Math.Max(0.0, (textBoxBorder.Right + textBoxPadding.Right + 4) - (textBlockPadding.Right + textBlockMargin.Right));
            double GridBottom = isEditing ? 0 : Math.Max(0.0, (textBoxBorder.Bottom + textBoxPadding.Bottom + 2) - (textBlockPadding.Bottom + textBlockMargin.Bottom));

            Thickness GridMargin = new Thickness(GridLeft, GridTop, GridRight, GridBottom);
            Thickness TextBoxMargin = new Thickness(TextBoxLeft, TextBoxTop, 0, 0);

            if (expectedResult == "GridMargin")
                Result = GridMargin;
            else
                Result = TextBoxMargin;

#if DEBUG
            Type[] ConversionTargetTypes = new Type[] { typeof(bool), typeof(Thickness), typeof(Thickness), typeof(Thickness), typeof(Thickness) };
            object ConversionParameters = new object[] { isEditing, textBlockMargin, textBlockPadding, textBoxBorder, textBoxPadding, expectedResult };
            object[] ConvertedBackValues = ConvertBack(Result, ConversionTargetTypes, ConversionParameters, CultureInfo.CurrentCulture);
            Debug.Assert(ConvertedBackValues.Length == 0);
#endif

            return Result;
        }

        /// <summary>
        /// Converts a binding target value to the source binding values.
        /// </summary>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// An array of values that have been converted from the target value back to the source values.
        /// </returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return Array.Empty<object>();
        }
    }
}
