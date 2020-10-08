namespace CustomControls
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the event data for a document selected completion event.
    /// </summary>
    internal interface IDocumentSelectedCompletionArgs
    {
        /// <summary>
        /// Gets the list of selected documents.
        /// </summary>
        IList<IDocumentPath> DocumentPathList { get; }
    }

    /// <summary>
    /// Represents the event data for a document selected completion event.
    /// </summary>
    internal class DocumentSelectedCompletionArgs : IDocumentSelectedCompletionArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentSelectedCompletionArgs"/> class.
        /// </summary>
        public DocumentSelectedCompletionArgs()
        {
            DocumentPathList = new List<IDocumentPath>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentSelectedCompletionArgs"/> class.
        /// </summary>
        /// <param name="documentPathList">The list of selected documents.</param>
        public DocumentSelectedCompletionArgs(IList<IDocumentPath> documentPathList)
        {
            DocumentPathList = documentPathList;
        }

        /// <summary>
        /// Gets the list of selected documents.
        /// </summary>
        public IList<IDocumentPath> DocumentPathList { get; private set; }
    }
}
