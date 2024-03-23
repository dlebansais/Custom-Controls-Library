namespace CustomControls;

using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

/// <summary>
/// Represents a combo box with specific support for enum types.
/// <para>Implemented as a derived class of the <see cref="ComboBox"/> parent.</para>
/// </summary>
/// <remarks>
/// <para>. Retain all features of a standard combo box.</para>
/// <para>. Include an additional property to use for binding specifically on an enum property.</para>
/// <para>. Provide support for localized value names by way of a converter property, to convert all values to their localized content.</para>
/// <para>. Measure the largest selectable value before it reports its own size.</para>
/// </remarks>
public partial class EnumComboBox : ComboBox
{
    #region Enum Binding
    /// <summary>
    /// Identifies the <see cref="EnumBinding"/> dependency property.
    /// </summary>
    /// <returns>The identifier for the <see cref="EnumBinding"/> dependency property.</returns>
    public static readonly DependencyProperty EnumBindingProperty = DependencyProperty.Register(nameof(EnumBinding), typeof(object), typeof(EnumComboBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnEnumBindingChanged));

#pragma warning disable SA1629 // Documentation text should end with a period
    /// <summary>
    /// Gets or sets the enum property to bind on.
    /// <para>This is a replacement for the ComboBox.ItemsSource property.</para>
    /// </summary>
    /// <example>
    /// For a type called <code>MyEnum { MyFirstValue, MySecondValue }</code> and a property <code>MyEnum MyEnumValue { get; set; }</code> one can use the following Xaml code:
    /// <code>
    /// <para>&lt;EnumComboBox EnumBinding="{Binding Path=MyEnumValue}"/&gt;</para>
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
        EnumComboBox Ctrl = (EnumComboBox)d;
        Ctrl.OnEnumBindingChanged(e);
    }

    private void OnEnumBindingChanged(DependencyPropertyChangedEventArgs e)
    {
        if (IsUserSelecting)
            return;

        if (e.NewValue is null)
            ResetContent();
        else if (e.OldValue is null || e.OldValue.GetType() != e.NewValue.GetType())
            UpdateContent(e.NewValue.GetType());

        int Index = EnumBindingAsIndex;
        if (SelectedIndex != Index)
            SelectedIndex = Index;
    }
    #endregion

    #region Name Converter
    /// <summary>
    /// Identifies the <see cref="NameConverter"/> dependency property.
    /// </summary>
    /// <returns>The identifier for the <see cref="NameConverter"/> dependency property.</returns>
    public static readonly DependencyProperty NameConverterProperty = DependencyProperty.Register(nameof(NameConverter), typeof(IValueConverter), typeof(EnumComboBox), new PropertyMetadata(new Converters.IdentityStringConverter()));

    /// <summary>
    /// Gets or sets the converter to use to convert an enum value to its localized content (usually a string).
    /// </summary>
    public IValueConverter NameConverter
    {
        get { return (IValueConverter)GetValue(NameConverterProperty); }
        set { SetValue(NameConverterProperty, value); }
    }
    #endregion

    #region Name Converter Parameter
    /// <summary>
    /// Identifies the <see cref="NameConverterParameter"/> dependency property.
    /// </summary>
    /// <returns>The identifier for the <see cref="NameConverterParameter"/> dependency property.</returns>
    public static readonly DependencyProperty NameConverterParameterProperty = DependencyProperty.Register(nameof(NameConverterParameter), typeof(object), typeof(EnumComboBox), new PropertyMetadata(string.Empty));

    /// <summary>
    /// Gets or sets the converter parameter to use when converting an enum value to its localized content.
    /// </summary>
    public object NameConverterParameter
    {
        get { return GetValue(NameConverterParameterProperty); }
        set { SetValue(NameConverterParameterProperty, value); }
    }
    #endregion

    #region Name Converter Culture
    /// <summary>
    /// Identifies the <see cref="NameConverterCulture"/> dependency property.
    /// </summary>
    /// <returns>The identifier for the <see cref="NameConverterCulture"/> dependency property.</returns>
    public static readonly DependencyProperty NameConverterCultureProperty = DependencyProperty.Register(nameof(NameConverterCulture), typeof(CultureInfo), typeof(EnumComboBox), new PropertyMetadata(CultureInfo.CurrentCulture));

    /// <summary>
    /// Gets or sets the converter culture to use when converting an enum value to its localized content.
    /// </summary>
    public CultureInfo NameConverterCulture
    {
        get { return (CultureInfo)GetValue(NameConverterCultureProperty); }
        set { SetValue(NameConverterCultureProperty, value); }
    }
    #endregion
}
