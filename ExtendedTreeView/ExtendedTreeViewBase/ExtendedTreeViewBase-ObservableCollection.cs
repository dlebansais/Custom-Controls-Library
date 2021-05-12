namespace CustomControls
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// Represents a control with a tree of nodes that can be moved around with Drag and Drop.
    /// </summary>
    public abstract partial class ExtendedTreeViewBase : MultiSelector
    {
        /// <summary>
        /// Called when children of an item have changed.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="e">The event data.</param>
        protected virtual void HandleChildrenChanged(object item, NotifyCollectionChangedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    OnItemAddChildren(item, e.NewStartingIndex, e.NewItems);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    OnItemRemoveChildren(item, e.OldStartingIndex, e.OldItems);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    OnItemRemoveChildren(item, e.OldStartingIndex, e.OldItems);
                    OnItemAddChildren(item, e.NewStartingIndex, e.NewItems);
                    break;

                case NotifyCollectionChangedAction.Move:
                    OnItemMoveChildren(item, e.OldStartingIndex, e.NewStartingIndex, e.NewItems);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    OnItemResetChildren(item);
                    break;
            }
        }

        /// <summary>
        /// Called when children have been added to an item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="startIndex">Index where the first child is added.</param>
        /// <param name="itemList">The list of children.</param>
        protected virtual void OnItemAddChildren(object item, int startIndex, IList? itemList)
        {
            NotifyPreviewCollectionModified(TreeViewCollectionOperation.Insert);

            if (IsExpanded(item))
            {
                int ShownPreviousChildrenCount = VisibleChildren.IndexOf(item);
                ShownPreviousChildrenCount += CountPreviousChildrenExpanded(item, startIndex, -1);

                int ShownIndex = ShownPreviousChildrenCount;

                IInsertItemContext Context = CreateInsertItemContext(item, ShownIndex);
                Context.Start();

                if (itemList != null)
                {
#if NETCOREAPP3_1
                    foreach (object? ChildItem in itemList)
                        if (ChildItem != null)
                            InsertChildren(Context, ChildItem, item);
#else
                    foreach (object ChildItem in itemList)
                        InsertChildren(Context, ChildItem, item);
#endif
                }

                Context.Complete();
                Context.Close();
            }

            NotifyCollectionModified(TreeViewCollectionOperation.Insert);
        }

        /// <summary>
        /// Called when children of an item have been removed.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="startIndex">Index of the first removed child.</param>
        /// <param name="itemList">The list of removed children.</param>
        protected virtual void OnItemRemoveChildren(object item, int startIndex, IList? itemList)
        {
            NotifyPreviewCollectionModified(TreeViewCollectionOperation.Remove);

            if (IsExpanded(item))
            {
                int ShownPreviousChildrenCount = VisibleChildren.IndexOf(item);
                ShownPreviousChildrenCount += CountPreviousChildrenExpanded(item, startIndex, -1);

                int ShownIndex = ShownPreviousChildrenCount;

                IRemoveItemContext Context = CreateRemoveItemContext(item, ShownIndex);
                Context.Start();

                if (itemList != null)
                {
#if NETCOREAPP3_1
                    foreach (object? ChildItem in itemList)
                        if (ChildItem != null)
                            RemoveChildren(Context, ChildItem, item);
#else
                    foreach (object ChildItem in itemList)
                        RemoveChildren(Context, ChildItem, item);
#endif
                }

                Context.Complete();
                Context.Close();
            }

            NotifyCollectionModified(TreeViewCollectionOperation.Remove);
        }

        /// <summary>
        /// Called when children of an item have been moved.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="oldIndex">Index of the previous position of the first child.</param>
        /// <param name="newIndex">Index of the new position of the first child.</param>
        /// <param name="itemList">The list of moved children.</param>
        protected virtual void OnItemMoveChildren(object item, int oldIndex, int newIndex, IList? itemList)
        {
            NotifyPreviewCollectionModified(TreeViewCollectionOperation.Move);

            if (IsExpanded(item))
            {
                if (oldIndex > newIndex)
                {
                    int ShownPreviousChildrenCount = VisibleChildren.IndexOf(item);

                    int RemoveIndex = ShownPreviousChildrenCount;
                    RemoveIndex += CountPreviousChildrenExpanded(item, oldIndex + 1, newIndex);

                    IRemoveItemContext RemoveContext = CreateRemoveItemContext(item, RemoveIndex);
                    RemoveContext.Start();

                    if (itemList != null)
                    {
#if NETCOREAPP3_1
                        foreach (object? ChildItem in itemList)
                            if (ChildItem != null)
                                RemoveChildren(RemoveContext, ChildItem, item);
#else
                        foreach (object ChildItem in itemList)
                            RemoveChildren(RemoveContext, ChildItem, item);
#endif
                    }

                    RemoveContext.Complete();
                    RemoveContext.Close();

                    int InsertIndex = ShownPreviousChildrenCount;
                    InsertIndex += CountPreviousChildrenExpanded(item, newIndex, -1);

                    IInsertItemContext InsertContext = CreateInsertItemContext(item, InsertIndex);
                    InsertContext.Start();

                    if (itemList != null)
                    {
#if NETCOREAPP3_1
                        foreach (object? ChildItem in itemList)
                            if (ChildItem != null)
                                InsertChildren(InsertContext, ChildItem, item);
#else
                        foreach (object ChildItem in itemList)
                            InsertChildren(InsertContext, ChildItem, item);
#endif
                    }

                    InsertContext.Complete();
                    InsertContext.Close();
                }
                else if (oldIndex < newIndex)
                {
                    int ShownPreviousChildrenCount = VisibleChildren.IndexOf(item);

                    int RemoveIndex = ShownPreviousChildrenCount;
                    RemoveIndex += CountPreviousChildrenExpanded(item, oldIndex, -1);

                    IRemoveItemContext RemoveContext = CreateRemoveItemContext(item, RemoveIndex);
                    RemoveContext.Start();

                    if (itemList != null)
                    {
#if NETCOREAPP3_1
                        foreach (object? ChildItem in itemList)
                            if (ChildItem != null)
                                RemoveChildren(RemoveContext, ChildItem, item);
#else
                        foreach (object ChildItem in itemList)
                            RemoveChildren(RemoveContext, ChildItem, item);
#endif
                    }

                    RemoveContext.Complete();
                    RemoveContext.Close();

                    int InsertIndex = ShownPreviousChildrenCount;
                    InsertIndex += CountPreviousChildrenExpanded(item, newIndex, -1);

                    IInsertItemContext InsertContext = CreateInsertItemContext(item, InsertIndex);
                    InsertContext.Start();

                    if (itemList != null)
                    {
#if NETCOREAPP3_1
                        foreach (object? ChildItem in itemList)
                            if (ChildItem != null)
                                InsertChildren(InsertContext, ChildItem, item);
#else
                        foreach (object ChildItem in itemList)
                            InsertChildren(InsertContext, ChildItem, item);
#endif
                    }

                    InsertContext.Complete();
                    InsertContext.Close();
                }
            }

            NotifyCollectionModified(TreeViewCollectionOperation.Move);
        }

        /// <summary>
        /// Called when children of an item are reset.
        /// </summary>
        /// <param name="item">The item.</param>
        protected virtual void OnItemResetChildren(object item)
        {
            NotifyPreviewCollectionModified(TreeViewCollectionOperation.Remove);

            if (IsExpanded(item))
            {
                object? ParentItem = GetItemParent(item);

                int StartIndex;
                int RemoveCount;

                if (ParentItem != null)
                {
                    StartIndex = VisibleChildren.IndexOf(item) + 1;

                    IList Siblings = GetItemChildren(ParentItem);
                    int ItemIndex = Siblings.IndexOf(item);
                    if (ItemIndex + 1 < Siblings.Count)
                    {
                        object NextItem = Siblings[ItemIndex + 1]!;
                        int EndIndex = VisibleChildren.IndexOf(NextItem);
                        RemoveCount = EndIndex - StartIndex;
                    }
                    else
                        RemoveCount = CountVisibleChildren(item);
                }
                else
                {
                    StartIndex = 1;
                    RemoveCount = VisibleChildren.Count - 1;
                }

                IRemoveItemContext Context = CreateRemoveItemContext(item, StartIndex);
                Context.Start();

                for (int i = 0; i < RemoveCount; i++)
                {
                    object RemovedItem = VisibleChildren[Context.ShownIndex];

                    InternalRemove(Context.ShownIndex, RemovedItem);
                    Context.NextIndex();

                    if (ExpandedChildren.ContainsKey(RemovedItem))
                        ExpandedChildren.Remove(RemovedItem);
                }

                Context.Complete();
                Context.Close();
            }

            NotifyCollectionModified(TreeViewCollectionOperation.Remove);
        }
    }
}
