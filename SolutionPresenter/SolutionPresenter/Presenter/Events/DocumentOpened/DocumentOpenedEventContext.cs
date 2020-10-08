namespace CustomControls
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a context for the <see cref="DocumentOpenedEventArgs"/> event data.
    /// </summary>
    public class DocumentOpenedEventContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentOpenedEventContext"/> class.
        /// </summary>
        /// <param name="documentOperation">The document operation.</param>
        /// <param name="destinationFolderPath">The destination folder path.</param>
        /// <param name="openedDocumentPathList">The list of opened documents.</param>
        /// <param name="documentPathList">The list of documents to open.</param>
        /// <param name="errorLocation">The location of the error, if any.</param>
        public DocumentOpenedEventContext(DocumentOperation documentOperation, IFolderPath destinationFolderPath, IList<IDocumentPath> openedDocumentPathList, IList<IDocumentPath> documentPathList, object? errorLocation)
        {
            DocumentOperation = documentOperation;
            DestinationFolderPath = destinationFolderPath;
            OpenedDocumentPathList = openedDocumentPathList;
            DocumentPathList = documentPathList;
            ErrorLocation = errorLocation;
        }

        /// <summary>
        /// Gets the document operation.
        /// </summary>
        public DocumentOperation DocumentOperation { get; }

        /// <summary>
        /// Gets the destination folder path.
        /// </summary>
        public IFolderPath DestinationFolderPath { get; }

        /// <summary>
        /// Gets the list of opened documents.
        /// </summary>
        public IList<IDocumentPath> OpenedDocumentPathList { get; }

        /// <summary>
        /// Gets the list of documents to open.
        /// </summary>
        public IList<IDocumentPath> DocumentPathList { get; }

        /// <summary>
        /// Gets the location of the error.
        /// </summary>
        public object? ErrorLocation { get; }
    }
}
