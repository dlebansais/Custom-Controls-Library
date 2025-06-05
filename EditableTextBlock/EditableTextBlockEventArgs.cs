namespace CustomControls;

using System.Threading;
using System.Windows;

/// <summary>
/// Contains state information and event data associated with events from the <see cref="EditableTextBlock"/> control.
/// </summary>
public class EditableTextBlockEventArgs : RoutedEventArgs
{
    #region Init
    /// <summary>
    /// Initializes a new instance of the <see cref="EditableTextBlockEventArgs"/> class.
    /// </summary>
    /// <param name="routedEvent">The event this argument is associated to.</param>
    /// <param name="source">The control from which editing is left.</param>
    /// <param name="text">The current content of the control.</param>
    /// <param name="cancellation">A token to hold cancellation information.</param>
    internal EditableTextBlockEventArgs(RoutedEvent routedEvent, EditableTextBlock source, string text, CancellationTokenSource cancellation)
        : base(routedEvent, source)
    {
        Text = text;
        Cancellation = cancellation;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets a value that is the current content of the control.
    /// </summary>
    /// <returns>The current content of the control.</returns>
    public string Text { get; }

    /// <summary>
    /// Gets a token that indicates if the associated operation has been canceled by any handler.
    /// </summary>
    private readonly CancellationTokenSource Cancellation;
    #endregion

    #region Client Interface
    /// <summary>
    /// Allows a handler to cancel the operation notified by the event associated to this object.
    /// </summary>
    public virtual void Cancel() => Cancellation.Cancel();
    #endregion
}
