namespace CustomControls
{
    using System.Collections.Generic;

    internal interface IBuildSolutionRequestedCompletionArgs
    {
        IReadOnlyList<ICompilationError> ErrorList { get; }
    }

    internal class BuildSolutionRequestedCompletionArgs : IBuildSolutionRequestedCompletionArgs
    {
        public BuildSolutionRequestedCompletionArgs()
        {
            this.ErrorList = new List<ICompilationError>();
        }

        public BuildSolutionRequestedCompletionArgs(IReadOnlyList<ICompilationError> errorList)
        {
            ErrorList = errorList;
        }

        public IReadOnlyList<ICompilationError> ErrorList { get; private set; }
    }
}
