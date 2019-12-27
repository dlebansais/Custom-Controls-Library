using System;
using System.Collections;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace CustomControls
{
    public class EmptyDragSourceControl : IDragSourceControl
    {
        public EmptyDragSourceControl()
        {
        }

        public FrameworkElement SourceControl { get; } = new System.Windows.Controls.TextBlock();
        public ExtendedTreeViewItemBase? SourceContainer { get; }
        public MouseEventArgs? SourceLocation { get; }
        public object? DragParentItem { get; }
        public bool AllowDropCopy { get; }
        public object? RootItem { get; }
        public IList? ItemList { get; }
        public IList? FlatItemList { get; }
        public Guid SourceGuid { get; }
        public DragActivity DragActivity { get; }
        public event EventHandler<EventArgs>? DragActivityChanged;

        protected void NotifyDragActivityChanged()
        {
            DragActivityChanged?.Invoke(this, EventArgs.Empty);
        }

        public void SetIsDragPossible(CanonicSelection canonicSelectedItemList)
        {
        }

        public void ClearIsDragPossible()
        {
        }

        public bool IsDragPossible()
        {
            return false;
        }

        public void DragAfterMouseMove(MouseEventArgs sourceLocation)
        {
        }

        public void CancelDrag()
        {
        }

        public void SetDragItemList(object rootItem, IList flatItemList)
        { 
        }
    }
}
