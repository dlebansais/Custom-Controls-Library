namespace CustomControls;

/// <summary>
/// Contains cancellation information.
/// </summary>
public class CancellationToken
{
    #region Properties
    /// <summary>
    /// Gets a value indicating whether the associated operation has been canceled by any handler.
    /// </summary>
    /// <returns>True indicates that a handler has canceled the operation. False indicates that the operation associated to the event should be completed.</returns>
    public bool IsCanceled { get; private set; }
    #endregion

    #region Client Interface
    /// <summary>
    /// Allows a handler to cancel the operation notified by the event associated to this object.
    /// </summary>
    public virtual void Cancel() => IsCanceled = true;
    #endregion
}
