namespace CustomControls;

using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;
using Contracts;

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
        Contract.RequireNotNull(canonicSelectedItemList, out CanonicSelection CanonicSelectedItemList);

        if (SelectedItems.Count == 0)
            return false;

        List<object> SortedSelectedItems = new();

#if NETCOREAPP3_1
        foreach (object? Item in SelectedItems)
            if (Item is not null)
                SortedSelectedItems.Add(Item);
#else
        foreach (object Item in SelectedItems)
            SortedSelectedItems.Add(Item);
#endif

        SortedSelectedItems.Sort(SortByIndex);

        object FirstItem = SortedSelectedItems[0];
        if (IsContent(FirstItem))
            return false;

        object? FirstItemParent = GetItemParent(FirstItem);

        if (FirstItemParent is not null)
        {
            if (GetItemsWithSameParent(SortedSelectedItems, FirstItemParent, CanonicSelectedItemList))
            {
                CanonicSelectedItemList.SetDraggedItemParent(FirstItemParent);
                return true;
            }

            if (GetItemsInSameBranch(SortedSelectedItems, FirstItemParent, CanonicSelectedItemList))
            {
                CanonicSelectedItemList.SetDraggedItemParent(FirstItemParent);
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
        if (sortedSelectedItems is null || canonicSelectedItemList is null)
            return false;

        canonicSelectedItemList.AllItemsCloneable = true;

#if NETCOREAPP3_1
        foreach (object? Item in sortedSelectedItems)
            if (Item is not null)
            {
                if (GetItemParent(Item) != firstItemParent)
                    return false;

                canonicSelectedItemList.ItemList.Add(Item);

                if (!IsItemCloneable(Item))
                    canonicSelectedItemList.AllItemsCloneable = false;
            }
#else
        foreach (object Item in sortedSelectedItems)
        {
            if (GetItemParent(Item) != firstItemParent)
                return false;

            canonicSelectedItemList.ItemList.Add(Item);

            if (!IsItemCloneable(Item))
                canonicSelectedItemList.AllItemsCloneable = false;
        }
#endif

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
        if (sortedSelectedItems is null || canonicSelectedItemList is null)
            return false;

        IList Children = GetItemChildren(firstItemParent);

#if NETCOREAPP3_1
        foreach (object? ChildItem in Children)
            if (ChildItem is not null)
            {
                if (sortedSelectedItems.Contains(ChildItem))
                {
                    if (!canonicSelectedItemList.ItemList.Contains(ChildItem))
                        canonicSelectedItemList.ItemList.Add(ChildItem);
                    if (!IsEntireBranchSelected(sortedSelectedItems, ChildItem, canonicSelectedItemList))
                        return false;
                }
            }
#else
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
#endif

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
        if (sortedSelectedItems is null || canonicSelectedItemList is null)
            return false;

        if (!IsItemCloneable(item))
            canonicSelectedItemList.AllItemsCloneable = false;

        canonicSelectedItemList.RecordCount++;

        if (IsExpanded(item))
        {
            IList Children = GetItemChildren(item);

#if NETCOREAPP3_1
            foreach (object? ChildItem in Children)
                if (ChildItem is not null)
                {
                    if (!sortedSelectedItems.Contains(ChildItem))
                        return false;

                    if (!IsEntireBranchSelected(sortedSelectedItems, ChildItem, canonicSelectedItemList))
                        return false;
                }
#else
            foreach (object ChildItem in Children)
            {
                if (!sortedSelectedItems.Contains(ChildItem))
                    return false;

                if (!IsEntireBranchSelected(sortedSelectedItems, ChildItem, canonicSelectedItemList))
                    return false;
            }
#endif
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

        if (other is not null)
        {
#if NETCOREAPP3_1
            foreach (object? Item in other)
                if (Item is not null)
                {
                    Result.Add(Item);

                    IList FlatChildren = FlatItemList(GetItemChildren(Item));
                    foreach (object? Child in FlatChildren)
                        if (Child is not null)
                            Result.Add(Child);
                }
#else
            foreach (object Item in other)
            {
                Result.Add(Item);

                IList FlatChildren = FlatItemList(GetItemChildren(Item));
                foreach (object Child in FlatChildren)
                    Result.Add(Child);
            }
#endif
        }

        return Result;
    }

    /// <summary>
    /// Merges allowed effects with those allowed by a drag drop event.
    /// </summary>
    /// <param name="args">The event data.</param>
    /// <returns>Merged allowed effects.</returns>
    protected virtual DragDropEffects MergedAllowedEffects(DragEventArgs args)
    {
        Contract.RequireNotNull(args, out DragEventArgs Args);

        if (Args.AllowedEffects.HasFlag(DragDropEffects.Move))
            if (Args.AllowedEffects.HasFlag(DragDropEffects.Copy) && Args.KeyStates.HasFlag(DragDropKeyStates.ControlKey))
                return DragDropEffects.Copy;
            else
                return DragDropEffects.Move;
        else if (Args.AllowedEffects.HasFlag(DragDropEffects.Copy))
            return DragDropEffects.Copy;
        else
            return DragDropEffects.None;
    }

    private IDragSourceControl DragSource = null!;
    private static ExtendedTreeViewItemBase? DropTargetContainer;
    private static int DropTargetContainerIndex;
}
