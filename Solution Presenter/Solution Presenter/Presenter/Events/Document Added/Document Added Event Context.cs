namespace CustomControls
{
    using System.Collections.Generic;

    public class DocumentAddedEventContext
    {
        public DocumentAddedEventContext(DocumentOperation documentOperation, IFolderPath destinationFolderPath, IList<IDocumentPath> documentPathList, IRootProperties rootProperties)
        {
            this.DocumentOperation = documentOperation;
            this.DestinationFolderPath = destinationFolderPath;
            this.DocumentPathList = documentPathList;
            this.RootProperties = rootProperties;
        }

        public DocumentOperation DocumentOperation { get; private set; }
        public IFolderPath DestinationFolderPath { get; private set; }
        public IList<IDocumentPath> DocumentPathList { get; private set; }
        public IRootProperties RootProperties { get; private set; }
    }
}
