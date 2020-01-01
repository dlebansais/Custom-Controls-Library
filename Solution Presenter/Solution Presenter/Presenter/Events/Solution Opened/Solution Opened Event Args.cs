namespace CustomControls
{
    using System.Collections.Generic;
    using System.Windows;

    /// <summary>
    /// Represents the event data for a solution opened event.
    /// </summary>
    public class SolutionOpenedEventArgs : SolutionPresenterEventArgs<SolutionOpenedEventArgs>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionOpenedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The event that occured.</param>
        /// <param name="eventContext">The event context.</param>
        public SolutionOpenedEventArgs(RoutedEvent routedEvent, SolutionOpenedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        /// <summary>
        /// Gets the path to the opened solution.
        /// </summary>
        public IRootPath OpenedRootPath { get { return ((SolutionOpenedEventContext)EventContext).OpenedRootPath; } }

        /// <summary>
        /// Notifies handlers that the operation is completed.
        /// </summary>
        /// <param name="openedRootProperties">The properties of the opened solution.</param>
        /// <param name="openedRootComparer">The comparer of the opened solution.</param>
        /// <param name="expandedFolderList">The list of expanded folders.</param>
        /// <param name="context">The open context.</param>
        public virtual void NotifyCompleted(IRootProperties openedRootProperties, IComparer<ITreeNodePath> openedRootComparer, IList<IFolderPath> expandedFolderList, object context)
        {
            ISolutionOpenedCompletionArgs CompletionArgs = new SolutionOpenedCompletionArgs(openedRootProperties, openedRootComparer, expandedFolderList, context);
            NotifyEventCompleted(CompletionArgs);
        }
    }
}
