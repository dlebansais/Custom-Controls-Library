using System.Windows;

namespace CustomControls
{
    public class SolutionTreeLoadedEventArgs : SolutionPresenterEventArgs
    {
        private static int HandlerCount = 0;
        public static void IncrementHandlerCount() { HandlerCount++; }
        public static void DecrementHandlerCount() { HandlerCount--; }
        public static bool HasHandler { get { return HandlerCount > 0; } }

        public SolutionTreeLoadedEventArgs(RoutedEvent routedEvent, SolutionTreeLoadedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        public bool IsCanceled { get { return ((SolutionTreeLoadedEventContext)EventContext).IsCanceled; } }

        public virtual void NotifyCompleted()
        {
            ISolutionTreeLoadedCompletionArgs CompletionArgs = new SolutionTreeLoadedCompletionArgs();
            NotifyEventCompleted(CompletionArgs);
        }
    }
}
