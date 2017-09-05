using System.Collections.Generic;
using Verification;

namespace CustomControls
{
    internal abstract class AddSingleOperation : AddRemoveSingleOperation
    {
        #region Init
        protected AddSingleOperation(ISolutionRoot root, IFolderPath destinationFolderPath, ITreeNodePath newPath, ITreeNodeProperties newProperties)
            : base(root, destinationFolderPath, newPath, newProperties)
        {
            Assert.ValidateReference(destinationFolderPath);

            this.DestinationFolderPath = destinationFolderPath;
        }
        #endregion

        #region Properties
        public IFolderPath DestinationFolderPath { get; private set; }
        #endregion

        #region Client Interface
        public override void Redo()
        {
            Add(PathTable);
            base.Redo();

            ISolutionFolder ParentFolder = Root.FindTreeNode(DestinationFolderPath) as ISolutionFolder;
            Assert.ValidateReference(ParentFolder);

            AddExpandedFolder(ParentFolder);
        }

        public override void Undo()
        {
            Remove(PathTable);
            base.Undo();
        }
        #endregion
    }
}
