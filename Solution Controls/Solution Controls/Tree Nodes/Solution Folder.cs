namespace CustomControls
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a folder in a solution.
    /// </summary>
    public interface ISolutionFolder : ISolutionTreeNode
    {
        /// <summary>
        /// Gets the node comparer.
        /// </summary>
        IComparer<ITreeNodePath> NodeComparer { get; }

        /// <summary>
        /// Gets the flat list of child folders.
        /// </summary>
        IReadOnlyDictionary<IFolderPath, ISolutionFolder> FlatFolderChildren { get; }

        /// <summary>
        /// Finds a node by its path.
        /// </summary>
        /// <param name="path">The node path.</param>
        /// <returns>The node if found; otherwise,  null.</returns>
        ISolutionTreeNode? FindTreeNode(ITreeNodePath? path);
    }

    /// <summary>
    /// Represents a folder in a solution.
    /// </summary>
    public class SolutionFolder : SolutionTreeNode, ISolutionFolder
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionFolder"/> class.
        /// </summary>
        /// <param name="path">The folder path.</param>
        /// <param name="properties">The folder properties.</param>
        /// <param name="nodeComparer">The node comparer.</param>
        public SolutionFolder(IFolderPath path, IFolderProperties properties, IComparer<ITreeNodePath> nodeComparer)
            : base(null, path, properties)
        {
            ChildrenInternal = new SolutionTreeNodeCollection(this, nodeComparer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionFolder"/> class.
        /// </summary>
        /// <param name="parent">The parent folder path.</param>
        /// <param name="path">The folder path.</param>
        /// <param name="properties">The folder properties.</param>
        public SolutionFolder(ISolutionFolder parent, IFolderPath path, IFolderProperties properties)
            : base(parent, path, properties)
        {
            if (parent == null)
                return;

            ChildrenInternal = new SolutionTreeNodeCollection(this, parent.NodeComparer);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the node comparer.
        /// </summary>
        public IComparer<ITreeNodePath> NodeComparer { get { return ChildrenInternal.NodeComparer; } }

        /// <summary>
        /// Gets the flat list of child folders.
        /// </summary>
        public override IExtendedTreeNodeCollection Children { get { return ChildrenInternal; } }
        private ISolutionTreeNodeCollection ChildrenInternal = new SolutionTreeNodeCollection();

        /// <summary>
        /// Gets the flat list of child folders.
        /// </summary>
        public IReadOnlyDictionary<IFolderPath, ISolutionFolder> FlatFolderChildren
        {
            get
            {
                Dictionary<IFolderPath, ISolutionFolder> FlatChildrenTable = new Dictionary<IFolderPath, ISolutionFolder>();
                AddChilrenToTable(FlatChildrenTable);

                return FlatChildrenTable;
            }
        }

        private void AddChilrenToTable(Dictionary<IFolderPath, ISolutionFolder> flatChildrenTable)
        {
            flatChildrenTable.Add((IFolderPath)Path, this);

            foreach (ISolutionTreeNode Child in Children)
                if (Child is SolutionFolder AsFolder)
                    AsFolder.AddChilrenToTable(flatChildrenTable);
        }
        #endregion

        #region Client Interface
        /// <summary>
        /// Finds a node by its path.
        /// </summary>
        /// <param name="path">The node path.</param>
        /// <returns>The node if found; otherwise,  null.</returns>
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
