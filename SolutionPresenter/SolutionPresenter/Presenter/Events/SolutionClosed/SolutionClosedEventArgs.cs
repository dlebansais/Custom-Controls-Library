namespace CustomControls
{
    using System.Windows;

    /// <summary>
    /// Represents the event data for a solution closed event.
    /// </summary>
    public class SolutionClosedEventArgs : SolutionPresenterEventArgs<SolutionClosedEventArgs>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionClosedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The event that occured.</param>
        /// <param name="eventContext">The event context.</param>
        public SolutionClosedEventArgs(RoutedEvent routedEvent, SolutionClosedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        /// <summary>
        /// Notifies handlers that the operation is completed.
        /// </summary>
        public virtual void NotifyCompleted()
        {
            ISolutionClosedCompletionArgs CompletionArgs = new SolutionClosedCompletionArgs();
            NotifyEventCompleted(CompletionArgs);
        }
    }
}
