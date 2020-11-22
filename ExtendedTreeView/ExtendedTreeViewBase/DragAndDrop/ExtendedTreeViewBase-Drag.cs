namespace CustomControls
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    /// <summary>
    /// Represents a control with a tree of nodes that can be moved around with Drag and Drop.
    /// </summary>
    public abstract partial class ExtendedTreeViewBase : MultiSelector
    {
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
        /// Occurs when activity of the current drag drop operation has changed.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnDragActivityChanged(object? sender, EventArgs e)
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
    }
}
