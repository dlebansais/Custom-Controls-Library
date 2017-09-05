using System.Collections.Generic;
using System.Windows;

namespace CustomControls
{
    public class DocumentSelectedEventArgs : SolutionPresenterEventArgs
    {
        private static int HandlerCount = 0;
        public static void IncrementHandlerCount() { HandlerCount++; }
        public static void DecrementHandlerCount() { HandlerCount--; }
        public static bool HasHandler { get { return HandlerCount > 0; } }

        public DocumentSelectedEventArgs(RoutedEvent routedEvent, DocumentSelectedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        public virtual void NotifyCompleted(IList<IDocumentPath> documentPathList)
        {
            IDocumentSelectedCompletionArgs CompletionArgs = new DocumentSelectedCompletionArgs(documentPathList);
            NotifyEventCompleted(CompletionArgs);
        }
    }
}
