namespace CustomControls
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the event data for a solution exported completion event.
    /// </summary>
    internal interface ISolutionExportedCompletionArgs
    {
    }

    /// <summary>
    /// Represents the event data for a solution exported completion event.
    /// </summary>
    internal class SolutionExportedCompletionArgs : ISolutionExportedCompletionArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionExportedCompletionArgs"/> class.
        /// </summary>
        public SolutionExportedCompletionArgs()
        {
            ContentTable = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionExportedCompletionArgs"/> class.
        /// </summary>
        /// <param name="contentTable">The table of exported content.</param>
        public SolutionExportedCompletionArgs(Dictionary<IDocumentPath, byte[]> contentTable)
        {
            ContentTable = contentTable;
        }

        /// <summary>
        /// Gets the table of exported content.
        /// </summary>
        public Dictionary<IDocumentPath, byte[]>? ContentTable { get; }
    }
}
