using System.Windows;

namespace CustomControls
{
    public class SolutionClosedEventArgs : SolutionPresenterEventArgs
    {
        private static int HandlerCount = 0;
        public static void IncrementHandlerCount() { HandlerCount++; }
        public static void DecrementHandlerCount() { HandlerCount--; }
        public static bool HasHandler { get { return HandlerCount > 0; } }

        public SolutionClosedEventArgs(RoutedEvent routedEvent, SolutionClosedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        public virtual void NotifyCompleted()
        {
            ISolutionClosedCompletionArgs CompletionArgs = new SolutionClosedCompletionArgs();
            NotifyEventCompleted(CompletionArgs);
        }
    }
}
