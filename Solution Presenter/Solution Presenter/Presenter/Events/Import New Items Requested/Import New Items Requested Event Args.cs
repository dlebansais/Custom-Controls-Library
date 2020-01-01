namespace CustomControls
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Threading;

    /// <summary>
    /// Represents the event data for a import new item request event.
    /// </summary>
    public class ImportNewItemsRequestedEventArgs : SolutionPresenterEventArgs<ImportNewItemsRequestedEventArgs>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportNewItemsRequestedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The event that occured.</param>
        /// <param name="eventContext">The event context.</param>
        public ImportNewItemsRequestedEventArgs(RoutedEvent routedEvent, ImportNewItemsRequestedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        /// <summary>
        /// Gets the table of imported items.
        /// </summary>
        public IReadOnlyDictionary<object, IDocumentType> ImportedDocumentTable { get { return (IReadOnlyDictionary<object, IDocumentType>)((ImportNewItemsRequestedEventContext)EventContext).ImportedDocumentTable; } }

        /// <summary>
        /// Notifies handlers that the operation is completed.
        /// </summary>
        /// <param name="dispatcher">The window dispatcher.</param>
        /// <param name="openedDocumentTable">The table of opened documents.</param>
        public virtual void NotifyCompletedAsync(Dispatcher dispatcher, IReadOnlyDictionary<object, IDocumentPath> openedDocumentTable)
        {
            IImportNewItemsRequestedCompletionArgs CompletionArgs = new ImportNewItemsRequestedCompletionArgs(openedDocumentTable);
            NotifyEventCompletedAsync(dispatcher, CompletionArgs);
        }
    }
}
