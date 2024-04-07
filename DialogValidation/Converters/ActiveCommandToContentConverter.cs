namespace CustomControls;

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

/// <summary>
/// Converter from a <see cref="ActiveCommand"/> to the associated content in a <see cref="DialogValidation"/>.
/// </summary>
[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification="Instanciated in Xaml")]
internal class ActiveCommandToContentConverter : IMultiValueConverter
{
    /// <summary>
    /// Converter from a <see cref="ActiveCommand"/> to the associated content in a <see cref="DialogValidation"/>.
    /// </summary>
    /// <param name="values">The array of values that the source bindings in the <see cref="MultiBinding"/> produces. The value <see cref="DependencyProperty.UnsetValue"/> indicates that the source binding has no value to provide for conversion.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>
    /// A <see cref="object"/> that represents the converted value.
    /// </returns>
    /// <remarks>
    /// <para>The first value must be a <see cref="DialogValidation"/>.</para>
    /// <para>The second value must be a <see cref="ActiveCommand"/>.</para>
    /// </remarks>
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        Debug.Assert(values.Length > 2);

        object Result;

        if (values[0] is DialogValidation Control && values[1] is bool IsLocalized && values[2] is ActiveCommand Command)
            Result = ConvertValidValues(Control, IsLocalized, Command);
        else
            Result = string.Empty;

        return Result;
    }

#pragma warning disable CA1822 // Mark members as static (for Release build)
    private object ConvertValidValues(DialogValidation control, bool isLocalized, ActiveCommand command)
#pragma warning restore CA1822 // Mark members as static
    {
        // This value is used to trigger a conversion when the content has changed. See DialogValidation.UpdateButtonContent().
        Debug.Assert(isLocalized == control.IsLocalized);

        object Result = DependencyProperty.UnsetValue;

        Result = command switch
        {
            ActiveCommandCancel => control.ContentCancel,
            ActiveCommandAbort => control.ContentAbort,
            ActiveCommandRetry => control.ContentRetry,
            ActiveCommandIgnore => control.ContentIgnore,
            ActiveCommandYes => control.ContentYes,
            ActiveCommandNo => control.ContentNo,
            ActiveCommandClose => control.ContentClose,
            ActiveCommandHelp => control.ContentHelp,
            ActiveCommandTryAgain => control.ContentTryAgain,
            ActiveCommandContinue => control.ContentContinue,
            _ => control.ContentOk,
        };

        Debug.Assert(Result != DependencyProperty.UnsetValue);

#if DEBUG
        Type[] ConversionTargetTypes = [typeof(DialogValidation), typeof(bool), typeof(ActiveCommand)];
        object ConversionParameters = new object[] { control, command };
        object[] ConvertedBackValues = ConvertBack(Result, ConversionTargetTypes, ConversionParameters, CultureInfo.CurrentCulture);
        Debug.Assert(ConvertedBackValues.Length > 2);
        Debug.Assert(ConvertedBackValues[0] == control);
        Debug.Assert(ConvertedBackValues[1] is bool);
        bool ConvertedBackIsLocalized = (bool)ConvertedBackValues[1];
        Debug.Assert(ConvertedBackIsLocalized == isLocalized);
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
        Debug.Assert(targetTypes.Length > 2);
        Debug.Assert(targetTypes[0] == typeof(DialogValidation));
        Debug.Assert(targetTypes[1] == typeof(bool));
        Debug.Assert(targetTypes[2] == typeof(ActiveCommand));

        Debug.Assert(parameter is object[]);
        object[] ConversionParameters = (object[])parameter;
        Debug.Assert(ConversionParameters.Length > 1);
        Debug.Assert(ConversionParameters[0] is DialogValidation);
        Debug.Assert(ConversionParameters[1] is ActiveCommand);

        DialogValidation Control = (DialogValidation)ConversionParameters[0];
        bool IsLocalized = Control.IsLocalized;
        ActiveCommand Command = (ActiveCommand)ConversionParameters[1];

        object[] Result = [Control, IsLocalized, Command];
        return Result;
    }
}
