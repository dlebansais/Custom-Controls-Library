namespace CustomControls;

using System.Windows;

/// <summary>
/// Represents data for a drag drop event.
/// </summary>
public class DragDropEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DragDropEventArgs"/> class.
    /// </summary>
    /// <param name="routedEvent">The event that occured.</param>
    /// <param name="dragSource">The drag source.</param>
    internal DragDropEventArgs(RoutedEvent routedEvent, IDragSourceControl dragSource)
        : base(routedEvent)
    {
        DragSource = dragSource;
    }

    /// <summary>
    /// Gets a value indicating whether copy on drop is allowed.
    /// </summary>
    public virtual bool AllowDropCopy => DragSource.AllowDropCopy;

    private readonly IDragSourceControl DragSource;
}
