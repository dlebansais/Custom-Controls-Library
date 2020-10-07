namespace CustomControls
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents an add operation in a solution explorer tree.
    /// </summary>
    internal class AddTreeOperation : AddRemoveTreeOperation
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="AddTreeOperation"/> class.
        /// </summary>
        /// <param name="root">The root path.</param>
        /// <param name="pathTable">The table of paths.</param>
        public AddTreeOperation(ISolutionRoot root, IReadOnlyDictionary<ITreeNodePath, IPathConnection> pathTable)
            : base(root, pathTable)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the operation name.
        /// </summary>
        public override string Name { get { return SolutionControlsInternal.Properties.Resources.AddTree; } }

        /// <summary>
        /// Gets a value indicating whether this operation is adding something.
        /// </summary>
        public override bool IsAdd { get { return true; } }
        #endregion

        #region Client Interface
        /// <summary>
        /// Redoes the operation.
        /// </summary>
        public override void Redo()
        {
            Add(PathTable);
            base.Redo();
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
