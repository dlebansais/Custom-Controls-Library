namespace CustomControls
{
    /// <summary>
    /// Represents a context for the <see cref="NodeMovedEventArgs"/> event data.
    /// </summary>
    public class NodeMovedEventContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeMovedEventContext"/> class.
        /// </summary>
        /// <param name="path">The moved node path.</param>
        /// <param name="newParentPath">The path to the new parent.</param>
        /// <param name="isUndoRedo">True if the operation can be undone.</param>
        /// <param name="rootProperties">The properties of the root object.</param>
        public NodeMovedEventContext(ITreeNodePath path, IFolderPath newParentPath, bool isUndoRedo, IRootProperties rootProperties)
        {
            Path = path;
            NewParentPath = newParentPath;
            IsUndoRedo = isUndoRedo;
            RootProperties = rootProperties;
        }

        /// <summary>
        /// Gets the moved node path.
        /// </summary>
        public ITreeNodePath Path { get; private set; }

        /// <summary>
        /// Gets the path to the new parent.
        /// </summary>
        public IFolderPath NewParentPath { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the operation can be undone.
        /// </summary>
        public bool IsUndoRedo { get; private set; }

        /// <summary>
        /// Gets the properties of the root object.
        /// </summary>
        public IRootProperties RootProperties { get; private set; }
    }
}
