namespace CustomControls
{
    using System.Windows;

    /// <summary>
    /// Contains state information and event data associated with the <see cref="SplitView.ZoomChanged"/> event.
    /// </summary>
    public class ZoomChangedEventArgs : RoutedEventArgs
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ZoomChangedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The event this argument is associated to.</param>
        /// <param name="viewContent">The control representing the view to which the new zoom applies.</param>
        /// <param name="zoom">The new zoom value.</param>
        /// <remarks>
        /// A value of <paramref name="zoom"/>=1.0 means no zoom (or 100%).
        /// </remarks>
        public ZoomChangedEventArgs(RoutedEvent routedEvent, FrameworkElement viewContent, double zoom)
            : base(routedEvent)
        {
            this.ViewContent = viewContent;
            this.Zoom = zoom;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the control representing the view to which the new zoom applies.
        /// </summary>
        /// <returns>
        /// The control representing the view to which the new zoom applies.
        /// </returns>
        public FrameworkElement ViewContent { get; private set; }

        /// <summary>
        /// Gets the new zoom value.
        /// </summary>
        /// <returns>
        /// The new zoom value.
        /// </returns>
        /// <remarks>
        /// A value of <see cref="Zoom"/>=1.0 means no zoom (or 100%).
        /// </remarks>
        public double Zoom { get; private set; }
        #endregion
    }
}
