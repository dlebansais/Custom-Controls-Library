namespace CustomControls
{
    using System.Windows;

    /// <summary>
    /// Represents the event data for a root properties request event.
    /// </summary>
    public class RootPropertiesRequestedEventArgs : SolutionPresenterEventArgs<RootPropertiesRequestedEventArgs>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RootPropertiesRequestedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The event that occured.</param>
        /// <param name="eventContext">The event context.</param>
        public RootPropertiesRequestedEventArgs(RoutedEvent routedEvent, RootPropertiesRequestedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        /// <summary>
        /// Gets the properties of the root object.
        /// </summary>
        public IRootProperties Properties { get { return ((RootPropertiesRequestedEventContext)EventContext).Properties; } }

        /// <summary>
        /// Notifies handlers that the operation is completed.
        /// </summary>
        public virtual void NotifyCompleted()
        {
            IRootPropertiesRequestedCompletionArgs CompletionArgs = new RootPropertiesRequestedCompletionArgs();
            NotifyEventCompleted(CompletionArgs);
        }
    }
}
