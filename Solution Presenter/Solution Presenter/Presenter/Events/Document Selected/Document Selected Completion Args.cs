namespace CustomControls
{
    using System.Collections.Generic;

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

        public DocumentSelectedCompletionArgs(IList<IDocumentPath> documentPathList)
        {
            DocumentPathList = documentPathList;
        }

        public IList<IDocumentPath> DocumentPathList { get; private set; }
    }
}
