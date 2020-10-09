namespace CustomControls
{
    using System;
    using System.Collections;
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// Represents a control with a tree of nodes that can be moved around with Drag and Drop.
    /// </summary>
    public abstract partial class ExtendedTreeViewBase : MultiSelector
    {
        private void InitializeImplementation()
        {
            ItemsSource = VisibleChildren;
        }

        /// <summary>
        /// Builds the flat children table.
        /// </summary>
        protected virtual void BuildFlatChildrenTables()
        {
            ResetFlatChildren();
            InsertChildrenFromRoot();
        }

        /// <summary>
        /// Resets the flat children table.
        /// </summary>
        protected virtual void ResetFlatChildren()
        {
            UninstallAllHandlers();

            VisibleChildren.Clear();
            ExpandedChildren.Clear();
        }

        /// <summary>
        /// Uninstall all handlers on items.
        /// </summary>
        protected virtual void UninstallAllHandlers()
        {
            foreach (object item in VisibleChildren)
                UninstallHandlers(item);
        }

        /// <summary>
        /// Inserts child items starting from the content root.
        /// </summary>
        protected virtual void InsertChildrenFromRoot()
        {
            NotifyPreviewCollectionModified(TreeViewCollectionOperation.Insert);

            if (Content != null)
            {
                IInsertItemContext Context = CreateInsertItemContext(Content, 0);
                Context.Start();

                InsertChildren(Context, Content, null);

                Context.Complete();
                Context.Close();
            }

            NotifyCollectionModified(TreeViewCollectionOperation.Insert);
        }

        /// <summary>
        /// Inserts children of an item.
        /// </summary>
        /// <param name="context">The insertion context.</param>
        /// <param name="item">The item with children.</param>
        /// <param name="parentItem">The parent item.</param>
        protected virtual void InsertChildren(IInsertItemContext context, object item, object? parentItem)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            IList Children = GetItemChildren(item);
            bool IsExpanded = IsItemExpandedAtStart || (parentItem == null && IsRootAlwaysExpanded);

            if (IsExpanded)
                ExpandedChildren.Add(item, Children);

            InternalInsert(context.ShownIndex, item);
            context.NextIndex();

            if (IsExpanded)
                foreach (object ChildItem in Children)
                    InsertChildren(context, ChildItem, item);
        }

        /// <summary>
        /// Removes items from the tree.
        /// </summary>
        /// <param name="context">The remove context.</param>
        /// <param name="item">The item from which to remove children.</param>
        /// <param name="parentItem">The parent item.</param>
        protected virtual void RemoveChildren(IRemoveItemContext context, object item, object? parentItem)
        {
            ExtendedTreeViewItemBase ItemContainer = ContainerFromItem(item);

            if ((ItemContainer != null && ItemContainer.IsExpanded) || (ItemContainer == null && IsItemExpandedAtStart))
            {
                IList Children = GetItemChildren(item);
                foreach (object ChildItem in Children)
                    RemoveChildren(context, ChildItem, item);
            }

            if (context != null)
            {
                InternalRemove(context.ShownIndex, item);
                context.NextIndex();
            }

            if (ExpandedChildren.ContainsKey(item))
                ExpandedChildren.Remove(item);
        }

        /// <summary>
        /// Performs the insertion operation.
        /// </summary>
        /// <param name="index">The item index in the list of visible children.</param>
        /// <param name="item">The item to insert.</param>
        protected virtual void InternalInsert(int index, object item)
        {
            VisibleChildren.Insert(index, item);
            InstallHandlers(item);
        }

        /// <summary>
        /// Performs the remove operation.
        /// </summary>
        /// <param name="index">The item index in the list of visible children.</param>
        /// <param name="item">The item to remove.</param>
        protected virtual void InternalRemove(int index, object item)
        {
            UninstallHandlers(item);
            VisibleChildren.RemoveAt(index);
        }
    }
}
