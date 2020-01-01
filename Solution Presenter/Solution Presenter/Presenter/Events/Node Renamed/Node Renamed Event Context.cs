namespace CustomControls
{
    /// <summary>
    /// Represents a context for the <see cref="NodeRenamedEventArgs"/> event data.
    /// </summary>
    public class NodeRenamedEventContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeRenamedEventContext"/> class.
        /// </summary>
        /// <param name="path">The path to the renamed node.</param>
        /// <param name="newName">The new name.</param>
        /// <param name="isUndoRedo">True if the operation can be undone.</param>
        /// <param name="rootProperties">The properties of the root object.</param>
        public NodeRenamedEventContext(ITreeNodePath path, string newName, bool isUndoRedo, IRootProperties rootProperties)
        {
            Path = path;
            NewName = newName;
            IsUndoRedo = isUndoRedo;
            RootProperties = rootProperties;
        }

        /// <summary>
        /// Gets the path to the renamed node.
        /// </summary>
        public ITreeNodePath Path { get; private set; }

        /// <summary>
        /// Gets the new name.
        /// </summary>
        public string NewName { get; private set; }

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
