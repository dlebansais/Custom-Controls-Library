using SolutionControlsInternal.Properties;
using System.Collections.Generic;

namespace CustomControls
{
    internal class AddItemOperation : AddSingleOperation
    {
        #region Init
        public AddItemOperation(ISolutionRoot root, IFolderPath destinationFolderPath, IItemPath newPath, IItemProperties newProperties)
            : base(root, destinationFolderPath, newPath, newProperties)
        {
        }
        #endregion

        #region Properties
        public override string Name { get { return Resources.AddItem; } }
        public override bool IsAdd { get { return true; } }
        #endregion
    }
}
