namespace CustomControls
{
    public class SolutionExportedEventContext
    {
        public SolutionExportedEventContext(IRootPath exportedRootPath, string destinationPath)
        {
            this.ExportedRootPath = exportedRootPath;
            this.DestinationPath = destinationPath;
        }

        public IRootPath ExportedRootPath { get; private set; }
        public string DestinationPath { get; private set; }
    }
}
