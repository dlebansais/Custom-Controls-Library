namespace CustomControls
{
    using System;
    using System.ComponentModel;
    using System.Windows.Controls;

    /// <summary>
    /// Represents an item in a tree view control.
    /// </summary>
    public partial class ExtendedTreeViewItemBase : ContentControl, INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the indentation level of the item.
        /// </summary>
        public int Level { get { return Host.ItemLevel(Content); } }

        /// <summary>
        /// Updates the <see cref="IsDropOver"/> property.
        /// </summary>
        public void UpdateIsDropOver()
        {
            SetValue(IsDropOverPropertyKey, ExtendedTreeViewBase.IsDropOver(this));
        }

        /// <summary>
        /// Updates the disconnected item.
        /// </summary>
        /// <param name="value">Candidate value for the disconnected object.</param>
        public static void UpdateDisconnectedItem(object value)
        {
            if (DisconnectedItem == null && value != null)
            {
                Type ItemType = value.GetType();
                if (ItemType.FullName == "MS.Internal.NamedObject")
                    DisconnectedItem = value;
            }
        }

        private static object? DisconnectedItem;
    }
}
