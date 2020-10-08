namespace CustomControls
{
    using System.Windows;
    using System.Windows.Threading;

    /// <summary>
    /// Represents the event data for a folder created event.
    /// </summary>
    public class FolderCreatedEventArgs : SolutionPresenterEventArgs<FolderCreatedEventArgs>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FolderCreatedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The event that occured.</param>
        /// <param name="eventContext">The event context.</param>
        public FolderCreatedEventArgs(RoutedEvent routedEvent, FolderCreatedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        /// <summary>
        /// Gets the parent path.
        /// </summary>
        public IFolderPath ParentPath { get { return ((FolderCreatedEventContext)EventContext).ParentPath; } }

        /// <summary>
        /// Gets the folder name.
        /// </summary>
        public string FolderName { get { return ((FolderCreatedEventContext)EventContext).FolderName; } }

        /// <summary>
        /// Gets the root object properties.
        /// </summary>
        public IRootProperties RootProperties { get { return ((FolderCreatedEventContext)EventContext).RootProperties; } }

        /// <summary>
        /// Notifies handlers that the operation is completed.
        /// </summary>
        /// <param name="dispatcher">The window dispatcher.</param>
        /// <param name="newFolderPath">The new folder path.</param>
        /// <param name="newFolderProperties">The new folder properties.</param>
        public virtual void NotifyCompletedAsync(Dispatcher dispatcher, IFolderPath newFolderPath, IFolderProperties newFolderProperties)
        {
            IFolderCreatedCompletionArgs CompletionArgs = new FolderCreatedCompletionArgs(newFolderPath, newFolderProperties);
            NotifyEventCompletedAsync(dispatcher, CompletionArgs);
        }
    }
}
