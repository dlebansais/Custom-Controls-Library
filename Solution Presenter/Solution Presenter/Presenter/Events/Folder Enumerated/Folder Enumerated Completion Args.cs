namespace CustomControls
{
    using System.Collections.Generic;

    internal interface IFolderEnumeratedCompletionArgs
    {
        IReadOnlyList<ITreeNodePath> Children { get; }
        IReadOnlyDictionary<ITreeNodePath, ITreeNodeProperties> ChildrenProperties { get; }
    }

    internal class FolderEnumeratedCompletionArgs : IFolderEnumeratedCompletionArgs
    {
        public FolderEnumeratedCompletionArgs()
        {
            this.Children = new List<ITreeNodePath>();
            this.ChildrenProperties = new Dictionary<ITreeNodePath, ITreeNodeProperties>();
        }

        public FolderEnumeratedCompletionArgs(IReadOnlyList<ITreeNodePath> children, IReadOnlyDictionary<ITreeNodePath, ITreeNodeProperties> childrenProperties)
        {
            Children = children;
            ChildrenProperties = childrenProperties;
        }

        public IReadOnlyList<ITreeNodePath> Children { get; private set; }
        public IReadOnlyDictionary<ITreeNodePath, ITreeNodeProperties> ChildrenProperties { get; private set; }
    }
}
