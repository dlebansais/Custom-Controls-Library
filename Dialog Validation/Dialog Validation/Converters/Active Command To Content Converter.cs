namespace Converters
{
    using System;
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
        /// <param name="values">The values to convert.</param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">This parameter is not used.</param>
        /// <param name="culture">This parameter is not used.</param>
        /// <returns>
        /// A System.Object that represents the converted value
        /// </returns>
        /// <remarks>
        /// <para>The first value must be a <see cref="DialogValidation"/>.</para>
        /// <para>The second value must be a <see cref="ActiveCommand"/>.</para>
        /// </remarks>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length > 1 && (values[0] is DialogValidation) && (values[1] is ActiveCommand))
            {
                DialogValidation Control = (DialogValidation)values[0];
                ActiveCommand Command = (ActiveCommand)values[1];

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
        /// This method is not used and will always return null.
        /// </summary>
        /// <param name="value">This parameter is not used.</param>
        /// <param name="targetTypes">This parameter is not used.</param>
        /// <param name="parameter">This parameter is not used.</param>
        /// <param name="culture">This parameter is not used.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return Array.Empty<object>();
        }
    }
}


