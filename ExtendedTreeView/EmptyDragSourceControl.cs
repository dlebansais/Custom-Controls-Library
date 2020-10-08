namespace CustomControls
{
    using System;
    using System.Collections;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;

    /// <summary>
    /// Represents a control source of a drag operation that is always empty.
    /// </summary>
    public class EmptyDragSourceControl : IDragSourceControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyDragSourceControl"/> class.
        /// </summary>
        public EmptyDragSourceControl()
        {
        }

        /// <summary>
        /// Gets the empty source control.
        /// </summary>
        public FrameworkElement SourceControl { get; } = new System.Windows.Controls.TextBlock();

        /// <summary>
        /// Gets the source container.
        /// </summary>
        public ExtendedTreeViewItemBase? SourceContainer { get; }

        /// <summary>
        /// Gets the drag source location.
        /// </summary>
        public MouseEventArgs? SourceLocation { get; }

        /// <summary>
        /// Gets the drag operation parent item.
        /// </summary>
        public object? DragParentItem { get; }

        /// <summary>
        /// Gets a value indicating whether a drop is allowed to be a copy operation.
        /// </summary>
        public bool AllowDropCopy { get; }

        /// <summary>
        /// Gets the root item for the operation.
        /// </summary>
        public object? RootItem { get; }

        /// <summary>
        /// Gets the list of dragged items.
        /// </summary>
        public IList? ItemList { get; }

        /// <summary>
        /// Gets the flat list of dragged items.
        /// </summary>
        public IList? FlatItemList { get; }

        /// <summary>
        /// Gets the GUID of the source.
        /// </summary>
        public Guid SourceGuid { get; }

        /// <summary>
        /// Gets the drag activity.
        /// </summary>
        public DragActivity DragActivity { get; }

        /// <summary>
        /// Gets.
        /// </summary>
        public event EventHandler<EventArgs>? DragActivityChanged;

        /// <summary>
        /// Invokes handlers for the <see cref="DragActivityChanged"/> event.
        /// </summary>
        protected void NotifyDragActivityChanged()
        {
            DragActivityChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Sets the drag state.
        /// </summary>
        /// <param name="canonicSelectedItemList">The list of selected items.</param>
        public void SetIsDragPossible(CanonicSelection canonicSelectedItemList)
        {
        }

        /// <summary>
        /// Clears the drag state.
        /// </summary>
        public void ClearIsDragPossible()
        {
        }

        /// <summary>
        /// Checks if a drag operation is possible.
        /// </summary>
        /// <returns>True if possible; Otherwise, false.</returns>
        public bool IsDragPossible()
        {
            return false;
        }

        /// <summary>
        /// Starts a drag operation after a mouse move.
        /// </summary>
        /// <param name="sourceLocation">The mouse move source.</param>
        public void DragAfterMouseMove(MouseEventArgs sourceLocation)
        {
        }

        /// <summary>
        /// Cancels a drag operation.
        /// </summary>
        public void CancelDrag()
        {
        }

        /// <summary>
        /// Sets the list of dragged items.
        /// </summary>
        /// <param name="rootItem">The root item.</param>
        /// <param name="flatItemList">The flat list of dragged items.</param>
        public void SetDragItemList(object rootItem, IList flatItemList)
        {
        }
    }
}
