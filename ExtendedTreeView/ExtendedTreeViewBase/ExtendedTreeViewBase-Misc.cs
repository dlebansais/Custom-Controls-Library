namespace CustomControls
{
    using System;
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;

    /// <summary>
    /// Represents a control with a tree of nodes that can be moved around with Drag and Drop.
    /// </summary>
    public abstract partial class ExtendedTreeViewBase : MultiSelector
    {
        /// <summary>
        /// Gets the container associated to an item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The container.</returns>
        protected virtual ExtendedTreeViewItemBase ContainerFromItem(object item)
        {
            return (ExtendedTreeViewItemBase)ItemContainerGenerator.ContainerFromItem(item);
        }

        /// <summary>
        /// Gets the container associated to the item at the provided position.
        /// </summary>
        /// <param name="index">the item position.</param>
        /// <returns>The container.</returns>
        protected virtual ExtendedTreeViewItemBase ContainerFromIndex(int index)
        {
            return (ExtendedTreeViewItemBase)ItemContainerGenerator.ContainerFromIndex(index);
        }

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

                foreach (object Child in Children)
                    Result += CountVisibleChildren(Child);
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
            foreach (object ChildItem in Children)
            {
                InternalInsert(NewIndex++, ChildItem);

                if (IsExpanded(ChildItem))
                    NewIndex = Expand(ChildItem, NewIndex);
            }

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
            foreach (object ChildItem in Children)
            {
                InternalRemove(index, ChildItem);

                if (IsExpanded(ChildItem))
                    Collapse(ChildItem, index);
            }
        }

        /// <summary>
        /// Checks whether an item can be cloned.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>True if cloneable; otherwise, false.</returns>
        protected virtual bool IsItemCloneable(object item)
        {
            return item is ICloneable;
        }

        /// <summary>
        /// Gets the indentation level of an item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The indentation level.</returns>
        public int ItemLevel(object item)
        {
            int Level = -1;

            object? CurrentItem = item;
            while (CurrentItem != null)
            {
                Level++;
                CurrentItem = GetItemParent(CurrentItem);
            }

            return Level;
        }

        /// <summary>
        /// Checks whether a container is the destination of a drop.
        /// </summary>
        /// <param name="itemContainer">The container.</param>
        /// <returns>True if destination of a drop; otherwise, false.</returns>
        public static bool IsDropOver(ExtendedTreeViewItemBase itemContainer)
        {
            return itemContainer == DropTargetContainer;
        }

        /// <summary>
        /// Clicks the item before <paramref name="item"/>.
        /// </summary>
        /// <param name="item">The item.</param>
        public void ClickPreviousItem(object item)
        {
            if (!IsCtrlDown())
            {
                int ItemIndex = VisibleChildren.IndexOf(item);
                if (ItemIndex > 0)
                    LeftClickSelect(VisibleChildren[ItemIndex - 1]);
            }
        }

        /// <summary>
        /// Clicks the item after <paramref name="item"/>.
        /// </summary>
        /// <param name="item">The item.</param>
        public void ClickNextItem(object item)
        {
            if (!IsCtrlDown())
            {
                int ItemIndex = VisibleChildren.IndexOf(item);
                if (ItemIndex >= 0 && ItemIndex + 1 < VisibleChildren.Count)
                    LeftClickSelect(VisibleChildren[ItemIndex + 1]);
            }
        }

        /// <summary>
        /// Clicks an item to select it.
        /// </summary>
        /// <param name="item">The item.</param>
        public void LeftClickSelect(object item)
        {
            switch (SelectionMode)
            {
                case SelectionMode.Single:
                    LeftClickSelectSingle(item);
                    break;

                case SelectionMode.Multiple:
                    LeftClickSelectMultiple(item);
                    break;

                case SelectionMode.Extended:
                    LeftClickSelectExtended(item);
                    break;
            }
        }

        private void LeftClickSelectSingle(object item)
        {
            if (!SelectedItems.Contains(item))
            {
                UnselectAll();
                AddSelectedItem(item);
                SetLastSelectedItem(item);
            }
        }

        private void LeftClickSelectMultiple(object item)
        {
            if (!SelectedItems.Contains(item))
            {
                AddSelectedItem(item);
                SetLastSelectedItem(item);
                SetLastClickedItem(item);
            }
        }

        private void LeftClickSelectExtended(object item)
        {
            if (IsShiftDown())
                LeftClickSelectExtendedShiftDown(item);
            else if (IsCtrlDown())
                LeftClickSelectExtendedCtrlDown(item);
            else if (!SelectedItems.Contains(item))
            {
                UnselectAll();
                AddSelectedItem(item);
                SetLastSelectedItem(item);
                SetLastClickedItem(item);
            }
        }

        private void LeftClickSelectExtendedShiftDown(object item)
        {
            int FirstIndex = VisibleChildren.IndexOf(item);
            int LastIndex = LastSelectedItem != null && IsLastSelectedItemSet ? VisibleChildren.IndexOf(LastSelectedItem) : FirstIndex;

            if (FirstIndex >= 0 && LastIndex >= 0)
            {
                if (FirstIndex > LastIndex)
                {
                    int Index = FirstIndex;
                    FirstIndex = LastIndex;
                    LastIndex = Index;
                }

                BeginUpdateSelectedItems();

                if (!IsCtrlDown())
                    SelectedItems.Clear();
                for (int i = FirstIndex; i <= LastIndex; i++)
                    AddSelectedItem(VisibleChildren[i]);

                EndUpdateSelectedItems();

                if (!IsLastSelectedItemSet)
                    SetLastSelectedItem(item);
            }
        }

        private void LeftClickSelectExtendedCtrlDown(object item)
        {
            if (!SelectedItems.Contains(item))
            {
                AddSelectedItem(item);
                SetLastClickedItem(item);
            }
        }

        /// <summary>
        /// Clicks an item and unselects it.
        /// </summary>
        /// <param name="item">The item.</param>
        public void LeftClickUnselect(object item)
        {
            switch (SelectionMode)
            {
                case SelectionMode.Single:
                    break;

                case SelectionMode.Multiple:
                    if (SelectedItems.Contains(item))
                        if (!IsLastClickedItem(item))
                            RemoveSelectedItem(item);
                        else
                            ClearLastClickedItem();
                    break;

                case SelectionMode.Extended:
                    if (IsShiftDown())
                    {
                    }
                    else if (IsCtrlDown())
                    {
                        if (SelectedItems.Contains(item))
                            if (!IsLastClickedItem(item))
                                RemoveSelectedItem(item);
                            else
                                ClearLastClickedItem();
                    }
                    else
                    {
                        UnselectAll();
                        AddSelectedItem(item);
                        SetLastSelectedItem(item);
                        ClearLastClickedItem();
                    }

                    break;
            }
        }

        /// <summary>
        /// Right-click an item to select it.
        /// </summary>
        /// <param name="item">The item.</param>
        public void RightClickSelect(object item)
        {
            switch (SelectionMode)
            {
                case SelectionMode.Single:
                    if (!SelectedItems.Contains(item))
                    {
                        UnselectAll();
                        AddSelectedItem(item);
                        SetLastSelectedItem(item);
                    }

                    break;

                case SelectionMode.Multiple:
                    if (!SelectedItems.Contains(item))
                    {
                        AddSelectedItem(item);
                        SetLastSelectedItem(item);
                        SetLastClickedItem(item);
                    }

                    break;

                case SelectionMode.Extended:
                    if (!IsShiftDown() && !IsCtrlDown() && !SelectedItems.Contains(item))
                    {
                        UnselectAll();
                        AddSelectedItem(item);
                        SetLastSelectedItem(item);
                    }

                    break;
            }
        }

        /// <summary>
        /// Right-click an item and unselects it.
        /// </summary>
        /// <param name="item">The item.</param>
        public void RightClickUnselect(object item)
        {
            switch (SelectionMode)
            {
                case SelectionMode.Single:
                    break;

                case SelectionMode.Multiple:
                    if (SelectedItems.Contains(item))
                        if (!IsLastClickedItem(item))
                            RemoveSelectedItem(item);
                        else
                            ClearLastClickedItem();
                    break;

                case SelectionMode.Extended:
                    break;
            }
        }

        /// <summary>
        /// Gets the source of an event.
        /// </summary>
        /// <param name="e">The event data.</param>
        /// <returns>The event source.</returns>
        protected virtual ExtendedTreeViewItemBase? GetEventSourceItem(RoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            DependencyObject? Current = e.OriginalSource as DependencyObject;
            while (Current != null)
            {
                if (Current is ExtendedTreeViewItemBase AsContainerItem)
                    return AsContainerItem;

                if (Current is ToggleButton)
                    return null;

                Current = VisualTreeHelper.GetParent(Current);
            }

            return null;
        }

        /// <summary>
        /// Gets the target of a drop event.
        /// </summary>
        /// <param name="e">The event data.</param>
        /// <returns>The event target.</returns>
        protected virtual ExtendedTreeViewItemBase? GetTarget(DragEventArgs e)
        {
            return GetEventSourceItem(e);
        }

        /// <summary>
        /// Checks whether the CTRL key is down.
        /// </summary>
        /// <returns>True if down; otherwise, false.</returns>
        protected virtual bool IsCtrlDown()
        {
            return (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && !(Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt));
        }

        /// <summary>
        /// Checks whether the SHIFT key is down.
        /// </summary>
        /// <returns>True if down; otherwise, false.</returns>
        protected virtual bool IsShiftDown()
        {
            return Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
        }

        /// <summary>
        /// Sorts two items by index.
        /// </summary>
        /// <param name="item1">The first item.</param>
        /// <param name="item2">The second item.</param>
        /// <returns>The sort result.</returns>
        protected virtual int SortByIndex(object item1, object item2)
        {
            return VisibleChildren.IndexOf(item1) - VisibleChildren.IndexOf(item2);
        }

        /// <summary>
        /// Adds an item to the list of selected items.
        /// </summary>
        /// <param name="item">The item.</param>
        protected virtual void AddSelectedItem(object item)
        {
            SelectedItems.Add(item);
        }

        /// <summary>
        /// Removes an item from the list of selected items.
        /// </summary>
        /// <param name="item">The item.</param>
        protected virtual void RemoveSelectedItem(object item)
        {
            if (IsLastSelectedItem(item))
                ClearLastSelectedItem();

            SelectedItems.Remove(item);
        }

        /// <summary>
        /// Gets a value indicating whether there is a last selected item.
        /// </summary>
        protected virtual bool IsLastSelectedItemSet
        {
            get { return LastSelectedItem != null; }
        }

        /// <summary>
        /// Sets the last selected item.
        /// </summary>
        /// <param name="item">The item.</param>
        protected virtual void SetLastSelectedItem(object item)
        {
            LastSelectedItem = item;
        }

        /// <summary>
        /// Checks whether an item is the last selected.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>True if the last selected.</returns>
        protected virtual bool IsLastSelectedItem(object item)
        {
            return LastSelectedItem == item;
        }

        /// <summary>
        /// Clears the last selected item.
        /// </summary>
        protected virtual void ClearLastSelectedItem()
        {
            LastSelectedItem = null;
        }

        /// <summary>
        /// Gets a value indicating whether there is a last clicked item.
        /// </summary>
        protected virtual bool IsLastClickedItemSet
        {
            get { return LastClickedItem != null; }
        }

        /// <summary>
        /// Sets the last clicked item.
        /// </summary>
        /// <param name="item">The item.</param>
        protected virtual void SetLastClickedItem(object item)
        {
            LastClickedItem = item;
        }

        /// <summary>
        /// Checks whether an item is the last clicked.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>True if the last clicked.</returns>
        protected virtual bool IsLastClickedItem(object item)
        {
            return LastClickedItem == item;
        }

        /// <summary>
        /// Clears the last clicked item.
        /// </summary>
        protected virtual void ClearLastClickedItem()
        {
            LastClickedItem = null;
        }

        /// <summary>
        /// Gets the last selected item.
        /// </summary>
        protected object? LastSelectedItem { get; private set; }

        /// <summary>
        /// Gets the last clicked item.
        /// </summary>
        protected object? LastClickedItem { get; private set; }
    }
}
