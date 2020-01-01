namespace CustomControls
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a context for the <see cref="FolderEnumeratedEventArgs"/> event data.
    /// </summary>
    public class FolderEnumeratedEventContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FolderEnumeratedEventContext"/> class.
        /// </summary>
        /// <param name="parentPath">The parent path.</param>
        /// <param name="parentPathList">The list of parents of enumerated folders.</param>
        /// <param name="rootProperties">The properties of the root object.</param>
        /// <param name="expandedFolderList">The list of expanded folders in the enumeration.</param>
        /// <param name="context">The enumeration context.</param>
        public FolderEnumeratedEventContext(IFolderPath parentPath, ICollection<IFolderPath> parentPathList, IRootProperties rootProperties, ICollection<IFolderPath> expandedFolderList, object context)
        {
            ParentPath = parentPath;
            ParentPathList = parentPathList;
            RootProperties = rootProperties;
            ExpandedFolderList = expandedFolderList;
            Context = context;
        }

        /// <summary>
        /// Gets the parent path.
        /// </summary>
        public IFolderPath ParentPath { get; private set; }

        /// <summary>
        /// Gets the list of parents of enumerated folders.
        /// </summary>
        public ICollection<IFolderPath> ParentPathList { get; private set; }

        /// <summary>
        /// Gets the properties of the root object.
        /// </summary>
        public IRootProperties RootProperties { get; private set; }

        /// <summary>
        /// Gets the list of expanded folders in the enumeration.
        /// </summary>
        public ICollection<IFolderPath> ExpandedFolderList { get; private set; }

        /// <summary>
        /// Gets the enumeration context.
        /// </summary>
        public object Context { get; private set; }
    }
}
