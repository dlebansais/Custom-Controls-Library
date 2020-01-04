namespace Converters
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Windows.Data;
    using CustomControls;

    /// <summary>
    /// Converter from a <see cref="ActiveCommand"/> to the associated content in a <see cref="DialogValidation"/>.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification="Instanciated in Xaml")]
    internal class ActiveCommandToContentConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converter from a <see cref="ActiveCommand"/> to the associated content in a <see cref="DialogValidation"/>.
        /// </summary>
        /// <param name="values">The array of values that the source bindings in the <see cref="MultiBinding"/> produces. The value <see cref="System.Windows.DependencyProperty.UnsetValue"/> indicates that the source binding has no value to provide for conversion.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A System.Object that represents the converted value.
        /// </returns>
        /// <remarks>
        /// <para>The first value must be a <see cref="DialogValidation"/>.</para>
        /// <para>The second value must be a <see cref="ActiveCommand"/>.</para>
        /// </remarks>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length > 2 && (values[0] is DialogValidation) && (values[1] is bool) && (values[2] is ActiveCommand))
            {
                DialogValidation Control = (DialogValidation)values[0];

                // This value is used to trigger a conversion when the content has changed. See DialogValidation.UpdateButtonContent().
                bool IsLocalized = (bool)values[1];
                Debug.Assert(IsLocalized == Control.IsLocalized);

                ActiveCommand Command = (ActiveCommand)values[2];

                if (Command is ActiveCommandOk)
                    return Control.ContentOk;
                else if (Command is ActiveCommandCancel)
                    return Control.ContentCancel;
                else if (Command is ActiveCommandAbort)
                    return Control.ContentAbort;
                else if (Command is ActiveCommandRetry)
                    return Control.ContentRetry;
                else if (Command is ActiveCommandIgnore)
                    return Control.ContentIgnore;
                else if (Command is ActiveCommandYes)
                    return Control.ContentYes;
                else if (Command is ActiveCommandNo)
                    return Control.ContentNo;
                else if (Command is ActiveCommandClose)
                    return Control.ContentClose;
                else if (Command is ActiveCommandHelp)
                    return Control.ContentHelp;
                else if (Command is ActiveCommandTryAgain)
                    return Control.ContentTryAgain;
                else if (Command is ActiveCommandContinue)
                    return Control.ContentContinue;
            }

            throw new ArgumentOutOfRangeException(nameof(values));
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
