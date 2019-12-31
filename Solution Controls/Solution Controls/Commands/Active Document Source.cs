namespace CustomControls
{
    /// <summary>
    /// Represents a document source.
    /// </summary>
    public interface IActiveDocumentSource
    {
        /// <summary>
        /// Gets the active document.
        /// </summary>
        IDocument? ActiveDocument { get; }
    }
}
