namespace CustomControls
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Threading;

    public class DocumentExportedEventArgs : SolutionPresenterEventArgs
    {
        private static int HandlerCount = 0;
        public static void IncrementHandlerCount() { HandlerCount++; }
        public static void DecrementHandlerCount() { HandlerCount--; }
        public static bool HasHandler { get { return HandlerCount > 0; } }

        public DocumentExportedEventArgs(RoutedEvent routedEvent, DocumentExportedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        public DocumentOperation DocumentOperation { get { return ((DocumentExportedEventContext)EventContext).DocumentOperation; } }
        public IReadOnlyCollection<IDocument> ExportedDocumentList { get { return (IReadOnlyCollection<IDocument>)((DocumentExportedEventContext)EventContext).ExportedDocumentList; } }
        public bool IsDestinationFolder { get { return ((DocumentExportedEventContext)EventContext).IsDestinationFolder; } }
        public string DestinationPath { get { return ((DocumentExportedEventContext)EventContext).DestinationPath; } }

        public virtual void NotifyCompletedAsync(Dispatcher dispatcher)
        {
            IDocumentExportedCompletionArgs CompletionArgs = new DocumentExportedCompletionArgs();
            NotifyEventCompletedAsync(dispatcher, CompletionArgs);
        }
    }
}
