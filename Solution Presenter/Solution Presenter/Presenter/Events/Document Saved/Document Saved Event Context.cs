namespace CustomControls
{
    public class DocumentSavedEventContext
    {
        public DocumentSavedEventContext(DocumentOperation documentOperation, IDocument savedDocument, string fileName)
        {
            this.DocumentOperation = documentOperation;
            this.SavedDocument = savedDocument;
            this.FileName = fileName;
        }

        public DocumentOperation DocumentOperation { get; private set; }
        public IDocument SavedDocument { get; private set; }
        public string FileName { get; private set; }
    }
}
