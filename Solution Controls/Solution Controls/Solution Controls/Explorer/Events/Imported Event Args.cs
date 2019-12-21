using System;
using System.Windows;

namespace CustomControls
{
    public class ImportedEventArgs : RoutedEventArgs
    {
        #region Init
        public ImportedEventArgs(RoutedEvent routedEvent, SolutionPackage package, string solutionName)
            : base(routedEvent)
        {
            if (solutionName == null)
                throw new ArgumentNullException(nameof(solutionName));

            this.Package = package;
            this.RootPath = null;
            this.CurrentFolderPath = null;
            this.Name = solutionName;
            this.Content = null;
        }

        public ImportedEventArgs(RoutedEvent routedEvent, SolutionPackage package, IRootPath rootPath, IFolderPath currentFolderPath, string folderName)
            : base(routedEvent)
        {
            if (rootPath == null)
                throw new ArgumentNullException(nameof(rootPath));
            if (folderName == null)
                throw new ArgumentNullException(nameof(folderName));

            this.Package = package;
            this.RootPath = rootPath;
            this.CurrentFolderPath = currentFolderPath;
            this.Name = folderName;
            this.Content = null;
        }

        public ImportedEventArgs(RoutedEvent routedEvent, SolutionPackage package, IRootPath rootPath, IFolderPath currentFolderPath, string itemName, byte[] content)
            : base(routedEvent)
        {
            if (rootPath == null)
                throw new ArgumentNullException(nameof(rootPath));
            if (currentFolderPath == null)
                throw new ArgumentNullException(nameof(currentFolderPath));
            if (itemName == null)
                throw new ArgumentNullException(nameof(itemName));
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            this.Package = package;
            this.RootPath = rootPath;
            this.CurrentFolderPath = currentFolderPath;
            this.Name = itemName;
            this.Content = content;
        }
        #endregion

        #region Properties
        public SolutionPackage Package { get; private set; }
        public IRootPath RootPath { get; private set; }
        public IFolderPath CurrentFolderPath { get; private set; }
        public string Name { get; private set; }
        #endregion

        #region Client Interface
        public void UpdateRootPath(IRootPath rootPath)
        {
            this.RootPath = rootPath;
        }

        public void UpdateFolderPath(IFolderPath currentFolderPath)
        {
            this.CurrentFolderPath = currentFolderPath;
        }

        public byte[] GetContent()
        {
            return Content;
        }

        private byte[] Content;
        #endregion
    }
}
