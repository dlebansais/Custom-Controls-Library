using System.Collections.Generic;

namespace CustomControls
{
    public interface ISolutionFolder : ISolutionTreeNode
    {
        IComparer<ITreeNodePath> NodeComparer { get; }
        IReadOnlyDictionary<IFolderPath, ISolutionFolder> FlatFolderChildren { get; }
        ISolutionTreeNode? FindTreeNode(ITreeNodePath? path);
    }

    public class SolutionFolder : SolutionTreeNode, ISolutionFolder
    {
        #region Init
        public SolutionFolder(IFolderPath path, IFolderProperties properties, IComparer<ITreeNodePath> nodeComparer)
            : base(null, path, properties)
        {
            _Children = new SolutionTreeNodeCollection(this, nodeComparer);
        }

        public SolutionFolder(ISolutionFolder parent, IFolderPath path, IFolderProperties properties)
            : base(parent, path, properties)
        {
            if (parent == null)
                return;

            _Children = new SolutionTreeNodeCollection(this, parent.NodeComparer);
        }
        #endregion

        #region Properties
        public IComparer<ITreeNodePath> NodeComparer { get { return _Children.NodeComparer; } }
        public override IExtendedTreeNodeCollection Children { get { return _Children; } }
        private ISolutionTreeNodeCollection _Children = new SolutionTreeNodeCollection();

        public IReadOnlyDictionary<IFolderPath, ISolutionFolder> FlatFolderChildren
        {
            get
            {
                Dictionary<IFolderPath, ISolutionFolder> FlatChildrenTable = new Dictionary<IFolderPath, ISolutionFolder>();
                AddChilrenToTable(FlatChildrenTable);

                return FlatChildrenTable;
            }
        }

        private void AddChilrenToTable(Dictionary<IFolderPath, ISolutionFolder> FlatChildrenTable)
        {
            FlatChildrenTable.Add((IFolderPath)Path, this);

            foreach (ISolutionTreeNode Child in Children)
                if (Child is SolutionFolder AsFolder)
                    AsFolder.AddChilrenToTable(FlatChildrenTable);
        }
        #endregion

        #region Client Interface
        public virtual ISolutionTreeNode? FindTreeNode(ITreeNodePath? path)
        {
            if (path == null)
                return null;

            if (path.IsEqual(Path))
                return this;

            foreach (ISolutionTreeNode Child in Children)
            {
                if (path.IsEqual(Child.Path))
                    return Child;

                if (Child is ISolutionFolder AsFolder)
                {
                    ISolutionTreeNode? TreeNode = AsFolder.FindTreeNode(path);
                    if (TreeNode != null)
                        return TreeNode;
                }
            }

            return null;
        }
        #endregion
    }
}
