namespace CustomControls
{
    using System.Collections.Generic;
    using System.Windows;

    public class SolutionExportedEventArgs : SolutionPresenterEventArgs
    {
        private static int HandlerCount = 0;
        public static void IncrementHandlerCount() { HandlerCount++; }
        public static void DecrementHandlerCount() { HandlerCount--; }
        public static bool HasHandler { get { return HandlerCount > 0; } }

        public SolutionExportedEventArgs(RoutedEvent routedEvent, SolutionExportedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        public IRootPath ExportedRootPath { get { return ((SolutionExportedEventContext)EventContext).ExportedRootPath; } }

        public virtual void NotifyCompleted(Dictionary<IDocumentPath, byte[]> contentTable)
        {
            ISolutionExportedCompletionArgs CompletionArgs = new SolutionExportedCompletionArgs(contentTable);
            NotifyEventCompleted(CompletionArgs);
        }
    }
}
