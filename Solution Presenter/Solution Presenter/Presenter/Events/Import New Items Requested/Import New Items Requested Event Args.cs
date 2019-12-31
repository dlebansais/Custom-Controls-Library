namespace CustomControls
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Threading;

    public class ImportNewItemsRequestedEventArgs : SolutionPresenterEventArgs
    {
        private static int HandlerCount = 0;
        public static void IncrementHandlerCount() { HandlerCount++; }
        public static void DecrementHandlerCount() { HandlerCount--; }
        public static bool HasHandler { get { return HandlerCount > 0; } }

        public ImportNewItemsRequestedEventArgs(RoutedEvent routedEvent, ImportNewItemsRequestedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        public IReadOnlyDictionary<object, IDocumentType> ImportedDocumentTable { get { return (IReadOnlyDictionary<object, IDocumentType>)((ImportNewItemsRequestedEventContext)EventContext).ImportedDocumentTable; } }

        public virtual void NotifyCompletedAsync(Dispatcher dispatcher, IReadOnlyDictionary<object, IDocumentPath> openedDocumentTable)
        {
            IImportNewItemsRequestedCompletionArgs CompletionArgs = new ImportNewItemsRequestedCompletionArgs(openedDocumentTable);
            NotifyEventCompletedAsync(dispatcher, CompletionArgs);
        }
    }
}
