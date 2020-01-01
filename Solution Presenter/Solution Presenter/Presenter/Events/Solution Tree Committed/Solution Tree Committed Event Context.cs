namespace CustomControls
{
    /// <summary>
    /// Represents a context for the <see cref="SolutionTreeCommittedEventArgs"/> event data.
    /// </summary>
    public class SolutionTreeCommittedEventContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionTreeCommittedEventContext"/> class.
        /// </summary>
        /// <param name="info">The commit information.</param>
        /// <param name="solutionOperation">The solution operation.</param>
        /// <param name="rootPath">The path to the solution.</param>
        /// <param name="newRootPath">The new root path.</param>
        /// <param name="destinationPath">The commit destination path.</param>
        public SolutionTreeCommittedEventContext(CommitInfo info, SolutionOperation solutionOperation, IRootPath rootPath, IRootPath newRootPath, string destinationPath)
        {
            Info = info;
            SolutionOperation = solutionOperation;
            RootPath = rootPath;
            NewRootPath = newRootPath;
            DestinationPath = destinationPath;
        }

        /// <summary>
        /// Gets the commit information.
        /// </summary>
        public CommitInfo Info { get; private set; }

        /// <summary>
        /// Gets the solution operation.
        /// </summary>
        public SolutionOperation SolutionOperation { get; private set; }

        /// <summary>
        /// Gets the path to the solution.
        /// </summary>
        public IRootPath RootPath { get; private set; }

        /// <summary>
        /// Gets the new root path.
        /// </summary>
        public IRootPath NewRootPath { get; private set; }

        /// <summary>
        /// Gets the commit destination path.
        /// </summary>
        public string DestinationPath { get; private set; }
    }
}
