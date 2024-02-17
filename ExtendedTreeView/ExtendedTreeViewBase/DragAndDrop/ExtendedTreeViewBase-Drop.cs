namespace CustomControls;

using System.Collections;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls.Primitives;
using Contracts;

/// <summary>
/// Represents a control with a tree of nodes that can be moved around with Drag and Drop.
/// </summary>
public abstract partial class ExtendedTreeViewBase : MultiSelector
{
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
    /// Invoked when an unhandled <see cref="UIElement.Drop"/> attached event reaches an element in its route that is derived from this class.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnDrop(DragEventArgs e)
    {
        DragEventArgs args = Contract.AssertNotNull(e);

        args.Effects = GetAllowedDropEffects(args);
        args.Handled = true;

        if (args.Effects != DragDropEffects.None)
        {
            ExtendedTreeViewItemBase? ItemContainer = GetEventSourceItem(args);
            IDragSourceControl AsDragSource = (IDragSourceControl)args.Data.GetData(DragSource.GetType());
            bool IsDragPossible = AsDragSource.IsDragPossible(out object SourceItem, out IList ItemList);
            object? DestinationItem = ItemContainer?.Content;

            UnselectAll();

            if (IsDragPossible && DestinationItem is not null)
            {
                IList CloneList = CreateItemList();

                Debug.Assert(CloneList.Count == 0);
                NotifyPreviewDropCompleted(DestinationItem, args.Effects, ItemList, CloneList);

                if (args.Effects == DragDropEffects.Copy)
                    DragDropCopy(SourceItem, DestinationItem, ItemList, CloneList);
                else if (args.Effects == DragDropEffects.Move)
                    DragDropMove(SourceItem, DestinationItem, ItemList);

                Debug.Assert(CloneList.Count > 0 || args.Effects != DragDropEffects.Copy);
                NotifyDropCompleted(DestinationItem, args.Effects, ItemList, CloneList);

                Expand(DestinationItem);
            }
        }
    }

    /// <summary>
    /// Gets the list of allowed drag drop effects for an operation.
    /// </summary>
    /// <param name="e">The event data.</param>
    /// <returns>The list of allowed effects.</returns>
    protected virtual DragDropEffects GetAllowedDropEffects(DragEventArgs e)
    {
        if (GetValidDragSourceFromArgs(e, out IDragSourceControl AsDragSource))
        {
            if (GetValidDropDestinationFromArgs(e, AsDragSource, out object DropDestinationItem))
            {
                PermissionToken permission = new();
                NotifyDropCheck(DropDestinationItem, permission);

                if (permission.IsAllowed)
                    return MergedAllowedEffects(e);
            }
        }

        return DragDropEffects.None;
    }

    /// <summary>
    /// Updates the target of a drag drop operation.
    /// </summary>
    /// <param name="e">The event data.</param>
    /// <param name="isLeaving">True if the operation is to leave the target.</param>
    protected virtual void UpdateCurrentDropTarget(DragEventArgs e, bool isLeaving)
    {
        ExtendedTreeViewItemBase? DropTarget;
        if (isLeaving)
            DropTarget = null;
        else
            DropTarget = GetEventSourceItem(e);

        if (DropTargetContainer != DropTarget)
        {
            ExtendedTreeViewItemBase? OldDropTarget = DropTargetContainer;
            DropTargetContainer = DropTarget;
            ExtendedTreeViewItemBase? NewDropTarget = DropTargetContainer;

            OldDropTarget?.UpdateIsDropOver();

            if (NewDropTarget is not null)
            {
                NewDropTarget.UpdateIsDropOver();

                object NewItem = NewDropTarget.Content;
                if (NewItem is not null)
                {
                    int NewIndex = VisibleChildren.IndexOf(NewItem);
                    if (NewIndex != -1 && DropTargetContainerIndex != -1)
                    {
                        if (NewIndex > DropTargetContainerIndex && NewIndex + 1 < VisibleChildren.Count)
                            ScrollIntoView(VisibleChildren[NewIndex + 1]);
                        if (NewIndex < DropTargetContainerIndex && NewIndex > 0)
                            ScrollIntoView(VisibleChildren[NewIndex - 1]);
                    }

                    DropTargetContainerIndex = NewIndex;
                }
            }
        }
    }

    /// <summary>
    /// Clears the target of a drag drop operation.
    /// </summary>
    protected virtual void ClearCurrentDropTarget()
    {
        _ = Dispatcher.BeginInvoke(new ClearCurrentDropTargetHandler(OnClearCurrentDropTarget));
    }

    /// <summary>
    /// Represents the method that will handle a ClearCurrentDropTarget event.
    /// </summary>
    protected delegate void ClearCurrentDropTargetHandler();

    /// <summary>
    /// Handles a ClearCurrentDropTarget event.
    /// </summary>
    protected virtual void OnClearCurrentDropTarget()
    {
        ExtendedTreeViewItemBase? OldDropTarget = DropTargetContainer;
        DropTargetContainer = null;
        DropTargetContainerIndex = -1;

        OldDropTarget?.UpdateIsDropOver();
    }

    /// <summary>
    /// Gets the drag target from arguments of a drag drop event.
    /// </summary>
    /// <param name="e">The event data.</param>
    /// <param name="asDragSource">The drag source.</param>
    /// <param name="destinationItem">The drag target upon return.</param>
    /// <returns>True if successful.</returns>
    protected virtual bool GetValidDropDestinationFromArgs(DragEventArgs e, IDragSourceControl asDragSource, out object destinationItem)
    {
        DragEventArgs Args = Contract.AssertNotNull(e);
        IDragSourceControl AsDragSource = Contract.AssertNotNull(asDragSource);

        if (DropTargetContainer is not null)
        {
            object DropDestinationItem = DropTargetContainer.Content;

            if (AsDragSource.SourceGuid == DragSource.SourceGuid)
            {
                if (!AsDragSource.IsDraggedItemParent(DropDestinationItem) || (Args.AllowedEffects.HasFlag(DragDropEffects.Copy) && Args.KeyStates.HasFlag(DragDropKeyStates.ControlKey)))
                {
                    if (!AsDragSource.HasDragItemList(out _, out IList FlatItemList) || !FlatItemList.Contains(DropDestinationItem))
                    {
                        destinationItem = DropDestinationItem;
                        return true;
                    }
                }
            }
        }

        Contract.Unused(out destinationItem);
        return false;
    }
}
