namespace CustomControls
{
    using System.Windows;

    /// <summary>
    /// Represents the event data for a solution selected event.
    /// </summary>
    public class SolutionSelectedEventArgs : SolutionPresenterEventArgs<SolutionSelectedEventArgs>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionSelectedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The event that occured.</param>
        /// <param name="eventContext">The event context.</param>
        public SolutionSelectedEventArgs(RoutedEvent routedEvent, SolutionSelectedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        /// <summary>
        /// Notifies handlers that the operation is completed.
        /// </summary>
        /// <param name="selectedRootPath">The path to the selected solution.</param>
        public virtual void NotifyCompleted(IRootPath selectedRootPath)
        {
            ISolutionSelectedCompletionArgs CompletionArgs = new SolutionSelectedCompletionArgs(selectedRootPath);
            NotifyEventCompleted(CompletionArgs);
        }
    }
}
