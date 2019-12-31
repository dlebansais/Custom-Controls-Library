namespace CustomControls
{
    using System.Collections.Generic;

    internal interface IAddNewItemsRequestedCompletionArgs
    {
        IList<IDocumentPath> DocumentPathList { get; }
    }

    internal class AddNewItemsRequestedCompletionArgs : IAddNewItemsRequestedCompletionArgs
    {
        public AddNewItemsRequestedCompletionArgs()
        {
            this.DocumentPathList = new List<IDocumentPath>();
        }

        public AddNewItemsRequestedCompletionArgs(IList<IDocumentPath> documentPathList)
        {
            DocumentPathList = documentPathList;
        }

        public IList<IDocumentPath> DocumentPathList { get; private set; }
    }
}
