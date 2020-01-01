namespace CustomControls
{
    /// <summary>
    /// Represents the event data for a solution created completion event.
    /// </summary>
    internal interface ISolutionCreatedCompletionArgs
    {
        /// <summary>
        /// Gets the root path of the created solution.
        /// </summary>
        IRootPath? CreatedRootPath { get; }
    }

    /// <summary>
    /// Represents the event data for a solution created completion event.
    /// </summary>
    internal class SolutionCreatedCompletionArgs : ISolutionCreatedCompletionArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionCreatedCompletionArgs"/> class.
        /// </summary>
        public SolutionCreatedCompletionArgs()
        {
            CreatedRootPath = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionCreatedCompletionArgs"/> class.
        /// </summary>
        /// <param name="createdRootPath">The root path of the created solution.</param>
        public SolutionCreatedCompletionArgs(IRootPath createdRootPath)
        {
            CreatedRootPath = createdRootPath;
        }

        /// <summary>
        /// Gets the root path of the created solution.
        /// </summary>
        public IRootPath? CreatedRootPath { get; }
    }
}
