namespace CustomControls
{
    using System.Windows;
    using System.Windows.Threading;

    public class FolderCreatedEventArgs : SolutionPresenterEventArgs
    {
        private static int HandlerCount = 0;
        public static void IncrementHandlerCount() { HandlerCount++; }
        public static void DecrementHandlerCount() { HandlerCount--; }
        public static bool HasHandler { get { return HandlerCount > 0; } }

        public FolderCreatedEventArgs(RoutedEvent routedEvent, FolderCreatedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        public IFolderPath ParentPath { get { return ((FolderCreatedEventContext)EventContext).ParentPath; } }
        public string FolderName { get { return ((FolderCreatedEventContext)EventContext).FolderName; } }
        public IRootProperties RootProperties { get { return ((FolderCreatedEventContext)EventContext).RootProperties; } }

        public virtual void NotifyCompletedAsync(Dispatcher dispatcher, IFolderPath newFolderPath, IFolderProperties newFolderProperties)
        {
            IFolderCreatedCompletionArgs CompletionArgs = new FolderCreatedCompletionArgs(newFolderPath, newFolderProperties);
            NotifyEventCompletedAsync(dispatcher, CompletionArgs);
        }
    }
}
