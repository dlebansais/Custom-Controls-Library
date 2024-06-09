namespace ExtendedTreeView.Demo;

using System;
using System.Globalization;
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

    /// <summary>
    /// Gets the node parent.
    /// </summary>
    public IExtendedTreeNode? Parent { get; private set; }

    /// <summary>
    /// Gets the node index.
    /// </summary>
    public int Index { get; private set; }

    /// <summary>
    /// Gets the node children.
    /// </summary>
    public IExtendedTreeNodeCollection Children { get; private set; }

    /// <summary>
    /// Gets the node text.
    /// </summary>
    public string Text { get { return $"CloneableTestNode #{Index}"; } }

    /// <summary>
    /// Changes the parent.
    /// </summary>
    /// <param name="newParent">The new parent.</param>
    public void ChangeParent(IExtendedTreeNode newParent)
    {
        Parent = newParent;
    }

    /// <summary>
    /// Clones the node.
    /// </summary>
    public object Clone()
    {
        CloneableTestNode Clone = new((CloneableTestNode?)Parent, Index);
        foreach (CloneableTestNode Child in Children)
            _ = Clone.Children.Add(Child.Clone());

        return Clone;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{base.ToString()} #{Index}";
    }
}
