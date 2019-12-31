namespace CustomControls
{
    using System;
    using System.Windows;

    /// <summary>
    /// Represents the event data for a move event.
    /// </summary>
    public class MovedEventArgs : RoutedEventArgs
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="MovedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The event that occured.</param>
        /// <param name="path">The moved item path.</param>
        /// <param name="oldParentPath">Path to the old parent.</param>
        /// <param name="newParentPath">Path to the new parent.</param>
        /// <param name="isUndoRedo">True if the operation is part of a undo or redo operation.</param>
        public MovedEventArgs(RoutedEvent routedEvent, ITreeNodePath path, IFolderPath oldParentPath, IFolderPath newParentPath, bool isUndoRedo)
            : base(routedEvent)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (oldParentPath == null)
                throw new ArgumentNullException(nameof(oldParentPath));
            if (newParentPath == null)
                throw new ArgumentNullException(nameof(newParentPath));

            Path = path;
            OldParentPath = oldParentPath;
            NewParentPath = newParentPath;
            IsUndoRedo = isUndoRedo;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the moved item path.
        /// </summary>
        public ITreeNodePath Path { get; private set; }

        /// <summary>
        /// Gets the path to the old parent.
        /// </summary>
        public IFolderPath OldParentPath { get; private set; }

        /// <summary>
        /// Gets the path to the new parent.
        /// </summary>
        public IFolderPath NewParentPath { get; private set; }

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
