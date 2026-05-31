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

    /// <inheritdoc cref="IExtendedTreeNode.Parent" />
    public IExtendedTreeNode? Parent { get; private set; }

    /// <summary>
    /// Gets the index.
    /// </summary>
    public int Index { get; }

    /// <inheritdoc cref="IExtendedTreeNode.Children" />
    public IExtendedTreeNodeCollection Children { get; }

    /// <summary>
    /// Gets the node text.
    /// </summary>
    public string Text => "TestNode #" + Index.ToString(CultureInfo.InvariantCulture);

    /// <inheritdoc cref="IExtendedTreeNode.ChangeParent(IExtendedTreeNode)" />
    public void ChangeParent(IExtendedTreeNode newParent) => Parent = newParent;

    /// <inheritdoc cref="object.ToString()" />
    public override string ToString() => $"{base.ToString()} #{Index}";
}
