using System;
using System.Windows;

namespace CustomControls
{
    public class MovedEventArgs : RoutedEventArgs
    {
        #region Init
        public MovedEventArgs(RoutedEvent routedEvent, ITreeNodePath path, IFolderPath oldParentPath, IFolderPath newParentPath, bool isUndoRedo)
            : base(routedEvent)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (oldParentPath == null)
                throw new ArgumentNullException(nameof(oldParentPath));
            if (newParentPath == null)
                throw new ArgumentNullException(nameof(newParentPath));

            this.Path = path;
            this.OldParentPath = oldParentPath;
            this.NewParentPath = newParentPath;
            this.IsUndoRedo = isUndoRedo;
        }
        #endregion

        #region Properties
        public ITreeNodePath Path { get; private set; }
        public IFolderPath OldParentPath { get; private set; }
        public IFolderPath NewParentPath { get; private set; }
        public bool IsUndoRedo { get; private set; }
        public bool IsCanceled { get; private set; }
        #endregion

        #region Client Interface
        public void Cancel()
        {
            IsCanceled = true;
        }
        #endregion
    }
}
