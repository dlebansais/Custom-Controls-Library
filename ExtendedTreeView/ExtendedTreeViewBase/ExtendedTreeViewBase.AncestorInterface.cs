namespace CustomControls;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Contracts;

/// <summary>
/// Represents a control with a tree of nodes that can be moved around with Drag and Drop.
/// </summary>
public abstract partial class ExtendedTreeViewBase : MultiSelector
{
    /// <summary>
    /// Overrides inherited metadata.
    /// </summary>
    protected static void OverrideAncestorMetadata()
    {
        OverrideMetadataItemsSource();
        OverrideMetadataDefaultStyleKey();
    }

    /// <summary>
    /// Overrides inherited metadata for the <see cref="ItemsControl.ItemsSource"/> property.
    /// </summary>
    protected static void OverrideMetadataItemsSource() => ItemsSourceProperty.OverrideMetadata(typeof(ExtendedTreeViewBase), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.NotDataBindable, null, new CoerceValueCallback(CoerceItemsSource), true));

    /// <summary>
    /// Overrides inherited metadata for the <see cref="FrameworkElement.DefaultStyleKey"/> property.
    /// </summary>
    protected static void OverrideMetadataDefaultStyleKey() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtendedTreeViewBase), new FrameworkPropertyMetadata(typeof(ExtendedTreeViewBase)));

    /// <summary>
    /// Ensures the <see cref="ItemsControl.ItemsSource"/> property contains a valid value.
    /// </summary>
    /// <param name="control">The object with the modified property.</param>
    /// <param name="value">The value to check.</param>
    /// <returns>True if valid; Otherwise, false.</returns>
    [Access("protected", "static")]
    [RequireNotNull(nameof(control), Type = "DependencyObject", Name = "d")]
    private static object CoerceItemsSourceVerified(ExtendedTreeViewBase control, object value) => control.CoerceItemsSource(value);

    /// <summary>
    /// Ensures the <see cref="ItemsControl.ItemsSource"/> property contains a valid value.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>True if valid; Otherwise, false.</returns>
    protected virtual object CoerceItemsSource(object value) => VisibleChildren;

    private void InitAncestor() => CanSelectMultipleItems = true;

    /// <inheritdoc />
    protected override DependencyObject GetContainerForItemOverride() => CreateContainerItem();

    /// <inheritdoc />
    protected override bool IsItemItsOwnContainerOverride(object item) => item is ExtendedTreeViewItemBase;

    /// <summary>
    /// Creates a container for an item.
    /// </summary>
    /// <returns>The created container.</returns>
    protected virtual ExtendedTreeViewItemBase CreateContainerItem() => new(this);

    /// <inheritdoc />
    protected override void OnSelectionChanged(SelectionChangedEventArgs e)
    {
        base.OnSelectionChanged(e);

        UpdateIsDragDropPossible();
    }
}
