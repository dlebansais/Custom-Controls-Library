namespace CustomControls
{
    using System.Windows;
    using System.Windows.Threading;

    /// <summary>
    /// Represents the event data for a document saved event.
    /// </summary>
    public class DocumentSavedEventArgs : SolutionPresenterEventArgs<DocumentSavedEventArgs>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentSavedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The event that occured.</param>
        /// <param name="eventContext">The event context.</param>
        public DocumentSavedEventArgs(RoutedEvent routedEvent, DocumentSavedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        /// <summary>
        /// Gets the document operation.
        /// </summary>
        public DocumentOperation DocumentOperation { get { return ((DocumentSavedEventContext)EventContext).DocumentOperation; } }

        /// <summary>
        /// Gets the saved document.
        /// </summary>
        public IDocument SavedDocument { get { return ((DocumentSavedEventContext)EventContext).SavedDocument; } }

        /// <summary>
        /// Notifies handlers that the operation is completed.
        /// </summary>
        /// <param name="dispatcher">The window dispatcher.</param>
        public virtual void NotifyCompletedAsync(Dispatcher dispatcher)
        {
            IDocumentSavedCompletionArgs CompletionArgs = new DocumentSavedCompletionArgs();
            NotifyEventCompletedAsync(dispatcher, CompletionArgs);
        }
    }
}
