namespace ExtendedTreeViewDemo;

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
        this.Parent = parent;
    }

    /// <summary>
    /// Gets the collection parent node.
    /// </summary>
    public IExtendedTreeNode Parent { get; }

    /// <inheritdoc/>
    public void Sort()
    {
    }

    /// <inheritdoc/>
    int IList<IExtendedTreeNode>.IndexOf(IExtendedTreeNode item)
    {
        return IndexOf((CloneableTestNode)item);
    }

    /// <inheritdoc/>
    void IList<IExtendedTreeNode>.Insert(int index, IExtendedTreeNode item)
    {
        Insert(index, (CloneableTestNode)item);
    }

    /// <inheritdoc/>
    IExtendedTreeNode IList<IExtendedTreeNode>.this[int index]
    {
        get { return this[index]; }
        set { this[index] = (CloneableTestNode)value; }
    }

    /// <inheritdoc/>
    void ICollection<IExtendedTreeNode>.Add(IExtendedTreeNode item)
    {
        Add((CloneableTestNode)item);
    }

    /// <inheritdoc/>
    bool ICollection<IExtendedTreeNode>.Contains(IExtendedTreeNode item)
    {
        return Contains((CloneableTestNode)item);
    }

    /// <inheritdoc/>
    void ICollection<IExtendedTreeNode>.CopyTo(IExtendedTreeNode[] array, int arrayIndex)
    {
        CopyTo((CloneableTestNode[])array, arrayIndex);
    }

    /// <inheritdoc/>
    bool ICollection<IExtendedTreeNode>.Remove(IExtendedTreeNode item)
    {
        return Remove((CloneableTestNode)item);
    }

    /// <inheritdoc/>
    public bool IsReadOnly
    {
        get { return false; }
    }

    /// <inheritdoc/>
    IEnumerator<IExtendedTreeNode> IEnumerable<IExtendedTreeNode>.GetEnumerator()
    {
        return GetEnumerator();
    }
}
