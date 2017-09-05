using System.Collections.Generic;
using System.Windows;

namespace CustomControls
{
    public class SolutionDeletedEventArgs : SolutionPresenterEventArgs
    {
        private static int HandlerCount = 0;
        public static void IncrementHandlerCount() { HandlerCount++; }
        public static void DecrementHandlerCount() { HandlerCount--; }
        public static bool HasHandler { get { return HandlerCount > 0; } }

        public SolutionDeletedEventArgs(RoutedEvent routedEvent, SolutionDeletedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        public IRootPath DeletedRootPath { get { return ((SolutionDeletedEventContext)EventContext).DeletedRootPath; } }
        public IReadOnlyCollection<ITreeNodePath> DeletedTree { get { return ((SolutionDeletedEventContext)EventContext).DeletedTree; } }

        public virtual void NotifyCompleted()
        {
            ISolutionDeletedCompletionArgs CompletionArgs = new SolutionDeletedCompletionArgs();
            NotifyEventCompleted(CompletionArgs);
        }
    }
}
