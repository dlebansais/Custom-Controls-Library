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
        /// Counts how many children are expanded.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="index">The position from where to count.</param>
        /// <param name="excludedIndex">Index of the child item excluded from the count.</param>
        /// <returns>The number of expanded children.</returns>
        protected virtual int CountPreviousChildrenExpanded(object item, int index, int excludedIndex)
        {
            int Result = 1;

            ExtendedTreeViewItemBase ItemContainer = ContainerFromItem(item);

            if ((ItemContainer != null && ItemContainer.IsExpanded) || (ItemContainer == null && IsItemExpandedAtStart) || (ItemContainer == null && item == Content && IsRootAlwaysExpanded))
                for (int i = 0; i < index; i++)
                    if (i != excludedIndex)
                    {
                        object Child = GetItemChild(item, i);
                        Result += CountPreviousChildrenExpanded(Child, GetItemChildrenCount(Child), -1);
                    }

            return Result;
        }

        /// <summary>
        /// Counts how many children are visible.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The number of visible children.</returns>
        protected virtual int CountVisibleChildren(object item)
        {
            int Result = 0;

            ExtendedTreeViewItemBase ItemContainer = ContainerFromItem(item);

            if ((ItemContainer != null && ItemContainer.IsExpanded) || (ItemContainer == null && IsItemExpandedAtStart))
            {
                IList Children = GetItemChildren(item);
                Result += Children.Count;

#if NETCOREAPP3_1
                foreach (object? Child in Children)
                    if (Child != null)
                        Result += CountVisibleChildren(Child);
#else
                foreach (object Child in Children)
                    Result += CountVisibleChildren(Child);
#endif
            }

            return Result;
        }

        /// <summary>
        /// Sets an item to be expanded.
        /// </summary>
        /// <param name="item">The item.</param>
        public void SetItemExpanded(object item)
        {
            IList Children = GetItemChildren(item);
            ExpandedChildren.Add(item, Children);

            int Index = VisibleChildren.IndexOf(item) + 1;
            Expand(item, Index);
        }

        /// <summary>
        /// Checks if an item can be expanded.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>True if expandable; othewise, false.</returns>
        public bool IsItemExpandable(object item)
        {
            return !IsCtrlDown() && GetItemChildrenCount(item) > 0;
        }

        /// <summary>
        /// Checks if an item can be collapsed.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>True if collapsible; othewise, false.</returns>
        public bool IsItemCollapsible(object item)
        {
            return !IsCtrlDown() && GetItemChildrenCount(item) > 0 && (item != Content || !IsRootAlwaysExpanded);
        }

        /// <summary>
        /// Expands an item at the provided position.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="index">The item position.</param>
        /// <returns>The new position.</returns>
        protected virtual int Expand(object item, int index)
        {
            int NewIndex = index;

            IList Children = GetItemChildren(item);

#if NETCOREAPP3_1
            foreach (object? ChildItem in Children)
                if (ChildItem != null)
                {
                    InternalInsert(NewIndex++, ChildItem);

                    if (IsExpanded(ChildItem))
                        NewIndex = Expand(ChildItem, NewIndex);
                }
#else
            foreach (object ChildItem in Children)
            {
                InternalInsert(NewIndex++, ChildItem);

                if (IsExpanded(ChildItem))
                    NewIndex = Expand(ChildItem, NewIndex);
            }
#endif

            return NewIndex;
        }

        /// <summary>
        /// Collapses an item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void SetItemCollapsed(object item)
        {
            ExpandedChildren.Remove(item);

            int Index = VisibleChildren.IndexOf(item) + 1;
            Collapse(item, Index);
        }

        /// <summary>
        /// Collapses an item at the provided position.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="index">The item position.</param>
        protected virtual void Collapse(object item, int index)
        {
            IList Children = GetItemChildren(item);

#if NETCOREAPP3_1
            foreach (object? ChildItem in Children)
                if (ChildItem != null)
                {
                    InternalRemove(index, ChildItem);

                    if (IsExpanded(ChildItem))
                        Collapse(ChildItem, index);
                }
#else
            foreach (object ChildItem in Children)
            {
                InternalRemove(index, ChildItem);

                if (IsExpanded(ChildItem))
                    Collapse(ChildItem, index);
            }
#endif
        }
    }
}
