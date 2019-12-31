namespace CustomControls
{
    using System;

    public class DocumentWindowEventArgs : EventArgs
    {
        public DocumentWindowEventArgs(IDocument document)
        {
            this.Document = document;
        }

        public IDocument Document { get; private set; }
    }
}
