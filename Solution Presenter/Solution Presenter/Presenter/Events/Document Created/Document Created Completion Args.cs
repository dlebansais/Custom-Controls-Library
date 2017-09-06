namespace CustomControls
{
    internal interface IDocumentCreatedCompletionArgs
    {
        IItemPath NewItemPath { get; }
        IItemProperties NewItemProperties { get; }
    }

    internal class DocumentCreatedCompletionArgs : IDocumentCreatedCompletionArgs
    {
        public DocumentCreatedCompletionArgs(IItemPath NewItemPath, IItemProperties NewItemProperties)
        {
            this.NewItemPath = NewItemPath;
            this.NewItemProperties = NewItemProperties;
        }

        public IItemPath NewItemPath { get; private set; }
        public IItemProperties NewItemProperties { get; private set; }
    }
}
