using System.Windows;

namespace CustomControls
{
    public class DragStartingEventArgs : DragDropEventArgs
    {
        internal DragStartingEventArgs(RoutedEvent routedEvent, IDragSourceControl dragSource, CancellationToken cancellation)
            : base(routedEvent, dragSource)
        {
            this.Cancellation = cancellation;
        }

        protected CancellationToken Cancellation { get; private set; }

        public void Cancel()
        {
            Cancellation.Cancel();
        }
    }
}
