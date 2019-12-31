namespace CustomControls
{
    internal interface IDocumentCreatedCompletionArgs
    {
        IItemPath NewItemPath { get; }
        IItemProperties NewItemProperties { get; }
    }

    internal class DocumentCreatedCompletionArgs : IDocumentCreatedCompletionArgs
    {
        public DocumentCreatedCompletionArgs(IItemPath newItemPath, IItemProperties newItemProperties)
        {
            NewItemPath = newItemPath;
            NewItemProperties = newItemProperties;
        }

        public IItemPath NewItemPath { get; private set; }
        public IItemProperties NewItemProperties { get; private set; }
    }
}
