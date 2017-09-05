using System.Collections.Generic;
using System.Windows;
using Verification;

namespace CustomControls
{
    public class TreeChangedEventArgs : RoutedEventArgs
    {
        #region Init
        public TreeChangedEventArgs(RoutedEvent routedEvent, IReadOnlyDictionary<ITreeNodePath, IPathConnection> pathTable, bool isAdd, bool isUndoRedo)
            : base(routedEvent)
        {
            Assert.ValidateReference(pathTable);

            this.PathTable = pathTable;
            this.IsAdd = isAdd;
            this.IsUndoRedo = isUndoRedo;
        }
        #endregion

        #region Properties
        public IReadOnlyDictionary<ITreeNodePath, IPathConnection> PathTable { get; private set; }
        public bool IsAdd { get; private set; }
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
