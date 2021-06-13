namespace CustomControls
{
    using System;
    using System.Collections;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;

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
        /// Gets the container where <see cref="SourceControl"/> can be found.
        /// </summary>
        ExtendedTreeViewItemBase? SourceContainer { get; }

        /// <summary>
        /// Gets the source location of a drag event.
        /// </summary>
        MouseEventArgs? SourceLocation { get; }

        /// <summary>
        /// Gets the parent item of the dragged item.
        /// </summary>
        object? DragParentItem { get; }

        /// <summary>
        /// Gets a value indicating whether copy is allowed on drop.
        /// </summary>
        bool AllowDropCopy { get; }

        /// <summary>
        /// Gets the root of dragged items.
        /// </summary>
        object? RootItem { get; }

        /// <summary>
        /// Gets the list of dragged items.
        /// </summary>
        IList? ItemList { get; }

        /// <summary>
        /// Gets the flat list of dragged items.
        /// </summary>
        IList? FlatItemList { get; }

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
        /// Clears the value of IsDragPossible.
        /// </summary>
        void ClearIsDragPossible();

        /// <summary>
        /// Gets the value indicating if drag is possible.
        /// </summary>
        /// <returns>True if drag is possible; otherwise, false.</returns>
        bool IsDragPossible();

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
        /// Sets the dragged items.
        /// </summary>
        /// <param name="rootItem">The root item.</param>
        /// <param name="flatItemList">The flat list of dragged items.</param>
        void SetDragItemList(object rootItem, IList flatItemList);
    }
}
