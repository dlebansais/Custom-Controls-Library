namespace ExtendedTreeView.Demo;

using System.Globalization;
using CustomControls;

/// <summary>
/// Represents a test node.
/// </summary>
internal class TestNode : IExtendedTreeNode
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestNode"/> class.
    /// </summary>
    /// <param name="parent">The parent.</param>
    /// <param name="index">The index.</param>
    public TestNode(TestNode? parent, int index)
    {
        Parent = parent;
        Index = index;

        Children = new TestNodeCollection(this);
    }

    /// <summary>
    /// Gets the parent.
    /// </summary>
    public IExtendedTreeNode? Parent { get; private set; }

    /// <summary>
    /// Gets the index.
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// Gets children.
    /// </summary>
    public IExtendedTreeNodeCollection Children { get; }

    /// <summary>
    /// Gets the node text.
    /// </summary>
    public string Text { get { return "TestNode #" + Index.ToString(CultureInfo.InvariantCulture); } }

    /// <summary>
    /// Changes the parent.
    /// </summary>
    /// <param name="newParent">The new parent.</param>
    public void ChangeParent(IExtendedTreeNode newParent)
    {
        Parent = newParent;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{base.ToString()} #{Index}";
    }
}
