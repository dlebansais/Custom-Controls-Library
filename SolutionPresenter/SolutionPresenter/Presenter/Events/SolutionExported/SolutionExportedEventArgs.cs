namespace CustomControls
{
    using System.Collections.Generic;
    using System.Windows;

    /// <summary>
    /// Represents the event data for a solution exported event.
    /// </summary>
    public class SolutionExportedEventArgs : SolutionPresenterEventArgs<SolutionExportedEventArgs>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionExportedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The event that occured.</param>
        /// <param name="eventContext">The event context.</param>
        public SolutionExportedEventArgs(RoutedEvent routedEvent, SolutionExportedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        /// <summary>
        /// Gets the path to the exported solution.
        /// </summary>
        public IRootPath ExportedRootPath { get { return ((SolutionExportedEventContext)EventContext).ExportedRootPath; } }

        /// <summary>
        /// Notifies handlers that the operation is completed.
        /// </summary>
        /// <param name="contentTable">The table of solution data..</param>
        public virtual void NotifyCompleted(Dictionary<IDocumentPath, byte[]> contentTable)
        {
            ISolutionExportedCompletionArgs CompletionArgs = new SolutionExportedCompletionArgs(contentTable);
            NotifyEventCompleted(CompletionArgs);
        }
    }
}
