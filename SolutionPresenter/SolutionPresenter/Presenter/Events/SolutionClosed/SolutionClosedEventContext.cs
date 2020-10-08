namespace CustomControls
{
    /// <summary>
    /// Represents a context for the <see cref="SolutionClosedEventArgs"/> event data.
    /// </summary>
    public class SolutionClosedEventContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionClosedEventContext"/> class.
        /// </summary>
        /// <param name="solutionOperation">The solution operation.</param>
        /// <param name="closedRootPath">The path to the closed solution.</param>
        /// <param name="newRootPath">The new root path.</param>
        public SolutionClosedEventContext(SolutionOperation solutionOperation, IRootPath closedRootPath, IRootPath newRootPath)
        {
            SolutionOperation = solutionOperation;
            ClosedRootPath = closedRootPath;
            NewRootPath = newRootPath;
        }

        /// <summary>
        /// Gets the solution operation.
        /// </summary>
        public SolutionOperation SolutionOperation { get; private set; }

        /// <summary>
        /// Gets the path to the closed solution.
        /// </summary>
        public IRootPath ClosedRootPath { get; private set; }

        /// <summary>
        /// Gets the new root path.
        /// </summary>
        public IRootPath NewRootPath { get; private set; }
    }
}
