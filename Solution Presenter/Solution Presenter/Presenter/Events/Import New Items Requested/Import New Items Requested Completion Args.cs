using System.Collections.Generic;

namespace CustomControls
{
    internal interface IImportNewItemsRequestedCompletionArgs
    {
        IReadOnlyDictionary<object, IDocumentPath> OpenedDocumentTable { get; }
    }

    internal class ImportNewItemsRequestedCompletionArgs : IImportNewItemsRequestedCompletionArgs
    {
        public ImportNewItemsRequestedCompletionArgs()
        {
            this.OpenedDocumentTable = new Dictionary<object, IDocumentPath>();
        }

        public ImportNewItemsRequestedCompletionArgs(IReadOnlyDictionary<object, IDocumentPath> OpenedDocumentTable)
        {
            this.OpenedDocumentTable = OpenedDocumentTable;
        }

        public IReadOnlyDictionary<object, IDocumentPath> OpenedDocumentTable { get; private set; }
    }
}
