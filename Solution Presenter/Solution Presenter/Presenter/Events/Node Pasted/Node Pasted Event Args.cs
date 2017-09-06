using System.Windows;
using System.Windows.Threading;

namespace CustomControls
{
    public class NodePastedEventArgs : SolutionPresenterEventArgs
    {
        private static int HandlerCount = 0;
        public static void IncrementHandlerCount() { HandlerCount++; }
        public static void DecrementHandlerCount() { HandlerCount--; }
        public static bool HasHandler { get { return HandlerCount > 0; } }

        public NodePastedEventArgs(RoutedEvent routedEvent, NodePastedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        public ITreeNodePath Path { get { return ((NodePastedEventContext)EventContext).Path; } }
        public IFolderPath ParentPath { get { return ((NodePastedEventContext)EventContext).ParentPath; } }
        public IRootProperties RootProperties { get { return ((NodePastedEventContext)EventContext).RootProperties; } }

        public virtual void NotifyCompletedAsync(Dispatcher dispatcher, ITreeNodePath newPath, ITreeNodeProperties newProperties)
        {
            INodePastedCompletionArgs CompletionArgs = new NodePastedCompletionArgs(newPath, newProperties);
            NotifyEventCompletedAsync(dispatcher, CompletionArgs);
        }
    }
}
