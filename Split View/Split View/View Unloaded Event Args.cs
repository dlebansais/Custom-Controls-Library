using System.Windows;

namespace CustomControls
{
    /// <summary>
    ///     Contains state information and event data associated with the <see cref="SplitView.ViewUnloaded"/> event.
    /// </summary>
    public class ViewUnloadedEventArgs : RoutedEventArgs
    {
        #region Init
        /// <summary>
        ///     Initializes a new instance of the <see cref="ViewUnloadedEventArgs"/> class.
        /// </summary>
        /// <parameters>
        /// <param name="routedEvent">The event this argument is associated to.</param>
        /// <param name="viewContent">The control representing the view that has been unloaded.</param>
        /// </parameters>
        public ViewUnloadedEventArgs(RoutedEvent routedEvent, FrameworkElement viewContent)
            : base(routedEvent)
        {
            this.ViewContent = viewContent;
        }
        #endregion

        #region Properties
        /// <summary>
        ///     Gets the control representing the view that has been unloaded.
        /// </summary>
        /// <returns>
        ///     The control representing the view that has been unloaded.
        /// </returns>
        public FrameworkElement ViewContent { get; private set; }
        #endregion
    }
}
