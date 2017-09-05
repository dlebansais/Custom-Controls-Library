namespace CustomControls
{
    public class NodeMovedEventContext
    {
        public NodeMovedEventContext(ITreeNodePath path, IFolderPath newParentPath, bool isUndoRedo, IRootProperties rootProperties)
        {
            this.Path = path;
            this.NewParentPath = newParentPath;
            this.IsUndoRedo = isUndoRedo;
            this.RootProperties = rootProperties;
        }

        public ITreeNodePath Path { get; private set; }
        public IFolderPath NewParentPath { get; private set; }
        public bool IsUndoRedo { get; private set; }
        public IRootProperties RootProperties { get; private set; }
    }
}
