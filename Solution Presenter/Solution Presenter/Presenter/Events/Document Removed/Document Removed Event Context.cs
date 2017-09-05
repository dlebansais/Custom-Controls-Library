using System.Collections.Generic;

namespace CustomControls
{
    public class DocumentRemovedEventContext
    {
        public DocumentRemovedEventContext(IRootPath rootPath, IReadOnlyDictionary<ITreeNodePath, IPathConnection> deletedTree, bool isUndoRedo, object clientInfo)
        {
            this.RootPath = rootPath;
            this.DeletedTree = deletedTree;
            this.IsUndoRedo = isUndoRedo;
            this.ClientInfo = clientInfo;
        }

        public IRootPath RootPath { get; private set; }
        public IReadOnlyDictionary<ITreeNodePath, IPathConnection> DeletedTree { get; private set; }
        public bool IsUndoRedo { get; private set; }
        public object ClientInfo { get; private set; }
    }
}
