using System;
using System.Collections;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace CustomControls
{
    public interface IDragSourceControl
    {
        FrameworkElement SourceControl { get; }
        ExtendedTreeViewItemBase SourceContainer { get; }
        MouseEventArgs SourceLocation { get; }
        object DragParentItem { get; }
        bool AllowDropCopy { get; }
        object RootItem { get; }
        IList ItemList { get; }
        IList FlatItemList { get; }
        Guid SourceGuid { get; }
        DragActivity DragActivity { get; }
        event EventHandler<EventArgs> DragActivityChanged;
        void SetIsDragPossible(CanonicSelection canonicSelectedItemList);
        void ClearIsDragPossible();
        bool IsDragPossible();
        void DragAfterMouseMove(MouseEventArgs sourceLocation);
        void CancelDrag();
        void SetDragItemList(object rootItem, IList flatItemList);
    }

    public class DragSourceControl : IDragSourceControl
    {
        public static readonly TimeSpan DefaultDragDelay = TimeSpan.FromSeconds(0.4);

        public DragSourceControl(FrameworkElement sourceControl)
        {
            this.SourceControl = sourceControl;

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

        public FrameworkElement SourceControl { get; private set; }
        public ExtendedTreeViewItemBase SourceContainer { get; private set; }
        public MouseEventArgs SourceLocation { get; private set; }
        public object DragParentItem { get; private set; }
        public bool AllowDropCopy { get; private set; }
        public object RootItem { get; private set; }
        public IList ItemList { get; private set; }
        public IList FlatItemList { get; private set; }
        public Guid SourceGuid { get; private set; }
        public DragActivity DragActivity { get; private set; }

        private DispatcherOperation InitiateDragOperation;

        public event EventHandler<EventArgs> DragActivityChanged;
        protected void NotifyDragActivityChanged()
        {
            if (DragActivityChanged != null)
                DragActivityChanged(this, EventArgs.Empty);
        }

        public virtual void SetIsDragPossible(CanonicSelection canonicSelectedItemList)
        {
            if (canonicSelectedItemList != null)
            {
                this.DragParentItem = canonicSelectedItemList.DraggedItemParent;
                this.ItemList = canonicSelectedItemList.ItemList;
                this.AllowDropCopy = canonicSelectedItemList.AllItemsCloneable;
            }
        }

        public virtual void ClearIsDragPossible()
        {
            this.DragParentItem = null;
            this.AllowDropCopy = false;
        }

        public virtual bool IsDragPossible()
        {
            return DragParentItem != null;
        }

        public virtual void DragAfterMouseMove(MouseEventArgs sourceLocation)
        {
            this.SourceLocation = sourceLocation;

            InitiateDrag();
        }

        public virtual void CancelDrag()
        {
            DragActivity = DragActivity.Canceled;
        }

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
    }
}
