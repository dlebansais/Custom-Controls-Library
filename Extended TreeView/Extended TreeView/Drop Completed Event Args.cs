using System.Collections;
using System.Windows;

namespace CustomControls
{
    public class DropCompletedEventArgs : DragDropEventArgs
    {
        internal DropCompletedEventArgs(RoutedEvent routedEvent, IDragSourceControl dragSource, object dropDestinationItem, IList? cloneList)
            : base(routedEvent, dragSource)
        {
            DropDestinationItem = dropDestinationItem;
            CloneList = cloneList;
        }

        public object DropDestinationItem { get; private set; }
        public bool IsCopy { get { return CloneList != null; } }
        public IList? CloneList { get; private set; }
    }
}
