namespace CustomControls
{
    using System.Windows;
    using System.Windows.Threading;

    /// <summary>
    /// Represents the event data for a node pasted event.
    /// </summary>
    public class NodePastedEventArgs : SolutionPresenterEventArgs<NodePastedEventArgs>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NodePastedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The event that occured.</param>
        /// <param name="eventContext">The event context.</param>
        public NodePastedEventArgs(RoutedEvent routedEvent, NodePastedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        /// <summary>
        /// Gets the path to the pasted node.
        /// </summary>
        public ITreeNodePath Path { get { return ((NodePastedEventContext)EventContext).Path; } }

        /// <summary>
        /// Gets the path to the parent.
        /// </summary>
        public IFolderPath ParentPath { get { return ((NodePastedEventContext)EventContext).ParentPath; } }

        /// <summary>
        /// Gets the properties of the root object.
        /// </summary>
        public IRootProperties RootProperties { get { return ((NodePastedEventContext)EventContext).RootProperties; } }

        /// <summary>
        /// Notifies handlers that the operation is completed.
        /// </summary>
        /// <param name="dispatcher">The window dispatcher.</param>
        /// <param name="newPath">The path to the new node.</param>
        /// <param name="newProperties">The properties of the new node.</param>
        public virtual void NotifyCompletedAsync(Dispatcher dispatcher, ITreeNodePath newPath, ITreeNodeProperties newProperties)
        {
            INodePastedCompletionArgs CompletionArgs = new NodePastedCompletionArgs(newPath, newProperties);
            NotifyEventCompletedAsync(dispatcher, CompletionArgs);
        }
    }
}
