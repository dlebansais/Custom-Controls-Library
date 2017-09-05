namespace CustomControls
{
    public class ValidEditOperations
    {
        #region Init
        public ValidEditOperations(bool isSolutionSelected, bool isFolderSelected, bool isDocumentSelected, bool isSingleTarget, bool canCutOrCopy, bool canPaste)
        {
            this.Add = isSingleTarget;
            this.AddFolder = (isSolutionSelected || isFolderSelected) && isSingleTarget;
            this.Open = !isSolutionSelected && !isFolderSelected && isDocumentSelected;
            this.Cut = canCutOrCopy;
            this.Copy = canCutOrCopy;
            this.Paste = canPaste;
            this.Delete = !isSolutionSelected && (isFolderSelected || isDocumentSelected);
            this.DeleteSolution = isSolutionSelected;
            this.Rename = isSingleTarget;
            this.Properties = (isSolutionSelected && !isFolderSelected && !isDocumentSelected) ||
                              (!isSolutionSelected && isFolderSelected && !isDocumentSelected) ||
                              (!isSolutionSelected && !isFolderSelected && isDocumentSelected);
        }
        #endregion

        #region Properties
        public bool Add { get; private set; }
        public bool AddFolder { get; private set; }
        public bool Open { get; private set; }
        public bool Cut { get; private set; }
        public bool Copy { get; private set; }
        public bool Paste { get; private set; }
        public bool Delete { get; private set; }
        public bool DeleteSolution { get; private set; }
        public bool Rename { get; private set; }
        public bool Properties { get; private set; }
        #endregion
    }
}
