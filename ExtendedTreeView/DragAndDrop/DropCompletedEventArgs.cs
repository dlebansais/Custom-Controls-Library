namespace CustomControls;

using System.Collections;
using System.Windows;

/// <summary>
/// Represents the data of a drop completed event.
/// </summary>
public class DropCompletedEventArgs : DragDropEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DropCompletedEventArgs"/> class.
    /// If the event is a preview, <see cref="CloneList"/> is always empty.
    /// </summary>
    /// <param name="routedEvent">The event that occured.</param>
    /// <param name="dragSource">The drag source.</param>
    /// <param name="dropDestinationItem">The destination of the drop.</param>
    /// <param name="effect">The drop effect.</param>
    /// <param name="itemList">The list of moved items.</param>
    /// <param name="cloneList">The list of cloned items.</param>
    internal DropCompletedEventArgs(RoutedEvent routedEvent, IDragSourceControl dragSource, object dropDestinationItem, DragDropEffects effect, IList itemList, IList cloneList)
        : base(routedEvent, dragSource)
    {
        DropDestinationItem = dropDestinationItem;
        Effect = effect;
        ItemList = itemList;
        CloneList = cloneList;
    }

    /// <summary>
    /// Gets the destination of the drop.
    /// </summary>
    public object DropDestinationItem { get; }

    /// <summary>
    /// Gets drop effect.
    /// </summary>
    public DragDropEffects Effect { get; }

    /// <summary>
    /// Gets the list of moved items.
    /// </summary>
    public IList ItemList { get; }

    /// <summary>
    /// Gets the list of cloned items.
    /// </summary>
    public IList CloneList { get; }
}
