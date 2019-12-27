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
            Package = package;
            RootPath = new EmptyPath();
            CurrentFolderPath = new EmptyPath();
            Name = solutionName;
            Content = Array.Empty<byte>();
        }

        public ImportedEventArgs(RoutedEvent routedEvent, SolutionPackage package, IRootPath rootPath, IFolderPath currentFolderPath, string folderName)
            : base(routedEvent)
        {
            Package = package;
            RootPath = rootPath;
            CurrentFolderPath = currentFolderPath;
            Name = folderName;
            Content = Array.Empty<byte>();
        }

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
