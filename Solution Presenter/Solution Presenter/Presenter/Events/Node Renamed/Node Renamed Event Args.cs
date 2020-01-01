namespace CustomControls
{
    using System.Windows;
    using System.Windows.Threading;

    /// <summary>
    /// Represents the event data for a node renamed event.
    /// </summary>
    public class NodeRenamedEventArgs : SolutionPresenterEventArgs<NodeRenamedEventArgs>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeRenamedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The event that occured.</param>
        /// <param name="eventContext">The event context.</param>
        public NodeRenamedEventArgs(RoutedEvent routedEvent, NodeRenamedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        /// <summary>
        /// Gets the path to the node.
        /// </summary>
        public ITreeNodePath Path { get { return ((NodeRenamedEventContext)EventContext).Path; } }

        /// <summary>
        /// Gets the new name.
        /// </summary>
        public string NewName { get { return ((NodeRenamedEventContext)EventContext).NewName; } }

        /// <summary>
        /// Gets the properties of the root object.
        /// </summary>
        public IRootProperties RootProperties { get { return ((NodeRenamedEventContext)EventContext).RootProperties; } }

        /// <summary>
        /// Notifies handlers that the operation is completed.
        /// </summary>
        /// <param name="dispatcher">The window dispatcher.</param>
        public virtual void NotifyCompletedAsync(Dispatcher dispatcher)
        {
            INodeRenamedCompletionArgs CompletionArgs = new NodeRenamedCompletionArgs();
            NotifyEventCompletedAsync(dispatcher, CompletionArgs);
        }
    }
}
