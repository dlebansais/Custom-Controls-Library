namespace CustomControls
{
    using System.Collections.Generic;
    using System.Windows;

    /// <summary>
    /// Represents the event data for a folder enumerated event.
    /// </summary>
    public class FolderEnumeratedEventArgs : SolutionPresenterEventArgs<FolderEnumeratedEventArgs>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FolderEnumeratedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The event that occured.</param>
        /// <param name="eventContext">The event context.</param>
        public FolderEnumeratedEventArgs(RoutedEvent routedEvent, FolderEnumeratedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        /// <summary>
        /// Gets the parent path.
        /// </summary>
        public IFolderPath ParentPath { get { return (IFolderPath)((FolderEnumeratedEventContext)EventContext).ParentPath; } }

        /// <summary>
        /// Gets properties of the root object.
        /// </summary>
        public IRootProperties RootProperties { get { return ((FolderEnumeratedEventContext)EventContext).RootProperties; } }

        /// <summary>
        /// Gets the list of expanded folders in the enumeration.
        /// </summary>
        public ICollection<IFolderPath> ExpandedFolderList { get { return ((FolderEnumeratedEventContext)EventContext).ExpandedFolderList; } }

        /// <summary>
        /// Gets the enumeration context.
        /// </summary>
        public object Context { get { return ((FolderEnumeratedEventContext)EventContext).Context; } }

        /// <summary>
        /// Notifies handlers that the operation is completed.
        /// </summary>
        /// <param name="children">The enumerated folders.</param>
        /// <param name="childrenProperties">Properties of enumerated folders.</param>
        public virtual void NotifyCompleted(IReadOnlyList<ITreeNodePath> children, IReadOnlyDictionary<ITreeNodePath, ITreeNodeProperties> childrenProperties)
        {
            IFolderEnumeratedCompletionArgs CompletionArgs = new FolderEnumeratedCompletionArgs(children, childrenProperties);
            NotifyEventCompleted(CompletionArgs);
        }
    }
}
