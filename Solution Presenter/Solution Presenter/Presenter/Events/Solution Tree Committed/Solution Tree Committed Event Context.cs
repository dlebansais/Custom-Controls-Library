using System.Collections.Generic;

namespace CustomControls
{
    public class SolutionTreeCommittedEventContext
    {
        public SolutionTreeCommittedEventContext(CommitInfo info, SolutionOperation solutionOperation, IRootPath rootPath, IRootPath newRootPath, string destinationPath)
        {
            this.Info = info;
            this.SolutionOperation = solutionOperation;
            this.RootPath = rootPath;
            this.NewRootPath = newRootPath;
            this.DestinationPath = destinationPath;
        }

        public CommitInfo Info { get; private set; }
        public SolutionOperation SolutionOperation { get; private set; }
        public IRootPath RootPath { get; private set; }
        public IRootPath NewRootPath { get; private set; }
        public string DestinationPath { get; private set; }
    }
}
