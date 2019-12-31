namespace CustomControls
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the remove operation of a single item.
    /// </summary>
    internal abstract class AddRemoveSingleOperation : AddRemoveOperation
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="AddRemoveSingleOperation"/> class.
        /// </summary>
        /// <param name="root">The root path.</param>
        /// <param name="parentPath">The oarent folder path.</param>
        /// <param name="path">The path of the item.</param>
        /// <param name="properties">The properties of the item.</param>
        protected AddRemoveSingleOperation(ISolutionRoot root, IFolderPath parentPath, ITreeNodePath path, ITreeNodeProperties properties)
            : base(root, CreateSinglePathTable(parentPath, path, properties))
        {
        }

        private static IReadOnlyDictionary<ITreeNodePath, IPathConnection> CreateSinglePathTable(IFolderPath parentPath, ITreeNodePath path, ITreeNodeProperties properties)
        {
            return new Dictionary<ITreeNodePath, IPathConnection>() { { path, new PathConnection(parentPath, properties, false) } };
        }
        #endregion
    }
}
