namespace CustomControls
{
    using System.Windows;
    using System.Windows.Threading;

    /// <summary>
    /// Represents the event data for document created event.
    /// </summary>
    public class DocumentCreatedEventArgs : SolutionPresenterEventArgs<DocumentCreatedEventArgs>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentCreatedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The event that occured.</param>
        /// <param name="eventContext">The event context.</param>
        public DocumentCreatedEventArgs(RoutedEvent routedEvent, DocumentCreatedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        /// <summary>
        /// Gets the destination folder path.
        /// </summary>
        public IFolderPath DestinationFolderPath { get { return ((DocumentCreatedEventContext)EventContext).DestinationFolderPath; } }

        /// <summary>
        /// Gets the document type.
        /// </summary>
        public IDocumentType DocumentType { get { return ((DocumentCreatedEventContext)EventContext).DocumentType; } }

        /// <summary>
        /// Gets the properties of the root object.
        /// </summary>
        public IRootProperties RootProperties { get { return ((DocumentCreatedEventContext)EventContext).RootProperties; } }

        /// <summary>
        /// Notifies handlers that the operation is completed.
        /// </summary>
        /// <param name="dispatcher">The window dispatcher.</param>
        /// <param name="newItemPath">The path to the new item.</param>
        /// <param name="newItemProperties">The properties of the new item.</param>
        public virtual void NotifyCompletedAsync(Dispatcher dispatcher, IItemPath newItemPath, IItemProperties newItemProperties)
        {
            IDocumentCreatedCompletionArgs CompletionArgs = new DocumentCreatedCompletionArgs(newItemPath, newItemProperties);
            NotifyEventCompletedAsync(dispatcher, CompletionArgs);
        }
    }
}
