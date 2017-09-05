using System.Windows;
using System.Windows.Threading;

namespace CustomControls
{
    public class RootPropertiesRequestedEventArgs : SolutionPresenterEventArgs
    {
        private static int HandlerCount = 0;
        public static void IncrementHandlerCount() { HandlerCount++; }
        public static void DecrementHandlerCount() { HandlerCount--; }
        public static bool HasHandler { get { return HandlerCount > 0; } }

        public RootPropertiesRequestedEventArgs(RoutedEvent routedEvent, RootPropertiesRequestedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        public IRootProperties Properties { get { return ((RootPropertiesRequestedEventContext)EventContext).Properties; } }
        
        public virtual void NotifyCompleted()
        {
            IRootPropertiesRequestedCompletionArgs CompletionArgs = new RootPropertiesRequestedCompletionArgs();
            NotifyEventCompleted(CompletionArgs);
        }
    }
}
