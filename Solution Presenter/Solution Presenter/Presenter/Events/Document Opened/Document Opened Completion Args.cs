using System.Collections.Generic;

namespace CustomControls
{
    internal interface IDocumentOpenedCompletionArgs
    {
        IReadOnlyList<IDocument> OpenedDocumentList { get; }
    }

    internal class DocumentOpenedCompletionArgs : IDocumentOpenedCompletionArgs
    {
        public DocumentOpenedCompletionArgs()
        {
            this.OpenedDocumentList = new List<IDocument>();
        }

        public DocumentOpenedCompletionArgs(IDocument OpenedDocument)
        {
            List<IDocument> OpenedDocumentList = new List<IDocument>();
            OpenedDocumentList.Add(OpenedDocument);

            this.OpenedDocumentList = OpenedDocumentList;
        }

        public DocumentOpenedCompletionArgs(IReadOnlyList<IDocument> OpenedDocumentList)
        {
            this.OpenedDocumentList = OpenedDocumentList;
        }

        public IReadOnlyList<IDocument> OpenedDocumentList { get; private set; }
    }
}
