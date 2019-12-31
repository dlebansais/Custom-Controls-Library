namespace CustomControls
{
    using System.Collections.Generic;

    public class DocumentExportedEventContext
    {
        public DocumentExportedEventContext(DocumentOperation documentOperation, ICollection<IDocument> exportedDocumentList, bool isDestinationFolder, string destinationPath)
        {
            this.DocumentOperation = documentOperation;
            this.ExportedDocumentList = exportedDocumentList;
            this.IsDestinationFolder = isDestinationFolder;
            this.DestinationPath = destinationPath;
        }

        public DocumentOperation DocumentOperation { get; private set; }
        public ICollection<IDocument> ExportedDocumentList { get; private set; }
        public bool IsDestinationFolder { get; private set; }
        public string DestinationPath { get; private set; }
    }
}
