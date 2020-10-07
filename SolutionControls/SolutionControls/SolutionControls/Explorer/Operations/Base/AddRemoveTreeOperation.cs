namespace CustomControls
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents an add or remove operation in a solution explorer tree.
    /// </summary>
    internal abstract class AddRemoveTreeOperation : AddRemoveOperation
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="AddRemoveTreeOperation"/> class.
        /// </summary>
        /// <param name="root">The root path.</param>
        /// <param name="pathTable">The table of paths.</param>
        protected AddRemoveTreeOperation(ISolutionRoot root, IReadOnlyDictionary<ITreeNodePath, IPathConnection> pathTable)
            : base(root, pathTable)
        {
        }
        #endregion
    }
}
