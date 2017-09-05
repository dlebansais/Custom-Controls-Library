using System.Windows;

namespace CustomControls
{
    public class TreeViewCollectionModifiedEventArgs : RoutedEventArgs
    {
        public TreeViewCollectionModifiedEventArgs(RoutedEvent routedEvent, TreeViewCollectionOperation treeViewCollectionOperation, int filledItemCount)
            : base(routedEvent)
        {
            this.TreeViewCollectionOperation = treeViewCollectionOperation;
            this.FilledItemCount = filledItemCount;
        }

        public TreeViewCollectionOperation TreeViewCollectionOperation { get; private set; }
        public int FilledItemCount { get; private set; }
    }
}
