namespace CustomControls
{
    public class NodeRenamedEventContext
    {
        public NodeRenamedEventContext(ITreeNodePath path, string newName, bool isUndoRedo, IRootProperties rootProperties)
        {
            this.Path = path;
            this.NewName = newName;
            this.IsUndoRedo = isUndoRedo;
            this.RootProperties = rootProperties;
        }

        public ITreeNodePath Path { get; private set; }
        public string NewName { get; private set; }
        public bool IsUndoRedo { get; private set; }
        public IRootProperties RootProperties { get; private set; }
    }
}
