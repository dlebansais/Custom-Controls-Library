namespace CustomControls
{
    using System.Collections.Generic;
    using System.Windows;

    public class BuildSolutionRequestedEventArgs : SolutionPresenterEventArgs
    {
        private static int HandlerCount = 0;
        public static void IncrementHandlerCount() { HandlerCount++; }
        public static void DecrementHandlerCount() { HandlerCount--; }
        public static bool HasHandler { get { return HandlerCount > 0; } }

        public BuildSolutionRequestedEventArgs(RoutedEvent routedEvent, BuildSolutionRequestedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        public virtual void NotifyCompleted(IReadOnlyList<ICompilationError> errorList)
        {
            IBuildSolutionRequestedCompletionArgs CompletionArgs = new BuildSolutionRequestedCompletionArgs(errorList);
            NotifyEventCompleted(CompletionArgs);
        }
    }
}
