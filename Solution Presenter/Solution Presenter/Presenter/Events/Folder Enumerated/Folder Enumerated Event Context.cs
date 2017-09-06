using System.Collections.Generic;

namespace CustomControls
{
    public class FolderEnumeratedEventContext
    {
        public FolderEnumeratedEventContext(IFolderPath parentPath, ICollection<IFolderPath> parentPathList, IRootProperties rootProperties, ICollection<IFolderPath> expandedFolderList, object context)
        {
            this.ParentPath = parentPath;
            this.ParentPathList = parentPathList;
            this.RootProperties = rootProperties;
            this.ExpandedFolderList = expandedFolderList;
            this.Context = context;
        }

        public IFolderPath ParentPath { get; private set; }
        public ICollection<IFolderPath> ParentPathList { get; private set; }
        public IRootProperties RootProperties { get; private set; }
        public ICollection<IFolderPath> ExpandedFolderList { get; private set; }
        public object Context { get; private set; }
    }
}
