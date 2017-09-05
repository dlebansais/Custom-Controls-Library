using System.Collections.Generic;

namespace CustomControls
{
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

        public DocumentAddedCompletionArgs(IReadOnlyDictionary<IDocumentPath, IItemPath> AddedItemTable, IReadOnlyDictionary<IDocumentPath, IItemProperties> AddedPropertiesTable)
        {
            this.AddedItemTable = AddedItemTable;
            this.AddedPropertiesTable = AddedPropertiesTable;
        }

        public IReadOnlyDictionary<IDocumentPath, IItemPath> AddedItemTable { get; private set; }
        public IReadOnlyDictionary<IDocumentPath, IItemProperties> AddedPropertiesTable { get; private set; }
    }
}
