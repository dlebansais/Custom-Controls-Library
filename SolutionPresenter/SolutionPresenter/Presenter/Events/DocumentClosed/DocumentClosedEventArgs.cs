namespace CustomControls
{
    using System.Collections.Generic;
    using System.Windows;

    /// <summary>
    /// Represents the event data for document closed event.
    /// </summary>
    public class DocumentClosedEventArgs : SolutionPresenterEventArgs<DocumentClosedEventArgs>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentClosedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The event that occured.</param>
        /// <param name="eventContext">The event context.</param>
        public DocumentClosedEventArgs(RoutedEvent routedEvent, DocumentClosedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        /// <summary>
        /// Gets the operation.
        /// </summary>
        public DocumentOperation DocumentOperation { get { return ((DocumentClosedEventContext)EventContext).DocumentOperation; } }

        /// <summary>
        /// Gets the list of closed documents.
        /// </summary>
        public IReadOnlyList<IDocument> ClosedDocumentList { get { return (IReadOnlyList<IDocument>)((DocumentClosedEventContext)EventContext).ClosedDocumentList; } }

        /// <summary>
        /// Gets the operation information.
        /// </summary>
        public object? ClientInfo { get { return ((DocumentClosedEventContext)EventContext).ClientInfo; } }

        /// <summary>
        /// Notifies handlers that the operation is completed.
        /// </summary>
        public virtual void NotifyCompleted()
        {
            IDocumentClosedCompletionArgs CompletionArgs = new DocumentClosedCompletionArgs();
            NotifyEventCompleted(CompletionArgs);
        }
    }
}
