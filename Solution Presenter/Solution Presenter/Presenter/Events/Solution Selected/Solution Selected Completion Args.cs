namespace CustomControls
{
    internal interface ISolutionSelectedCompletionArgs
    {
        IRootPath? SelectedRootPath { get; }
    }

    internal class SolutionSelectedCompletionArgs : ISolutionSelectedCompletionArgs
    {
        public SolutionSelectedCompletionArgs()
        {
        }

        public SolutionSelectedCompletionArgs(IRootPath selectedRootPath)
        {
            SelectedRootPath = selectedRootPath;
        }

        public IRootPath? SelectedRootPath { get; }
    }
}
