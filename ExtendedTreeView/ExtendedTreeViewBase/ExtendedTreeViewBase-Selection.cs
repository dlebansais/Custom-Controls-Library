namespace CustomControls
{
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// Represents a control with a tree of nodes that can be moved around with Drag and Drop.
    /// </summary>
    public abstract partial class ExtendedTreeViewBase : MultiSelector
    {
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
