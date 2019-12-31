namespace CustomControls
{
    using System.Windows;
    using System.Windows.Threading;

    public class NodeMovedEventArgs : SolutionPresenterEventArgs
    {
        private static int HandlerCount = 0;
        public static void IncrementHandlerCount() { HandlerCount++; }
        public static void DecrementHandlerCount() { HandlerCount--; }
        public static bool HasHandler { get { return HandlerCount > 0; } }

        public NodeMovedEventArgs(RoutedEvent routedEvent, NodeMovedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        public ITreeNodePath Path { get { return ((NodeMovedEventContext)EventContext).Path; } }
        public IFolderPath NewParentPath { get { return ((NodeMovedEventContext)EventContext).NewParentPath; } }
        public IRootProperties RootProperties { get { return ((NodeMovedEventContext)EventContext).RootProperties; } }

        public virtual void NotifyCompletedAsync(Dispatcher dispatcher)
        {
            INodeMovedCompletionArgs CompletionArgs = new NodeMovedCompletionArgs();
            NotifyEventCompletedAsync(dispatcher, CompletionArgs);
        }
    }
}
