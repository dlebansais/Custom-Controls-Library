namespace CustomControls
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents information used to commiy changes.
    /// </summary>
    public class CommitInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommitInfo"/> class.
        /// </summary>
        /// <param name="option">The commit options.</param>
        /// <param name="dirtyItemList">The list of modified items.</param>
        /// <param name="dirtyPropertiesList">The list of modified properties.</param>
        /// <param name="dirtyDocumentList">The list of modified documents.</param>
        public CommitInfo(CommitOption option, ICollection<ITreeNodePath>? dirtyItemList, ICollection<ITreeNodePath>? dirtyPropertiesList, ICollection<IDocument>? dirtyDocumentList)
        {
            Option = option;
            DirtyItemList = dirtyItemList;
            DirtyPropertiesList = dirtyPropertiesList;
            DirtyDocumentList = dirtyDocumentList;
        }

        /// <summary>
        /// Gets the commit options.
        /// </summary>
        public CommitOption Option { get; private set; }

        /// <summary>
        /// Gets the list of modified items.
        /// </summary>
        public ICollection<ITreeNodePath>? DirtyItemList { get; private set; }

        /// <summary>
        /// Gets the list of modified properties.
        /// </summary>
        public ICollection<ITreeNodePath>? DirtyPropertiesList { get; private set; }

        /// <summary>
        /// Gets the list of modified documents.
        /// </summary>
        public ICollection<IDocument>? DirtyDocumentList { get; private set; }
    }
}
