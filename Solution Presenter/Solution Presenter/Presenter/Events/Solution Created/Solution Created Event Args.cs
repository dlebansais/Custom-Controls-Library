using System.Windows;
using System.Windows.Threading;

namespace CustomControls
{
    public class SolutionCreatedEventArgs : SolutionPresenterEventArgs
    {
        private static int HandlerCount = 0;
        public static void IncrementHandlerCount() { HandlerCount++; }
        public static void DecrementHandlerCount() { HandlerCount--; }
        public static bool HasHandler { get { return HandlerCount > 0; } }

        public SolutionCreatedEventArgs(RoutedEvent routedEvent, SolutionCreatedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        public virtual void NotifyCompletedAsync(Dispatcher dispatcher, IRootPath createdRootPath)
        {
            ISolutionCreatedCompletionArgs CompletionArgs = new SolutionCreatedCompletionArgs(createdRootPath);
            NotifyEventCompletedAsync(dispatcher, CompletionArgs);
        }
    }
}
