using System;
using System.Windows;
using System.Windows.Threading;
using Verification;

namespace CustomControls
{
    public class SolutionPresenterEventArgs : RoutedEventArgs
    {
        public SolutionPresenterEventArgs(RoutedEvent routedEvent, object eventContext)
            : base(routedEvent)
        {
            this.EventContext = eventContext;
        }

        protected object EventContext { get; private set; }

        public event EventHandler<SolutionPresenterEventCompletedEventArgs> EventCompleted;

        protected virtual void NotifyEventCompleted(object completionArgs)
        {
            EventCompleted(this, new SolutionPresenterEventCompletedEventArgs(EventContext, completionArgs));
        }

        protected virtual void NotifyEventCompletedAsync(Dispatcher dispatcher, object completionArgs)
        {
            Assert.ValidateReference(dispatcher);

            dispatcher.BeginInvoke(new NotifyEventCompletedAsyncHandler(OnNotifyEventCompletedAsync), completionArgs);
        }

        protected delegate void NotifyEventCompletedAsyncHandler(object completionArgs);
        protected virtual void OnNotifyEventCompletedAsync(object completionArgs)
        {
            EventCompleted(this, new SolutionPresenterEventCompletedEventArgs(EventContext, completionArgs));
        }
    }
}
