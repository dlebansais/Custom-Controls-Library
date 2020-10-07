namespace CustomControls
{
    using System;

    /// <summary>
    /// Represents the interface of a document editor.
    /// </summary>
    public interface IDocumentEditor
    {
        /// <summary>
        /// Occurs when the caret position changed.
        /// </summary>
        event EventHandler<EventArgs> CaretPositionChanged;

        /// <summary>
        /// Gets the caret line.
        /// </summary>
        int CaretLine { get; }

        /// <summary>
        /// Gets the caret column.
        /// </summary>
        int CaretColumn { get; }

        /// <summary>
        /// Gets a value indicating whether the caret is in override mode.
        /// </summary>
        bool IsCaretOverride { get; }
    }
}
