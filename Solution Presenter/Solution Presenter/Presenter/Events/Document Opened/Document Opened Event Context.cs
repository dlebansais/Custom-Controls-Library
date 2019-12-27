using System.Collections.Generic;

namespace CustomControls
{
    public class DocumentOpenedEventContext
    {
        public DocumentOpenedEventContext(DocumentOperation documentOperation, IFolderPath destinationFolderPath, IList<IDocumentPath> openedDocumentPathList, IList<IDocumentPath> documentPathList, object? errorLocation)
        {
            DocumentOperation = documentOperation;
            DestinationFolderPath = destinationFolderPath;
            OpenedDocumentPathList = openedDocumentPathList;
            DocumentPathList = documentPathList;
            ErrorLocation = errorLocation;
        }

        public DocumentOperation DocumentOperation { get; }
        public IFolderPath DestinationFolderPath { get; }
        public IList<IDocumentPath> OpenedDocumentPathList { get; }
        public IList<IDocumentPath> DocumentPathList { get; }
        public object? ErrorLocation { get; }
    }
}
