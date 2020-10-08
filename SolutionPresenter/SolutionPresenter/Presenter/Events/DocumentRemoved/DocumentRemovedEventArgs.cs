namespace CustomControls
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Threading;

    /// <summary>
    /// Represents the event data for document removed event.
    /// </summary>
    public class DocumentRemovedEventArgs : SolutionPresenterEventArgs<DocumentRemovedEventArgs>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentRemovedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The event that occured.</param>
        /// <param name="eventContext">The event context.</param>
        public DocumentRemovedEventArgs(RoutedEvent routedEvent, DocumentRemovedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        /// <summary>
        /// Gets the root path.
        /// </summary>
        public IRootPath RootPath { get { return ((DocumentRemovedEventContext)EventContext).RootPath; } }

        /// <summary>
        /// Gets the deleted tree.
        /// </summary>
        public IReadOnlyDictionary<ITreeNodePath, IPathConnection> DeletedTree { get { return ((DocumentRemovedEventContext)EventContext).DeletedTree; } }

        /// <summary>
        /// Gets the operation data.
        /// </summary>
        public object? ClientInfo { get { return ((DocumentRemovedEventContext)EventContext).ClientInfo; } }

        /// <summary>
        /// Notifies handlers that the operation is completed.
        /// </summary>
        /// <param name="dispatcher">The window dispatcher.</param>
        public virtual void NotifyCompletedAsync(Dispatcher dispatcher)
        {
            IDocumentRemovedCompletionArgs CompletionArgs = new DocumentRemovedCompletionArgs();
            NotifyEventCompletedAsync(dispatcher, CompletionArgs);
        }
    }
}
