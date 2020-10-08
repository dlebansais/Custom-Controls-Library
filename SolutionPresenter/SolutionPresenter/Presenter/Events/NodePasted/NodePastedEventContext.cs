namespace CustomControls
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a context for the <see cref="NodePastedEventArgs"/> event data.
    /// </summary>
    public class NodePastedEventContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NodePastedEventContext"/> class.
        /// </summary>
        /// <param name="path">The path to the pasted node.</param>
        /// <param name="parentPath">The path to the parent.</param>
        /// <param name="pathTable">The table of children.</param>
        /// <param name="updatedParentTable">The table of updated parent in children.</param>
        /// <param name="rootProperties">The properties of the root object.</param>
        /// <param name="isUndoRedo">True if the operation can be undone.</param>
        public NodePastedEventContext(ITreeNodePath path, IFolderPath parentPath, IReadOnlyDictionary<ITreeNodePath, IPathConnection> pathTable, Dictionary<ITreeNodePath, IFolderPath> updatedParentTable, IRootProperties rootProperties, bool isUndoRedo)
        {
            Path = path;
            ParentPath = parentPath;
            PathTable = pathTable;
            UpdatedParentTable = updatedParentTable;
            RootProperties = rootProperties;
            IsUndoRedo = isUndoRedo;
        }

        /// <summary>
        /// Gets the path to the pasted node.
        /// </summary>
        public ITreeNodePath Path { get; private set; }

        /// <summary>
        /// Gets the path to the parent.
        /// </summary>
        public IFolderPath ParentPath { get; private set; }

        /// <summary>
        /// Gets the table of children.
        /// </summary>
        public IReadOnlyDictionary<ITreeNodePath, IPathConnection> PathTable { get; private set; }

        /// <summary>
        /// Gets the table of updated parent in children.
        /// </summary>
        public Dictionary<ITreeNodePath, IFolderPath> UpdatedParentTable { get; private set; }

        /// <summary>
        /// Gets the properties of the root object.
        /// </summary>
        public IRootProperties RootProperties { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the operation can be undone.
        /// </summary>
        public bool IsUndoRedo { get; private set; }
    }
}
