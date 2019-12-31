namespace CustomControls
{
    /// <summary>
    /// Represents valid edit operations.
    /// </summary>
    public class ValidEditOperations
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidEditOperations"/> class.
        /// </summary>
        /// <param name="isSolutionSelected">True if the solution is selected.</param>
        /// <param name="isFolderSelected">True if a folder is selected.</param>
        /// <param name="isDocumentSelected">True if a document is selected.</param>
        /// <param name="isSingleTarget">True if the operation is on a single target.</param>
        /// <param name="canCutOrCopy">True if cut and copy are permitted.</param>
        /// <param name="canPaste">True if paste is permitted.</param>
        public ValidEditOperations(bool isSolutionSelected, bool isFolderSelected, bool isDocumentSelected, bool isSingleTarget, bool canCutOrCopy, bool canPaste)
        {
            Add = isSingleTarget;
            AddFolder = (isSolutionSelected || isFolderSelected) && isSingleTarget;
            Open = !isSolutionSelected && !isFolderSelected && isDocumentSelected;
            Cut = canCutOrCopy;
            Copy = canCutOrCopy;
            Paste = canPaste;
            Delete = !isSolutionSelected && (isFolderSelected || isDocumentSelected);
            DeleteSolution = isSolutionSelected;
            Rename = isSingleTarget;
            Properties = (isSolutionSelected && !isFolderSelected && !isDocumentSelected) ||
                         (!isSolutionSelected && isFolderSelected && !isDocumentSelected) ||
                         (!isSolutionSelected && !isFolderSelected && isDocumentSelected);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether the Add operation is allowed.
        /// </summary>
        public bool Add { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the AddFolder operation is allowed.
        /// </summary>
        public bool AddFolder { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the Open operation is allowed.
        /// </summary>
        public bool Open { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the Cut operation is allowed.
        /// </summary>
        public bool Cut { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the Copy operation is allowed.
        /// </summary>
        public bool Copy { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the Paste operation is allowed.
        /// </summary>
        public bool Paste { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the Delete operation is allowed.
        /// </summary>
        public bool Delete { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the DeleteSolution operation is allowed.
        /// </summary>
        public bool DeleteSolution { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the Rename operation is allowed.
        /// </summary>
        public bool Rename { get; private set; }

        /// <summary>
        /// Gets a value indicating whether changing properties is allowed.
        /// </summary>
        public bool Properties { get; private set; }
        #endregion
    }
}
