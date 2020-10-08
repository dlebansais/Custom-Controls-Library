namespace CustomControls
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a context for the <see cref="DocumentExportedEventArgs"/> event data.
    /// </summary>
    public class DocumentExportedEventContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentExportedEventContext"/> class.
        /// </summary>
        /// <param name="documentOperation">The document operation.</param>
        /// <param name="exportedDocumentList">The list of exported documents.</param>
        /// <param name="isDestinationFolder">True if the destination is a folder.</param>
        /// <param name="destinationPath">The destination path.</param>
        public DocumentExportedEventContext(DocumentOperation documentOperation, ICollection<IDocument> exportedDocumentList, bool isDestinationFolder, string destinationPath)
        {
            DocumentOperation = documentOperation;
            ExportedDocumentList = exportedDocumentList;
            IsDestinationFolder = isDestinationFolder;
            DestinationPath = destinationPath;
        }

        /// <summary>
        /// Gets the document operation.
        /// </summary>
        public DocumentOperation DocumentOperation { get; private set; }

        /// <summary>
        /// Gets the list of exported documents.
        /// </summary>
        public ICollection<IDocument> ExportedDocumentList { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the destination is a folder.
        /// </summary>
        public bool IsDestinationFolder { get; private set; }

        /// <summary>
        /// Gets the destination path.
        /// </summary>
        public string DestinationPath { get; private set; }
    }
}
