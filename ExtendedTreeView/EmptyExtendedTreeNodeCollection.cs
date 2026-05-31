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
    /// <inheritdoc cref="IExtendedTreeNodeCollection.Parent" />
    public IExtendedTreeNode? Parent => null;

    /// <inheritdoc cref="INotifyCollectionChanged.CollectionChanged" />
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <summary>
    /// Notify when the collection is changed.
    /// </summary>
    protected void NotifyCollectionChanged() => CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
}
