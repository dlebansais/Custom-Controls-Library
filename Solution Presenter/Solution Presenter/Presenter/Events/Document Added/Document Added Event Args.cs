namespace CustomControls
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Threading;

    /// <summary>
    /// Represents the event data for document added event.
    /// </summary>
    public class DocumentAddedEventArgs : SolutionPresenterEventArgs<DocumentAddedEventArgs>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentAddedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The event that occured.</param>
        /// <param name="eventContext">The event context.</param>
        public DocumentAddedEventArgs(RoutedEvent routedEvent, DocumentAddedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        /// <summary>
        /// Gets the destination folder path.
        /// </summary>
        public IFolderPath DestinationFolderPath { get { return ((DocumentAddedEventContext)EventContext).DestinationFolderPath; } }

        /// <summary>
        /// Gets the list of document paths.
        /// </summary>
        public IReadOnlyList<IDocumentPath> DocumentPathList { get { return (IReadOnlyList<IDocumentPath>)((DocumentAddedEventContext)EventContext).DocumentPathList; } }

        /// <summary>
        /// Gets the properties of the root object.
        /// </summary>
        public IRootProperties RootProperties { get { return ((DocumentAddedEventContext)EventContext).RootProperties; } }

        /// <summary>
        /// Notifies handlers that the operation is completed.
        /// </summary>
        /// <param name="dispatcher">The window dispatcher.</param>
        /// <param name="addedItemTable">The table of added items.</param>
        /// <param name="addedPropertiesTable">The table of added item properties.</param>
        public virtual void NotifyCompletedAsync(Dispatcher dispatcher, IReadOnlyDictionary<IDocumentPath, IItemPath> addedItemTable, IReadOnlyDictionary<IDocumentPath, IItemProperties> addedPropertiesTable)
        {
            IDocumentAddedCompletionArgs CompletionArgs = new DocumentAddedCompletionArgs(addedItemTable, addedPropertiesTable);
            NotifyEventCompletedAsync(dispatcher, CompletionArgs);
        }
    }
}
