using System.Collections.Generic;
using System.Windows;

namespace CustomControls
{
    public class DocumentOpenedEventArgs : SolutionPresenterEventArgs
    {
        private static int HandlerCount = 0;
        public static void IncrementHandlerCount() { HandlerCount++; }
        public static void DecrementHandlerCount() { HandlerCount--; }
        public static bool HasHandler { get { return HandlerCount > 0; } }

        public DocumentOpenedEventArgs(RoutedEvent routedEvent, DocumentOpenedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        public IReadOnlyList<IDocumentPath> OpenedDocumentPathList { get { return (IReadOnlyList<IDocumentPath>)((DocumentOpenedEventContext)EventContext).OpenedDocumentPathList; } }

        public virtual void NotifyCompleted(IDocument openedDocument)
        {
            IDocumentOpenedCompletionArgs CompletionArgs = new DocumentOpenedCompletionArgs(openedDocument);
            NotifyEventCompleted(CompletionArgs);
        }

        public virtual void NotifyCompleted(IReadOnlyList<IDocument> openedDocumentList)
        {
            IDocumentOpenedCompletionArgs CompletionArgs = new DocumentOpenedCompletionArgs(openedDocumentList);
            NotifyEventCompleted(CompletionArgs);
        }
    }
}
