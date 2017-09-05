namespace CustomControls
{
    public class SolutionClosedEventContext
    {
        public SolutionClosedEventContext(SolutionOperation solutionOperation, IRootPath closedRootPath, IRootPath newRootPath)
        {
            this.SolutionOperation = solutionOperation;
            this.ClosedRootPath = closedRootPath;
            this.NewRootPath = newRootPath;
        }

        public SolutionOperation SolutionOperation { get; private set; }
        public IRootPath ClosedRootPath { get; private set; }
        public IRootPath NewRootPath { get; private set; }
    }
}
