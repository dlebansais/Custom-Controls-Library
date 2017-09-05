using System.Windows;

namespace CustomControls
{
    public class DropCheckEventArgs : DragDropEventArgs
    {
        internal DropCheckEventArgs(RoutedEvent routedEvent, IDragSourceControl dragSource, object dropDestinationItem, PermissionToken permission)
            : base(routedEvent, dragSource)
        {
            this.DropDestinationItem = dropDestinationItem;
            this.Permission = permission;
        }

        protected PermissionToken Permission { get; private set; }

        public object DropDestinationItem { get; private set; }

        public void Deny()
        {
            Permission.Deny();
        }
    }
}
