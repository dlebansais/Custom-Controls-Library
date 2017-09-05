using System.Windows;

namespace CustomControls
{
    public class SolutionSelectedEventArgs : SolutionPresenterEventArgs
    {
        private static int HandlerCount = 0;
        public static void IncrementHandlerCount() { HandlerCount++; }
        public static void DecrementHandlerCount() { HandlerCount--; }
        public static bool HasHandler { get { return HandlerCount > 0; } }

        public SolutionSelectedEventArgs(RoutedEvent routedEvent, SolutionSelectedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        public virtual void NotifyCompleted(IRootPath selectedRootPath)
        {
            ISolutionSelectedCompletionArgs CompletionArgs = new SolutionSelectedCompletionArgs(selectedRootPath);
            NotifyEventCompleted(CompletionArgs);
        }
    }
}
