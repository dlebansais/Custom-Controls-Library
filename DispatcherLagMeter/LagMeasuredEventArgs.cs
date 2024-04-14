namespace CustomControls;

using System.Windows;

/// <summary>
/// Contains state information and event data associated with events from the <see cref="DispatcherLagMeter"/> control.
/// </summary>
public class LagMeasuredEventArgs : RoutedEventArgs
{
    #region Init
    /// <summary>
    /// Initializes a new instance of the <see cref="LagMeasuredEventArgs"/> class.
    /// </summary>
    /// <param name="routedEvent">The event this argument is associated to.</param>
    /// <param name="source">The source sending the event.</param>
    /// <param name="dispatcherLag">The measured lag.</param>
    internal LagMeasuredEventArgs(RoutedEvent routedEvent, DispatcherLagMeter source, DispatcherLag dispatcherLag)
        : base(routedEvent, source)
    {
        DispatcherLag = dispatcherLag;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets the last measured dispatcher lag.
    /// </summary>
    /// <returns>The current content of the control.</returns>
    public DispatcherLag DispatcherLag { get; }
    #endregion
}
