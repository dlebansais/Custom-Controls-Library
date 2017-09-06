using SolutionControlsInternal.Properties;

namespace CustomControls
{
    internal class AddFolderOperation : AddSingleOperation
    {
        #region Init
        public AddFolderOperation(ISolutionRoot root, IFolderPath destinationFolderPath, IFolderPath newPath, IFolderProperties newProperties)
            : base(root, destinationFolderPath, newPath, newProperties)
        {
        }
        #endregion

        #region Properties
        public override string Name { get { return Resources.AddFolder; } }
        public override bool IsAdd { get { return true; } }
        #endregion
    }
}
