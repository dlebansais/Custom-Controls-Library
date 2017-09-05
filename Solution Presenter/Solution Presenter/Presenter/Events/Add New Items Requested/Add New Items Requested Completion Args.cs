using System.Collections.Generic;

namespace CustomControls
{
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

        public AddNewItemsRequestedCompletionArgs(IList<IDocumentPath> DocumentPathList)
        {
            this.DocumentPathList = DocumentPathList;
        }

        public IList<IDocumentPath> DocumentPathList { get; private set; }
    }
}
