namespace ExtendedTreeView.Demo;

using System;
using CustomControls;

/// <summary>
/// Represents a cloneable test node.
/// </summary>
internal class CloneableTestNode : IExtendedTreeNode, ICloneable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CloneableTestNode"/> class.
    /// </summary>
    /// <param name="parent">The parent node.</param>
    /// <param name="index">The node index.</param>
    public CloneableTestNode(CloneableTestNode? parent, int index)
    {
        Parent = parent;
        Index = index;

        Children = new CloneableTestNodeCollection(this);
    }

    /// <inheritdoc cref="IExtendedTreeNode.Parent" />
    public IExtendedTreeNode? Parent { get; private set; }

    /// <summary>
    /// Gets the node index.
    /// </summary>
    public int Index { get; private set; }

    /// <inheritdoc cref="IExtendedTreeNode.Children" />
    public IExtendedTreeNodeCollection Children { get; private set; }

    /// <summary>
    /// Gets the node text.
    /// </summary>
    public string Text => $"CloneableTestNode #{Index}";

    /// <inheritdoc cref="IExtendedTreeNode.ChangeParent(IExtendedTreeNode)" />
    public void ChangeParent(IExtendedTreeNode newParent) => Parent = newParent;

    /// <inheritdoc cref="ICloneable.Clone()" />
    public object Clone()
    {
        CloneableTestNode Clone = new((CloneableTestNode?)Parent, Index);
        foreach (CloneableTestNode Child in Children)
            _ = Clone.Children.Add(Child.Clone());

        return Clone;
    }

    /// <inheritdoc cref="object.ToString()" />
    public override string ToString() => $"{base.ToString()} #{Index}";
}
