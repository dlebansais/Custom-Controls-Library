using System.Collections.Generic;

namespace CustomControls
{
    public class DocumentRemovedEventContext
    {
        public DocumentRemovedEventContext(IRootPath rootPath, IReadOnlyDictionary<ITreeNodePath, IPathConnection> deletedTree, bool isUndoRedo, object? clientInfo)
        {
            RootPath = rootPath;
            DeletedTree = deletedTree;
            IsUndoRedo = isUndoRedo;
            ClientInfo = clientInfo;
        }

        public IRootPath RootPath { get; }
        public IReadOnlyDictionary<ITreeNodePath, IPathConnection> DeletedTree { get; }
        public bool IsUndoRedo { get; }
        public object? ClientInfo { get; }
    }
}
