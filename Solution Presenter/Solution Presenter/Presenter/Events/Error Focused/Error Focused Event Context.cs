using System.Collections.Generic;

namespace CustomControls
{
    public class ErrorFocusedEventContext
    {
        public ErrorFocusedEventContext(IDocument document, object errorLocation)
        {
            this.Document = document;
            this.ErrorLocation = errorLocation;
        }

        public IDocument Document { get; private set; }
        public object ErrorLocation { get; private set; }
    }
}
