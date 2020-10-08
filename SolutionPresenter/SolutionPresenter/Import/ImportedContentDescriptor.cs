namespace CustomControls
{
    /// <summary>
    /// Represents the content of an imported object.
    /// </summary>
    public class ImportedContentDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportedContentDescriptor"/> class.
        /// </summary>
        /// <param name="importedContent">The content.</param>
        /// <param name="documentType">The associated document type.</param>
        public ImportedContentDescriptor(object importedContent, IDocumentType documentType)
        {
            ImportedContent = importedContent;
            DocumentType = documentType;
        }

        /// <summary>
        /// Gets The content.
        /// </summary>
        public object ImportedContent { get; private set; }

        /// <summary>
        /// Gets the associated document type.
        /// </summary>
        public IDocumentType DocumentType { get; private set; }
    }
}
