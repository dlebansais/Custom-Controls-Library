namespace CustomControls;

using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Contracts;

/// <summary>
/// Represents a control with a tree of nodes that can be moved around with Drag and Drop.
/// </summary>
public abstract partial class ExtendedTreeViewBase : MultiSelector
{
    private void InitializeContextMenu()
    {
        CurrentlyFocusedContainer = null;
    }

    /// <summary>
    /// Calleds when a container looses the focus.
    /// </summary>
    public void ContainerLostFocus()
    {
        RemoveKeyboardFocusWithinHandler();
    }

    /// <summary>
    /// Calleds when a container gets the focus.
    /// </summary>
    /// <param name="container">The container.</param>
    public void ContainerGotFocus(ExtendedTreeViewItemBase container)
    {
        AddKeyboardFocusWithinHandler(container);
    }

    /// <summary>
    /// Adds a keyboard focus handler for a container.
    /// </summary>
    /// <param name="container">The container.</param>
    protected virtual void AddKeyboardFocusWithinHandler(ExtendedTreeViewItemBase container)
    {
        RemoveKeyboardFocusWithinHandler();

        CurrentlyFocusedContainer = container ?? throw new ArgumentNullException(nameof(container));
        CurrentlyFocusedContainer.IsKeyboardFocusWithinChanged += OnIsKeyboardFocusWithinChanged;
    }

    /// <summary>
    /// Removes a keyboard focus handler from a container.
    /// </summary>
    protected virtual void RemoveKeyboardFocusWithinHandler()
    {
        if (CurrentlyFocusedContainer is not null)
        {
            CurrentlyFocusedContainer.IsKeyboardFocusWithinChanged -= OnIsKeyboardFocusWithinChanged;
            CurrentlyFocusedContainer = null;
        }
    }

    /// <summary>
    /// Calleds when the focus changed in a container.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    protected virtual void OnIsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (Keyboard.FocusedElement is ContextMenu AsContextMenu)
        {
            ClearAllContainerTags();

#if NETCOREAPP3_1
            foreach (object? Item in SelectedItems)
                if (Item is not null)
                {
                    ExtendedTreeViewItemBase? Container = ContainerFromItem(Item);
                    if (Container is not null)
                        TagContainer(Container);
                }
#else
            foreach (object Item in SelectedItems)
            {
                ExtendedTreeViewItemBase? Container = ContainerFromItem(Item);
                if (Container is not null)
                    TagContainer(Container);
            }
#endif

            AsContextMenu.Closed += OnContextMenuClosed;
        }
    }

    /// <summary>
    /// Called when a context menu is closed.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    protected virtual void OnContextMenuClosed(object sender, RoutedEventArgs e)
    {
        if (sender is ContextMenu AsContextMenu)
        {
            AsContextMenu.Closed -= OnContextMenuClosed;
            ClearAllContainerTags();
        }
    }

    /// <summary>
    /// Adds a tag to a container.
    /// </summary>
    /// <param name="container">The container.</param>
    [Access("protected")]
    [RequireNotNull(nameof(container))]
    private void TagContainerVerified(ExtendedTreeViewItemBase container)
    {
        container.SetValue(HasContextMenuOpenPropertyKey, true);
        MarkedContainerList.Add(container);
    }

    /// <summary>
    /// Removes all tags in containers.
    /// </summary>
    protected virtual void ClearAllContainerTags()
    {
        foreach (ExtendedTreeViewItemBase Container in MarkedContainerList)
            Container.SetValue(HasContextMenuOpenPropertyKey, false);

        MarkedContainerList.Clear();
    }

    /// <summary>
    /// Gets the currently focused container.
    /// </summary>
    protected ExtendedTreeViewItemBase? CurrentlyFocusedContainer { get; private set; }

    /// <summary>
    /// Gets the list of tagged containers.
    /// </summary>
    protected Collection<ExtendedTreeViewItemBase> MarkedContainerList { get; } = new Collection<ExtendedTreeViewItemBase>();
}
