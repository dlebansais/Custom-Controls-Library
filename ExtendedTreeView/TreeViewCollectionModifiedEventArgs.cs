namespace CustomControls;

using System.Windows;

/// <summary>
/// Represents arguments of a collection modified event.
/// </summary>
public class TreeViewCollectionModifiedEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TreeViewCollectionModifiedEventArgs"/> class.
    /// </summary>
    /// <param name="routedEvent">The routed event.</param>
    /// <param name="treeViewCollectionOperation">The modifying operation.</param>
    /// <param name="filledItemCount">The number of filled items.</param>
    public TreeViewCollectionModifiedEventArgs(RoutedEvent routedEvent, TreeViewCollectionOperation treeViewCollectionOperation, int filledItemCount)
        : base(routedEvent)
    {
        TreeViewCollectionOperation = treeViewCollectionOperation;
        FilledItemCount = filledItemCount;
    }

    /// <summary>
    /// Gets the modifying operation.
    /// </summary>
    public TreeViewCollectionOperation TreeViewCollectionOperation { get; }

    /// <summary>
    /// Gets the number of filled items.
    /// </summary>
    public int FilledItemCount { get; private set; }
}
