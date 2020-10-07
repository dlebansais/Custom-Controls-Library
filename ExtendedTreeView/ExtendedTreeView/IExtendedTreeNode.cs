namespace CustomControls
{
    /// <summary>
    /// Represents a node in the tree.
    /// </summary>
    public interface IExtendedTreeNode
    {
        /// <summary>
        /// Gets the parent node.
        /// </summary>
        IExtendedTreeNode? Parent { get; }

        /// <summary>
        /// Gets the collection of child nodes.
        /// </summary>
        IExtendedTreeNodeCollection Children { get; }

        /// <summary>
        /// Changes the node parent.
        /// </summary>
        /// <param name="newParent">The new parent node.</param>
        void ChangeParent(IExtendedTreeNode newParent);
    }
}
