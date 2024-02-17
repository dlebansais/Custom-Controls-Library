namespace CustomControls;

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

/// <summary>
/// Represents an item in a tree view control.
/// </summary>
public partial class ExtendedTreeViewItemBase : ContentControl, INotifyPropertyChanged
{
    #region Selected
    /// <summary>
    /// Identifies the <see cref="Selected"/> routed event.
    /// </summary>
    public static readonly RoutedEvent SelectedEvent = Selector.SelectedEvent;

    /// <summary>
    /// Occurs after the item is selected.
    /// </summary>
    public event RoutedEventHandler Selected
    {
        add { AddHandler(SelectedEvent, value); }
        remove { RemoveHandler(SelectedEvent, value); }
    }

    /// <summary>
    /// Invokes handlers of the <see cref="Selected"/> event.
    /// </summary>
    protected virtual void NotifySelected()
    {
        RoutedEventArgs Args = new(SelectedEvent, this);
        RaiseEvent(Args);
    }
    #endregion

    #region Unselected
    /// <summary>
    /// Identifies the <see cref="Unselected"/> routed event.
    /// </summary>
    public static readonly RoutedEvent UnselectedEvent = Selector.UnselectedEvent;

    /// <summary>
    /// Occurs after the item is unselected.
    /// </summary>
    public event RoutedEventHandler Unselected
    {
        add { AddHandler(UnselectedEvent, value); }
        remove { RemoveHandler(UnselectedEvent, value); }
    }

    /// <summary>
    /// Invokes handlers of the <see cref="Unselected"/> event.
    /// </summary>
    protected virtual void NotifyUnselected()
    {
        RoutedEventArgs Args = new(UnselectedEvent, this);
        RaiseEvent(Args);
    }
    #endregion

    #region Expanded
    /// <summary>
    /// Identifies the <see cref="Expanded"/> routed event.
    /// </summary>
    public static readonly RoutedEvent ExpandedEvent = EventManager.RegisterRoutedEvent("Expanded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ExtendedTreeViewItemBase));

    /// <summary>
    /// Occurs after the item is expanded.
    /// </summary>
    public event RoutedEventHandler Expanded
    {
        add { AddHandler(ExpandedEvent, value); }
        remove { RemoveHandler(ExpandedEvent, value); }
    }

    /// <summary>
    /// Invokes handlers of the <see cref="Expanded"/> event.
    /// </summary>
    protected virtual void NotifyExpanded()
    {
        RoutedEventArgs Args = new(ExpandedEvent, this);
        RaiseEvent(Args);
    }
    #endregion

    #region Collapsed
    /// <summary>
    /// Identifies the <see cref="Collapsed"/> routed event.
    /// </summary>
    public static readonly RoutedEvent CollapsedEvent = EventManager.RegisterRoutedEvent("Collapsed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ExtendedTreeViewItemBase));

    /// <summary>
    /// Occurs after the item is collapsed.
    /// </summary>
    public event RoutedEventHandler Collapsed
    {
        add { AddHandler(CollapsedEvent, value); }
        remove { RemoveHandler(CollapsedEvent, value); }
    }

    /// <summary>
    /// Invokes handlers of the <see cref="Collapsed"/> event.
    /// </summary>
    protected virtual void NotifyCollapsed()
    {
        RoutedEventArgs Args = new(CollapsedEvent, this);
        RaiseEvent(Args);
    }
    #endregion
}
