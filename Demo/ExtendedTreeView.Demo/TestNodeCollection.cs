namespace ExtendedTreeViewDemo;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using CustomControls;

/// <summary>
/// Represents a collection of nodes.
/// </summary>
internal class TestNodeCollection : ObservableCollection<TestNode>, IExtendedTreeNodeCollection
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestNodeCollection"/> class.
    /// </summary>
    /// <param name="parent">The parent node.</param>
    public TestNodeCollection(TestNode parent)
    {
        Parent = parent;
    }

    /// <summary>
    /// Gets the parent node.
    /// </summary>
    public IExtendedTreeNode Parent { get; private set; }

    /// <inheritdoc/>
    public void Sort()
    {
    }

    /// <inheritdoc/>
    int IList<IExtendedTreeNode>.IndexOf(IExtendedTreeNode item)
    {
        return IndexOf((TestNode)item);
    }

    /// <inheritdoc/>
    void IList<IExtendedTreeNode>.Insert(int index, IExtendedTreeNode item)
    {
        Insert(index, (TestNode)item);
    }

    /// <inheritdoc/>
    IExtendedTreeNode IList<IExtendedTreeNode>.this[int index]
    {
        get { return this[index]; }
        set { this[index] = (TestNode)value; }
    }

    /// <inheritdoc/>
    void ICollection<IExtendedTreeNode>.Add(IExtendedTreeNode item)
    {
        Add((TestNode)item);
    }

    /// <inheritdoc/>
    bool ICollection<IExtendedTreeNode>.Contains(IExtendedTreeNode item)
    {
        return Contains((TestNode)item);
    }

    /// <inheritdoc/>
    void ICollection<IExtendedTreeNode>.CopyTo(IExtendedTreeNode[] array, int arrayIndex)
    {
        CopyTo((TestNode[])array, arrayIndex);
    }

    /// <inheritdoc/>
    bool ICollection<IExtendedTreeNode>.Remove(IExtendedTreeNode item)
    {
        return Remove((TestNode)item);
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
