namespace CustomControls
{
    /// <summary>
    /// Represents the event data for node pasted completion event.
    /// </summary>
    internal interface INodePastedCompletionArgs
    {
        /// <summary>
        /// Gets the new node path.
        /// </summary>
        ITreeNodePath NewPath { get; }

        /// <summary>
        /// Gets the new node properties.
        /// </summary>
        ITreeNodeProperties NewProperties { get; }
    }

    /// <summary>
    /// Represents the event data for node pasted completion event.
    /// </summary>
    internal class NodePastedCompletionArgs : INodePastedCompletionArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NodePastedCompletionArgs"/> class.
        /// </summary>
        /// <param name="newPath">The new node path.</param>
        /// <param name="newProperties">The new node properties.</param>
        public NodePastedCompletionArgs(ITreeNodePath newPath, ITreeNodeProperties newProperties)
        {
            NewPath = newPath;
            NewProperties = newProperties;
        }

        /// <summary>
        /// Gets the new node path.
        /// </summary>
        public ITreeNodePath NewPath { get; private set; }

        /// <summary>
        /// Gets the new node properties.
        /// </summary>
        public ITreeNodeProperties NewProperties { get; private set; }
    }
}
