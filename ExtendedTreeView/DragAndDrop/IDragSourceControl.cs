namespace CustomControls
{
    using System;
    using System.Collections;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Represents the interface of a control providing drag and drop features.
    /// </summary>
    public interface IDragSourceControl
    {
        /// <summary>
        /// Gets the control source of the dragged content.
        /// </summary>
        FrameworkElement SourceControl { get; }

        /// <summary>
        /// Gets a value indicating whether copy is allowed on drop.
        /// </summary>
        bool AllowDropCopy { get; }

        /// <summary>
        /// Gets the GUID of the source.
        /// </summary>
        Guid SourceGuid { get; }

        /// <summary>
        /// Gets the activity state of the drag operation.
        /// </summary>
        DragActivity DragActivity { get; }

        /// <summary>
        /// Occurs when <see cref="DragActivity"/> changed.
        /// </summary>
        event EventHandler<EventArgs> DragActivityChanged;

        /// <summary>
        /// Changes the value of IsDragPossible.
        /// </summary>
        /// <param name="canonicSelectedItemList">The lost of selected items.</param>
        void SetIsDragPossible(CanonicSelection canonicSelectedItemList);

        /// <summary>
        /// Checks if an item is the parent of the dragged item.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>True if the parent of the dragged item; otherwise, false.</returns>
        bool IsDraggedItemParent(object item);

        /// <summary>
        /// Clears the value of IsDragPossible.
        /// </summary>
        void ClearIsDragPossible();

        /// <summary>
        /// Gets the value indicating if drag is possible.
        /// </summary>
        /// <param name="draggedItemParent">The dragged parent item upon return.</param>
        /// <param name="itemList">The list of dragged items upon return.</param>
        /// <returns>True if drag is possible; otherwise, false.</returns>
        bool IsDragPossible(out object draggedItemParent, out IList itemList);

        /// <summary>
        /// Called when a drag should begin after the mouse moved.
        /// </summary>
        /// <param name="sourceLocation">The source location.</param>
        void DragAfterMouseMove(MouseEventArgs sourceLocation);

        /// <summary>
        /// Cancels the drag operation.
        /// </summary>
        void CancelDrag();

        /// <summary>
        /// Sets the flat list of dragged items.
        /// </summary>
        /// <param name="rootItem">The root item.</param>
        /// <param name="flatItemList">The flat list of dragged items.</param>
        void SetFlatDraggedItemList(object rootItem, IList flatItemList);

        /// <summary>
        /// Clears the flat list of dragged items.
        /// </summary>
        void ClearFlatDraggedItemList();

        /// <summary>
        /// Checks if there are dragged items.
        /// </summary>
        /// <param name="rootItem">The root of dragged items upon return.</param>
        /// <param name="flatItemList">The flat list of dragged items upon return.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        bool HasDragItemList(out object rootItem, out IList flatItemList);
    }
}
