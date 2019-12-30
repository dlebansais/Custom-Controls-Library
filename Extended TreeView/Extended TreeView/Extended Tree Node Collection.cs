namespace CustomControls
{
    using System.Collections;
    using System.Collections.Specialized;

    /// <summary>
    /// Represents a collection of nodes.
    /// </summary>
    public interface IExtendedTreeNodeCollection : IList, INotifyCollectionChanged
    {
        /// <summary>
        /// Gets the parent of the collection.
        /// </summary>
        IExtendedTreeNode? Parent { get; }

        /// <summary>
        /// Sorts the collection.
        /// </summary>
        void Sort();
    }
}
