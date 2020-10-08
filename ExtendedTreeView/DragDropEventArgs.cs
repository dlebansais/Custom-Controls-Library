namespace CustomControls
{
    using System.Collections;
    using System.Windows;

    /// <summary>
    /// Represents data for a drag drop event.
    /// </summary>
    public class DragDropEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DragDropEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The event that occured.</param>
        /// <param name="dragSource">The drag source.</param>
        internal DragDropEventArgs(RoutedEvent routedEvent, IDragSourceControl dragSource)
            : base(routedEvent)
        {
            DragSource = dragSource;
        }

        private IDragSourceControl DragSource;

        /// <summary>
        /// Gets the parent of the dragged item.
        /// </summary>
        public virtual object? DragParentItem { get { return DragSource.DragParentItem; } }

        /// <summary>
        /// Gets a value indicating whether copy on drop is allowed.
        /// </summary>
        public virtual bool AllowDropCopy { get { return DragSource.AllowDropCopy; } }

        /// <summary>
        /// Gets the root item.
        /// </summary>
        public virtual object? RootItem { get { return DragSource.RootItem; } }

        /// <summary>
        /// Gets the list of dragged items.
        /// </summary>
        public virtual IList? ItemList { get { return DragSource.ItemList; } }

        /// <summary>
        /// Gets the flat list of dragged items.
        /// </summary>
        public virtual IList? FlatItemList { get { return DragSource.FlatItemList; } }
    }
}
