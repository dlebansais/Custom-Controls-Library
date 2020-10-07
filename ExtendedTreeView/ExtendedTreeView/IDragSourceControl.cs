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

    /// <summary>
    /// Represents a control providing drag and drop features.
    /// </summary>
    public class DragSourceControl : IDragSourceControl
    {
        /// <summary>
        /// Default delay for starting a drag operation.
        /// </summary>
        public static readonly TimeSpan DefaultDragDelay = TimeSpan.FromSeconds(0.4);

        /// <summary>
        /// Initializes a new instance of the <see cref="DragSourceControl"/> class.
        /// </summary>
        /// <param name="sourceControl">The control source of the drag.</param>
        public DragSourceControl(FrameworkElement sourceControl)
        {
            SourceControl = sourceControl;

            SourceContainer = null;
            SourceLocation = null;
            DragParentItem = null;
            AllowDropCopy = false;
            RootItem = null;
            ItemList = null;
            FlatItemList = null;
            SourceGuid = Guid.NewGuid();
            DragActivity = DragActivity.Idle;
            InitiateDragOperation = null;
        }

        /// <summary>
        /// Gets the control source of the dragged content.
        /// </summary>
        public FrameworkElement SourceControl { get; private set; }

        /// <summary>
        /// Gets the container where <see cref="SourceControl"/> can be found.
        /// </summary>
        public ExtendedTreeViewItemBase? SourceContainer { get; private set; }

        /// <summary>
        /// Gets the source location of a drag event.
        /// </summary>
        public MouseEventArgs? SourceLocation { get; private set; }

        /// <summary>
        /// Gets the parent item of the dragged item.
        /// </summary>
        public object? DragParentItem { get; private set; }

        /// <summary>
        /// Gets a value indicating whether copy is allowed on drop.
        /// </summary>
        public bool AllowDropCopy { get; private set; }

        /// <summary>
        /// Gets the root of dragged items.
        /// </summary>
        public object? RootItem { get; private set; }

        /// <summary>
        /// Gets the list of dragged items.
        /// </summary>
        public IList? ItemList { get; private set; }

        /// <summary>
        /// Gets the flat list of dragged items.
        /// </summary>
        public IList? FlatItemList { get; private set; }

        /// <summary>
        /// Gets the GUID of the source.
        /// </summary>
        public Guid SourceGuid { get; private set; }

        /// <summary>
        /// Gets the activity state of the drag operation.
        /// </summary>
        public DragActivity DragActivity { get; private set; }

        /// <summary>
        /// Occurs when <see cref="DragActivity"/> changed.
        /// </summary>
        public event EventHandler<EventArgs>? DragActivityChanged;

        /// <summary>
        /// Invokes handlers of the <see cref="DragActivityChanged"/> event.
        /// </summary>
        protected void NotifyDragActivityChanged()
        {
            DragActivityChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Changes the drag activity.
        /// </summary>
        /// <param name="canonicSelectedItemList">The lost of selected items.</param>
        public virtual void SetIsDragPossible(CanonicSelection canonicSelectedItemList)
        {
            if (canonicSelectedItemList != null)
            {
                DragParentItem = canonicSelectedItemList.DraggedItemParent;
                ItemList = canonicSelectedItemList.ItemList;
                AllowDropCopy = canonicSelectedItemList.AllItemsCloneable;
            }
        }

        /// <summary>
        /// Clears the value of IsDragPossible.
        /// </summary>
        public virtual void ClearIsDragPossible()
        {
            DragParentItem = null;
            AllowDropCopy = false;
        }

        /// <summary>
        /// Gets the value indicating if drag is possible.
        /// </summary>
        /// <returns>True if drag is possible; otherwise, false.</returns>
        public virtual bool IsDragPossible()
        {
            return DragParentItem != null;
        }

        /// <summary>
        /// Called when a drag should begin after the mouse moved.
        /// </summary>
        /// <param name="sourceLocation">The source location.</param>
        public virtual void DragAfterMouseMove(MouseEventArgs sourceLocation)
        {
            SourceLocation = sourceLocation;

            InitiateDrag();
        }

        /// <summary>
        /// Cancels the drag operation.
        /// </summary>
        public virtual void CancelDrag()
        {
            DragActivity = DragActivity.Canceled;
        }

        /// <summary>
        /// Sets the dragged items.
        /// </summary>
        /// <param name="rootItem">The root item.</param>
        /// <param name="flatItemList">The flat list of dragged items.</param>
        public virtual void SetDragItemList(object rootItem, IList flatItemList)
        {
            this.RootItem = rootItem;
            this.FlatItemList = flatItemList;
        }

        private void InitiateDrag()
        {
            if (InitiateDragOperation == null || InitiateDragOperation.Status == DispatcherOperationStatus.Completed)
                InitiateDragOperation = SourceControl.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new DragInitiatedHandler(OnDragInitiated));
        }

        private delegate void DragInitiatedHandler();
        private void OnDragInitiated()
        {
            DragActivity = DragActivity.Starting;
            NotifyDragActivityChanged();

            if (DragActivity != DragActivity.Canceled)
            {
                DragActivity = DragActivity.Started;
                NotifyDragActivityChanged();
            }
        }

        private DispatcherOperation? InitiateDragOperation;
    }
}
