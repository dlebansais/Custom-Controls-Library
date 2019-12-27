namespace CustomControls
{
    public class ErrorFocusedEventContext
    {
        public ErrorFocusedEventContext(IDocument document, object? errorLocation)
        {
            Document = document;
            ErrorLocation = errorLocation;
        }

        public IDocument Document { get; }
        public object? ErrorLocation { get; }
    }
}
