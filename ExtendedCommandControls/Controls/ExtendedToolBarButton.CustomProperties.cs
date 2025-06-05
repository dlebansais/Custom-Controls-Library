namespace CustomControls;

using System.Windows;
using System.Windows.Controls;

/// <summary>
/// Represents a button with additional properties indicating if it should be displayed in a tool bar.
/// </summary>
public partial class ExtendedToolBarButton : Button
{
    #region Reference
    /// <summary>
    /// Identifies the <see cref="Reference"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="Reference"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty ReferenceProperty = DependencyProperty.Register(nameof(Reference), typeof(CommandResourceReference), typeof(ExtendedToolBarButton), new FrameworkPropertyMetadata(null));

    /// <summary>
    /// Gets or sets The reference to an assembly where to find resources associated to the button.
    /// Used when the command does not have enough information to locate these resources. For instance, if the command is one of the ApplicationCommands.
    /// Can be null otherwise.
    /// </summary>
    public CommandResourceReference Reference
    {
        get => (CommandResourceReference)GetValue(ReferenceProperty);
        set => SetValue(ReferenceProperty, value);
    }
    #endregion

    #region Is Checkable
    /// <summary>
    /// Identifies the <see cref="IsCheckable"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="IsCheckable"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty IsCheckableProperty = DependencyProperty.Register(nameof(IsCheckable), typeof(bool), typeof(ExtendedToolBarButton), new FrameworkPropertyMetadata(true, OnIsCheckableChanged));

    /// <summary>
    /// Gets or sets a value indicating whether the user can check the button as visible.
    /// True, the user can check the button as visible or uncheck it as hidden in the tool bar.
    /// False, the button is always visible.
    /// </summary>
    public bool IsCheckable
    {
        get => (bool)GetValue(IsCheckableProperty);
        set => SetValue(IsCheckableProperty, value);
    }

    /// <summary>
    /// Called when the <see cref="IsCheckable"/> dependency property is changed on <paramref name="modifiedObject"/>.
    /// </summary>
    /// <param name="modifiedObject">The object that had its property modified.</param>
    /// <param name="e">Information about the change.</param>
    private static void OnIsCheckableChanged(DependencyObject modifiedObject, DependencyPropertyChangedEventArgs e)
    {
        ExtendedToolBarButton ctrl = (ExtendedToolBarButton)modifiedObject;
        ctrl.OnIsCheckableChanged();
    }

    /// <summary>
    /// Called when the <see cref="IsCheckable"/> dependency property is changed.
    /// </summary>
    private void OnIsCheckableChanged() => UpdateVisibility();
    #endregion

    #region Is Default Active
    /// <summary>
    /// Identifies the <see cref="IsDefaultActive"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="IsDefaultActive"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty IsDefaultActiveProperty = DependencyProperty.Register(nameof(IsDefaultActive), typeof(bool), typeof(ExtendedToolBarButton), new FrameworkPropertyMetadata(true, OnIsDefaultActiveChanged));

    /// <summary>
    /// Gets or sets a value indicating whether the default setting  is True.
    /// True, the default setting for the <see cref="IsActive"/> property is True.
    /// False, the default setting for the <see cref="IsActive"/> property is False.
    /// </summary>
    public bool IsDefaultActive
    {
        get => (bool)GetValue(IsDefaultActiveProperty);
        set => SetValue(IsDefaultActiveProperty, value);
    }

    /// <summary>
    /// Called when the <see cref="IsDefaultActive"/> dependency property is changed on <paramref name="modifiedObject"/>.
    /// </summary>
    /// <param name="modifiedObject">The object that had its property modified.</param>
    /// <param name="e">Information about the change.</param>
    private static void OnIsDefaultActiveChanged(DependencyObject modifiedObject, DependencyPropertyChangedEventArgs e)
    {
        ExtendedToolBarButton ctrl = (ExtendedToolBarButton)modifiedObject;
        ctrl.OnIsDefaultActiveChanged();
    }

    /// <summary>
    /// Called when the <see cref="IsDefaultActive"/> dependency property is changed.
    /// </summary>
    private void OnIsDefaultActiveChanged() => InitializeIsActive();
    #endregion

    #region Is Active
    /// <summary>
    /// Identifies the <see cref="IsActive"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="IsActive"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(nameof(IsActive), typeof(bool), typeof(ExtendedToolBarButton), new FrameworkPropertyMetadata(true, OnIsActiveChanged));

    /// <summary>
    /// Gets or sets a value indicating whether the the button is visible.
    /// True, the button is visible in the tool bar.
    /// False, the button is hidden.
    /// </summary>
    public bool IsActive
    {
        get => (bool)GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    /// <summary>
    /// Called when the <see cref="IsActive"/> dependency property is changed on <paramref name="modifiedObject"/>.
    /// </summary>
    /// <param name="modifiedObject">The object that had its property modified.</param>
    /// <param name="e">Information about the change.</param>
    private static void OnIsActiveChanged(DependencyObject modifiedObject, DependencyPropertyChangedEventArgs e)
    {
        ExtendedToolBarButton ctrl = (ExtendedToolBarButton)modifiedObject;
        ctrl.OnIsActiveChanged();
    }

    /// <summary>
    /// Called when the <see cref="IsActive"/> dependency property is changed.
    /// </summary>
    private void OnIsActiveChanged()
    {
        UpdateVisibility();
        NotifyIsActiveChanged();
    }
    #endregion

    #region Is Active Changed
    /// <summary>
    /// Identifies the <see cref="IsActiveChanged"/> routed event.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="IsActiveChanged"/> routed event.
    /// </returns>
    public static readonly RoutedEvent IsActiveChangedEvent = EventManager.RegisterRoutedEvent(nameof(IsActiveChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ExtendedToolBarButton));

    /// <summary>
    /// Sent when the <see cref="IsActive"/> has changed.
    /// </summary>
    public event RoutedEventHandler IsActiveChanged
    {
        add { AddHandler(IsActiveChangedEvent, value); }
        remove { RemoveHandler(IsActiveChangedEvent, value); }
    }

    /// <summary>
    /// Sends a <see cref="IsActiveChanged"/> event.
    /// </summary>
    protected virtual void NotifyIsActiveChanged() => RaiseEvent(new RoutedEventArgs(IsActiveChangedEvent));
    #endregion
}
