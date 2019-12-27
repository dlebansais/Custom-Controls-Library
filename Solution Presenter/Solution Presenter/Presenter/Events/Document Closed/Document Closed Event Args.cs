using System.Collections.Generic;
using System.Windows;

namespace CustomControls
{
    public class DocumentClosedEventArgs : SolutionPresenterEventArgs
    {
        private static int HandlerCount = 0;
        public static void IncrementHandlerCount() { HandlerCount++; }
        public static void DecrementHandlerCount() { HandlerCount--; }
        public static bool HasHandler { get { return HandlerCount > 0; } }

        public DocumentClosedEventArgs(RoutedEvent routedEvent, DocumentClosedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        public DocumentOperation DocumentOperation { get { return ((DocumentClosedEventContext)EventContext).DocumentOperation; } }
        public IReadOnlyList<IDocument> ClosedDocumentList { get { return (IReadOnlyList<IDocument>)((DocumentClosedEventContext)EventContext).ClosedDocumentList; } }
        public object? ClientInfo { get { return ((DocumentClosedEventContext)EventContext).ClientInfo; } }

        public virtual void NotifyCompleted()
        {
            IDocumentClosedCompletionArgs CompletionArgs = new DocumentClosedCompletionArgs();
            NotifyEventCompleted(CompletionArgs);
        }
    }
}
