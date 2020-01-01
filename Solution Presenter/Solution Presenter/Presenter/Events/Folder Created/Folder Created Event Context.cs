namespace CustomControls
{
    /// <summary>
    /// Represents a context for the <see cref="FolderCreatedEventArgs"/> event data.
    /// </summary>
    public class FolderCreatedEventContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FolderCreatedEventContext"/> class.
        /// </summary>
        /// <param name="parentPath">The parent path.</param>
        /// <param name="folderName">The folder name.</param>
        /// <param name="rootProperties">The root object properties.</param>
        public FolderCreatedEventContext(IFolderPath parentPath, string folderName, IRootProperties rootProperties)
        {
            ParentPath = parentPath;
            FolderName = folderName;
            RootProperties = rootProperties;
        }

        /// <summary>
        /// Gets the parent path.
        /// </summary>
        public IFolderPath ParentPath { get; private set; }

        /// <summary>
        /// Gets the folder name.
        /// </summary>
        public string FolderName { get; private set; }

        /// <summary>
        /// Gets the root object properties.
        /// </summary>
        public IRootProperties RootProperties { get; private set; }
    }
}
