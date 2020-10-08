namespace CustomControls
{
    /// <summary>
    /// Represents the event data for a solution selected completion event.
    /// </summary>
    internal interface ISolutionSelectedCompletionArgs
    {
        /// <summary>
        /// Gets the root path of the selected solutioon.
        /// </summary>
        IRootPath? SelectedRootPath { get; }
    }

    /// <summary>
    /// Represents the event data for a solution selected completion event.
    /// </summary>
    internal class SolutionSelectedCompletionArgs : ISolutionSelectedCompletionArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionSelectedCompletionArgs"/> class.
        /// </summary>
        public SolutionSelectedCompletionArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionSelectedCompletionArgs"/> class.
        /// </summary>
        /// <param name="selectedRootPath">The root path of the selected solutioon.</param>
        public SolutionSelectedCompletionArgs(IRootPath selectedRootPath)
        {
            SelectedRootPath = selectedRootPath;
        }

        /// <summary>
        /// Gets the root path of the selected solutioon.
        /// </summary>
        public IRootPath? SelectedRootPath { get; }
    }
}
