using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

namespace CustomControls
{
    public class DocumentRemovedEventArgs : SolutionPresenterEventArgs
    {
        private static int HandlerCount = 0;
        public static void IncrementHandlerCount() { HandlerCount++; }
        public static void DecrementHandlerCount() { HandlerCount--; }
        public static bool HasHandler { get { return HandlerCount > 0; } }

        public DocumentRemovedEventArgs(RoutedEvent routedEvent, DocumentRemovedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        public IRootPath RootPath { get { return ((DocumentRemovedEventContext)EventContext).RootPath; } }
        public IReadOnlyDictionary<ITreeNodePath, IPathConnection> DeletedTree { get { return ((DocumentRemovedEventContext)EventContext).DeletedTree; } }
        public object ClientInfo { get { return ((DocumentRemovedEventContext)EventContext).ClientInfo; } }
        
        public virtual void NotifyCompletedAsync(Dispatcher dispatcher)
        {
            IDocumentRemovedCompletionArgs CompletionArgs = new DocumentRemovedCompletionArgs();
            NotifyEventCompletedAsync(dispatcher, CompletionArgs);
        }
    }
}
