namespace CustomControls
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the event data for a build solution completion event.
    /// </summary>
    internal interface IBuildSolutionRequestedCompletionArgs
    {
        /// <summary>
        /// Gets the list of errors.
        /// </summary>
        IReadOnlyList<ICompilationError> ErrorList { get; }
    }

    /// <summary>
    /// Represents the event data for a build solution completion event.
    /// </summary>
    internal class BuildSolutionRequestedCompletionArgs : IBuildSolutionRequestedCompletionArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildSolutionRequestedCompletionArgs"/> class.
        /// </summary>
        public BuildSolutionRequestedCompletionArgs()
        {
            ErrorList = new List<ICompilationError>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildSolutionRequestedCompletionArgs"/> class.
        /// </summary>
        /// <param name="errorList">The list of errors.</param>
        public BuildSolutionRequestedCompletionArgs(IReadOnlyList<ICompilationError> errorList)
        {
            ErrorList = errorList;
        }

        /// <summary>
        /// Gets the list of errors.
        /// </summary>
        public IReadOnlyList<ICompilationError> ErrorList { get; private set; }
    }
}
