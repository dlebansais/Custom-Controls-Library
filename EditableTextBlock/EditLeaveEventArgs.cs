namespace CustomControls;

using System.Threading;
using System.Windows;

/// <summary>
/// Contains state information and event data associated with the EditLeave event.
/// </summary>
public class EditLeaveEventArgs : EditableTextBlockEventArgs
{
    #region Init
    /// <summary>
    /// Initializes a new instance of the <see cref="EditLeaveEventArgs"/> class.
    /// If IsEditCanceled is true, the IsCanceled member will be ignored by the sender.
    /// </summary>
    /// <param name="routedEvent">The event this argument is associated to.</param>
    /// <param name="source">The control from which editing is left.</param>
    /// <param name="text">The current content of the control.</param>
    /// <param name="cancellation">A token to hold cancellation information.</param>
    /// <param name="isEditCanceled">A value that indicates if editing has been canceled.</param>
    internal EditLeaveEventArgs(RoutedEvent routedEvent, EditableTextBlock source, string text, CancellationTokenSource cancellation, bool isEditCanceled)
        : base(routedEvent, source, text, cancellation)
    {
        IsEditCanceled = isEditCanceled;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets a value indicating whether editing has been canceled.
    /// </summary>
    /// <returns>True indicates that editing has been canceled. False indicates that editing has been completed successfully.</returns>
    public bool IsEditCanceled { get; }
    #endregion
}
