using System.Windows;

namespace CustomControls
{
    /// <summary>
    ///     Contains state information and event data associated with the <see cref="SplitView.ViewLoaded"/> event.
    /// </summary>
    public class ViewLoadedEventArgs : RoutedEventArgs
    {
        #region Init
        /// <summary>
        ///     Initializes a new instance of the <see cref="ViewLoadedEventArgs"/> class.
        /// </summary>
        /// <parameters>
        /// <param name="routedEvent">The event this argument is associated to.</param>
        /// <param name="viewContent">The control representing the view that has been loaded.</param>
        /// </parameters>
        public ViewLoadedEventArgs(RoutedEvent routedEvent, FrameworkElement viewContent)
            : base(routedEvent)
        {
            this.ViewContent = viewContent;
        }
        #endregion

        #region Properties
        /// <summary>
        ///     Gets the control representing the view that has been loaded.
        /// </summary>
        /// <returns>
        ///     The control representing the view that has been loaded.
        /// </returns>
        public FrameworkElement ViewContent { get; private set; }
        #endregion
    }
}
