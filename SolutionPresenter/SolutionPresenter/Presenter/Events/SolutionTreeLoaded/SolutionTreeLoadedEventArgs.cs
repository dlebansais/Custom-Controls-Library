namespace CustomControls
{
    using System.Windows;

    /// <summary>
    /// Represents the event data for a solution tree loaded event.
    /// </summary>
    public class SolutionTreeLoadedEventArgs : SolutionPresenterEventArgs<SolutionTreeLoadedEventArgs>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionTreeLoadedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The event that occured.</param>
        /// <param name="eventContext">The event context.</param>
        public SolutionTreeLoadedEventArgs(RoutedEvent routedEvent, SolutionTreeLoadedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        /// <summary>
        /// Gets a value indicating whether the load has been canceled.
        /// </summary>
        public bool IsCanceled { get { return ((SolutionTreeLoadedEventContext)EventContext).IsCanceled; } }

        /// <summary>
        /// Notifies handlers that the operation is completed.
        /// </summary>
        public virtual void NotifyCompleted()
        {
            ISolutionTreeLoadedCompletionArgs CompletionArgs = new SolutionTreeLoadedCompletionArgs();
            NotifyEventCompleted(CompletionArgs);
        }
    }
}
