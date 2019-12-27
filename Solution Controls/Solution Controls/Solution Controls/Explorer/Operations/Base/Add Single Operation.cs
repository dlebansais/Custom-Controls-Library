using System;
using System.Diagnostics;

namespace CustomControls
{
    internal abstract class AddSingleOperation : AddRemoveSingleOperation
    {
        #region Init
        protected AddSingleOperation(ISolutionRoot root, IFolderPath destinationFolderPath, ITreeNodePath newPath, ITreeNodeProperties newProperties)
            : base(root, destinationFolderPath, newPath, newProperties)
        {
            if (destinationFolderPath== null)
                throw new ArgumentNullException(nameof(destinationFolderPath));

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

            if (Root is ISolutionRoot AsValidRoot)
            {
                if (AsValidRoot.FindTreeNode(DestinationFolderPath) is ISolutionFolder ParentFolder)
                    AddExpandedFolder(ParentFolder);
            }
        }

        public override void Undo()
        {
            Remove(PathTable);
            base.Undo();
        }
        #endregion
    }
}
