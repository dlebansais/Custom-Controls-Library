using System.Collections.Generic;

namespace CustomControls
{
    public class FolderCreatedEventContext
    {
        public FolderCreatedEventContext(IFolderPath parentPath, string folderName, IRootProperties rootProperties)
        {
            this.ParentPath = parentPath;
            this.FolderName = folderName;
            this.RootProperties = rootProperties;
        }

        public IFolderPath ParentPath { get; private set; }
        public string FolderName { get; private set; }
        public IRootProperties RootProperties { get; private set; }
    }
}
