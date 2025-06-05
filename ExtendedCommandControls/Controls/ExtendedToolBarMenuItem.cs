namespace CustomControls;

using System.Windows;
using System.Windows.Controls;

/// <summary>
/// Represents a menu item with additional properties indicating if it should be displayed in a menu.
/// </summary>
public partial class ExtendedToolBarMenuItem : MenuItem
{
    #region Init
    /// <summary>
    /// Initializes static members of the <see cref="ExtendedToolBarMenuItem"/> class.
    /// </summary>
    static ExtendedToolBarMenuItem()
    {
        OverrideAncestorMetadata();
    }
    #endregion

    #region Ancestor Interface
    /// <summary>
    /// Overrides metadata associated to the ancestor control with new ones associated to this control specifically.
    /// </summary>
    private static void OverrideAncestorMetadata() => OverrideMetadataDefaultStyleKey();

    /// <summary>
    /// Overrides the DefaultStyleKey metadata associated to the ancestor control with a new one associated to this control specifically.
    /// </summary>
    private static void OverrideMetadataDefaultStyleKey() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtendedToolBarMenuItem), new FrameworkPropertyMetadata(typeof(ExtendedToolBarMenuItem)));
    #endregion
}
