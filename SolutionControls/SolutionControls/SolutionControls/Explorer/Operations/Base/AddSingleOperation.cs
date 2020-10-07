namespace CustomControls
{
    using System;

    /// <summary>
    /// Represents the add operation of a single item.
    /// </summary>
    internal abstract class AddSingleOperation : AddRemoveSingleOperation
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="AddSingleOperation"/> class.
        /// </summary>
        /// <param name="root">The root path.</param>
        /// <param name="destinationFolderPath">The destination folder path.</param>
        /// <param name="newPath">The path of the added item.</param>
        /// <param name="newProperties">The properties of the added item.</param>
        protected AddSingleOperation(ISolutionRoot root, IFolderPath destinationFolderPath, ITreeNodePath newPath, ITreeNodeProperties newProperties)
            : base(root, destinationFolderPath, newPath, newProperties)
        {
            if (destinationFolderPath == null)
                throw new ArgumentNullException(nameof(destinationFolderPath));

            this.DestinationFolderPath = destinationFolderPath;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the destination path.
        /// </summary>
        public IFolderPath DestinationFolderPath { get; private set; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Redoes the operation.
        /// </summary>
        public override void Redo()
        {
            Add(PathTable);
            base.Redo();

            if (Root is ISolutionRoot AsValidRoot)
            {
                if (AsValidRoot.FindTreeNode(DestinationFolderPath) is ISolutionFolder ParentFolder)
                    AddExpandedFolder(ParentFolder);
            }
        }

        /// <summary>
        /// Undoes the operation.
        /// </summary>
        public override void Undo()
        {
            Remove(PathTable);
            base.Undo();
        }
        #endregion
    }
}
