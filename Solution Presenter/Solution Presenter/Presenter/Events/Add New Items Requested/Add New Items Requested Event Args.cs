namespace CustomControls
{
    using System.Collections.Generic;
    using System.Windows;

    /// <summary>
    /// Represents the event data for an add new items request event.
    /// </summary>
    public class AddNewItemsRequestedEventArgs : SolutionPresenterEventArgs<AddNewItemsRequestedEventArgs>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddNewItemsRequestedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The event that occured.</param>
        /// <param name="eventContext">The event context.</param>
        public AddNewItemsRequestedEventArgs(RoutedEvent routedEvent, IAddNewItemsRequestedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        /// <summary>
        /// Gets the destination folder path.
        /// </summary>
        public IFolderPath DestinationFolderPath { get { return ((IAddNewItemsRequestedEventContext)EventContext).DestinationFolderPath; } }

        /// <summary>
        /// Notifies handlers that the operation is completed.
        /// </summary>
        /// <param name="documentPathList">The list of document paths.</param>
        public virtual void NotifyCompleted(IList<IDocumentPath> documentPathList)
        {
            IAddNewItemsRequestedCompletionArgs CompletionArgs = new AddNewItemsRequestedCompletionArgs(documentPathList);
            NotifyEventCompleted(CompletionArgs);
        }
    }
}
