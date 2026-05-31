namespace CustomControls;

using System.Collections.Specialized;

/// <summary>
/// Represents an empty tree node.
/// </summary>
/// <typeparam name="TItem">The type of items.</typeparam>
/// <typeparam name="TCollection">The type of collection of items.</typeparam>
internal class EmptyExtendedTreeNode<TItem, TCollection> : IExtendedTreeNode
    where TItem : class, IExtendedTreeNode
    where TCollection : IExtendedTreeNodeCollection
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmptyExtendedTreeNode{TItem, TCollection}"/> class.
    /// </summary>
    public EmptyExtendedTreeNode()
    {
        Children.CollectionChanged += OnCollectionChanged;
    }

    /// <inheritdoc cref="IExtendedTreeNode.Parent" />
    public IExtendedTreeNode? Parent => null;

    /// <inheritdoc cref="IExtendedTreeNode.Children" />
    public IExtendedTreeNodeCollection Children { get; } = new EmptyExtendedTreeNodeCollection<TItem, TCollection>();

    /// <inheritdoc cref="IExtendedTreeNode.ChangeParent(IExtendedTreeNode)" />
    public void ChangeParent(IExtendedTreeNode newParent)
    {
    }

    /// <summary>
    /// Handles the CollectionChanged event.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">Information about the event.</param>
    protected void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
    }
}
