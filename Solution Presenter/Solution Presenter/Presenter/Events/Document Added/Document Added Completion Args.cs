namespace CustomControls
{
    using System.Collections.Generic;

    internal interface IDocumentAddedCompletionArgs
    {
        IReadOnlyDictionary<IDocumentPath, IItemPath> AddedItemTable { get; }
        IReadOnlyDictionary<IDocumentPath, IItemProperties> AddedPropertiesTable { get; }
    }

    internal class DocumentAddedCompletionArgs : IDocumentAddedCompletionArgs
    {
        public DocumentAddedCompletionArgs()
        {
            this.AddedItemTable = new Dictionary<IDocumentPath, IItemPath>();
            this.AddedPropertiesTable = new Dictionary<IDocumentPath, IItemProperties>();
        }

        public DocumentAddedCompletionArgs(IReadOnlyDictionary<IDocumentPath, IItemPath> addedItemTable, IReadOnlyDictionary<IDocumentPath, IItemProperties> addedPropertiesTable)
        {
            AddedItemTable = addedItemTable;
            AddedPropertiesTable = addedPropertiesTable;
        }

        public IReadOnlyDictionary<IDocumentPath, IItemPath> AddedItemTable { get; private set; }
        public IReadOnlyDictionary<IDocumentPath, IItemProperties> AddedPropertiesTable { get; private set; }
    }
}
