namespace CustomControls;

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

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
    #region Enum Binding
    /// <summary>
    /// Identifies the <see cref="EnumBinding"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="EnumBinding"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty EnumBindingProperty = DependencyProperty.Register(nameof(EnumBinding), typeof(object), typeof(EnumRadioButton), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnEnumBindingChanged));

#pragma warning disable SA1629 // Documentation text should end with a period
    /// <summary>
    /// Gets or sets the enum property to bind on.
    /// <para>In combination with the <see cref="EnumValue"/> property, this is a replacement for the RadioButton.IsChecked property.</para>
    /// </summary>
    /// <example>
    /// Example: for a type called <code>MyEnum { MyFirstValue, MySecondValue }</code> and a property <code>MyEnum MyEnumValue { get; set; }</code> one can use the following Xaml code:
    /// <code>
    /// &lt;EnumRadioButton EnumBinding="{Binding Path=MyEnumValue}" EnumValue="{Binding Path=MyFirstValue}"&gt;First&lt;EnumRadioButton/&gt;
    /// &lt;EnumRadioButton EnumBinding="{Binding Path=MyEnumValue}" EnumValue="{Binding Path=MySecondValue}"&gt;Second&lt;EnumRadioButton/&gt;
    /// </code>
    /// </example>
    [Bindable(true)]
#pragma warning restore SA1629 // Documentation text should end with a period
    public object? EnumBinding
    {
        get { return GetValue(EnumBindingProperty); }
        set { SetValue(EnumBindingProperty, value); }
    }

    private static void OnEnumBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        EnumRadioButton Ctrl = (EnumRadioButton)d;
        Ctrl.OnEnumBindingChanged(e);
    }

    private void OnEnumBindingChanged(DependencyPropertyChangedEventArgs e)
    {
        object Value = ConvertedValue(EnumValue);

        if (Value.Equals(e.NewValue))
            SetCurrentValue(IsCheckedProperty, true);
    }
    #endregion

    #region Enum Value
    /// <summary>
    /// Identifies the <see cref="EnumValue"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="EnumValue"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty EnumValueProperty = DependencyProperty.Register(nameof(EnumValue), typeof(object), typeof(EnumRadioButton), new UIPropertyMetadata(null));

#pragma warning disable SA1629 // Documentation text should end with a period
    /// <summary>
    /// Gets or sets the enum value this radio button value is associated to.
    /// <para>In combination with the <see cref="EnumBinding"/> property, this is a replacement for the RadioButton.IsChecked property.</para>
    /// </summary>
    /// <example>
    /// Example: for a type called <code>MyEnum { MyFirstValue, MySecondValue }</code> and a property <code>MyEnum MyEnumValue { get; set; }</code> one can use the following Xaml code:
    /// <code>
    /// &lt;EnumRadioButton EnumBinding="{Binding Path=MyEnumValue}" EnumValue="{Binding Path=MyFirstValue}"&gt;First&lt;EnumRadioButton/&gt;
    /// &lt;EnumRadioButton EnumBinding="{Binding Path=MyEnumValue}" EnumValue="{Binding Path=MySecondValue}"&gt;Second&lt;EnumRadioButton/&gt;
    /// </code>
    /// </example>
    [Browsable(true)]
#pragma warning restore SA1629 // Documentation text should end with a period
    public object EnumValue
    {
        get { return GetValue(EnumValueProperty); }
        set { SetValue(EnumValueProperty, value); }
    }
    #endregion
}
