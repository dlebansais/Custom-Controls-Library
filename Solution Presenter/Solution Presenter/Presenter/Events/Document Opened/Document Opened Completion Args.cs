namespace CustomControls
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the event data for a document opened completion event.
    /// </summary>
    internal interface IDocumentOpenedCompletionArgs
    {
        /// <summary>
        /// Gets the list of opened documents.
        /// </summary>
        IReadOnlyList<IDocument> OpenedDocumentList { get; }
    }

    /// <summary>
    /// Represents the event data for a document opened completion event.
    /// </summary>
    internal class DocumentOpenedCompletionArgs : IDocumentOpenedCompletionArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentOpenedCompletionArgs"/> class.
        /// </summary>
        public DocumentOpenedCompletionArgs()
        {
            this.OpenedDocumentList = new List<IDocument>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentOpenedCompletionArgs"/> class.
        /// </summary>
        /// <param name="openedDocument">The opened document.</param>
        public DocumentOpenedCompletionArgs(IDocument openedDocument)
        {
            List<IDocument> OpenedDocumentList = new List<IDocument>();
            OpenedDocumentList.Add(openedDocument);

            this.OpenedDocumentList = OpenedDocumentList;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentOpenedCompletionArgs"/> class.
        /// </summary>
        /// <param name="openedDocumentList">The list of opened document.</param>
        public DocumentOpenedCompletionArgs(IReadOnlyList<IDocument> openedDocumentList)
        {
            OpenedDocumentList = openedDocumentList;
        }

        /// <summary>
        /// Gets the list of opened documents.
        /// </summary>
        public IReadOnlyList<IDocument> OpenedDocumentList { get; private set; }
    }
}
