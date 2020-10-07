namespace CustomControls
{
    using System;
    using SolutionControlsInternal.Properties;

    /// <summary>
    /// Represents a rename operation in a solution explorer.
    /// </summary>
    internal class RenameOperation : SolutionExplorerOperation
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="RenameOperation"/> class.
        /// </summary>
        /// <param name="root">The root path.</param>
        /// <param name="path">The path of the renamed item.</param>
        /// <param name="newName">The new name.</param>
        public RenameOperation(ISolutionRoot root, ITreeNodePath path, string newName)
            : base(root)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (newName == null)
                throw new ArgumentNullException(nameof(newName));

            this.Path = path;
            this.OldName = Path.FriendlyName;
            this.NewName = newName;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the operation name.
        /// </summary>
        public override string Name { get { return Resources.Rename; } }

        /// <summary>
        /// Gets the path to the renamed node.
        /// </summary>
        public ITreeNodePath Path { get; private set; }

        /// <summary>
        /// Gets the old name.
        /// </summary>
        public string OldName { get; private set; }

        /// <summary>
        /// Gets the new name.
        /// </summary>
        public string NewName { get; private set; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Redoes the operation.
        /// </summary>
        public override void Redo()
        {
            ChangeName(NewName);
            base.Redo();
        }

        /// <summary>
        /// Undoes the operation.
        /// </summary>
        public override void Undo()
        {
            ChangeName(OldName);
            base.Undo();
        }
        #endregion

        #region Implementation
        private void ChangeName(string name)
        {
            ISolutionTreeNode? Node = Root.FindTreeNode(Path);
            if (Node != null)
                Node.ChangeName(name);
        }
        #endregion
    }
}
