using System.Windows;
using System.Windows.Threading;

namespace CustomControls
{
    public class ErrorFocusedEventArgs : SolutionPresenterEventArgs
    {
        private static int HandlerCount = 0;
        public static void IncrementHandlerCount() { HandlerCount++; }
        public static void DecrementHandlerCount() { HandlerCount--; }
        public static bool HasHandler { get { return HandlerCount > 0; } }

        public ErrorFocusedEventArgs(RoutedEvent routedEvent, ErrorFocusedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        public IDocument Document { get { return ((ErrorFocusedEventContext)EventContext).Document; } }
        public object ErrorLocation { get { return ((ErrorFocusedEventContext)EventContext).ErrorLocation; } }
        
        public virtual void NotifyCompletedAsync(Dispatcher dispatcher)
        {
            IErrorFocusedCompletionArgs CompletionArgs = new ErrorFocusedCompletionArgs();
            NotifyEventCompletedAsync(dispatcher, CompletionArgs);
        }
    }
}
