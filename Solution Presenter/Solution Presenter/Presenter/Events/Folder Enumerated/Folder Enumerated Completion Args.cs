using System.Collections.Generic;

namespace CustomControls
{
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

        public FolderEnumeratedCompletionArgs(IReadOnlyList<ITreeNodePath> Children, IReadOnlyDictionary<ITreeNodePath, ITreeNodeProperties> ChildrenProperties)
        {
            this.Children = Children;
            this.ChildrenProperties = ChildrenProperties;
        }

        public IReadOnlyList<ITreeNodePath> Children { get; private set; }
        public IReadOnlyDictionary<ITreeNodePath, ITreeNodeProperties> ChildrenProperties { get; private set; }
    }
}
