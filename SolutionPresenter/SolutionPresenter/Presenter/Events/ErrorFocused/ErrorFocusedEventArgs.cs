namespace CustomControls
{
    using System.Windows;
    using System.Windows.Threading;

    /// <summary>
    /// Represents the event data for en error focused event.
    /// </summary>
    public class ErrorFocusedEventArgs : SolutionPresenterEventArgs<ErrorFocusedEventArgs>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorFocusedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The event that occured.</param>
        /// <param name="eventContext">The event context.</param>
        public ErrorFocusedEventArgs(RoutedEvent routedEvent, ErrorFocusedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        /// <summary>
        /// Gets the associated document.
        /// </summary>
        public IDocument Document { get { return ((ErrorFocusedEventContext)EventContext).Document; } }

        /// <summary>
        /// Gets the error location.
        /// </summary>
        public object? ErrorLocation { get { return ((ErrorFocusedEventContext)EventContext).ErrorLocation; } }

        /// <summary>
        /// Notifies handlers that the operation is completed.
        /// </summary>
        /// <param name="dispatcher">The window dispatcher.</param>
        public virtual void NotifyCompletedAsync(Dispatcher dispatcher)
        {
            IErrorFocusedCompletionArgs CompletionArgs = new ErrorFocusedCompletionArgs();
            NotifyEventCompletedAsync(dispatcher, CompletionArgs);
        }
    }
}
