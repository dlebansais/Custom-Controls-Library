namespace CustomControls
{
    using System;
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// Represents a control with a tree of nodes that can be moved around with Drag and Drop.
    /// </summary>
    public abstract partial class ExtendedTreeViewBase : MultiSelector
    {
        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.UIElement.Drop"/> attached event reaches an element in its route that is derived from this class.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnDrop(DragEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            e.Effects = GetAllowedDropEffects(e);
            e.Handled = true;

            if (e.Effects != DragDropEffects.None)
            {
                ExtendedTreeViewItemBase? ItemContainer = GetEventSourceItem(e);
                IDragSourceControl AsDragSource = (IDragSourceControl)e.Data.GetData(DragSource.GetType());
                object? SourceItem = AsDragSource.DragParentItem;
                IList? ItemList = AsDragSource.ItemList;
                object? DestinationItem = ItemContainer != null ? ItemContainer.Content : null;

                UnselectAll();

                if (SourceItem != null && DestinationItem != null)
                {
                    if (e.Effects == DragDropEffects.Copy)
                    {
                        IList CloneList = CreateItemList();
                        NotifyPreviewDropCompleted(DestinationItem, CloneList);
                        DragDropCopy(SourceItem, DestinationItem, ItemList, CloneList);
                        NotifyDropCompleted(DestinationItem, CloneList);
                    }
                    else if (e.Effects == DragDropEffects.Move)
                    {
                        NotifyPreviewDropCompleted(DestinationItem, null);
                        DragDropMove(SourceItem, DestinationItem, ItemList);
                        NotifyDropCompleted(DestinationItem, null);
                    }

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
            IDragSourceControl? AsDragSource = ValidDragSourceFromArgs(e);
            if (AsDragSource != null)
            {
                object? DropDestinationItem = ValidDropDestinationFromArgs(e, AsDragSource);
                if (DropDestinationItem != null)
                {
                    PermissionToken permission = new PermissionToken();
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
                DropTarget = GetTarget(e);

            if (DropTargetContainer != DropTarget)
            {
                ExtendedTreeViewItemBase? OldDropTarget = DropTargetContainer;
                DropTargetContainer = DropTarget;
                ExtendedTreeViewItemBase? NewDropTarget = DropTargetContainer;

                if (OldDropTarget != null)
                    OldDropTarget.UpdateIsDropOver();
                if (NewDropTarget != null)
                {
                    NewDropTarget.UpdateIsDropOver();

                    object NewItem = NewDropTarget.Content;
                    if (NewItem != null)
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
            Dispatcher.BeginInvoke(new ClearCurrentDropTargetHandler(OnClearCurrentDropTarget));
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

            if (OldDropTarget != null)
                OldDropTarget.UpdateIsDropOver();
        }

        /// <summary>
        /// Gets the drag target from arguments of a drag drop event.
        /// </summary>
        /// <param name="e">The event data.</param>
        /// <param name="asDragSource">The drag source.</param>
        /// <returns>The drag target.</returns>
        protected virtual object? ValidDropDestinationFromArgs(DragEventArgs e, IDragSourceControl asDragSource)
        {
            if (DropTargetContainer == null || e == null || asDragSource == null)
                return null;

            object DropDestinationItem = DropTargetContainer.Content;

            if (asDragSource.SourceGuid != DragSource.SourceGuid)
                return null;

            if (asDragSource.DragParentItem == DropDestinationItem)
                if (!e.AllowedEffects.HasFlag(DragDropEffects.Copy) || !e.KeyStates.HasFlag(DragDropKeyStates.ControlKey))
                    return null;

            IList? FlatItemList = asDragSource.FlatItemList;
            if (FlatItemList != null && FlatItemList.Contains(DropDestinationItem))
                return null;

            return DropDestinationItem;
        }
    }
}
