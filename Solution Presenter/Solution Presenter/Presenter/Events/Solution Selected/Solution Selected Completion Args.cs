namespace CustomControls
{
    internal interface ISolutionSelectedCompletionArgs
    {
        IRootPath SelectedRootPath { get; }
    }

    internal class SolutionSelectedCompletionArgs : ISolutionSelectedCompletionArgs
    {
        public SolutionSelectedCompletionArgs()
        {
            this.SelectedRootPath = null;
        }

        public SolutionSelectedCompletionArgs(IRootPath SelectedRootPath)
        {
            this.SelectedRootPath = SelectedRootPath;
        }

        public IRootPath SelectedRootPath { get; private set; }
    }
}
