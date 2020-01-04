namespace Converters
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;
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
            Debug.Assert(values.Length > 2);

            object Result;

            if ((values[0] is DialogValidation Control) && (values[1] is bool IsLocalized) && (values[2] is ActiveCommand Command))
                Result = ConvertValidValues(Control, IsLocalized, Command);
            else
            {
                // This code applies for instance to collapsed DialogValidation controls.
                Debug.Assert(values[0] == DependencyProperty.UnsetValue);
                Debug.Assert(values[1] == DependencyProperty.UnsetValue);
                Debug.Assert(values[2] == DependencyProperty.UnsetValue);

                Result = string.Empty;
            }

            return Result;
        }

        private object ConvertValidValues(DialogValidation control, bool isLocalized, ActiveCommand command)
        {
            // This value is used to trigger a conversion when the content has changed. See DialogValidation.UpdateButtonContent().
            Debug.Assert(isLocalized == control.IsLocalized);

            object Result = DependencyProperty.UnsetValue;

            switch (command)
            {
                case ActiveCommandOk AsOk:
                    Result = control.ContentOk;
                    break;

                case ActiveCommandCancel AsCancel:
                    Result = control.ContentCancel;
                    break;

                case ActiveCommandAbort AsAbort:
                    Result = control.ContentAbort;
                    break;

                case ActiveCommandRetry AsRetry:
                    Result = control.ContentRetry;
                    break;

                case ActiveCommandIgnore AsIgnore:
                    Result = control.ContentIgnore;
                    break;

                case ActiveCommandYes AsYes:
                    Result = control.ContentYes;
                    break;

                case ActiveCommandNo AsNo:
                    Result = control.ContentNo;
                    break;

                case ActiveCommandClose AsClose:
                    Result = control.ContentClose;
                    break;

                case ActiveCommandHelp AsHelp:
                    Result = control.ContentHelp;
                    break;

                case ActiveCommandTryAgain AsTryAgain:
                    Result = control.ContentTryAgain;
                    break;

                case ActiveCommandContinue AsContinue:
                    Result = control.ContentContinue;
                    break;
            }

            Debug.Assert(Result != DependencyProperty.UnsetValue);

#if DEBUG
            object[] ConvertedBackValues = ConvertBack(Result, Array.Empty<Type>(), control, CultureInfo.CurrentCulture);
            Debug.Assert(ConvertedBackValues.Length > 2);
            Debug.Assert(ConvertedBackValues[0] == control);
            Debug.Assert(ConvertedBackValues[1] is bool ConvertedBackIsLocalized && ConvertedBackIsLocalized == isLocalized);
            Debug.Assert(ConvertedBackValues[2] == command);
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
            DialogValidation Control = (DialogValidation)parameter;
            bool IsLocalized = Control.IsLocalized;

            ICommand Command = Control.CommandOk;

            if (value == Control.ContentOk)
                Command = Control.CommandOk;
            else if (value == Control.ContentCancel)
                Command = Control.CommandCancel;
            else if (value == Control.ContentAbort)
                Command = Control.CommandAbort;
            else if (value == Control.ContentRetry)
                Command = Control.CommandRetry;
            else if (value == Control.ContentIgnore)
                Command = Control.CommandIgnore;
            else if (value == Control.ContentYes)
                Command = Control.CommandYes;
            else if (value == Control.ContentNo)
                Command = Control.CommandNo;
            else if (value == Control.ContentClose)
                Command = Control.CommandClose;
            else if (value == Control.ContentHelp)
                Command = Control.CommandHelp;
            else if (value == Control.ContentTryAgain)
                Command = Control.CommandTryAgain;
            else if (value == Control.ContentContinue)
                Command = Control.CommandContinue;

            object[] Result = new object[3] { Control, IsLocalized, Command };
            return Result;
        }
    }
}
