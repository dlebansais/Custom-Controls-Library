namespace CustomControls
{
    internal interface ISolutionCreatedCompletionArgs
    {
        IRootPath CreatedRootPath { get; }
    }

    internal class SolutionCreatedCompletionArgs : ISolutionCreatedCompletionArgs
    {
        public SolutionCreatedCompletionArgs()
        {
            this.CreatedRootPath = null;
        }

        public SolutionCreatedCompletionArgs(IRootPath CreatedRootPath)
        {
            this.CreatedRootPath = CreatedRootPath;
        }

        public IRootPath CreatedRootPath { get; private set; }
    }
}
