namespace CustomControls
{
    internal interface ISolutionCreatedCompletionArgs
    {
        IRootPath? CreatedRootPath { get; }
    }

    internal class SolutionCreatedCompletionArgs : ISolutionCreatedCompletionArgs
    {
        public SolutionCreatedCompletionArgs()
        {
            CreatedRootPath = null;
        }

        public SolutionCreatedCompletionArgs(IRootPath createdRootPath)
        {
            CreatedRootPath = createdRootPath;
        }

        public IRootPath? CreatedRootPath { get; }
    }
}
