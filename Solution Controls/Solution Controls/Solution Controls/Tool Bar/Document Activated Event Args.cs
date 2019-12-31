namespace CustomControls
{
    using System.Windows;

    /// <summary>
    /// Represents data for a document activated event.
    /// </summary>
    public class DocumentActivatedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentActivatedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The event that occured.</param>
        /// <param name="activeDocument">The activated document.</param>
        public DocumentActivatedEventArgs(RoutedEvent routedEvent, IDocument activeDocument)
            : base(routedEvent)
        {
            ActiveDocument = activeDocument;
        }

        /// <summary>
        /// Gets the activated document.
        /// </summary>
        public IDocument ActiveDocument { get; }
    }
}
