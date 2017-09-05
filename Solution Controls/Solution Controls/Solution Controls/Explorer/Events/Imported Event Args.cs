using System.Collections.Generic;
using System.Windows;
using Verification;

namespace CustomControls
{
    public class ImportedEventArgs : RoutedEventArgs
    {
        #region Init
        public ImportedEventArgs(RoutedEvent routedEvent, SolutionPackage package, string solutionName)
            : base(routedEvent)
        {
            Assert.ValidateReference(solutionName);

            this.Package = package;
            this.RootPath = null;
            this.CurrentFolderPath = null;
            this.Name = solutionName;
            this.Content = null;
        }

        public ImportedEventArgs(RoutedEvent routedEvent, SolutionPackage package, IRootPath rootPath, IFolderPath currentFolderPath, string folderName)
            : base(routedEvent)
        {
            Assert.ValidateReference(rootPath);
            Assert.ValidateReference(folderName);

            this.Package = package;
            this.RootPath = rootPath;
            this.CurrentFolderPath = currentFolderPath;
            this.Name = folderName;
            this.Content = null;
        }

        public ImportedEventArgs(RoutedEvent routedEvent, SolutionPackage package, IRootPath rootPath, IFolderPath currentFolderPath, string itemName, byte[] content)
            : base(routedEvent)
        {
            Assert.ValidateReference(rootPath);
            Assert.ValidateReference(currentFolderPath);
            Assert.ValidateReference(itemName);
            Assert.ValidateReference(content);

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
