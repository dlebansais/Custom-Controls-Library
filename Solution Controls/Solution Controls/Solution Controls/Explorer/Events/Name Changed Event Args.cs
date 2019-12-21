﻿using System;
using System.Windows;

namespace CustomControls
{
    public class NameChangedEventArgs : RoutedEventArgs
    {
        #region Init
        public NameChangedEventArgs(RoutedEvent routedEvent, ITreeNodePath path, string oldName, string newName, bool isUndoRedo)
            : base(routedEvent)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (oldName == null)
                throw new ArgumentNullException(nameof(oldName));
            if (newName == null)
                throw new ArgumentNullException(nameof(newName));

            this.Path = path;
            this.OldName = oldName;
            this.NewName = newName;
            this.IsUndoRedo = isUndoRedo;
        }
        #endregion

        #region Properties
        public ITreeNodePath Path { get; private set; }
        public string OldName { get; private set; }
        public string NewName { get; private set; }
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
