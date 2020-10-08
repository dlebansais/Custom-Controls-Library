namespace CustomControls
{
    /// <summary>
    /// Represents a context for the <see cref="DocumentCreatedEventArgs"/> event data.
    /// </summary>
    public class DocumentCreatedEventContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentCreatedEventContext"/> class.
        /// </summary>
        /// <param name="destinationFolderPath">The destination folder path.</param>
        /// <param name="documentType">The document type.</param>
        /// <param name="rootProperties">The properties of the root object.</param>
        public DocumentCreatedEventContext(IFolderPath destinationFolderPath, IDocumentType documentType, IRootProperties rootProperties)
        {
            this.DestinationFolderPath = destinationFolderPath;
            this.DocumentType = documentType;
            this.RootProperties = rootProperties;
        }

        /// <summary>
        /// Gets the destination folder path.
        /// </summary>
        public IFolderPath DestinationFolderPath { get; private set; }

        /// <summary>
        /// Gets the document type.
        /// </summary>
        public IDocumentType DocumentType { get; private set; }

        /// <summary>
        /// Gets properties of the root object.
        /// </summary>
        public IRootProperties RootProperties { get; private set; }
    }
}
