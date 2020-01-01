namespace CustomControls
{
    using System.Collections.Generic;
    using System.Windows;

    /// <summary>
    /// Represents the event data for build solution request event.
    /// </summary>
    public class BuildSolutionRequestedEventArgs : SolutionPresenterEventArgs<BuildSolutionRequestedEventArgs>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildSolutionRequestedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The event that occured.</param>
        /// <param name="eventContext">The event context.</param>
        public BuildSolutionRequestedEventArgs(RoutedEvent routedEvent, BuildSolutionRequestedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        /// <summary>
        /// Notifies handlers that the operation is completed.
        /// </summary>
        /// <param name="errorList">The list of errors.</param>
        public virtual void NotifyCompleted(IReadOnlyList<ICompilationError> errorList)
        {
            IBuildSolutionRequestedCompletionArgs CompletionArgs = new BuildSolutionRequestedCompletionArgs(errorList);
            NotifyEventCompleted(CompletionArgs);
        }
    }
}
