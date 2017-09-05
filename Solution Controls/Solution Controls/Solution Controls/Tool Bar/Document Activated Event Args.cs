using System.Windows;

namespace CustomControls
{
    public class DocumentActivatedEventArgs : RoutedEventArgs
    {
        public DocumentActivatedEventArgs(RoutedEvent routedEvent, IDocument activeDocument)
            : base(routedEvent)
        {
            this.ActiveDocument = activeDocument;
        }

        public IDocument ActiveDocument { get; private set; }
    }
}
