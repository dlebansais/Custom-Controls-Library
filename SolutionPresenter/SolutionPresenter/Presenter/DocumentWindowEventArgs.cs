namespace CustomControls
{
    using System;

    /// <summary>
    /// Represents the event data for an event on a document window.
    /// </summary>
    public class DocumentWindowEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentWindowEventArgs"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        public DocumentWindowEventArgs(IDocument document)
        {
            Document = document;
        }

        /// <summary>
        /// Gets the document.
        /// </summary>
        public IDocument Document { get; private set; }
    }
}
