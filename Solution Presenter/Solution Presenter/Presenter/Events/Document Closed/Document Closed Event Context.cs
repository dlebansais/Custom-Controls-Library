using System.Collections.Generic;

namespace CustomControls
{
    public class DocumentClosedEventContext
    {
        public DocumentClosedEventContext(DocumentOperation documentOperation, IList<IDocument> closedDocumentList, IReadOnlyDictionary<ITreeNodePath, IPathConnection> closedTree, bool isUndoRedo, object? clientInfo)
        {
            DocumentOperation = documentOperation;
            ClosedDocumentList = closedDocumentList;
            ClosedTree = closedTree;
            IsUndoRedo = isUndoRedo;
            ClientInfo = clientInfo;
        }

        public DocumentOperation DocumentOperation { get; private set; }
        public IList<IDocument> ClosedDocumentList { get; private set; }
        public IReadOnlyDictionary<ITreeNodePath, IPathConnection> ClosedTree { get; private set; }
        public bool IsUndoRedo { get; private set; }
        public object? ClientInfo { get; private set; }
    }
}
