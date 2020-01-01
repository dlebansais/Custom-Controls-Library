namespace CustomControls
{
    /// <summary>
    /// Represents a context for the <see cref="DocumentSavedEventArgs"/> event data.
    /// </summary>
    public class DocumentSavedEventContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentSavedEventContext"/> class.
        /// </summary>
        /// <param name="documentOperation">The document operation.</param>
        /// <param name="savedDocument">The saved document.</param>
        /// <param name="fileName">The file name.</param>
        public DocumentSavedEventContext(DocumentOperation documentOperation, IDocument savedDocument, string fileName)
        {
            DocumentOperation = documentOperation;
            SavedDocument = savedDocument;
            FileName = fileName;
        }

        /// <summary>
        /// Gets the document operation.
        /// </summary>
        public DocumentOperation DocumentOperation { get; private set; }

        /// <summary>
        /// Gets the saved document.
        /// </summary>
        public IDocument SavedDocument { get; private set; }

        /// <summary>
        /// Gets the file name.
        /// </summary>
        public string FileName { get; private set; }
    }
}
