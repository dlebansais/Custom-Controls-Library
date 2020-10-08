namespace CustomControls
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the event data for an import new item request completion event.
    /// </summary>
    internal interface IImportNewItemsRequestedCompletionArgs
    {
        /// <summary>
        /// Gets the table of opened documents.
        /// </summary>
        IReadOnlyDictionary<object, IDocumentPath> OpenedDocumentTable { get; }
    }

    /// <summary>
    /// Represents the event data for an import new item request completion event.
    /// </summary>
    internal class ImportNewItemsRequestedCompletionArgs : IImportNewItemsRequestedCompletionArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportNewItemsRequestedCompletionArgs"/> class.
        /// </summary>
        public ImportNewItemsRequestedCompletionArgs()
        {
            this.OpenedDocumentTable = new Dictionary<object, IDocumentPath>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportNewItemsRequestedCompletionArgs"/> class.
        /// </summary>
        /// <param name="openedDocumentTable">The table of opened documents.</param>
        public ImportNewItemsRequestedCompletionArgs(IReadOnlyDictionary<object, IDocumentPath> openedDocumentTable)
        {
            OpenedDocumentTable = openedDocumentTable;
        }

        /// <summary>
        /// Gets the table of opened documents.
        /// </summary>
        public IReadOnlyDictionary<object, IDocumentPath> OpenedDocumentTable { get; private set; }
    }
}
