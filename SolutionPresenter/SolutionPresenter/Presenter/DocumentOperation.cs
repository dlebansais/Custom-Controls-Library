namespace CustomControls
{
    /// <summary>
    /// Specifies an operation on a document.
    /// </summary>
    public enum DocumentOperation
    {
        /// <summary>
        /// Save the document.
        /// </summary>
        Save,

        /// <summary>
        /// Close the document.
        /// </summary>
        Close,

        /// <summary>
        /// Open the document.
        /// </summary>
        Open,

        /// <summary>
        /// Add the document to the solution.
        /// </summary>
        Add,

        /// <summary>
        /// Remove the document from the solution.
        /// </summary>
        Remove,

        /// <summary>
        /// Export the document.
        /// </summary>
        Export,

        /// <summary>
        /// Show error in the document.
        /// </summary>
        ShowError,

        /// <summary>
        /// Paste the document in the solution.
        /// </summary>
        Paste,
    }
}
