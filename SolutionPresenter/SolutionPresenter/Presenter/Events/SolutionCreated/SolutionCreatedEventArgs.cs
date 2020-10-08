namespace CustomControls
{
    using System.Windows;
    using System.Windows.Threading;

    /// <summary>
    /// Represents the event data for a solution created event.
    /// </summary>
    public class SolutionCreatedEventArgs : SolutionPresenterEventArgs<SolutionCreatedEventArgs>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionCreatedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The event that occured.</param>
        /// <param name="eventContext">The event context.</param>
        public SolutionCreatedEventArgs(RoutedEvent routedEvent, SolutionCreatedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        /// <summary>
        /// Notifies handlers that the operation is completed.
        /// </summary>
        /// <param name="dispatcher">The window dispatcher.</param>
        /// <param name="createdRootPath">The path to the created solution.</param>
        public virtual void NotifyCompletedAsync(Dispatcher dispatcher, IRootPath createdRootPath)
        {
            ISolutionCreatedCompletionArgs CompletionArgs = new SolutionCreatedCompletionArgs(createdRootPath);
            NotifyEventCompletedAsync(dispatcher, CompletionArgs);
        }
    }
}
