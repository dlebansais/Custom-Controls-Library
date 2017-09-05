using CustomControls;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Verification;

namespace Converters
{
    /// <summary>
    ///     Represents the converter that converts the current margin, border and padding of the <see cref="EditableTextBlock"/> control to a margin.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Instantiated in Xaml")]
    internal class EditableTextBlockMarginConverter : IMultiValueConverter
    {
        /// <summary>
        ///     Converts the current margin, border and padding of the <see cref="EditableTextBlock"/> control to a margin.
        /// </summary>
        /// <param name="values">The values to convert. The first value indicates if the control is being edited, the second and third values are the margin and padding of the TextBlock part respectively, the fourth and fifth values are the border and padding of the TextBox part respectively.</param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">This parameter is a string that indicates which margin is desired, either "GridMargin" or "TextBoxMargin".</param>
        /// <param name="culture">This parameter is not used.</param>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Assert.ValidateReference(values);

            if (values.Length > 4 && (values[0] is bool) && (values[1] is Thickness) && (values[2] is Thickness) && (values[3] is Thickness) && (values[4] is Thickness))
            {
                bool IsEditing = (bool)values[0];
                Thickness TextBlockMargin = (Thickness)values[1];
                Thickness TextBlockPadding = (Thickness)values[2];
                Thickness TextBoxBorder = (Thickness)values[3];
                Thickness TextBoxPadding = (Thickness)values[4];

                double TextBoxLeft = (TextBlockMargin.Left + TextBlockPadding.Left) - (TextBoxBorder.Left + TextBoxPadding.Left + 3);
                double GridLeft = Math.Max(0.0, -TextBoxLeft);
                double TextBoxTop = (TextBlockMargin.Top + TextBlockPadding.Top) - (TextBoxBorder.Top + TextBoxPadding.Top);
                double GridTop = Math.Max(0.0, -TextBoxTop);

                double GridRight = IsEditing ? 0 : Math.Max(0.0, (TextBoxBorder.Right + TextBoxPadding.Right + 4) - (TextBlockPadding.Right + TextBlockMargin.Right));
                double GridBottom = IsEditing ? 0 : Math.Max(0.0, (TextBoxBorder.Bottom + TextBoxPadding.Bottom + 2) - (TextBlockPadding.Bottom + TextBlockMargin.Bottom));

                Thickness GridMargin = new Thickness(GridLeft, GridTop, GridRight, GridBottom);
                Thickness TextBoxMargin = new Thickness(TextBoxLeft, TextBoxTop, 0, 0);

                string ExpectedResult = parameter as string;

                if (ExpectedResult == "GridMargin")
                    return GridMargin;

                else if (ExpectedResult == "TextBoxMargin")
                    return TextBoxMargin;
            }

            return new Thickness(0);
        }

        /// <summary>
        ///     This method is not used.
        /// </summary>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
