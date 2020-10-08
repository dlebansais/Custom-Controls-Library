namespace CustomControls
{
    /// <summary>
    /// Represents the event data for folder created completion event.
    /// </summary>
    internal interface IFolderCreatedCompletionArgs
    {
        /// <summary>
        /// Gets the created folder path.
        /// </summary>
        IFolderPath NewFolderPath { get; }

        /// <summary>
        /// Gets the created folder properties.
        /// </summary>
        IFolderProperties NewFolderProperties { get; }
    }

    /// <summary>
    /// Represents the event data for folder created completion event.
    /// </summary>
    internal class FolderCreatedCompletionArgs : IFolderCreatedCompletionArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FolderCreatedCompletionArgs"/> class.
        /// </summary>
        /// <param name="newFolderPath">The created folder path.</param>
        /// <param name="newFolderProperties">The created folder properties.</param>
        public FolderCreatedCompletionArgs(IFolderPath newFolderPath, IFolderProperties newFolderProperties)
        {
            NewFolderPath = newFolderPath;
            NewFolderProperties = newFolderProperties;
        }

        /// <summary>
        /// Gets the created folder path.
        /// </summary>
        public IFolderPath NewFolderPath { get; private set; }

        /// <summary>
        /// Gets the created folder properties.
        /// </summary>
        public IFolderProperties NewFolderProperties { get; private set; }
    }
}
