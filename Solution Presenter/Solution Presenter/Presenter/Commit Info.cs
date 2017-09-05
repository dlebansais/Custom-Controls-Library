using System.Collections.Generic;

namespace CustomControls
{
    public class CommitInfo
    {
        public CommitInfo(CommitOption option, ICollection<ITreeNodePath> dirtyItemList, ICollection<ITreeNodePath> dirtyPropertiesList, ICollection<IDocument> dirtyDocumentList)
        {
            this.Option = option;
            this.DirtyItemList = dirtyItemList;
            this.DirtyPropertiesList = dirtyPropertiesList;
            this.DirtyDocumentList = dirtyDocumentList;
        }

        public CommitOption Option { get; private set; }
        public ICollection<ITreeNodePath> DirtyItemList { get; private set; }
        public ICollection<ITreeNodePath> DirtyPropertiesList { get; private set; }
        public ICollection<IDocument> DirtyDocumentList { get; private set; }
    }
}
