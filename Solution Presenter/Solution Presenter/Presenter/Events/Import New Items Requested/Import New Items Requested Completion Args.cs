namespace CustomControls
{
    using System.Collections.Generic;

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

        public ImportNewItemsRequestedCompletionArgs(IReadOnlyDictionary<object, IDocumentPath> openedDocumentTable)
        {
            OpenedDocumentTable = openedDocumentTable;
        }

        public IReadOnlyDictionary<object, IDocumentPath> OpenedDocumentTable { get; private set; }
    }
}
