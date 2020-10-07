namespace CustomControls
{
    using System;
    using System.Windows;

    /// <summary>
    /// Represents the event data for a name changed event.
    /// </summary>
    public class NameChangedEventArgs : RoutedEventArgs
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="NameChangedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The event that occured.</param>
        /// <param name="path">The changed item path.</param>
        /// <param name="oldName">The old name.</param>
        /// <param name="newName">The new name.</param>
        /// <param name="isUndoRedo">True if the operation is part of a undo or redo operation.</param>
        public NameChangedEventArgs(RoutedEvent routedEvent, ITreeNodePath path, string oldName, string newName, bool isUndoRedo)
            : base(routedEvent)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (oldName == null)
                throw new ArgumentNullException(nameof(oldName));
            if (newName == null)
                throw new ArgumentNullException(nameof(newName));

            Path = path;
            OldName = oldName;
            NewName = newName;
            IsUndoRedo = isUndoRedo;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the changed item path.
        /// </summary>
        public ITreeNodePath Path { get; private set; }

        /// <summary>
        /// Gets the old item name.
        /// </summary>
        public string OldName { get; private set; }

        /// <summary>
        /// Gets the new item name.
        /// </summary>
        public string NewName { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the operation is part of a undo or redo operation.
        /// </summary>
        public bool IsUndoRedo { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the operation is canceled.
        /// </summary>
        public bool IsCanceled { get; private set; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Cancels the operation.
        /// </summary>
        public void Cancel()
        {
            IsCanceled = true;
        }
        #endregion
    }
}
