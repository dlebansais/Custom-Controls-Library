namespace CustomControls
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Threading;

    public class DocumentAddedEventArgs : SolutionPresenterEventArgs
    {
        private static int HandlerCount = 0;
        public static void IncrementHandlerCount() { HandlerCount++; }
        public static void DecrementHandlerCount() { HandlerCount--; }
        public static bool HasHandler { get { return HandlerCount > 0; } }

        public DocumentAddedEventArgs(RoutedEvent routedEvent, DocumentAddedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        public IFolderPath DestinationFolderPath { get { return ((DocumentAddedEventContext)EventContext).DestinationFolderPath; } }
        public IReadOnlyList<IDocumentPath> DocumentPathList { get { return (IReadOnlyList<IDocumentPath>)((DocumentAddedEventContext)EventContext).DocumentPathList; } }
        public IRootProperties RootProperties { get { return ((DocumentAddedEventContext)EventContext).RootProperties; } }

        public virtual void NotifyCompletedAsync(Dispatcher dispatcher, IReadOnlyDictionary<IDocumentPath, IItemPath> addedItemTable, IReadOnlyDictionary<IDocumentPath, IItemProperties> addedPropertiesTable)
        {
            IDocumentAddedCompletionArgs CompletionArgs = new DocumentAddedCompletionArgs(addedItemTable, addedPropertiesTable);
            NotifyEventCompletedAsync(dispatcher, CompletionArgs);
        }
    }
}
