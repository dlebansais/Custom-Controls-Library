namespace CustomControls
{
    using System.ComponentModel;
    using System.Windows.Controls;

    /// <summary>
    /// Represents an item in a tree view control.
    /// </summary>
    public partial class ExtendedTreeViewItemBase : ContentControl, INotifyPropertyChanged
    {
        /// <summary>
        /// Selects the item because a left button down event occured.
        /// </summary>
        protected void SelectItemOnLeftButtonDown()
        {
            Focus();
            Host.LeftClickSelect(Content);
        }

        /// <summary>
        /// Unselects the item because a left button up event occured.
        /// </summary>
        protected void UnselectItemOnLeftButtonUp()
        {
            Host.LeftClickUnselect(Content);
        }

        /// <summary>
        /// Selects the item because a right button down event occured.
        /// </summary>
        protected void SelectItemOnRightButtonDown()
        {
            Focus();
            Host.RightClickSelect(Content);
        }

        /// <summary>
        /// Unselects the item because a right button up event occured.
        /// </summary>
        protected void UnselectItemOnRightButtonUp()
        {
            Host.RightClickUnselect(Content);
        }
    }
}
