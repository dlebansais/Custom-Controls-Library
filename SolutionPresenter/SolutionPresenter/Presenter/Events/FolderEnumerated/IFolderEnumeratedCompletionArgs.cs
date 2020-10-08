namespace CustomControls
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the event data for folder enumerated completion event.
    /// </summary>
    internal interface IFolderEnumeratedCompletionArgs
    {
        /// <summary>
        /// Gets the list of enumerated children.
        /// </summary>
        IReadOnlyList<ITreeNodePath> Children { get; }

        /// <summary>
        /// Gets the list of properties of enumerated children.
        /// </summary>
        IReadOnlyDictionary<ITreeNodePath, ITreeNodeProperties> ChildrenProperties { get; }
    }

    /// <summary>
    /// Represents the event data for folder enumerated completion event.
    /// </summary>
    internal class FolderEnumeratedCompletionArgs : IFolderEnumeratedCompletionArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FolderEnumeratedCompletionArgs"/> class.
        /// </summary>
        public FolderEnumeratedCompletionArgs()
        {
            Children = new List<ITreeNodePath>();
            ChildrenProperties = new Dictionary<ITreeNodePath, ITreeNodeProperties>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FolderEnumeratedCompletionArgs"/> class.
        /// </summary>
        /// <param name="children">The list of enumerated children.</param>
        /// <param name="childrenProperties">The list of properties of enumerated children.</param>
        public FolderEnumeratedCompletionArgs(IReadOnlyList<ITreeNodePath> children, IReadOnlyDictionary<ITreeNodePath, ITreeNodeProperties> childrenProperties)
        {
            Children = children;
            ChildrenProperties = childrenProperties;
        }

        /// <summary>
        /// Gets the list of enumerated children.
        /// </summary>
        public IReadOnlyList<ITreeNodePath> Children { get; private set; }

        /// <summary>
        /// Gets the list of properties of enumerated children.
        /// </summary>
        public IReadOnlyDictionary<ITreeNodePath, ITreeNodeProperties> ChildrenProperties { get; private set; }
    }
}
