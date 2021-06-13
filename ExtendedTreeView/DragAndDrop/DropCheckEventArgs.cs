namespace CustomControls
{
    using System.Windows;

    /// <summary>
    /// Represents the data of a drop check event.
    /// </summary>
    public class DropCheckEventArgs : DragDropEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DropCheckEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The event that occured.</param>
        /// <param name="dragSource">The drag source.</param>
        /// <param name="dropDestinationItem">The destination of the drop.</param>
        /// <param name="permission">The permission token.</param>
        internal DropCheckEventArgs(RoutedEvent routedEvent, IDragSourceControl dragSource, object dropDestinationItem, PermissionToken permission)
            : base(routedEvent, dragSource)
        {
            DropDestinationItem = dropDestinationItem;
            Permission = permission;
        }

        /// <summary>
        /// Gets the destination of the drop.
        /// </summary>
        public object DropDestinationItem { get; private set; }

        /// <summary>
        /// Gets the permission token.
        /// </summary>
        protected PermissionToken Permission { get; private set; }

        /// <summary>
        /// Deny the drop.
        /// </summary>
        public void Deny()
        {
            Permission.Deny();
        }
    }
}
