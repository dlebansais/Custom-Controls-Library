using System.Collections.Generic;

namespace CustomControls
{
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

        public BuildSolutionRequestedCompletionArgs(IReadOnlyList<ICompilationError> ErrorList)
        {
            this.ErrorList = ErrorList;
        }

        public IReadOnlyList<ICompilationError> ErrorList { get; private set; }
    }
}
