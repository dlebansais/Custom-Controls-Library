namespace CustomControls
{
    using System;
    using System.Collections.Generic;
    using System.Windows;

    /// <summary>
    /// Represents the event data when a menu context is opened.
    /// </summary>
    public class ContextMenuOpenedEventArgs : RoutedEventArgs
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ContextMenuOpenedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The event that occured.</param>
        /// <param name="selectedItems">Selected items to which the menu applies.</param>
        /// <param name="canShowCommandList">The list of commands that can be shown.</param>
        public ContextMenuOpenedEventArgs(RoutedEvent routedEvent, IReadOnlyCollection<ITreeNodePath> selectedItems, ICollection<ExtendedRoutedCommand> canShowCommandList)
            : base(routedEvent)
        {
            if (selectedItems == null)
                throw new ArgumentNullException(nameof(selectedItems));
            if (canShowCommandList == null)
                throw new ArgumentNullException(nameof(canShowCommandList));

            this.SelectedItems = selectedItems;
            this.CanShowCommandList = canShowCommandList;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets selected items to which the menu applies.
        /// </summary>
        public IReadOnlyCollection<ITreeNodePath> SelectedItems { get; private set; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Checks if a command applies to selected items.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>True if the command applies; otherwise, false.</returns>
        public virtual bool ContainsCommand(ExtendedRoutedCommand command)
        {
            return CanShowCommandList.Contains(command);
        }

        /// <summary>
        /// Removes a command from the list of commands that can be shown.
        /// </summary>
        /// <param name="command">The command.</param>
        public virtual void RemoveCommand(ExtendedRoutedCommand command)
        {
            CanShowCommandList.Remove(command);
        }

        private ICollection<ExtendedRoutedCommand> CanShowCommandList;
        #endregion
    }
}
