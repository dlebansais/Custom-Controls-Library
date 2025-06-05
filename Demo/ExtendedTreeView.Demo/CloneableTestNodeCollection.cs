namespace ExtendedTreeView.Demo;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using CustomControls;

/// <summary>
/// Represents a collection of cleaneable nodes.
/// </summary>
internal class CloneableTestNodeCollection : ObservableCollection<CloneableTestNode>, IExtendedTreeNodeCollection
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CloneableTestNodeCollection"/> class.
    /// </summary>
    /// <param name="parent">The parent node.</param>
    public CloneableTestNodeCollection(CloneableTestNode parent)
    {
        Parent = parent;
    }

    /// <summary>
    /// Gets the collection parent node.
    /// </summary>
    public IExtendedTreeNode Parent { get; }

    /// <inheritdoc cref="IExtendedTreeNodeCollection.Sort"/>
    public void Sort()
    {
    }

    /// <inheritdoc/>
    int IList<IExtendedTreeNode>.IndexOf(IExtendedTreeNode item) => IndexOf((CloneableTestNode)item);

    /// <inheritdoc/>
    void IList<IExtendedTreeNode>.Insert(int index, IExtendedTreeNode item) => Insert(index, (CloneableTestNode)item);

    /// <inheritdoc/>
    IExtendedTreeNode IList<IExtendedTreeNode>.this[int index]
    {
        get { return this[index]; }
        set { this[index] = (CloneableTestNode)value; }
    }

    /// <inheritdoc/>
    void ICollection<IExtendedTreeNode>.Add(IExtendedTreeNode item) => Add((CloneableTestNode)item);

    /// <inheritdoc/>
    bool ICollection<IExtendedTreeNode>.Contains(IExtendedTreeNode item) => Contains((CloneableTestNode)item);

    /// <inheritdoc/>
    void ICollection<IExtendedTreeNode>.CopyTo(IExtendedTreeNode[] array, int arrayIndex) => CopyTo((CloneableTestNode[])array, arrayIndex);

    /// <inheritdoc/>
    bool ICollection<IExtendedTreeNode>.Remove(IExtendedTreeNode item) => Remove((CloneableTestNode)item);

    /// <inheritdoc cref="ICollection{T}.IsReadOnly"/>
    public bool IsReadOnly => false;

    /// <inheritdoc/>
    IEnumerator<IExtendedTreeNode> IEnumerable<IExtendedTreeNode>.GetEnumerator() => GetEnumerator();
}
