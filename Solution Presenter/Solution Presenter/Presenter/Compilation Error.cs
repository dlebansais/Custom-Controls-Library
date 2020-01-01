namespace CustomControls
{
    /// <summary>
    /// Represents an error during compilation.
    /// </summary>
    public interface ICompilationError
    {
        /// <summary>
        /// Gets the error description.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the path to the document.
        /// </summary>
        IDocumentPath Source { get; }

        /// <summary>
        /// Gets the error location.
        /// </summary>
        object Location { get; }
    }
}
