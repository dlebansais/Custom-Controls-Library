namespace CustomControls
{
    using System.Collections.Generic;
    using System.Windows;

    /// <summary>
    /// Represents the event data for a solution deleted event.
    /// </summary>
    public class SolutionDeletedEventArgs : SolutionPresenterEventArgs<SolutionDeletedEventArgs>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionDeletedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The event that occured.</param>
        /// <param name="eventContext">The event context.</param>
        public SolutionDeletedEventArgs(RoutedEvent routedEvent, SolutionDeletedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        /// <summary>
        /// Gets the path to the deleted solution.
        /// </summary>
        public IRootPath DeletedRootPath { get { return ((SolutionDeletedEventContext)EventContext).DeletedRootPath; } }

        /// <summary>
        /// Gets the tree of deleted nodes.
        /// </summary>
        public IReadOnlyCollection<ITreeNodePath>? DeletedTree { get { return ((SolutionDeletedEventContext)EventContext).DeletedTree; } }

        /// <summary>
        /// Notifies handlers that the operation is completed.
        /// </summary>
        public virtual void NotifyCompleted()
        {
            ISolutionDeletedCompletionArgs CompletionArgs = new SolutionDeletedCompletionArgs();
            NotifyEventCompleted(CompletionArgs);
        }
    }
}
