namespace CustomControls
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a context for the <see cref="ImportNewItemsRequestedEventArgs"/> event data.
    /// </summary>
    public class ImportNewItemsRequestedEventContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportNewItemsRequestedEventContext"/> class.
        /// </summary>
        /// <param name="importedDocumentTable">The table of imported documents.</param>
        /// <param name="documentPathList">The list of document paths.</param>
        public ImportNewItemsRequestedEventContext(Dictionary<object, IDocumentType> importedDocumentTable, IList<IDocumentPath> documentPathList)
        {
            ImportedDocumentTable = importedDocumentTable;
            DocumentPathList = documentPathList;
        }

        /// <summary>
        /// Gets the table of imported documents.
        /// </summary>
        public Dictionary<object, IDocumentType> ImportedDocumentTable { get; private set; }

        /// <summary>
        /// Gets the list of document paths.
        /// </summary>
        public IList<IDocumentPath> DocumentPathList { get; private set; }
    }
}
