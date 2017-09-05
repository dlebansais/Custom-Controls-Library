namespace CustomControls
{
    public class ImportedContentDescriptor
    {
        public ImportedContentDescriptor(object importedContent, IDocumentType documentType)
        {
            this.ImportedContent = importedContent;
            this.DocumentType = documentType;
        }

        public object ImportedContent { get; private set; }
        public IDocumentType DocumentType { get; private set; }
    }
}
