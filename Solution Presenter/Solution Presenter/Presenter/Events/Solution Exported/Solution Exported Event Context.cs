namespace CustomControls
{
    /// <summary>
    /// Represents a context for the <see cref="NodeRenamedEventArgs"/> event data.
    /// </summary>
    public class SolutionExportedEventContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionExportedEventContext"/> class.
        /// </summary>
        /// <param name="exportedRootPath">The path of the exported solution.</param>
        /// <param name="destinationPath">The destination path.</param>
        public SolutionExportedEventContext(IRootPath exportedRootPath, string destinationPath)
        {
            ExportedRootPath = exportedRootPath;
            DestinationPath = destinationPath;
        }

        /// <summary>
        /// Gets the path of the exported solution.
        /// </summary>
        public IRootPath ExportedRootPath { get; private set; }

        /// <summary>
        /// Gets the destination path.
        /// </summary>
        public string DestinationPath { get; private set; }
    }
}
