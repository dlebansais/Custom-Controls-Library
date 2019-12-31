namespace CustomControls
{
    using SolutionControlsInternal.Properties;

    /// <summary>
    /// Represents the add item operation.
    /// </summary>
    internal class AddItemOperation : AddSingleOperation
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="AddItemOperation"/> class.
        /// </summary>
        /// <param name="root">The root path.</param>
        /// <param name="destinationFolderPath">The destination folder path.</param>
        /// <param name="newPath">The path of the added item.</param>
        /// <param name="newProperties">The properties of the added item.</param>
        public AddItemOperation(ISolutionRoot root, IFolderPath destinationFolderPath, IItemPath newPath, IItemProperties newProperties)
            : base(root, destinationFolderPath, newPath, newProperties)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the operation name.
        /// </summary>
        public override string Name { get { return Resources.AddItem; } }

        /// <summary>
        /// Gets a value indicating whether this operation is adding something.
        /// </summary>
        public override bool IsAdd { get { return true; } }
        #endregion
    }
}
