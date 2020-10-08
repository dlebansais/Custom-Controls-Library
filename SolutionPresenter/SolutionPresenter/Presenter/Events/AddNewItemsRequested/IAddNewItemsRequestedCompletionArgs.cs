namespace CustomControls
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the event data for a add new items requested completion event.
    /// </summary>
    internal interface IAddNewItemsRequestedCompletionArgs
    {
        /// <summary>
        /// Gets the list of added documents.
        /// </summary>
        IList<IDocumentPath> DocumentPathList { get; }
    }

    /// <summary>
    /// Represents the event data for a add new items requested completion event.
    /// </summary>
    internal class AddNewItemsRequestedCompletionArgs : IAddNewItemsRequestedCompletionArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddNewItemsRequestedCompletionArgs"/> class.
        /// </summary>
        public AddNewItemsRequestedCompletionArgs()
        {
            this.DocumentPathList = new List<IDocumentPath>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddNewItemsRequestedCompletionArgs"/> class.
        /// </summary>
        /// <param name="documentPathList">The list of added documents.</param>
        public AddNewItemsRequestedCompletionArgs(IList<IDocumentPath> documentPathList)
        {
            DocumentPathList = documentPathList;
        }

        /// <summary>
        /// Gets the list of added documents.
        /// </summary>
        public IList<IDocumentPath> DocumentPathList { get; private set; }
    }
}
