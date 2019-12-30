namespace CustomControls
{
    using System.Collections;
    using System.Windows;

    /// <summary>
    /// Represents the data of a drop completed event.
    /// </summary>
    public class DropCompletedEventArgs : DragDropEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DropCompletedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The event that occured.</param>
        /// <param name="dragSource">The drag source.</param>
        /// <param name="dropDestinationItem">The destination of the drop.</param>
        /// <param name="cloneList">The list of items to clone.</param>
        internal DropCompletedEventArgs(RoutedEvent routedEvent, IDragSourceControl dragSource, object dropDestinationItem, IList? cloneList)
            : base(routedEvent, dragSource)
        {
            DropDestinationItem = dropDestinationItem;
            CloneList = cloneList;
        }

        /// <summary>
        /// Gets the destination of the drop.
        /// </summary>
        public object DropDestinationItem { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the drop is a copy or move.
        /// </summary>
        public bool IsCopy { get { return CloneList != null; } }

        /// <summary>
        /// Gets the list of items to clone.
        /// </summary>
        public IList? CloneList { get; private set; }
    }
}
