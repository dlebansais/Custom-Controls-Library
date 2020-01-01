namespace CustomControls
{
    using System.Collections.Generic;
    using System.Windows;

    /// <summary>
    /// Represents the event data for a document selected event.
    /// </summary>
    public class DocumentSelectedEventArgs : SolutionPresenterEventArgs<DocumentSelectedEventArgs>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentSelectedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The event that occured.</param>
        /// <param name="eventContext">The event context.</param>
        public DocumentSelectedEventArgs(RoutedEvent routedEvent, DocumentSelectedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        /// <summary>
        /// Notifies handlers that the operation is completed.
        /// </summary>
        /// <param name="documentPathList">The list of selected documents.</param>
        public virtual void NotifyCompleted(IList<IDocumentPath> documentPathList)
        {
            IDocumentSelectedCompletionArgs CompletionArgs = new DocumentSelectedCompletionArgs(documentPathList);
            NotifyEventCompleted(CompletionArgs);
        }
    }
}
