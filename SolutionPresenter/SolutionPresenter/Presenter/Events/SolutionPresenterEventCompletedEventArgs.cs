namespace CustomControls
{
    using System;

    /// <summary>
    /// Represents the event data for a solution event completed event.
    /// </summary>
    public class SolutionPresenterEventCompletedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionPresenterEventCompletedEventArgs"/> class.
        /// </summary>
        /// <param name="eventContext">The event context.</param>
        /// <param name="completionArgs">The event completion data.</param>
        public SolutionPresenterEventCompletedEventArgs(object eventContext, object completionArgs)
        {
            EventContext = eventContext;
            CompletionArgs = completionArgs;
        }

        /// <summary>
        /// Gets the event context.
        /// </summary>
        public object EventContext { get; }

        /// <summary>
        /// Gets the event completion data.
        /// </summary>
        public object CompletionArgs { get; }
    }
}
