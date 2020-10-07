namespace CustomControls
{
    using System.Collections.Generic;
    using SolutionControlsInternal.Properties;

    /// <summary>
    /// Represents a remove operation in a solution explorer tree.
    /// </summary>
    internal class RemoveTreeOperation : AddRemoveTreeOperation
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveTreeOperation"/> class.
        /// </summary>
        /// <param name="root">The root path.</param>
        /// <param name="pathTable">The table of paths.</param>
        public RemoveTreeOperation(ISolutionRoot root, IReadOnlyDictionary<ITreeNodePath, IPathConnection> pathTable)
            : base(root, pathTable)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the operation name.
        /// </summary>
        public override string Name { get { return Resources.RemoveTree; } }

        /// <summary>
        /// Gets a value indicating whether this operation is adding something.
        /// </summary>
        public override bool IsAdd { get { return false; } }
        #endregion

        #region Client Interface
        /// <summary>
        /// Redoes the operation.
        /// </summary>
        public override void Redo()
        {
            Remove(PathTable);
            base.Redo();
        }

        /// <summary>
        /// Undoes the operation.
        /// </summary>
        public override void Undo()
        {
            Add(PathTable);
            base.Undo();
        }
        #endregion
    }
}
