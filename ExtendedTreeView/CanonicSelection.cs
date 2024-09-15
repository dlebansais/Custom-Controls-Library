namespace CustomControls;

using System.Collections;

/// <summary>
/// Represents a selection of items.
/// </summary>
public class CanonicSelection
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CanonicSelection"/> class.
    /// </summary>
    /// <param name="itemList">The list of selected items.</param>
    public CanonicSelection(IList itemList)
    {
        ItemList = itemList;
        AllItemsCloneable = true;
        RecordCount = 0;
    }

    /// <summary>
    /// Gets the parent item when dragging.
    /// </summary>
    public object? DraggedItemParent { get; private set; }

    /// <summary>
    /// Gets the list of selected items.
    /// </summary>
    public IList ItemList { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether all items in a branch are cloneable.
    /// </summary>
    public bool AllItemsCloneable { get; set; }

    /// <summary>
    /// Gets or sets the number of records in a branch.
    /// </summary>
    public int RecordCount { get; set; }

    /// <summary>
    /// Sets the dragged item parent.
    /// </summary>
    /// <param name="draggedItemParent">The dragged item parent.</param>
    public void SetDraggedItemParent(object draggedItemParent)
    {
        DraggedItemParent = draggedItemParent;
    }
}
