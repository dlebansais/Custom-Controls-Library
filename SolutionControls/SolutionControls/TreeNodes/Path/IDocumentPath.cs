namespace CustomControls
{
    /// <summary>
    /// Represents a path to a document.
    /// </summary>
    public interface IDocumentPath
    {
        /// <summary>
        /// Gets the header name.
        /// </summary>
        string HeaderName { get; }

        /// <summary>
        /// Gets the content ID.
        /// </summary>
        string ContentId { get; }

        /// <summary>
        /// Gets the export ID.
        /// </summary>
        string ExportId { get; }

        /// <summary>
        /// Checks if two document paths are the same.
        /// </summary>
        /// <param name="other">The other document path.</param>
        /// <returns>True if they are the same; otherwise, false.</returns>
        bool IsEqual(IDocumentPath other);
    }
}
