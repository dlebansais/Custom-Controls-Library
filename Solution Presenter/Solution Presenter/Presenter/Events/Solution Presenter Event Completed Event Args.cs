using System;

namespace CustomControls
{
    public class SolutionPresenterEventCompletedEventArgs : EventArgs
    {
        public SolutionPresenterEventCompletedEventArgs(object eventContext, object completionArgs)
        {
            this.EventContext = eventContext;
            this.CompletionArgs = completionArgs;
        }

        public object EventContext { get; private set; }
        public object CompletionArgs { get; private set; }
    }
}
