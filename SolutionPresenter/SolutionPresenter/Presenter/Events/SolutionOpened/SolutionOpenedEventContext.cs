namespace CustomControls
{
    /// <summary>
    /// Represents a context for the <see cref="SolutionOpenedEventArgs"/> event data.
    /// </summary>
    public class SolutionOpenedEventContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionOpenedEventContext"/> class.
        /// </summary>
        /// <param name="openedRootPath">The opened solution path.</param>
        public SolutionOpenedEventContext(IRootPath openedRootPath)
        {
            OpenedRootPath = openedRootPath;
        }

        /// <summary>
        /// Gets the opened solution path.
        /// </summary>
        public IRootPath OpenedRootPath { get; private set; }
    }
}
