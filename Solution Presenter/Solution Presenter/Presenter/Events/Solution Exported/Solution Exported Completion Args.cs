using System.Collections.Generic;

namespace CustomControls
{
    internal interface ISolutionExportedCompletionArgs
    {
    }

    internal class SolutionExportedCompletionArgs : ISolutionExportedCompletionArgs
    {
        public SolutionExportedCompletionArgs()
        {
            this.ContentTable = null;
        }

        public SolutionExportedCompletionArgs(Dictionary<IDocumentPath, byte[]> contentTable)
        {
            this.ContentTable = contentTable;
        }

        public Dictionary<IDocumentPath, byte[]> ContentTable { get; private set; }
    }
}
