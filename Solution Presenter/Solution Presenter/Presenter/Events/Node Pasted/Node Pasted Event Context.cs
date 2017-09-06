using System.Collections.Generic;

namespace CustomControls
{
    public class NodePastedEventContext
    {
        public NodePastedEventContext(ITreeNodePath path, IFolderPath parentPath, IReadOnlyDictionary<ITreeNodePath, IPathConnection> pathTable, Dictionary<ITreeNodePath, IFolderPath> updatedParentTable, IRootProperties rootProperties, bool isUndoRedo)
        {
            this.Path = path;
            this.ParentPath = parentPath;
            this.PathTable = pathTable;
            this.UpdatedParentTable = updatedParentTable;
            this.RootProperties = rootProperties;
            this.IsUndoRedo = isUndoRedo;
        }

        public ITreeNodePath Path { get; private set; }
        public IFolderPath ParentPath { get; private set; }
        public IReadOnlyDictionary<ITreeNodePath, IPathConnection> PathTable { get; private set; }
        public Dictionary<ITreeNodePath, IFolderPath> UpdatedParentTable { get; private set; }
        public IRootProperties RootProperties { get; private set; }
        public bool IsUndoRedo { get; private set; }
    }
}
