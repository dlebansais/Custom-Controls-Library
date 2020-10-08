namespace CustomControls
{
    using System.Collections.Generic;
    using System.Windows;

    /// <summary>
    /// Represents the event data for a document opened event.
    /// </summary>
    public class DocumentOpenedEventArgs : SolutionPresenterEventArgs<DocumentOpenedEventArgs>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentOpenedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The event that occured.</param>
        /// <param name="eventContext">The event context.</param>
        public DocumentOpenedEventArgs(RoutedEvent routedEvent, DocumentOpenedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        /// <summary>
        /// Gets the list of opened documents.
        /// </summary>
        public IReadOnlyList<IDocumentPath> OpenedDocumentPathList { get { return (IReadOnlyList<IDocumentPath>)((DocumentOpenedEventContext)EventContext).OpenedDocumentPathList; } }

        /// <summary>
        /// Notifies handlers that a document is opened.
        /// </summary>
        /// <param name="openedDocument">The document.</param>
        public virtual void NotifyDocumentOpened(IDocument openedDocument)
        {
            IDocumentOpenedCompletionArgs CompletionArgs = new DocumentOpenedCompletionArgs(openedDocument);
            NotifyEventCompleted(CompletionArgs);
        }

        /// <summary>
        /// Notifies handlers that the operation is completed.
        /// </summary>
        /// <param name="openedDocumentList">The list of opened document.</param>
        public virtual void NotifyCompleted(IReadOnlyList<IDocument> openedDocumentList)
        {
            IDocumentOpenedCompletionArgs CompletionArgs = new DocumentOpenedCompletionArgs(openedDocumentList);
            NotifyEventCompleted(CompletionArgs);
        }
    }
}
