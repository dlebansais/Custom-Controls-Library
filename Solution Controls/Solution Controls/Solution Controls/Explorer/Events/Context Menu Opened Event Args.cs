using System.Collections.Generic;
using System.Windows;
using Verification;

namespace CustomControls
{
    public class ContextMenuOpenedEventArgs : RoutedEventArgs
    {
        #region Init
        public ContextMenuOpenedEventArgs(RoutedEvent routedEvent, IReadOnlyCollection<ITreeNodePath> selectedItems, ICollection<ExtendedRoutedCommand> canShowCommandList)
            : base(routedEvent)
        {
            Assert.ValidateReference(selectedItems);
            Assert.ValidateReference(canShowCommandList);

            this.SelectedItems = selectedItems;
            this.CanShowCommandList = canShowCommandList;
        }
        #endregion

        #region Properties
        public IReadOnlyCollection<ITreeNodePath> SelectedItems { get; private set; }
        #endregion

        #region Client Interface
        public virtual bool ContainsCommand(ExtendedRoutedCommand command)
        {
            return CanShowCommandList.Contains(command);
        }

        public virtual void RemoveCommand(ExtendedRoutedCommand command)
        {
            CanShowCommandList.Remove(command);
        }

        private ICollection<ExtendedRoutedCommand> CanShowCommandList;
        #endregion
    }
}
