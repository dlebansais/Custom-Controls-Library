namespace CustomControls;

using System.Collections;
using System.Collections.Specialized;
using System.Windows.Controls.Primitives;
using Contracts;

/// <summary>
/// Represents a control with a tree of nodes that can be moved around with Drag and Drop.
/// </summary>
public abstract partial class ExtendedTreeViewBase : MultiSelector
{
    /// <summary>
    /// Called when children of an item have changed.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="args">The event data.</param>
    [Access("protected", "virtual")]
    [RequireNotNull(nameof(args))]
    private void HandleChildrenChangedVerified(object item, NotifyCollectionChangedEventArgs args)
    {
        IList OldItems;
        IList NewItems;

        switch (args.Action)
        {
            case NotifyCollectionChangedAction.Add:
                NewItems = Contract.AssertNotNull(args.NewItems);
                OnItemAddChildren(item, args.NewStartingIndex, NewItems);
                break;

            case NotifyCollectionChangedAction.Remove:
                OldItems = Contract.AssertNotNull(args.OldItems);
                OnItemRemoveChildren(item, args.OldStartingIndex, OldItems);
                break;

            case NotifyCollectionChangedAction.Replace:
                OldItems = Contract.AssertNotNull(args.OldItems);
                NewItems = Contract.AssertNotNull(args.NewItems);
                OnItemRemoveChildren(item, args.OldStartingIndex, OldItems);
                OnItemAddChildren(item, args.NewStartingIndex, NewItems);
                break;

            case NotifyCollectionChangedAction.Move:
                NewItems = Contract.AssertNotNull(args.NewItems);
                OnItemMoveChildren(item, args.OldStartingIndex, args.NewStartingIndex, NewItems);
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
    [Access("protected", "virtual")]
    [RequireNotNull(nameof(itemList))]
    private void OnItemAddChildrenVerified(object item, int startIndex, IList itemList)
    {
        NotifyPreviewCollectionModified(TreeViewCollectionOperation.Insert);

        if (IsExpanded(item))
        {
            int ShownPreviousChildrenCount = VisibleChildren.IndexOf(item);
            ShownPreviousChildrenCount += CountPreviousChildrenExpanded(item, startIndex, -1);

            int ShownIndex = ShownPreviousChildrenCount;

            IInsertItemContext Context = CreateInsertItemContext(item, ShownIndex);
            Context.Start();

#if NETCOREAPP3_1
            foreach (object? ChildItem in itemList)
                if (ChildItem is not null)
                    InsertChildren(Context, ChildItem, item);
#else
            foreach (object ChildItem in itemList)
                InsertChildren(Context, ChildItem, item);
#endif

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
    [Access("protected", "virtual")]
    [RequireNotNull(nameof(itemList))]
    private void OnItemRemoveChildrenVerified(object item, int startIndex, IList itemList)
    {
        NotifyPreviewCollectionModified(TreeViewCollectionOperation.Remove);

        if (IsExpanded(item))
        {
            int ShownPreviousChildrenCount = VisibleChildren.IndexOf(item);
            ShownPreviousChildrenCount += CountPreviousChildrenExpanded(item, startIndex, -1);

            int ShownIndex = ShownPreviousChildrenCount;

            IRemoveItemContext Context = CreateRemoveItemContext(item, ShownIndex);
            Context.Start();

#if NETCOREAPP3_1
            foreach (object? ChildItem in itemList)
                if (ChildItem is not null)
                    RemoveChildren(Context, ChildItem);
#else
            foreach (object ChildItem in itemList)
                RemoveChildren(Context, ChildItem);
#endif

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
    [Access("protected", "virtual")]
    [RequireNotNull(nameof(itemList))]
    private void OnItemMoveChildrenVerified(object item, int oldIndex, int newIndex, IList itemList)
    {
        NotifyPreviewCollectionModified(TreeViewCollectionOperation.Move);

        if (IsExpanded(item))
        {
            if (oldIndex < newIndex)
                OnItemMoveChildrenPreviousBefore(item, oldIndex, newIndex, itemList);
            else if (oldIndex > newIndex)
                OnItemMoveChildrenPreviousAfter(item, oldIndex, newIndex, itemList);
        }

        NotifyCollectionModified(TreeViewCollectionOperation.Move);
    }

    private void OnItemMoveChildrenPreviousBefore(object item, int oldIndex, int newIndex, IList itemList)
    {
        int ShownPreviousChildrenCount = VisibleChildren.IndexOf(item);

        int RemoveIndex = ShownPreviousChildrenCount;
        RemoveIndex += CountPreviousChildrenExpanded(item, oldIndex, -1);

        IRemoveItemContext RemoveContext = CreateRemoveItemContext(item, RemoveIndex);
        RemoveContext.Start();

#if NETCOREAPP3_1
        foreach (object? ChildItem in itemList)
            if (ChildItem is not null)
                RemoveChildren(RemoveContext, ChildItem);
#else
        foreach (object ChildItem in itemList)
            RemoveChildren(RemoveContext, ChildItem);
#endif

        RemoveContext.Complete();
        RemoveContext.Close();

        int InsertIndex = ShownPreviousChildrenCount;
        InsertIndex += CountPreviousChildrenExpanded(item, newIndex, -1);

        IInsertItemContext InsertContext = CreateInsertItemContext(item, InsertIndex);
        InsertContext.Start();

#if NETCOREAPP3_1
        foreach (object? ChildItem in itemList)
            if (ChildItem is not null)
                InsertChildren(InsertContext, ChildItem, item);
#else
        foreach (object ChildItem in itemList)
            InsertChildren(InsertContext, ChildItem, item);
#endif

        InsertContext.Complete();
        InsertContext.Close();
    }

    private void OnItemMoveChildrenPreviousAfter(object item, int oldIndex, int newIndex, IList itemList)
    {
        int ShownPreviousChildrenCount = VisibleChildren.IndexOf(item);

        int RemoveIndex = ShownPreviousChildrenCount;
        RemoveIndex += CountPreviousChildrenExpanded(item, oldIndex + 1, newIndex);

        IRemoveItemContext RemoveContext = CreateRemoveItemContext(item, RemoveIndex);
        RemoveContext.Start();

#if NETCOREAPP3_1
        foreach (object? ChildItem in itemList)
            if (ChildItem is not null)
                RemoveChildren(RemoveContext, ChildItem);
#else
        foreach (object ChildItem in itemList)
            RemoveChildren(RemoveContext, ChildItem);
#endif

        RemoveContext.Complete();
        RemoveContext.Close();

        int InsertIndex = ShownPreviousChildrenCount;
        InsertIndex += CountPreviousChildrenExpanded(item, newIndex, -1);

        IInsertItemContext InsertContext = CreateInsertItemContext(item, InsertIndex);
        InsertContext.Start();

#if NETCOREAPP3_1
        foreach (object? ChildItem in itemList)
            if (ChildItem is not null)
                InsertChildren(InsertContext, ChildItem, item);
#else
        foreach (object ChildItem in itemList)
            InsertChildren(InsertContext, ChildItem, item);
#endif

        InsertContext.Complete();
        InsertContext.Close();
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

            if (ParentItem is not null)
            {
                StartIndex = VisibleChildren.IndexOf(item) + 1;

                IList Siblings = GetItemChildren(ParentItem);
                int ItemIndex = Siblings.IndexOf(item);
                if (ItemIndex + 1 < Siblings.Count)
                {
                    object NextItem = Contract.AssertNotNull(Siblings[ItemIndex + 1]);
                    int EndIndex = VisibleChildren.IndexOf(NextItem);
                    RemoveCount = EndIndex - StartIndex;
                }
                else
                {
                    RemoveCount = CountVisibleChildren(item);
                }
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

                _ = ExpandedChildren.Remove(RemovedItem);
            }

            Context.Complete();
            Context.Close();
        }

        NotifyCollectionModified(TreeViewCollectionOperation.Remove);
    }
}
