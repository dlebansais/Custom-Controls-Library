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
        /// If the event is a preview, <see cref="CloneList"/> is always empty.
        /// </summary>
        /// <param name="routedEvent">The event that occured.</param>
        /// <param name="dragSource">The drag source.</param>
        /// <param name="dropDestinationItem">The destination of the drop.</param>
        /// <param name="effect">The drop effect.</param>
        /// <param name="cloneList">The list of items to clone.</param>
        internal DropCompletedEventArgs(RoutedEvent routedEvent, IDragSourceControl dragSource, object dropDestinationItem, DragDropEffects effect, IList cloneList)
            : base(routedEvent, dragSource)
        {
            DropDestinationItem = dropDestinationItem;
            Effect = effect;
            CloneList = cloneList;
        }

        /// <summary>
        /// Gets the destination of the drop.
        /// </summary>
        public object DropDestinationItem { get; }

        /// <summary>
        /// Gets drop effect.
        /// </summary>
        public DragDropEffects Effect { get; }

        /// <summary>
        /// Gets the list of items to clone.
        /// </summary>
        public IList CloneList { get; }
    }
}
