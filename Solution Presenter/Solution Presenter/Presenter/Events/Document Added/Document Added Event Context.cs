namespace CustomControls
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a context for the <see cref="DocumentAddedEventArgs"/> event data.
    /// </summary>
    public class DocumentAddedEventContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentAddedEventContext"/> class.
        /// </summary>
        /// <param name="documentOperation">The operation.</param>
        /// <param name="destinationFolderPath">The destination folder path.</param>
        /// <param name="documentPathList">The list of documents added.</param>
        /// <param name="rootProperties">The properties of the root object.</param>
        public DocumentAddedEventContext(DocumentOperation documentOperation, IFolderPath destinationFolderPath, IList<IDocumentPath> documentPathList, IRootProperties rootProperties)
        {
            DocumentOperation = documentOperation;
            DestinationFolderPath = destinationFolderPath;
            DocumentPathList = documentPathList;
            RootProperties = rootProperties;
        }

        /// <summary>
        /// Gets the operation.
        /// </summary>
        public DocumentOperation DocumentOperation { get; private set; }

        /// <summary>
        /// Gets the destination folder path.
        /// </summary>
        public IFolderPath DestinationFolderPath { get; private set; }

        /// <summary>
        /// Gets the list of documents added.
        /// </summary>
        public IList<IDocumentPath> DocumentPathList { get; private set; }

        /// <summary>
        /// Gets the properties of the root object.
        /// </summary>
        public IRootProperties RootProperties { get; private set; }
    }
}
