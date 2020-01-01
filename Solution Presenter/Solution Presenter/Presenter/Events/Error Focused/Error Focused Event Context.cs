namespace CustomControls
{
    /// <summary>
    /// Represents a context for the <see cref="ErrorFocusedEventArgs"/> event data.
    /// </summary>
    public class ErrorFocusedEventContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorFocusedEventContext"/> class.
        /// </summary>
        /// <param name="document">The associated document.</param>
        /// <param name="errorLocation">The error location.</param>
        public ErrorFocusedEventContext(IDocument document, object? errorLocation)
        {
            Document = document;
            ErrorLocation = errorLocation;
        }

        /// <summary>
        /// Gets the associated document.
        /// </summary>
        public IDocument Document { get; }

        /// <summary>
        /// Gets the error location.
        /// </summary>
        public object? ErrorLocation { get; }
    }
}
