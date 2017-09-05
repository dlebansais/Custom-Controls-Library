namespace CustomControls
{
    public class SolutionOpenedEventContext
    {
        public SolutionOpenedEventContext(IRootPath openedRootPath)
        {
            this.OpenedRootPath = openedRootPath;
        }

        public IRootPath OpenedRootPath { get; private set; }
    }
}
