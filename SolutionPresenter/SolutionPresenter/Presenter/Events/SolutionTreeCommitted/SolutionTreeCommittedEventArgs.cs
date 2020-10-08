namespace CustomControls
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Threading;

    /// <summary>
    /// Represents the event data for a solution tree committed event.
    /// </summary>
    public class SolutionTreeCommittedEventArgs : SolutionPresenterEventArgs<SolutionTreeCommittedEventArgs>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionTreeCommittedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The event that occured.</param>
        /// <param name="eventContext">The event context.</param>
        public SolutionTreeCommittedEventArgs(RoutedEvent routedEvent, SolutionTreeCommittedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        /// <summary>
        /// Gets the list of modified items.
        /// </summary>
        public IReadOnlyList<ITreeNodePath>? DirtyItemList { get { return (IReadOnlyList<ITreeNodePath>?)((SolutionTreeCommittedEventContext)EventContext).Info.DirtyItemList; } }

        /// <summary>
        /// Gets the list of modified properties.
        /// </summary>
        public IReadOnlyList<ITreeNodePath>? DirtyPropertiesList { get { return (IReadOnlyList<ITreeNodePath>?)((SolutionTreeCommittedEventContext)EventContext).Info.DirtyPropertiesList; } }

        /// <summary>
        /// Gets the list of modified documents.
        /// </summary>
        public IReadOnlyList<IDocument>? DirtyDocumentList { get { return (IReadOnlyList<IDocument>?)((SolutionTreeCommittedEventContext)EventContext).Info.DirtyDocumentList; } }

        /// <summary>
        /// Notifies handlers that the operation is completed.
        /// </summary>
        /// <param name="dispatcher">The window dispatcher.</param>
        public virtual void NotifyCompletedAsync(Dispatcher dispatcher)
        {
            ISolutionTreeCommittedCompletionArgs CompletionArgs = new SolutionTreeCommittedCompletionArgs();
            NotifyEventCompletedAsync(dispatcher, CompletionArgs);
        }
    }
}
