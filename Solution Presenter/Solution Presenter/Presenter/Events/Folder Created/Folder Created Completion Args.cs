namespace CustomControls
{
    internal interface IFolderCreatedCompletionArgs
    {
        IFolderPath NewFolderPath { get; }
        IFolderProperties NewFolderProperties { get; }
    }

    internal class FolderCreatedCompletionArgs : IFolderCreatedCompletionArgs
    {
        public FolderCreatedCompletionArgs(IFolderPath newFolderPath, IFolderProperties newFolderProperties)
        {
            NewFolderPath = newFolderPath;
            NewFolderProperties = newFolderProperties;
        }

        public IFolderPath NewFolderPath { get; private set; }
        public IFolderProperties NewFolderProperties { get; private set; }
    }
}
