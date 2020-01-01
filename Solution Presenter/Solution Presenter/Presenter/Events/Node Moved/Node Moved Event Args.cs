namespace CustomControls
{
    using System.Windows;
    using System.Windows.Threading;

    /// <summary>
    /// Represents the event data for a node moved event.
    /// </summary>
    public class NodeMovedEventArgs : SolutionPresenterEventArgs<NodeMovedEventArgs>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeMovedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The event that occured.</param>
        /// <param name="eventContext">The event context.</param>
        public NodeMovedEventArgs(RoutedEvent routedEvent, NodeMovedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        /// <summary>
        /// Gets the path of the moved node.
        /// </summary>
        public ITreeNodePath Path { get { return ((NodeMovedEventContext)EventContext).Path; } }

        /// <summary>
        /// Gets the parent path.
        /// </summary>
        public IFolderPath NewParentPath { get { return ((NodeMovedEventContext)EventContext).NewParentPath; } }

        /// <summary>
        /// Gets properties of the root object.
        /// </summary>
        public IRootProperties RootProperties { get { return ((NodeMovedEventContext)EventContext).RootProperties; } }

        /// <summary>
        /// Notifies handlers that the operation is completed.
        /// </summary>
        /// <param name="dispatcher">The window dispatcher.</param>
        public virtual void NotifyCompletedAsync(Dispatcher dispatcher)
        {
            INodeMovedCompletionArgs CompletionArgs = new NodeMovedCompletionArgs();
            NotifyEventCompletedAsync(dispatcher, CompletionArgs);
        }
    }
}
