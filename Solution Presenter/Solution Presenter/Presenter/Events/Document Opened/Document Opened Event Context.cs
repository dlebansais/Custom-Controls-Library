using System.Collections.Generic;

namespace CustomControls
{
    public class DocumentOpenedEventContext
    {
        public DocumentOpenedEventContext(DocumentOperation documentOperation, IFolderPath destinationFolderPath, IList<IDocumentPath> openedDocumentPathList, IList<IDocumentPath> documentPathList, object errorLocation)
        {
            this.DocumentOperation = documentOperation;
            this.DestinationFolderPath = destinationFolderPath;
            this.OpenedDocumentPathList = openedDocumentPathList;
            this.DocumentPathList = documentPathList;
            this.ErrorLocation = errorLocation;
        }

        public DocumentOperation DocumentOperation { get; private set; }
        public IFolderPath DestinationFolderPath { get; private set; }
        public IList<IDocumentPath> OpenedDocumentPathList { get; private set; }
        public IList<IDocumentPath> DocumentPathList { get; private set; }
        public object ErrorLocation { get; private set; }
    }
}
