using System.Collections.Generic;

namespace CustomControls
{
    internal interface IDocumentSelectedCompletionArgs
    {
        IList<IDocumentPath> DocumentPathList { get; }
    }

    internal class DocumentSelectedCompletionArgs : IDocumentSelectedCompletionArgs
    {
        public DocumentSelectedCompletionArgs()
        {
            this.DocumentPathList = new List<IDocumentPath>();
        }

        public DocumentSelectedCompletionArgs(IList<IDocumentPath> DocumentPathList)
        {
            this.DocumentPathList = DocumentPathList;
        }

        public IList<IDocumentPath> DocumentPathList { get; private set; }
    }
}
