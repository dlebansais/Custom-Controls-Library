namespace CustomControls
{
    using System.Collections.Generic;

    public class CommitInfo
    {
        public CommitInfo(CommitOption option, ICollection<ITreeNodePath>? dirtyItemList, ICollection<ITreeNodePath>? dirtyPropertiesList, ICollection<IDocument>? dirtyDocumentList)
        {
            Option = option;
            DirtyItemList = dirtyItemList;
            DirtyPropertiesList = dirtyPropertiesList;
            DirtyDocumentList = dirtyDocumentList;
        }

        public CommitOption Option { get; private set; }
        public ICollection<ITreeNodePath>? DirtyItemList { get; private set; }
        public ICollection<ITreeNodePath>? DirtyPropertiesList { get; private set; }
        public ICollection<IDocument>? DirtyDocumentList { get; private set; }
    }
}
