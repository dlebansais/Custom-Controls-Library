namespace CustomControls
{
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// Represents a control with a tree of nodes that can be moved around with Drag and Drop.
    /// </summary>
    public abstract partial class ExtendedTreeViewBase : MultiSelector
    {
        #region Preview Collection Modified
        /// <summary>
        /// Identifies the <see cref="PreviewCollectionModified"/> routed event.
        /// </summary>
        public static readonly RoutedEvent PreviewCollectionModifiedEvent = EventManager.RegisterRoutedEvent("PreviewCollectionModified", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(ExtendedTreeViewBase));

        /// <summary>
        /// Occurs before the content collection is modified.
        /// </summary>
        public event RoutedEventHandler PreviewCollectionModified
        {
            add { AddHandler(PreviewCollectionModifiedEvent, value); }
            remove { RemoveHandler(PreviewCollectionModifiedEvent, value); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="PreviewCollectionModified"/> event.
        /// </summary>
        /// <param name="treeViewCollectionOperation">The modifying operation.</param>
        protected virtual void NotifyPreviewCollectionModified(TreeViewCollectionOperation treeViewCollectionOperation)
        {
            RaiseEvent(new TreeViewCollectionModifiedEventArgs(PreviewCollectionModifiedEvent, treeViewCollectionOperation, Items.Count));
        }
        #endregion
        #region Collection Modified
        /// <summary>
        /// Identifies the <see cref="CollectionModified"/> routed event.
        /// </summary>
        public static readonly RoutedEvent CollectionModifiedEvent = EventManager.RegisterRoutedEvent("CollectionModified", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ExtendedTreeViewBase));

        /// <summary>
        /// Occurs after the content collection is modified.
        /// </summary>
        public event RoutedEventHandler CollectionModified
        {
            add { AddHandler(CollectionModifiedEvent, value); }
            remove { RemoveHandler(CollectionModifiedEvent, value); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="CollectionModified"/> event.
        /// </summary>
        /// <param name="treeViewCollectionOperation">The modifying operation.</param>
        protected virtual void NotifyCollectionModified(TreeViewCollectionOperation treeViewCollectionOperation)
        {
            RaiseEvent(new TreeViewCollectionModifiedEventArgs(CollectionModifiedEvent, treeViewCollectionOperation, Items.Count));
        }
        #endregion
        #region Drag Starting
        /// <summary>
        /// Identifies the <see cref="DragStarting"/> routed event.
        /// </summary>
        public static readonly RoutedEvent DragStartingEvent = EventManager.RegisterRoutedEvent("DragStarting", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ExtendedTreeViewBase));

        /// <summary>
        /// Occurs when drag is starting.
        /// </summary>
        public event RoutedEventHandler DragStarting
        {
            add { AddHandler(DragStartingEvent, value); }
            remove { RemoveHandler(DragStartingEvent, value); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="DragStarting"/> event.
        /// </summary>
        /// <param name="cancellation">The cancellation token.</param>
        protected virtual void NotifyDragStarting(CancellationToken cancellation)
        {
            DragStartingEventArgs Args = new DragStartingEventArgs(DragStartingEvent, DragSource, cancellation);
            RaiseEvent(Args);
        }
        #endregion
        #region Drop Check
        /// <summary>
        /// Identifies the <see cref="DropCheck"/> routed event.
        /// </summary>
        public static readonly RoutedEvent DropCheckEvent = EventManager.RegisterRoutedEvent("DropCheck", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ExtendedTreeViewBase));

        /// <summary>
        /// Occurs when checking if drop is permitted.
        /// </summary>
        public event RoutedEventHandler DropCheck
        {
            add { AddHandler(DropCheckEvent, value); }
            remove { RemoveHandler(DropCheckEvent, value); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="DropCheck"/> event.
        /// </summary>
        /// <param name="dropDestinationItem">The drop destination item.</param>
        /// <param name="permission">The drop permission token.</param>
        protected virtual void NotifyDropCheck(object dropDestinationItem, PermissionToken permission)
        {
            DropCheckEventArgs Args = new DropCheckEventArgs(DropCheckEvent, DragSource, dropDestinationItem, permission);
            RaiseEvent(Args);
        }
        #endregion
        #region Preview Drop Completed
        /// <summary>
        /// Identifies the <see cref="PreviewDropCompleted"/> routed event.
        /// </summary>
        public static readonly RoutedEvent PreviewDropCompletedEvent = EventManager.RegisterRoutedEvent("PreviewDropCompleted", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(ExtendedTreeViewBase));

        /// <summary>
        /// Occurs before a drop operation is completed.
        /// </summary>
        public event RoutedEventHandler PreviewDropCompleted
        {
            add { AddHandler(PreviewDropCompletedEvent, value); }
            remove { RemoveHandler(PreviewDropCompletedEvent, value); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="PreviewDropCompleted"/> event.
        /// </summary>
        /// <param name="dropDestinationItem">The drop destination item.</param>
        /// <param name="cloneList">The list of dropped items.</param>
        protected virtual void NotifyPreviewDropCompleted(object dropDestinationItem, IList? cloneList)
        {
            DropCompletedEventArgs Args = new DropCompletedEventArgs(PreviewDropCompletedEvent, DragSource, dropDestinationItem, cloneList);
            RaiseEvent(Args);
        }
        #endregion
        #region Drop Completed
        /// <summary>
        /// Identifies the <see cref="DropCompleted"/> routed event.
        /// </summary>
        public static readonly RoutedEvent DropCompletedEvent = EventManager.RegisterRoutedEvent("DropCompleted", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ExtendedTreeViewBase));

        /// <summary>
        /// Occurs after a drop operation is completed.
        /// </summary>
        public event RoutedEventHandler DropCompleted
        {
            add { AddHandler(DropCompletedEvent, value); }
            remove { RemoveHandler(DropCompletedEvent, value); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="DropCompleted"/> event.
        /// </summary>
        /// <param name="dropDestinationItem">The drop destination item.</param>
        /// <param name="cloneList">The list of dropped items.</param>
        protected virtual void NotifyDropCompleted(object dropDestinationItem, IList? cloneList)
        {
            DropCompletedEventArgs Args = new DropCompletedEventArgs(DropCompletedEvent, DragSource, dropDestinationItem, cloneList);
            RaiseEvent(Args);
        }
        #endregion
    }
}
