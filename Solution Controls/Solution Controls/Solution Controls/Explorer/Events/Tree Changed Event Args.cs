namespace CustomControls
{
    using System;
    using System.Collections.Generic;
    using System.Windows;

    /// <summary>
    /// Represents the event data for a tree changed event.
    /// </summary>
    public class TreeChangedEventArgs : RoutedEventArgs
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="TreeChangedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The event that occured.</param>
        /// <param name="pathTable">The table of paths.</param>
        /// <param name="isAdd">True if the operation is to add nodes.</param>
        /// <param name="isUndoRedo">True if the operation can be undone.</param>
        public TreeChangedEventArgs(RoutedEvent routedEvent, IReadOnlyDictionary<ITreeNodePath, IPathConnection> pathTable, bool isAdd, bool isUndoRedo)
            : base(routedEvent)
        {
            if (pathTable == null)
                throw new ArgumentNullException(nameof(pathTable));

            PathTable = pathTable;
            IsAdd = isAdd;
            IsUndoRedo = isUndoRedo;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the table of paths.
        /// </summary>
        public IReadOnlyDictionary<ITreeNodePath, IPathConnection> PathTable { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the operation is to add nodes.
        /// </summary>
        public bool IsAdd { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the operation can be undone.
        /// </summary>
        public bool IsUndoRedo { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the operation has been canceled.
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
