using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

namespace CustomControls
{
    public class NodeRenamedEventArgs : SolutionPresenterEventArgs
    {
        private static int HandlerCount = 0;
        public static void IncrementHandlerCount() { HandlerCount++; }
        public static void DecrementHandlerCount() { HandlerCount--; }
        public static bool HasHandler { get { return HandlerCount > 0; } }

        public NodeRenamedEventArgs(RoutedEvent routedEvent, NodeRenamedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        public ITreeNodePath Path { get { return ((NodeRenamedEventContext)EventContext).Path; } }
        public string NewName { get { return ((NodeRenamedEventContext)EventContext).NewName; } }
        public IRootProperties RootProperties { get { return ((NodeRenamedEventContext)EventContext).RootProperties; } }

        public virtual void NotifyCompletedAsync(Dispatcher dispatcher)
        {
            INodeRenamedCompletionArgs CompletionArgs = new NodeRenamedCompletionArgs();
            NotifyEventCompletedAsync(dispatcher, CompletionArgs);
        }
    }
}
