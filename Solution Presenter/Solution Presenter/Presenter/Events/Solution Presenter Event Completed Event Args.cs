using System;

namespace CustomControls
{
    public class SolutionPresenterEventCompletedEventArgs : EventArgs
    {
        public SolutionPresenterEventCompletedEventArgs(object eventContext, object completionArgs)
        {
            EventContext = eventContext;
            CompletionArgs = completionArgs;
        }

        public object EventContext { get; }
        public object CompletionArgs { get; }
    }
}
