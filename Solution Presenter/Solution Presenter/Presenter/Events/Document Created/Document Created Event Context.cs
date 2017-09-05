using System.Collections.Generic;

namespace CustomControls
{
    public class DocumentCreatedEventContext
    {
        public DocumentCreatedEventContext(IFolderPath destinationFolderPath, IDocumentType documentType, IRootProperties rootProperties)
        {
            this.DestinationFolderPath = destinationFolderPath;
            this.DocumentType = documentType;
            this.RootProperties = rootProperties;
        }

        public IFolderPath DestinationFolderPath { get; private set; }
        public IDocumentType DocumentType { get; private set; }
        public IRootProperties RootProperties { get; private set; }
    }
}
