namespace CustomControls;

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

/// <summary>
/// Represents a control with a tree of nodes that can be moved around with Drag and Drop.
/// </summary>
public abstract partial class ExtendedTreeViewBase : MultiSelector
{
    /// <summary>
    /// Gets a value indicating whether traces are enabled.
    /// </summary>
    public static bool EnableTraces { get => false; }

    /// <summary>
    /// Gets the container associated to an item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The container.</returns>
    protected virtual ExtendedTreeViewItemBase? ContainerFromItem(object item)
    {
        return ItemContainerGenerator.ContainerFromItem(item) as ExtendedTreeViewItemBase;
    }

    /// <summary>
    /// Gets the container associated to the item at the provided position.
    /// </summary>
    /// <param name="index">the item position.</param>
    /// <returns>The container.</returns>
    protected virtual ExtendedTreeViewItemBase? ContainerFromIndex(int index)
    {
        return ItemContainerGenerator.ContainerFromIndex(index) as ExtendedTreeViewItemBase;
    }

    /// <summary>
    /// Checks whether an item can be cloned.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>True if cloneable; otherwise, false.</returns>
    protected virtual bool IsItemCloneable(object item)
    {
        return item is ICloneable;
    }

    /// <summary>
    /// Gets the indentation level of an item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The indentation level.</returns>
    public int ItemLevel(object item)
    {
        int Level = -1;

        object? CurrentItem = item;
        while (CurrentItem is not null)
        {
            Level++;
            CurrentItem = GetItemParent(CurrentItem);
        }

        return Level;
    }

    /// <summary>
    /// Gets the source of an event.
    /// </summary>
    /// <param name="args">The event data.</param>
    /// <returns>The event source.</returns>
    protected virtual ExtendedTreeViewItemBase? GetEventSourceItem(RoutedEventArgs args)
    {
        DependencyObject? Current = args?.OriginalSource as DependencyObject;

        while (Current is not null)
        {
            if (Current is ExtendedTreeViewItemBase AsContainerItem)
                return AsContainerItem;

            if (Current is ToggleButton)
                return null;

            Current = VisualTreeHelper.GetParent(Current);
        }

        return null;
    }

    /// <summary>
    /// Checks whether the CTRL key is down.
    /// </summary>
    /// <returns>True if down; otherwise, false.</returns>
    protected virtual bool IsCtrlDown()
    {
        return (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && !(Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt));
    }

    /// <summary>
    /// Checks whether the SHIFT key is down.
    /// </summary>
    /// <returns>True if down; otherwise, false.</returns>
    protected virtual bool IsShiftDown()
    {
        return Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
    }

    /// <summary>
    /// Sorts two items by index.
    /// </summary>
    /// <param name="item1">The first item.</param>
    /// <param name="item2">The second item.</param>
    /// <returns>The sort result.</returns>
    protected virtual int SortByIndex(object item1, object item2)
    {
        return VisibleChildren.IndexOf(item1) - VisibleChildren.IndexOf(item2);
    }
}
