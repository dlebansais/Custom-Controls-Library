using System.Collections.Generic;

namespace CustomControls
{
    internal interface IFolderCreatedCompletionArgs
    {
        IFolderPath NewFolderPath { get; }
        IFolderProperties NewFolderProperties { get; }
    }

    internal class FolderCreatedCompletionArgs : IFolderCreatedCompletionArgs
    {
        public FolderCreatedCompletionArgs(IFolderPath NewFolderPath, IFolderProperties NewFolderProperties)
        {
            this.NewFolderPath = NewFolderPath;
            this.NewFolderProperties = NewFolderProperties;
        }

        public IFolderPath NewFolderPath { get; private set; }
        public IFolderProperties NewFolderProperties { get; private set; }
    }
}
