namespace CustomControls
{
    /// <summary>
    /// Represents a descriptor of documents during import.
    /// </summary>
    public interface IDocumentImportDescriptor
    {
        /// <summary>
        /// Gets the file extension.
        /// </summary>
        string FileExtension { get; }

        /// <summary>
        /// Gets the friendly name of the operation.
        /// </summary>
        string FriendlyImportName { get; }

        /// <summary>
        /// Gets a value indicating whether this is a default document.
        /// </summary>
        bool IsDefault { get; }

        /// <summary>
        /// Gets the content descriptor from a file name.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>The content descriptor.</returns>
        ImportedContentDescriptor Import(string fileName);
    }
}
