namespace CustomControls;

using System.Windows;
using System.Windows.Controls;

/// <summary>
/// Represents a button with additional properties indicating if it should be displayed in a tool bar.
/// </summary>
public partial class ExtendedToolBarButton : Button
{
    #region Init
    /// <summary>
    /// Initializes static members of the <see cref="ExtendedToolBarButton"/> class.
    /// </summary>
    static ExtendedToolBarButton()
    {
        OverrideAncestorMetadata();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedToolBarButton"/> class.
    /// </summary>
    public ExtendedToolBarButton()
    {
        InitializeIsActive();
        ToolTipService.SetShowOnDisabled(this, true);
    }
    #endregion

    #region Ancestor Interface
    /// <summary>
    /// Overrides metadata associated to the ancestor control with new ones associated to this control specifically.
    /// </summary>
    private static void OverrideAncestorMetadata()
    {
        OverrideMetadataDefaultStyleKey();
        OverrideMetadataCommandKey();
    }

    /// <summary>
    /// Overrides the DefaultStyleKey metadata associated to the ancestor control with a new one associated to this control specifically.
    /// </summary>
    private static void OverrideMetadataDefaultStyleKey()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtendedToolBarButton), new FrameworkPropertyMetadata(typeof(ExtendedToolBarButton)));
    }

    /// <summary>
    /// Overrides the CommandKey metadata associated to the ancestor control with a new one associated to this control specifically.
    /// </summary>
    private static void OverrideMetadataCommandKey()
    {
        CommandProperty.OverrideMetadata(typeof(ExtendedToolBarButton), new FrameworkPropertyMetadata(OnCommandChanged));
    }

    /// <summary>
    /// Called when the Command dependency property is changed on <paramref name="modifiedObject"/>.
    /// </summary>
    /// <param name="modifiedObject">The object that had its property modified.</param>
    /// <param name="e">Information about the change.</param>
    private static void OnCommandChanged(DependencyObject modifiedObject, DependencyPropertyChangedEventArgs e)
    {
        ExtendedToolBarButton ctrl = (ExtendedToolBarButton)modifiedObject;
        ctrl.OnCommandChanged();
    }

    /// <summary>
    /// Called when the Command dependency property is changed.
    /// </summary>
    private void OnCommandChanged()
    {
        UpdateVisibility();
    }
    #endregion

    #region Client Interface
    /// <summary>
    /// Returns the <see cref="IsActive"/> dependency property to its default value.
    /// </summary>
    public virtual void ResetIsActive()
    {
        IsActive = IsDefaultActive;
    }
    #endregion

    #region Implementation
    /// <summary>
    /// Initializes the <see cref="IsActive"/> dependency property to its default value.
    /// </summary>
    private void InitializeIsActive()
    {
        IsActive = IsDefaultActive;
    }

    /// <summary>
    /// Updates the button visibility according to the current value of its properties.
    /// </summary>
    private void UpdateVisibility()
    {
        bool IsCommandGroupEnabled = true;

        if (Command is ExtendedRoutedCommand AsExtendedCommand)
            if (!AsExtendedCommand.CommandGroup.IsEnabled)
                IsCommandGroupEnabled = false;

        Visibility = (IsCommandGroupEnabled && (!IsCheckable || IsActive)) ? Visibility.Visible : Visibility.Collapsed;
    }
    #endregion
}
