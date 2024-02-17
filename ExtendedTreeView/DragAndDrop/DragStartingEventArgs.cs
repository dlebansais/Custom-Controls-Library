namespace CustomControls;

using System.Windows;

/// <summary>
/// Represents the data of a drag starting event.
/// </summary>
public class DragStartingEventArgs : DragDropEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DragStartingEventArgs"/> class.
    /// </summary>
    /// <param name="routedEvent">The event that occured.</param>
    /// <param name="dragSource">The drag source.</param>
    /// <param name="cancellation">The cancellation token.</param>
    internal DragStartingEventArgs(RoutedEvent routedEvent, IDragSourceControl dragSource, CancellationToken cancellation)
        : base(routedEvent, dragSource)
    {
        Cancellation = cancellation;
    }

    /// <summary>
    /// Gets the cancellation token.
    /// </summary>
    protected CancellationToken Cancellation { get; private set; }

    /// <summary>
    /// Cancels the operation.
    /// </summary>
    public void Cancel()
    {
        Cancellation.Cancel();
    }
}
