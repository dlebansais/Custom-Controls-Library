namespace CustomControls
{
    /// <summary>
    /// Represents a document.
    /// </summary>
    public interface IDocument
    {
        /// <summary>
        /// Gets the document type.
        /// </summary>
        IDocumentType Type { get; }

        /// <summary>
        /// Gets the path to the document file.
        /// </summary>
        IDocumentPath Path { get; }

        /// <summary>
        /// Gets a value indicating whether the document has been modified.
        /// </summary>
        bool IsDirty { get; }

        /// <summary>
        /// Called when the document view got the focus.
        /// </summary>
        void SetViewGotFocus();

        /// <summary>
        /// Clears the <see cref="IsDirty"/> flag.
        /// </summary>
        void ClearIsDirty();

        /// <summary>
        /// Checks if an undo operation can be executed on the document.
        /// </summary>
        /// <returns>True if an undo operation can be executed; otherwise, false.</returns>
        bool CanUndo();

        /// <summary>
        /// Called when an undo operation is executed on the document.
        /// </summary>
        void OnUndo();

        /// <summary>
        /// Checks if a redo operation can be executed on the document.
        /// </summary>
        /// <returns>True if a redo operation can be executed; otherwise, false.</returns>
        bool CanRedo();

        /// <summary>
        /// Called when a redo operation is executed on the document.
        /// </summary>
        void OnRedo();

        /// <summary>
        /// Checks if the entire document can be selected.
        /// </summary>
        /// <returns>True if the entire document can be selected; otherwise, false.</returns>
        bool CanSelectAll();

        /// <summary>
        /// Called when the entire document is selected.
        /// </summary>
        void OnSelectAll();

        /// <summary>
        /// Checks if a cut operation can be executed on the document.
        /// </summary>
        /// <returns>True if a cut operation can be executed; otherwise, false.</returns>
        bool CanCut();

        /// <summary>
        /// Called when a cut operation is executed on the document.
        /// </summary>
        void OnCut();

        /// <summary>
        /// Checks if a copy operation can be executed on the document.
        /// </summary>
        /// <returns>True if a copy operation can be executed; otherwise, false.</returns>
        bool CanCopy();

        /// <summary>
        /// Called when a copy operation is executed on the document.
        /// </summary>
        void OnCopy();

        /// <summary>
        /// Checks if a paste operation can be executed on the document.
        /// </summary>
        /// <returns>True if a paste operation can be executed; otherwise, false.</returns>
        bool CanPaste();

        /// <summary>
        /// Called when a paste operation is executed on the document.
        /// </summary>
        void OnPaste();

        /// <summary>
        /// Checks if a delete operation can be executed on the document.
        /// </summary>
        /// <returns>True if a delete operation can be executed; otherwise, false.</returns>
        bool CanDelete();

        /// <summary>
        /// Called when a delete operation is executed on the document.
        /// </summary>
        void OnDelete();
    }
}
