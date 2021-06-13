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
    }
}
