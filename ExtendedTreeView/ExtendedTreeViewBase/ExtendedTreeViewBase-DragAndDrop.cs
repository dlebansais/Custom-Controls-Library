namespace CustomControls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

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
        /// Performs a drag after the mouse has moved.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void DragAfterMouseMove(MouseEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (AllowDragDrop && e.LeftButton == MouseButtonState.Pressed && (Keyboard.FocusedElement is ExtendedTreeViewItemBase))
                if (IsCopyPossible)
                {
                    ExtendedTreeViewItemBase? ItemContainer = GetEventSourceItem(e);
                    if (ItemContainer != null)
                    {
                        DebugMessage("Drag Started");

                        DragSource.DragAfterMouseMove(e);
                    }
                }
        }

        /// <summary>
        /// Updates the drag drop allowed properties.
        /// </summary>
        protected virtual void UpdateIsDragDropPossible()
        {
            CanonicSelection canonicSelectedItemList = new CanonicSelection(CreateItemList());
            if (GetCanonicSelectedItemList(canonicSelectedItemList))
                DragSource.SetIsDragPossible(canonicSelectedItemList);
            else
                DragSource.ClearIsDragPossible();
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
        /// Occurs when activity of the current drag drop operation has changed.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnDragActivityChanged(object sender, EventArgs e)
        {
            if (sender == null)
                throw new ArgumentNullException(nameof(sender));

            IDragSourceControl Ctrl = (IDragSourceControl)sender;
            bool IsHandled = false;

            switch (Ctrl.DragActivity)
            {
                case DragActivity.Idle:
                    IsHandled = true;
                    break;

                case DragActivity.Starting:
                    DragSource.SetDragItemList(Content, FlatItemList(DragSource.ItemList));

                    CancellationToken cancellation = new CancellationToken();
                    NotifyDragStarting(cancellation);
                    if (cancellation.IsCanceled)
                        Ctrl.CancelDrag();

                    IsHandled = true;
                    break;

                case DragActivity.Started:
                    DataObject Data = new DataObject(DragSource.GetType(), DragSource);
                    DragDropEffects AllowedEffects = DragDropEffects.Move;
                    if (DragSource.AllowDropCopy)
                        AllowedEffects |= DragDropEffects.Copy;
                    DragDrop.DoDragDrop(this, Data, AllowedEffects);
                    ClearCurrentDropTarget();

                    IsHandled = true;
                    break;

                default:
                    break;
            }

            Debug.Assert(IsHandled);
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
        /// Called when feedback for the user is needed.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
            if (e != null)
            {
                Cursor? FeedbackCursor;

                if (UseDefaultCursors)
                    FeedbackCursor = null;
                else if (e.Effects.HasFlag(DragDropEffects.Copy))
                    FeedbackCursor = (CursorCopy != null) ? CursorCopy : DefaultCursorCopy;
                else if (e.Effects.HasFlag(DragDropEffects.Move))
                    FeedbackCursor = (CursorMove != null) ? CursorMove : DefaultCursorMove;
                else
                    FeedbackCursor = (CursorForbidden != null) ? CursorForbidden : DefaultCursorForbidden;

                if (FeedbackCursor != null)
                {
                    e.UseDefaultCursors = false;
                    Mouse.SetCursor(FeedbackCursor);
                    e.Handled = true;
                }
                else
                    base.OnGiveFeedback(e);
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.UIElement.DragEnter"/> attached event reaches an element in its route that is derived from this class.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnDragEnter(DragEventArgs e)
        {
            if (e != null)
            {
                UpdateCurrentDropTarget(e, false);

                e.Effects = GetAllowedDropEffects(e);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.UIElement.DragLeave"/> attached event reaches an element in its route that is derived from this class.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnDragLeave(DragEventArgs e)
        {
            if (e != null)
            {
                UpdateCurrentDropTarget(e, true);

                e.Effects = GetAllowedDropEffects(e);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.UIElement.DragOver"/> attached event reaches an element in its route that is derived from this class.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnDragOver(DragEventArgs e)
        {
            if (e != null)
            {
                UpdateCurrentDropTarget(e, false);

                e.Effects = GetAllowedDropEffects(e);
                e.Handled = true;
            }
        }

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
        /// Gets the drag source from arguments of a drag drop event.
        /// </summary>
        /// <param name="e">The event data.</param>
        /// <returns>The drag source.</returns>
        protected virtual IDragSourceControl? ValidDragSourceFromArgs(DragEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (!e.Data.GetDataPresent(DragSource.GetType()))
                return null;

            IDragSourceControl? AsDragSource = e.Data.GetData(DragSource.GetType()) as IDragSourceControl;
            if (AsDragSource == null)
                return null;

            if (AsDragSource.RootItem == null || Content == null)
                return null;

            if (AsDragSource.RootItem.GetType() != Content.GetType())
                return null;

            IList? FlatItemList = AsDragSource.FlatItemList;
            if (FlatItemList == null || FlatItemList.Count == 0)
                return null;

            return AsDragSource;
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
