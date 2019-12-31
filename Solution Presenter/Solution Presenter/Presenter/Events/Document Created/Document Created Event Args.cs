namespace CustomControls
{
    using System.Windows;
    using System.Windows.Threading;

    public class DocumentCreatedEventArgs : SolutionPresenterEventArgs
    {
        private static int HandlerCount = 0;
        public static void IncrementHandlerCount() { HandlerCount++; }
        public static void DecrementHandlerCount() { HandlerCount--; }
        public static bool HasHandler { get { return HandlerCount > 0; } }

        public DocumentCreatedEventArgs(RoutedEvent routedEvent, DocumentCreatedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        public IFolderPath DestinationFolderPath { get { return ((DocumentCreatedEventContext)EventContext).DestinationFolderPath; } }
        public IDocumentType DocumentType { get { return ((DocumentCreatedEventContext)EventContext).DocumentType; } }
        public IRootProperties RootProperties { get { return ((DocumentCreatedEventContext)EventContext).RootProperties; } }

        public virtual void NotifyCompletedAsync(Dispatcher dispatcher, IItemPath newItemPath, IItemProperties newItemProperties)
        {
            IDocumentCreatedCompletionArgs CompletionArgs = new DocumentCreatedCompletionArgs(newItemPath, newItemProperties);
            NotifyEventCompletedAsync(dispatcher, CompletionArgs);
        }
    }
}
