namespace CustomControls
{
    internal interface INodePastedCompletionArgs
    {
        ITreeNodePath NewPath { get; }
        ITreeNodeProperties NewProperties { get; }
    }

    internal class NodePastedCompletionArgs : INodePastedCompletionArgs
    {
        public NodePastedCompletionArgs(ITreeNodePath newPath, ITreeNodeProperties newProperties)
        {
            NewPath = newPath;
            NewProperties = newProperties;
        }

        public ITreeNodePath NewPath { get; private set; }
        public ITreeNodeProperties NewProperties { get; private set; }
    }
}
