namespace CustomControls
{
    using System;
    using System.Windows;

    /// <summary>
    /// Represents the event data for an import event.
    /// </summary>
    public class ImportedEventArgs : RoutedEventArgs
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The event that occured.</param>
        /// <param name="package">The solution package.</param>
        /// <param name="solutionName">The solution name.</param>
        public ImportedEventArgs(RoutedEvent routedEvent, SolutionPackage package, string solutionName)
            : base(routedEvent)
        {
            Package = package;
            RootPath = new EmptyPath();
            CurrentFolderPath = new EmptyPath();
            Name = solutionName;
            Content = Array.Empty<byte>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The event that occured.</param>
        /// <param name="package">The solution package.</param>
        /// <param name="rootPath">The root path.</param>
        /// <param name="currentFolderPath">The current folder path.</param>
        /// <param name="folderName">The folder name.</param>
        public ImportedEventArgs(RoutedEvent routedEvent, SolutionPackage package, IRootPath rootPath, IFolderPath currentFolderPath, string folderName)
            : base(routedEvent)
        {
            Package = package;
            RootPath = rootPath;
            CurrentFolderPath = currentFolderPath;
            Name = folderName;
            Content = Array.Empty<byte>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The event that occured.</param>
        /// <param name="package">The solution package.</param>
        /// <param name="rootPath">The root path.</param>
        /// <param name="currentFolderPath">The current folder path.</param>
        /// <param name="itemName">The item name.</param>
        /// <param name="content">The item content.</param>
        public ImportedEventArgs(RoutedEvent routedEvent, SolutionPackage package, IRootPath rootPath, IFolderPath currentFolderPath, string itemName, byte[] content)
            : base(routedEvent)
        {
            Package = package;
            RootPath = rootPath;
            CurrentFolderPath = currentFolderPath;
            Name = itemName;
            Content = content;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the solution package.
        /// </summary>
        public SolutionPackage Package { get; private set; }

        /// <summary>
        /// Gets the root path.
        /// </summary>
        public IRootPath RootPath { get; private set; }

        /// <summary>
        /// Gets the current folder path.
        /// </summary>
        public IFolderPath CurrentFolderPath { get; private set; }

        /// <summary>
        /// Gets the imported object name.
        /// </summary>
        public string Name { get; private set; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Updates the root path.
        /// </summary>
        /// <param name="rootPath">The new root path.</param>
        public void UpdateRootPath(IRootPath rootPath)
        {
            RootPath = rootPath;
        }

        /// <summary>
        /// Updates the folder path.
        /// </summary>
        /// <param name="currentFolderPath">The new folder path.</param>
        public void UpdateFolderPath(IFolderPath currentFolderPath)
        {
            CurrentFolderPath = currentFolderPath;
        }

        /// <summary>
        /// Gets the imported object content.
        /// </summary>
        /// <returns>The content.</returns>
        public byte[] GetContent()
        {
            return Content;
        }

        private byte[] Content;
        #endregion
    }
}
