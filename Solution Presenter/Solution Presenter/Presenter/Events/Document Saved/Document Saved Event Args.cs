using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

namespace CustomControls
{
    public class DocumentSavedEventArgs : SolutionPresenterEventArgs
    {
        private static int HandlerCount = 0;
        public static void IncrementHandlerCount() { HandlerCount++; }
        public static void DecrementHandlerCount() { HandlerCount--; }
        public static bool HasHandler { get { return HandlerCount > 0; } }

        public DocumentSavedEventArgs(RoutedEvent routedEvent, DocumentSavedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        public DocumentOperation DocumentOperation { get { return ((DocumentSavedEventContext)EventContext).DocumentOperation; } }
        public IDocument SavedDocument { get { return ((DocumentSavedEventContext)EventContext).SavedDocument; } }

        public virtual void NotifyCompletedAsync(Dispatcher dispatcher)
        {
            IDocumentSavedCompletionArgs CompletionArgs = new DocumentSavedCompletionArgs();
            NotifyEventCompletedAsync(dispatcher, CompletionArgs);
        }
    }
}
