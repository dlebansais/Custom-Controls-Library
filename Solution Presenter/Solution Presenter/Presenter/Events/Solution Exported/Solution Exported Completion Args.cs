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
            ContentTable = null;
        }

        public SolutionExportedCompletionArgs(Dictionary<IDocumentPath, byte[]> contentTable)
        {
            ContentTable = contentTable;
        }

        public Dictionary<IDocumentPath, byte[]>? ContentTable { get; }
    }
}
