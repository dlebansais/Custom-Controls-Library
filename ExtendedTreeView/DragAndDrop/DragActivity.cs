namespace CustomControls;

/// <summary>
/// States of a drag drop operation.
/// </summary>
public enum DragActivity
{
    /// <summary>
    /// No operation.
    /// </summary>
    Idle,

    /// <summary>
    /// The drag should start shortly.
    /// </summary>
    Scheduled,

    /// <summary>
    /// Drag is starting.
    /// </summary>
    Starting,

    /// <summary>
    /// Drag started.
    /// </summary>
    Started,

    /// <summary>
    /// The operation has been canceled.
    /// </summary>
    Canceled,
}
