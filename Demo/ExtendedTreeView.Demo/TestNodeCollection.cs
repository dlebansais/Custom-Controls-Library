namespace ExtendedTreeView.Demo;

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

    /// <inheritdoc cref="IExtendedTreeNodeCollection.Parent" />
    public IExtendedTreeNode Parent { get; private set; }

    /// <inheritdoc cref="IExtendedTreeNodeCollection.Sort"/>
    public void Sort()
    {
    }

    /// <inheritdoc cref="IList{IExtendedTreeNode}.IndexOf(IExtendedTreeNode)"/>
    int IList<IExtendedTreeNode>.IndexOf(IExtendedTreeNode item) => IndexOf((TestNode)item);

    /// <inheritdoc cref="IList{IExtendedTreeNode}.Insert(int, IExtendedTreeNode)"/>
    void IList<IExtendedTreeNode>.Insert(int index, IExtendedTreeNode item) => Insert(index, (TestNode)item);

    /// <inheritdoc cref="IList{IExtendedTreeNode}.this[int]"/>
    IExtendedTreeNode IList<IExtendedTreeNode>.this[int index]
    {
        get { return this[index]; }
        set { this[index] = (TestNode)value; }
    }

    /// <inheritdoc cref="ICollection{IExtendedTreeNode}.Add(IExtendedTreeNode)"/>
    void ICollection<IExtendedTreeNode>.Add(IExtendedTreeNode item) => Add((TestNode)item);

    /// <inheritdoc cref="ICollection{IExtendedTreeNode}.Contains(IExtendedTreeNode)"/>
    bool ICollection<IExtendedTreeNode>.Contains(IExtendedTreeNode item) => Contains((TestNode)item);

    /// <inheritdoc cref="ICollection{IExtendedTreeNode}.CopyTo(IExtendedTreeNode[], int)"/>
    void ICollection<IExtendedTreeNode>.CopyTo(IExtendedTreeNode[] array, int arrayIndex) => CopyTo((TestNode[])array, arrayIndex);

    /// <inheritdoc cref="ICollection{IExtendedTreeNode}.Remove(IExtendedTreeNode)"/>
    bool ICollection<IExtendedTreeNode>.Remove(IExtendedTreeNode item) => Remove((TestNode)item);

    /// <inheritdoc cref="ICollection{T}.IsReadOnly"/>
    public bool IsReadOnly => false;

    /// <inheritdoc cref="IEnumerable{IExtendedTreeNode}.GetEnumerator()"/>
    IEnumerator<IExtendedTreeNode> IEnumerable<IExtendedTreeNode>.GetEnumerator() => GetEnumerator();
}
