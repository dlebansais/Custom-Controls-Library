namespace CustomControls;

using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

/// <summary>
/// Represents a menu item with additional properties indicating if it should be displayed in a menu.
/// </summary>
public partial class ExtendedToolBarMenuItem : MenuItem
{
    #region Can Show
    /// <summary>
    /// Identifies the <see cref="CanShow"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="CanShow"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty CanShowProperty = DependencyProperty.Register(nameof(CanShow), typeof(bool), typeof(ExtendedToolBarMenuItem), new PropertyMetadata(true));

    /// <summary>
    /// Gets or sets a value indicating whether the button can be shown in any menu.
    /// True, the button can be shown in any menu.
    /// False, the button should not appear in any menu.
    /// <para>Note: this allows to specify the menu generic visibility when multi binding is used to decide if it is visible.</para>
    /// </summary>
    public bool CanShow
    {
        get { return (bool)GetValue(CanShowProperty); }
        set { SetValue(CanShowProperty, value); }
    }
    #endregion
}
