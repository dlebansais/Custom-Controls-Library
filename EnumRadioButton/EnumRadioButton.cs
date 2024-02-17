namespace CustomControls;

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Converters;

/// <summary>
/// Represents a radio button with specific support for enum types.
/// <para>Implemented as a derived class of the <see cref="RadioButton"/> parent.</para>
/// </summary>
/// <remarks>
/// <para>. Retain all features of a standard radio button.</para>
/// <para>. Include two additional properties to use for binding specifically on an enum property.</para>
/// <para>. Provide support for showing all values an enum can take in as many radio buttons as there are values.</para>
/// </remarks>
public partial class EnumRadioButton : RadioButton
{
    #region Init
    /// <summary>
    /// Identifies a converter that any client can use to perform conversion from an enum to all of its values.
    /// <para><see cref="EnumItems"/> is available in Xaml code using the following syntax:</para>
    /// <code>
    /// <para>Converter={x:Static ctrl:EnumRadioButton.EnumItems}</para>
    /// </code>
    /// <para>With ctrl: the namespace prefix (you can use any prefix you like) for the custom control library.</para>
    /// </summary>
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification="This class has not field that could mutate")]
    public static readonly IValueConverter EnumItems = new EnumToItemsConverter();

    /// <summary>
    /// Identifies a converter that any client can use to perform conversion from an enum to the localized name of its current value.
    /// <para><see cref="EnumName"/> is available in Xaml code using the following syntax:</para>
    /// <code>
    /// <para>Converter={x:Static ctrl:EnumRadioButton.EnumName}</para>
    /// </code>
    /// <para>With ctrl: the namespace prefix (you can use any prefix you like) for the custom control library.</para>
    /// </summary>
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This class has not field that could mutate")]
    public static readonly IValueConverter EnumName = new EnumToNameConverter();

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumRadioButton"/> class.
    /// </summary>
    public EnumRadioButton()
        : base()
    {
    }
    #endregion

    #region Events
    /// <summary>
    /// Override the <see cref="OnChecked"/> event handler, to modify the enum property bound to the <see cref="EnumBinding"/> dependency property when this radio button is checked.
    /// <para>The enum property bound to the <see cref="EnumBinding"/> property is set to the value designed by the <see cref="EnumValue"/> property at the time of the click.</para>
    /// </summary>
    /// <param name="e">Provides data for <see cref="RoutedEventArgs"/>.</param>
    protected override void OnChecked(RoutedEventArgs e)
    {
        base.OnChecked(e);

        object Value = ConvertedValue(EnumValue);
        SetCurrentValue(EnumBindingProperty, Value);
    }
    #endregion

    #region Implementation
    /// <summary>
    /// Convert the an enum value, designed by its name (a string) to the corresponding value as an object.
    /// </summary>
    /// <remarks>
    /// The Value parameter is an object and not a string to leave open the support of value names by other means than strings.
    /// </remarks>
    /// <parameters>
    /// <param name="value">A string containing the value name. If <paramref name="value"/> is not recognized as a valid enum value, this function returns <paramref name="value"/> as is.</param>
    /// </parameters>
    private object ConvertedValue(object value)
    {
        if (value is string AsString && EnumBinding is not null)
        {
            Type EnumType = EnumBinding.GetType();
            if (EnumType.IsEnum)
            {
                string[] EnumNames = EnumType.GetEnumNames();
                Array EnumValues = EnumType.GetEnumValues();

                for (int i = 0; i < EnumNames.Length && i < EnumValues.Length; i++)
                    if (EnumNames[i] == AsString)
                        value = EnumValues.GetValue(i)!;
            }
        }

        return value;
    }
    #endregion
}
