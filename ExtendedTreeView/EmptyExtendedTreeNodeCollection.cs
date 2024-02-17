namespace CustomControls;

using System.Collections.Generic;
using System.Collections.Specialized;

/// <summary>
/// Represents an empty collection of nodes.
/// </summary>
/// <typeparam name="TItem">The type of items.</typeparam>
/// <typeparam name="TCollection">The type of collection of items.</typeparam>
internal class EmptyExtendedTreeNodeCollection<TItem, TCollection> : List<IExtendedTreeNode>, IExtendedTreeNodeCollection
    where TItem : class, IExtendedTreeNode
    where TCollection : IExtendedTreeNodeCollection
{
    /// <summary>
    /// Gets the parent of the collection.
    /// </summary>
    public IExtendedTreeNode? Parent { get { return null; } }

    /// <summary>
    /// Occurs when the collection changes.
    /// </summary>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <summary>
    /// Notify when the collection is changed.
    /// </summary>
    protected void NotifyCollectionChanged()
    {
        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }
}
