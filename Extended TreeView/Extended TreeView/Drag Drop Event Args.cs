using System.Collections;
using System.Windows;

namespace CustomControls
{
    public class DragDropEventArgs : RoutedEventArgs
    {
        internal DragDropEventArgs(RoutedEvent routedEvent, IDragSourceControl dragSource)
            : base(routedEvent)
        {
            DragSource = dragSource;
        }

        private IDragSourceControl DragSource;

        public virtual object? DragParentItem { get { return DragSource.DragParentItem; } }
        public virtual bool AllowDropCopy { get { return DragSource.AllowDropCopy; } }
        public virtual object? RootItem { get { return DragSource.RootItem; } }
        public virtual IList? ItemList { get { return DragSource.ItemList; } }
        public virtual IList? FlatItemList { get { return DragSource.FlatItemList; } }
    }
}
