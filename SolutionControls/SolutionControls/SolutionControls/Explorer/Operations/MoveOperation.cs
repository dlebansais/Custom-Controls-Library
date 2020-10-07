namespace CustomControls
{
    using System;
    using SolutionControlsInternal.Properties;

    /// <summary>
    /// Represents a move operation in a solution explorer.
    /// </summary>
    internal class MoveOperation : SolutionExplorerOperation
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="MoveOperation"/> class.
        /// </summary>
        /// <param name="root">The root path.</param>
        /// <param name="path">The path to the moved node.</param>
        /// <param name="oldParent">The old parent.</param>
        /// <param name="newParent">The new parent.</param>
        public MoveOperation(ISolutionRoot root, ITreeNodePath path, ISolutionFolder oldParent, ISolutionFolder newParent)
            : base(root)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (oldParent == null)
                throw new ArgumentNullException(nameof(oldParent));
            if (newParent == null)
                throw new ArgumentNullException(nameof(newParent));

            this.Path = path;
            this.OldParent = oldParent;
            this.NewParent = newParent;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the operation name.
        /// </summary>
        public override string Name { get { return Resources.Move; } }

        /// <summary>
        /// Gets the path to the moved node.
        /// </summary>
        public ITreeNodePath Path { get; private set; }

        /// <summary>
        /// Gets the old parent folder.
        /// </summary>
        public ISolutionFolder OldParent { get; private set; }

        /// <summary>
        /// Gets the new parent folder.
        /// </summary>
        public ISolutionFolder NewParent { get; private set; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Redoes the operation.
        /// </summary>
        public override void Redo()
        {
            ChangeParent(OldParent, NewParent);
            base.Redo();
        }

        /// <summary>
        /// Undoes the operation.
        /// </summary>
        public override void Undo()
        {
            ChangeParent(NewParent, OldParent);
            base.Undo();
        }
        #endregion

        #region Implementation
        private void ChangeParent(ISolutionFolder oldParent, ISolutionFolder newParent)
        {
            ISolutionTreeNode? Node = Root.FindTreeNode(Path);
            if (Node != null)
            {
                ISolutionTreeNodeCollection OldChildrenCollection = (ISolutionTreeNodeCollection)oldParent.Children;
                OldChildrenCollection.Remove(Node);

                ISolutionTreeNodeCollection NewChildrenCollection = (ISolutionTreeNodeCollection)newParent.Children;
                NewChildrenCollection.Add(Node);

                NewChildrenCollection.Sort();
            }
        }
        #endregion
    }
}
