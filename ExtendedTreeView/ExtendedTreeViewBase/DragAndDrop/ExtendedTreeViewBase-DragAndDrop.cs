namespace CustomControls
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// Represents a control with a tree of nodes that can be moved around with Drag and Drop.
    /// </summary>
    public abstract partial class ExtendedTreeViewBase : MultiSelector
    {
        private void InitializeDragAndDrop()
        {
            DragSource = CreateSourceControl();
            DragSource.DragActivityChanged += OnDragActivityChanged;
            DropTargetContainer = null;
            DropTargetContainerIndex = -1;
        }

        /// <summary>
        /// Gets the list of selected ims.
        /// </summary>
        /// <param name="canonicSelectedItemList">The list to fill.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        protected virtual bool GetCanonicSelectedItemList(CanonicSelection canonicSelectedItemList)
        {
            if (canonicSelectedItemList == null)
                return false;

            if (SelectedItems.Count == 0)
                return false;

            List<object> SortedSelectedItems = new List<object>();
            foreach (object item in SelectedItems)
                SortedSelectedItems.Add(item);

            SortedSelectedItems.Sort(SortByIndex);

            object FirstItem = SortedSelectedItems[0];
            if (FirstItem == Content)
                return false;

            object? FirstItemParent = GetItemParent(FirstItem);

            if (FirstItemParent != null)
            {
                if (GetItemsWithSameParent(SortedSelectedItems, FirstItemParent, canonicSelectedItemList))
                {
                    canonicSelectedItemList.DraggedItemParent = FirstItemParent;
                    return true;
                }

                if (GetItemsInSameBranch(SortedSelectedItems, FirstItemParent, canonicSelectedItemList))
                {
                    canonicSelectedItemList.DraggedItemParent = FirstItemParent;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the list of selected items with the same parent.
        /// </summary>
        /// <param name="sortedSelectedItems">The list of selected items, sorted.</param>
        /// <param name="firstItemParent">The parent of the first item.</param>
        /// <param name="canonicSelectedItemList">The list of selected items, unsorted.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        protected virtual bool GetItemsWithSameParent(IList sortedSelectedItems, object firstItemParent, CanonicSelection canonicSelectedItemList)
        {
            if (sortedSelectedItems == null || canonicSelectedItemList == null)
                return false;

            canonicSelectedItemList.AllItemsCloneable = true;

            foreach (object item in sortedSelectedItems)
            {
                if (GetItemParent(item) != firstItemParent)
                    return false;

                canonicSelectedItemList.ItemList.Add(item);

                if (!IsItemCloneable(item))
                    canonicSelectedItemList.AllItemsCloneable = false;
            }

            return true;
        }

        /// <summary>
        /// Gets the list of selected items in the same branch.
        /// </summary>
        /// <param name="sortedSelectedItems">The list of selected items, sorted.</param>
        /// <param name="firstItemParent">The parent of the first item.</param>
        /// <param name="canonicSelectedItemList">The list of selected items, unsorted.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        protected virtual bool GetItemsInSameBranch(IList sortedSelectedItems, object firstItemParent, CanonicSelection canonicSelectedItemList)
        {
            if (sortedSelectedItems == null || canonicSelectedItemList == null)
                return false;

            IList Children = GetItemChildren(firstItemParent);
            foreach (object ChildItem in Children)
            {
                if (sortedSelectedItems.Contains(ChildItem))
                {
                    if (!canonicSelectedItemList.ItemList.Contains(ChildItem))
                        canonicSelectedItemList.ItemList.Add(ChildItem);
                    if (!IsEntireBranchSelected(sortedSelectedItems, ChildItem, canonicSelectedItemList))
                        return false;
                }
            }

            if (canonicSelectedItemList.RecordCount < sortedSelectedItems.Count)
                return false;

            return true;
        }

        /// <summary>
        /// Checks if all items of a branch are selected.
        /// </summary>
        /// <param name="sortedSelectedItems">The list of selected items, sorted.</param>
        /// <param name="item">The parent item.</param>
        /// <param name="canonicSelectedItemList">The list of selected items, unsorted.</param>
        /// <returns>True if in the same branch; otherwise, false.</returns>
        protected virtual bool IsEntireBranchSelected(IList sortedSelectedItems, object item, CanonicSelection canonicSelectedItemList)
        {
            if (sortedSelectedItems == null || canonicSelectedItemList == null)
                return false;

            if (!IsItemCloneable(item))
                canonicSelectedItemList.AllItemsCloneable = false;

            canonicSelectedItemList.RecordCount++;

            if (IsExpanded(item))
            {
                IList Children = GetItemChildren(item);
                foreach (object ChildItem in Children)
                {
                    if (!sortedSelectedItems.Contains(ChildItem))
                        return false;

                    if (!IsEntireBranchSelected(sortedSelectedItems, ChildItem, canonicSelectedItemList))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets the flattened list of items.
        /// </summary>
        /// <param name="other">the source list.</param>
        /// <returns>The flattened list.</returns>
        protected virtual IList FlatItemList(IList? other)
        {
            IList Result = CreateItemList();

            if (other != null)
                foreach (object item in other)
                {
                    Result.Add(item);

                    IList FlatChildren = FlatItemList(GetItemChildren(item));
                    foreach (object Child in FlatChildren)
                        Result.Add(Child);
                }

            return Result;
        }

        /// <summary>
        /// Merges allowed effects with those allowed by a drag drop event.
        /// </summary>
        /// <param name="e">The event data.</param>
        /// <returns>Merged allowed effects.</returns>
        protected virtual DragDropEffects MergedAllowedEffects(DragEventArgs e)
        {
            if (e != null)
            {
                if (e.AllowedEffects.HasFlag(DragDropEffects.Move))
                    if (e.AllowedEffects.HasFlag(DragDropEffects.Copy) && e.KeyStates.HasFlag(DragDropKeyStates.ControlKey))
                        return DragDropEffects.Copy;
                    else
                        return DragDropEffects.Move;
                else if (e.AllowedEffects.HasFlag(DragDropEffects.Copy))
                    return DragDropEffects.Copy;
                else
                    return DragDropEffects.None;
            }
            else
                return DragDropEffects.None;
        }

        private IDragSourceControl DragSource = new EmptyDragSourceControl();
        private static ExtendedTreeViewItemBase? DropTargetContainer;
        private static int DropTargetContainerIndex;
    }
}
