namespace CustomControls
{
    using SolutionControlsInternal.Properties;

    /// <summary>
    /// Represents the add folder operation.
    /// </summary>
    internal class AddFolderOperation : AddSingleOperation
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="AddFolderOperation"/> class.
        /// </summary>
        /// <param name="root">The root path.</param>
        /// <param name="destinationFolderPath">The destination folder path.</param>
        /// <param name="newPath">The path of the added folder.</param>
        /// <param name="newProperties">The properties of the added folder.</param>
        public AddFolderOperation(ISolutionRoot root, IFolderPath destinationFolderPath, IFolderPath newPath, IFolderProperties newProperties)
            : base(root, destinationFolderPath, newPath, newProperties)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the operation name.
        /// </summary>
        public override string Name { get { return Resources.AddFolder; } }

        /// <summary>
        /// Gets a value indicating whether this operation is adding something.
        /// </summary>
        public override bool IsAdd { get { return true; } }
        #endregion
    }
}
