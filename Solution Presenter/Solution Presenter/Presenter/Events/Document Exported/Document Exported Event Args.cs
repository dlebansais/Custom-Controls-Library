namespace CustomControls
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Threading;

    /// <summary>
    /// Represents the event data for document exported event.
    /// </summary>
    public class DocumentExportedEventArgs : SolutionPresenterEventArgs<DocumentExportedEventArgs>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentExportedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The event that occured.</param>
        /// <param name="eventContext">The event context.</param>
        public DocumentExportedEventArgs(RoutedEvent routedEvent, DocumentExportedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        /// <summary>
        /// Gets the document operation.
        /// </summary>
        public DocumentOperation DocumentOperation { get { return ((DocumentExportedEventContext)EventContext).DocumentOperation; } }

        /// <summary>
        /// Gets the list of exported documents.
        /// </summary>
        public IReadOnlyCollection<IDocument> ExportedDocumentList { get { return (IReadOnlyCollection<IDocument>)((DocumentExportedEventContext)EventContext).ExportedDocumentList; } }

        /// <summary>
        /// Gets a value indicating whether the destination is a folder.
        /// </summary>
        public bool IsDestinationFolder { get { return ((DocumentExportedEventContext)EventContext).IsDestinationFolder; } }

        /// <summary>
        /// Gets the destination path.
        /// </summary>
        public string DestinationPath { get { return ((DocumentExportedEventContext)EventContext).DestinationPath; } }

        /// <summary>
        /// Notifies handlers that the operation is completed.
        /// </summary>
        /// <param name="dispatcher">The window dispatcher.</param>
        public virtual void NotifyCompletedAsync(Dispatcher dispatcher)
        {
            IDocumentExportedCompletionArgs CompletionArgs = new DocumentExportedCompletionArgs();
            NotifyEventCompletedAsync(dispatcher, CompletionArgs);
        }
    }
}
