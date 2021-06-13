namespace CustomControls
{
    using System.Collections;
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// Represents a control with a tree of nodes that can be moved around with Drag and Drop.
    /// </summary>
    public abstract partial class ExtendedTreeViewBase : MultiSelector
    {
        /// <summary>
        /// Gets the list of visible items.
        /// </summary>
        public virtual IList VisibleItems()
        {
            IList Result = CreateItemList();
            foreach (object item in VisibleChildren)
                Result.Add(item);

            return Result;
        }

        /// <summary>
        /// Scrolls the view to make selected items visible.
        /// </summary>
        /// <param name="item">Item to be made visible.</param>
        public virtual void ScrollIntoView(object item)
        {
            ExtendedTreeViewItemBase ItemContainer = ContainerFromItem(item);
            if (ItemContainer != null)
                ItemContainer.BringIntoView();
        }

        /// <summary>
        /// Checks if an item is expanded.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>True if expanded; Otherwise, false.</returns>
        public virtual bool IsExpanded(object item)
        {
            return ExpandedChildren.ContainsKey(item);
        }

        /// <summary>
        /// Expands an item.
        /// </summary>
        /// <param name="item">The item.</param>
        public virtual void Expand(object item)
        {
            ExtendedTreeViewItemBase ItemContainer = ContainerFromItem(item);
            if (ItemContainer != null)
                ItemContainer.IsExpanded = true;
        }

        /// <summary>
        /// Collapses an item.
        /// </summary>
        /// <param name="item">The item.</param>
        public virtual void Collapse(object item)
        {
            ExtendedTreeViewItemBase ItemContainer = ContainerFromItem(item);
            if (ItemContainer != null)
                ItemContainer.IsExpanded = false;
        }

        /// <summary>
        /// Toggles whether an item is expanded.
        /// </summary>
        /// <param name="item">The item.</param>
        public virtual void ToggleIsExpanded(object item)
        {
            ExtendedTreeViewItemBase ItemContainer = ContainerFromItem(item);
            if (ItemContainer != null)
                ItemContainer.IsExpanded = !ItemContainer.IsExpanded;
        }

        /// <summary>
        /// Checks if an item is selected.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>True if selected; Otherwise, false.</returns>
        public virtual bool IsSelected(object item)
        {
            return SelectedItems.Contains(item);
        }

        /// <summary>
        /// Adds an item to the selection.
        /// </summary>
        /// <param name="item">The item.</param>
        public virtual void SetSelected(object item)
        {
            SelectedItem = item;
        }

        /// <summary>
        /// Checks if an item is visible in the scrolled view.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>True if visible; Otherwise, false.</returns>
        public virtual bool IsItemVisible(object item)
        {
            return VisibleChildren.Contains(item);
        }

        /// <summary>
        /// Gets a value indicating whether the control allows to copy items.
        /// </summary>
        /// <returns>True if allowed; Otherwise, false.</returns>
        public virtual bool IsCopyPossible
        {
            get { return DragSource.IsDragPossible(); }
        }
    }
}
