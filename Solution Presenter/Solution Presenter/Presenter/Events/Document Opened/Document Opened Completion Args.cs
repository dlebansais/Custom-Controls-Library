namespace CustomControls
{
    using System.Collections.Generic;

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

        public DocumentOpenedCompletionArgs(IDocument openedDocument)
        {
            List<IDocument> OpenedDocumentList = new List<IDocument>();
            OpenedDocumentList.Add(openedDocument);

            this.OpenedDocumentList = OpenedDocumentList;
        }

        public DocumentOpenedCompletionArgs(IReadOnlyList<IDocument> openedDocumentList)
        {
            OpenedDocumentList = openedDocumentList;
        }

        public IReadOnlyList<IDocument> OpenedDocumentList { get; private set; }
    }
}
