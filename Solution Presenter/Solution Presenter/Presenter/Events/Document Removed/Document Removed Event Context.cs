namespace CustomControls
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a context for the <see cref="DocumentRemovedEventArgs"/> event data.
    /// </summary>
    public class DocumentRemovedEventContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentRemovedEventContext"/> class.
        /// </summary>
        /// <param name="rootPath">The root path.</param>
        /// <param name="deletedTree">The delete tree.</param>
        /// <param name="isUndoRedo">True if the operation can be undone.</param>
        /// <param name="clientInfo">The operation data.</param>
        public DocumentRemovedEventContext(IRootPath rootPath, IReadOnlyDictionary<ITreeNodePath, IPathConnection> deletedTree, bool isUndoRedo, object? clientInfo)
        {
            RootPath = rootPath;
            DeletedTree = deletedTree;
            IsUndoRedo = isUndoRedo;
            ClientInfo = clientInfo;
        }

        /// <summary>
        /// Gets the root path.
        /// </summary>
        public IRootPath RootPath { get; }

        /// <summary>
        /// Gets the deleted tree.
        /// </summary>
        public IReadOnlyDictionary<ITreeNodePath, IPathConnection> DeletedTree { get; }

        /// <summary>
        /// Gets a value indicating whether the operation can be undone.
        /// </summary>
        public bool IsUndoRedo { get; }

        /// <summary>
        /// Gets the operation data.
        /// </summary>
        public object? ClientInfo { get; }
    }
}
